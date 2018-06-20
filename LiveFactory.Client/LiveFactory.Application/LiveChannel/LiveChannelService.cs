using AutoMapper;
using JFJT.Framework.Application.Dto;
using JFJT.Framework.AutoMapper;
using JFJT.Framework.Domain.Repositories;
using JFJT.Framework.Extensions;
using LiveCommand.Common;
using LiveCommand.Common.NetMQ;
using LiveFactory.Application.Dto;
using LiveFactory.Core.Enums;
using LiveFactory.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace LiveFactory.Application
{
    public interface ILiveChannelService
    {
        PageListResult<LiveChannelDto> GetLiveChannelPageList(LiveChannelSearchDto liveChannelSearchDto);

        void Delete(Guid id);

        Task InsertOrUpdate(LiveChannelDto liveChannelDto);

        List<LiveChannelDto> GetLiveChannels(LiveChannelSearchDto liveChannelSearchDto);

        Task<List<LiveChannelWebDto>> GetLiveChannelWeb();

        Task<LiveChannelDto> Get(Guid id);


        int GetLiveChannelCount();

        //Task UpdateState(LogDto data);


    }

    public class LiveChannelService : ILiveChannelService
    {
        IRepository<LiveChannel, Guid> _liveChannelRepository;
        private readonly IRepository<PlayInfo> _playInfoRepository;
        IRepository<PullInfo> _pullInfoRepository;
        //private readonly ILiveChannelManager _liveChannelManager;
        private static bool isLoadLiveChannelManagerData = false;

        public LiveChannelService(
            IRepository<LiveChannel, Guid> liveChannelRepository,
            IRepository<PlayInfo> playInfoRepository,
            IRepository<PullInfo> pullInfoRepository
            //,ILiveChannelManager liveChannelManager
            )
        {
            _liveChannelRepository = liveChannelRepository;
            _playInfoRepository = playInfoRepository;
            _pullInfoRepository = pullInfoRepository;
            //_liveChannelManager = liveChannelManager;

            if (!isLoadLiveChannelManagerData)
            {
                LoadLiveChannelManagerData();
                isLoadLiveChannelManagerData = true;
            }
        }

        public void LoadLiveChannelManagerData()
        {
            var data = _liveChannelRepository.GetAll().ToList();
            var pullData = _pullInfoRepository.GetAll().ToList();
            foreach (var item in data)
            {
                if (item.FactoryCode != "123456789")
                {
                    continue;
                }
                var model = new LiveChannelManagerDto
                {
                    Name = item.Name,
                    Id  = item.Id,
                    BannedVideoLoop = item.BannedVideoLoop,
                    Disabled = item.Disabled,
                    FactoryCode = item.FactoryCode,
                    FactoryName = item.FactoryName,
                    Index = item.Index,
                    PlayInterval = item.PlayInterval,
                    PullInterval = item.PullInterval,
                    PushUrl = item.PushUrl,
                    VideoId = item.VideoId
                };
                LiveChannelManager.Data.Add(model);
            }
            foreach (var item in pullData)
            {
                var model = new PullInfoManagerDto()
                {
                    ChannelId = item.ChannelId,
                    Id = item.Id,
                    Name = item.Name,
                    Url = item.Url
                };
                LiveChannelManager.PullData.Add(model);
            }

        }


        public void Delete(Guid id)
        {
            _liveChannelRepository.Delete(id);
        }

        public PageListResult<LiveChannelDto> GetLiveChannelPageList(LiveChannelSearchDto liveChannelSearchDto)
        {
            var result = _liveChannelRepository.GetAll().WhereIf(!liveChannelSearchDto.KeyWord.IsNullOrWhiteSpace(), m => (m.Name.Contains(liveChannelSearchDto.KeyWord) || m.FactoryName.Contains(liveChannelSearchDto.KeyWord)))
                .OrderBy(liveChannelSearchDto.Orderby).PageBy<LiveChannel, LiveChannelDto>(liveChannelSearchDto);
            return result;
        }

        public async Task InsertOrUpdate(LiveChannelDto liveChannelDto)
        {
            var model = liveChannelDto.MapTo<LiveChannel>();
            if (!model.IsNewData())
            {
                await _liveChannelRepository.UpdateAsync(model);
            }
            else
            {
                var result = await _liveChannelRepository.InsertAsync(model);
            }
        }

        public List<LiveChannelDto> GetLiveChannels(LiveChannelSearchDto liveChannelSearchDto)
        {
            var result = _liveChannelRepository.GetAll().WhereIf(!liveChannelSearchDto.KeyWord.IsNullOrWhiteSpace(), m => (m.Name.Contains(liveChannelSearchDto.KeyWord) || m.FactoryName.Contains(liveChannelSearchDto.KeyWord)))
                .ToList();
            var mapResult = result.MapTo<List<LiveChannelDto>>();
            return mapResult;
        }

        public async Task<List<LiveChannelWebDto>> GetLiveChannelWeb()
        {
            var channels = await _liveChannelRepository.GetAll().Where(w => !w.Disabled).ToListAsync();
            var ids = channels.Select(s => s.Id).ToArray();
            var playInfos = await _playInfoRepository.GetAll().Where(w => ids.Contains(w.ChannelId)).ToListAsync();
            var result = channels.Select(s => new LiveChannelWebDto
            {
                Id = s.Id,
                Name = s.Name,
                Image = s.Image,
                ErrPlayUrl = s.ErrPlayUrl,
                Type = playInfos.FirstOrDefault(f => f.ChannelId == s.Id)?.Type ?? 0,
                PlayUrl = playInfos.FirstOrDefault(f => f.ChannelId == s.Id)?.PlayUrl,
                ChannelDesc = s.ChannelDesc
            }).Where(w => !w.PlayUrl.IsNullOrWhiteSpace()).ToList();
            return result;
        }

        public async Task<LiveChannelDto> Get(Guid id)
        {
            var result = await _liveChannelRepository.GetAsync(id);
            return result.MapTo<LiveChannelDto>();
        }

        public int GetLiveChannelCount()
        {
            var count = _liveChannelRepository.GetAll().ToList().Count;
            return count;
        }
        //public async Task UpdateState(LogDto data)
        //{
        //    if (data != null)
        //    {
        //        //var ids = data.GroupBy(g => g.ChanneId).Select(s => s.Key);
        //        //var entitys = _liveChannelRepository.GetAll().Where(b => ids.Contains(b.Id)).ToList();
        //        //foreach (var item in entitys)
        //        //{
        //        var first = await _liveChannelRepository.GetAsync(data.ChanneId);
        //        if (first != null)
        //        {
        //            first.State = data.LogType;
        //            //switch (data.LogType)
        //            //{

        //            //    case LiveStateType.Pushing:
        //            //        first.State = LiveChannelState.Play;
        //            //        break;
        //            //    case LiveStateType.Error:
        //            //        first.State = LiveChannelState.Abnormal;
        //            //        break;
        //            //    case LiveStateType.Waiting:
        //            //    case LiveStateType.Started:
        //            //    case LiveStateType.Stoped:
        //            //        first.State = LiveChannelState.Stop;
        //            //        break;
        //            //    default:
        //            //        break;
        //            //}
        //            await _liveChannelRepository.UpdateAsync(first);
        //        }
        //        //}
        //    }
        //}
    }
}

