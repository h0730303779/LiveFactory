using JFJT.Framework.Application.Dto;
using JFJT.Framework.AutoMapper;
using JFJT.Framework.Domain.Repositories;
using JFJT.Framework.Domain.Uow;
using JFJT.Framework.Extensions;
using LiveFactory.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace LiveFactory.Application
{
    public interface IVideoService
    {
        Task InsertOrUpdate(VideoDto videoDto);

        PageListResult<VideoDto> GetVideoPageList(VideoSearchDto videoSearchDto);

        void DeleteById(Guid id);
        List<VideoDto> GetVideo(VideoSearchDto liveChannelSearchDto);
        Task<VideoDto> GetVideo(Guid id);
        string GetDefaultPlayUrl();
    }
    public class VideoService : IVideoService
    {
        IRepository<Video, Guid> _videoRepository;
        public VideoService(IRepository<Video, Guid> videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public PageListResult<VideoDto> GetVideoPageList(VideoSearchDto videoSearchDto)
        {
            var result = _videoRepository.GetAll().WhereIf(!videoSearchDto.KeyWord.IsNullOrWhiteSpace(), m => m.Name == videoSearchDto.KeyWord)
                .OrderBy(videoSearchDto.Orderby).PageBy<Video, VideoDto>(videoSearchDto);
            return result;
        }

        public async Task InsertOrUpdate(VideoDto videoDto)
        {
            var model = videoDto.MapTo<Video>();
            if (!model.IsNewData())
            {
                await _videoRepository.UpdateAsync(model);
            }
            else
            {
                var result = await _videoRepository.InsertAsync(model);
            }
        }
        public void DeleteById(Guid id)
        {
            _videoRepository.Delete(id);
        }
        public List<VideoDto> GetVideo(VideoSearchDto videoSearchDto)
        {
            var result = _videoRepository.GetAll().WhereIf(!videoSearchDto.KeyWord.IsNullOrWhiteSpace(), m => (m.Name.Contains(videoSearchDto.KeyWord) || m.Url.Contains(videoSearchDto.KeyWord)))
                .ToList();
            var mapResult = result.MapTo<List<VideoDto>>();
            return mapResult;
        }

        public async Task<VideoDto> GetVideo(Guid id)
        {
            var result = await _videoRepository.GetAsync(id);
            return result.MapTo<VideoDto>();
        }
        
        public string GetDefaultPlayUrl()
        {
            string defaultUrl = "";
            var result = _videoRepository.GetAll().FirstOrDefault(n => n.IsDefaultPlay);
            if (result != null && result.Url != null && result.Url.Length > 0)
            {
                defaultUrl = result.Name;
            }
            return defaultUrl;
        }
    }
}
