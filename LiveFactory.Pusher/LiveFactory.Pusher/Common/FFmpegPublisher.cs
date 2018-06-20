using FFmpeg.AutoGen;
using LiveCommand.Common;
using System;
using System.Threading;
using static FFmpeg.AutoGen.ffmpeg;


namespace LiveFactory.Pusher
{
    /// <summary>
    /// 不能使用Null与指针对象做对比!!!!!!!!!!!!
    /// </summary>
    public sealed unsafe class FFmpegPublisher : IDisposable
    {
        private AVFormatContext* _inputContext;
        private AVFormatContext* _outputContext;
        private readonly AVPacket* _pPacket;
        private CommandDto _command;
        private int _errorId;
        //private int _videoStreamIndex;

        public FFmpegPublisher(CommandDto command)
        {
            _pPacket = av_packet_alloc();
            _command = command;
        }
        public void Start(CancellationToken cancellationToken)
        {
            if (InitInput() < 0) return;
            if (InitOutput() < 0) return;
            // 推流器启动完成
            OnStartCompleted(_command);
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                _errorId = ReadPacketFromSource();
                if (CheckError(_errorId, "读取Pkt超时.   ")) break;
                WritePacket();
                av_packet_unref(_pPacket);
            }
            //停止推送
            OnPushStoped(_command);
        }

        #region Events
        public event Action<int, CommandDto> OnFinishedWritePktEvent;
        private void OnFinishedWritePkt(int retId, CommandDto command)
        {
            OnFinishedWritePktEvent?.Invoke(retId, command);
        }
        public event Action<CommandDto> OnPushStopedEvent;
        private void OnPushStoped(CommandDto command)
        {
            OnPushStopedEvent?.Invoke(command);
        }
        public event Action<CommandDto> OnStartCompletedEvent;
        private void OnStartCompleted(CommandDto command)
        {
            OnStartCompletedEvent?.Invoke(command);
        }
        public event Action<int, CommandDto, string> OnErrorEvent;
        private void OnError(int errorId, CommandDto command, string errorMsg)
        {
            OnErrorEvent?.Invoke(errorId, command, errorMsg);
        }
        private bool CheckError(int errorId, string errorMsg = "")
        {
            if (errorId >= 0) return false;
            OnError(errorId, _command, errorMsg + FFmpegHelper.GetStrError(errorId));
            return true;
        }
        #endregion

        #region Read/Write Packet
        private void WritePacket()
        {

            var inputStream = _inputContext->streams[_pPacket->stream_index];
            var outputStream = _outputContext->streams[_pPacket->stream_index];
            //var inputStream = _inputContext->streams[_videoStreamIndex];
            //var outputStream = _outputContext->streams[_videoStreamIndex];
            //调整DTS/PTS,预防视频速率不正常. 
            av_packet_rescale_ts(_pPacket, inputStream->time_base, outputStream->time_base);
            //FFMPeg处理的太快,此处经常返回-541478725错误, End of File. -22 invalid argument
            int ret = av_interleaved_write_frame(_outputContext, _pPacket);
            OnFinishedWritePkt(ret, _command);
        }
        private int ReadPacketFromSource()
        {
            /* 
             *作者:Jack
             * 时间:2018-06-08
             * 描述:增加读取frame回调函数
             */
            DateTime startDT = DateTime.Now;
            AVIOInterruptCB_callback readFrameCallback = (p0) =>
            {
                //   var p = (AVFormatContext*)p0;
                var endDT = DateTime.Now;
                int seconds = (endDT - startDT).Seconds;
                if (seconds >= 3)
                {
                    return 1;
                }
                return 0;
            };
            _inputContext->interrupt_callback.callback = readFrameCallback;
            _inputContext->interrupt_callback.opaque = _inputContext;

            var ret = av_read_frame(_inputContext, _pPacket);
            return ret;
        }
        #endregion

        #region Init Input/Output
        private int InitOutput()
        {
            var outputContext = _outputContext;
            _errorId = avformat_alloc_output_context2(&outputContext, null, "flv", _command.PushAddress);
            if (CheckError(_errorId)) return _errorId;

            _outputContext = outputContext;
            _errorId = avio_open2(&_outputContext->pb, _command.PushAddress, AVIO_FLAG_READ_WRITE, null, null);
            if (CheckError(_errorId)) return _errorId;

            for (int i = 0; i < _inputContext->nb_streams; i++)
            {
                /*                
                 * 作者:Jack
                 * 日期:2018-05-29
                 * 描述:只推送视频流.
                 */
                //if (_inputContext->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                //{
                //    _videoStreamIndex = i;
                //    AVStream* stream = avformat_new_stream(_outputContext, _inputContext->streams[i]->codec->codec);
                //    avcodec_copy_context(stream->codec, _inputContext->streams[i]->codec)
                //    break;
                //}
                AVStream* stream = avformat_new_stream(_outputContext, _inputContext->streams[i]->codec->codec);
                _errorId = avcodec_copy_context(stream->codec, src: _inputContext->streams[i]->codec);
                if (CheckError(_errorId)) return _errorId;
            }
            _errorId = avformat_write_header(_outputContext, null);
            if (CheckError(_errorId)) return _errorId;

            return 0;
        }
        private int InitInput()
        {
            _inputContext = avformat_alloc_context();

            AVDictionary* options = null;
            _errorId = av_dict_set(&options, "rtsp_transport", "udp", 0);
            if (CheckError(_errorId)) return _errorId;

            /* 
             * 作者:Jack
             * 日期:2018-06-08
             * 描述:增加打开视频源超时.
          */
            _errorId = av_dict_set(&options, "stimeout", "5000000", 0);
            if (CheckError(_errorId)) return _errorId;
            //var startOpen = DateTime.Now;
            //AVIOInterruptCB_callback interruptCB_Callback = (p0) =>
            //{
            //    /// AVFormatContext* formatContext = (AVFormatContext*)p0;
            //    var endDate = DateTime.Now;
            //    if ((endDate - startOpen).Seconds >= 5)
            //    {
            //        Console.WriteLine((endDate - startOpen).Seconds);
            //        return 1;
            //    }
            //    return 0;
            //};
            //_inputContext->interrupt_callback.callback = interruptCB_Callback;
            // _inputContext->interrupt_callback.opaque = _inputContext;

            var inputContext = _inputContext;
            _errorId = avformat_open_input(&inputContext, _command.PullAddress, null, &options);
            if (CheckError(_errorId, "读取源地址超时.  "))
            {
                //如果读取失败, inpuContext分配内存空间也失败, 执行Dispose方法会报错.
                _inputContext = avformat_alloc_context();
                return _errorId;
            };

            _errorId = avformat_find_stream_info(_inputContext, null);
            if (CheckError(_errorId)) return _errorId;

            return 0;
        }
        #endregion

        public void Dispose()
        {
            av_packet_unref(_pPacket);
            av_free(_pPacket);

            var inputContext = _inputContext;
            avformat_close_input(&inputContext);

            var outputContext = _outputContext;
            avformat_close_input(&outputContext);
        }
    }
}
