using JFJT.Framework.Application.Dto;
using JFJT.Framework.AutoMapper;
using JFJT.Framework.Domain.Repositories;
using JFJT.Framework.Extensions;
using LiveFactory.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace LiveFactory.Application
{
    public interface IPlayInfoService
    {
        PageListResult<PlayInfoDto> GetPlayInfoPageList(PlayInfoSearchDto playInfoSearchDto);

        void Delete(int id);

        Task InsertOrUpdate(PlayInfoDto playInfoDto);

        void DeleteByChannelId(Guid id);

    }
    public class PlayInfoService : IPlayInfoService
    {
        IRepository<PlayInfo> _playInfoRepository;

        public PlayInfoService(IRepository<PlayInfo> playInfoRepository)
        {
            _playInfoRepository = playInfoRepository;
        }

        public void Delete(int id)
        {
            _playInfoRepository.Delete(id);
        }

        public void DeleteByChannelId(Guid id)
        {
            _playInfoRepository.Delete(m => m.ChannelId == id);
        }

        public PageListResult<PlayInfoDto> GetPlayInfoPageList(PlayInfoSearchDto playInfoSearchDto)
        {
            var result = _playInfoRepository.GetAll().WhereIf(!playInfoSearchDto.KeyWord.IsNullOrWhiteSpace(), m => m.Name.Contains(playInfoSearchDto.KeyWord))
                .WhereIf(playInfoSearchDto.ChannelId.HasValue, m => m.ChannelId == playInfoSearchDto.ChannelId)
                .OrderBy(playInfoSearchDto.Orderby).PageBy<PlayInfo, PlayInfoDto>(playInfoSearchDto);
            return result;
        }


        public async Task InsertOrUpdate(PlayInfoDto playInfoDto)
        {
            var model = playInfoDto.MapTo<PlayInfo>();
            if (!model.IsNewData())
            {
                await _playInfoRepository.UpdateAsync(model);
            }
            else
            {
                await _playInfoRepository.InsertAsync(model);
            }
        }


    }
}
