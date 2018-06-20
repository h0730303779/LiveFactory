using System;
using System.Collections.Generic;
using System.Text;

namespace LiveFactory.Application.Base
{
    public class ResultDto
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public ResultDto(string error)
        {
            Message = error;
            Success = false;
        }
        public ResultDto()
        {

        }
    }

    public class ResultDto<T> : ResultDto
    {
        public T Data { get; set; }
        public ResultDto(string error) : base(error)
        {

        }
        public ResultDto() : base()
        {

        }
        public ResultDto(T data)
        {
            Data = data;
        }
    }
}
