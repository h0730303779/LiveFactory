using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FFmpeg.AutoGen;

namespace LiveFactory.Pusher
{
    public static class FFmpegHelper
    {
        public static unsafe string GetStrError(int errorId)
        {
            var bufferSize = 1024;
            var buffer = stackalloc byte[bufferSize];
            ffmpeg.av_strerror(errorId, buffer, (ulong)bufferSize);
            var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
            return message;
        }
        public static int ThrowExceptionIfError(this int errorId)
        {
            if (errorId < 0)
            { throw new ApplicationException(GetStrError(errorId)); };
            return errorId;
        }
    }
}
