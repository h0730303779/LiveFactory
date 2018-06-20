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
    public interface IPullInfoService
    {
        PageListResult<PullInfoDto> GetPullInfoPageList(PullInfoSearchDto pullInfoSearchDto);

        Task InsertOrUpdate(PullInfoDto pullInfoDto);

        void Delete(int id);
        void DeleteByChannelId(Guid id);
    }
    public class PullInfoService : IPullInfoService
    {
        IRepository<PullInfo> _pullInfoRepository;
        public PullInfoService(IRepository<PullInfo> pullInfoRepository)
        {
            _pullInfoRepository = pullInfoRepository;
        }

        public void Delete(int id)
        {
            _pullInfoRepository.Delete(id);
        }

        public void DeleteByChannelId(Guid id)
        {
            _pullInfoRepository.Delete(m => m.ChannelId == id);
        }

        public PageListResult<PullInfoDto> GetPullInfoPageList(PullInfoSearchDto pullInfoSearchDto)
        {
            var result = _pullInfoRepository.GetAll().WhereIf(!pullInfoSearchDto.KeyWord.IsNullOrWhiteSpace(), m => m.Name.Contains(pullInfoSearchDto.KeyWord))
                .WhereIf(pullInfoSearchDto.ChannelId.HasValue, m => m.ChannelId == pullInfoSearchDto.ChannelId)
                .OrderBy(pullInfoSearchDto.Orderby).PageBy<PullInfo, PullInfoDto>(pullInfoSearchDto);
            return result;
        }

        public async Task InsertOrUpdate(PullInfoDto pullInfoDto)
        {
            var model = pullInfoDto.MapTo<PullInfo>();
            if (!model.IsNewData())
            {
                await _pullInfoRepository.UpdateAsync(model);
            }
            else
            {
                await _pullInfoRepository.InsertAsync(model);
            }
        }
    }
}
