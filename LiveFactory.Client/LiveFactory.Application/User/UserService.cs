using JFJT.Framework.Application.Dto;
using JFJT.Framework.AutoMapper;
using JFJT.Framework.Domain.Repositories;
using JFJT.Framework.Extensions;
using LiveFactory.Application.Base;
using LiveFactory.Core;
using LiveFactory.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace LiveFactory.Application
{
    public interface IUserService
    {
        Task InsertOrUpdate(UserDto userDto);

        PageListResult<UserDto> GetUserPageList(UserSearchDto videoSearchDto);

        void DeleteById(Guid id);
        List<UserDto> GetUser(UserSearchDto liveUserSearchDto);
        List<UserDto> GetUser(Guid id);
        /// <summary>
        /// 登录
        /// </summary>
        ResultDto<UserDto> Login(UserDto param);
    }
    public class UserService : IUserService
    {
        IRepository<Users, Guid> _userRepository;
        public UserService(IRepository<Users, Guid> userRepository)
        {
            _userRepository = userRepository;
        }

        public PageListResult<UserDto> GetUserPageList(UserSearchDto userSearchDto)
        {
            var result = _userRepository.GetAll().WhereIf(!userSearchDto.KeyWord.IsNullOrWhiteSpace(), m => m.Account.Contains(userSearchDto.KeyWord))
                .OrderBy(userSearchDto.Orderby).PageBy<Users, UserDto>(userSearchDto);
            return result;
        }

        public async Task InsertOrUpdate(UserDto userDto)
        {
            var model = userDto.MapTo<Users>();
            if (!model.IsNewData())
            {
                await _userRepository.UpdateAsync(model);
            }
            else
            {
                var result = await _userRepository.InsertAsync(model);
            }
        }
        public void DeleteById(Guid id)
        {
            _userRepository.Delete(id);
        }
        public List<UserDto> GetUser(UserSearchDto userSearchDto)
        {
            var result = _userRepository.GetAll().WhereIf(!userSearchDto.KeyWord.IsNullOrWhiteSpace(), m => (m.Account.Contains(userSearchDto.KeyWord)))
                .ToList();
            var mapResult = result.MapTo<List<UserDto>>();
            return mapResult;
        }

        public List<UserDto> GetUser(Guid id)
        {
            var result = _userRepository.GetAll().WhereIf(id!=null, m => (m.Id==id))
                .ToList();
            var mapResult = result.MapTo<List<UserDto>>();
            return mapResult;
        }
        
        public ResultDto<UserDto> Login(UserDto param)
        {
            var user = _userRepository.GetAll().WhereIf(!param.Account.IsNullOrWhiteSpace(),b => b.Account == param.Account).FirstOrDefault();
            if (user == null)
                return new ResultDto<UserDto>("用户不存在");
            if (Crypto.VerifyHashedPassword(user.Password, param.Password)) {
                if (user.IsDisabled)
                    return new ResultDto<UserDto>("用户已失效");
                else {
                    param.Id = user.Id;
                    param.Password = user.Password;
                    InsertOrUpdate(param);
                    return new ResultDto<UserDto>(user.MapTo<UserDto>());
                }
            }
            else
                return new ResultDto<UserDto>("密码错误");
        }
    }
}
