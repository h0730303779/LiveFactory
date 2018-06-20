using LiveCommand.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using LiveFactory.Application.Command.Dto;
using System.Linq;

namespace LiveFactory.Application.Command
{
    public static class LiveChannelStateManage
    {
        static List<LiveStateDto> dictState = new List<LiveStateDto>();
        static string _dictKey = "LiveStateDto";

        public static List<LiveStateDto> StateData
        {
            get
            {
                if (dictState == null)
                    return new List<LiveStateDto>();
                return dictState;
            }
        }
        static object lockObj = new object();
        public static void UpdateState(Guid ChannelId, LiveStateType state)
        {
            lock (lockObj)
            {
                if (dictState == null)
                {
                    dictState = new List<LiveStateDto>();
                }
                else
                {
                    var fist = dictState.FirstOrDefault(b => b.Id == ChannelId);
                    if (fist == null)
                        dictState.Add(new LiveStateDto() { Id = ChannelId, State = state });
                    else
                        fist.State = state;
                }
            }
        }

        public static LiveStateDto GetModel(Guid id)
        {
            var fist = dictState.FirstOrDefault(b => b.Id == id);
            return fist;
        }
    }
}
