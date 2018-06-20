using LiveCommand.Common;
using System.Collections.Generic;
using System.Linq;

namespace LiveFactory.Pusher
{
    public static class ChannelPusherService
    {
        //如果使用静态变量进行锁线程, 所有使用此类内的外部调用将排队执行.
        //等于此类不允许并行, 没有执行多线程.
        private static object _lockObj = new object();
        public static List<ChannelPusher> ChannelPushers { get; set; } = new List<ChannelPusher>();

        /// 将一个频道压入工作列表
        /// </summary>
        /// <param name="liveChannel"></param>
        /// <returns></returns>
        public static ChannelPusher PushChannelIntoWorkingList(CommandDto commandDto)
        {
            lock (_lockObj)
            {
                if (!IsChannelExistingInWorkingList(commandDto))
                {
                    var channelPusher = new ChannelPusher
                    {
                        CommandDto = commandDto
                    };
                    //所有压入到工作表的状态都会修改为开始
                    channelPusher.CommandDto.CommandType = CommandTypeEnum.Start;
                    ChannelPushers.Add(channelPusher);
                    return channelPusher;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get ChannelPusher
        /// </summary>
        /// <param name="commandDto"></param>
        /// <returns></returns>
        public static ChannelPusher GetChannelPusherById(CommandDto commandDto)
        {
            return ChannelPushers.FirstOrDefault(p => p.CommandDto.ChanneId == commandDto.ChanneId);
        }
        public static void RemoveChannelPusher(ChannelPusher channelPusher)
        {
            lock (_lockObj)
            {
                ChannelPushers.Remove(channelPusher);
            }
        }
        /// <summary>
        /// check a specified channel is existing in working list or Not.
        /// </summary>
        /// <param name="liveChannel"></param>
        /// <returns></returns>
        private static bool IsChannelExistingInWorkingList(CommandDto commandDto)
        {
            return ChannelPushers.Any(a => a.CommandDto.ChanneId == commandDto.ChanneId);
        }
    }
}
