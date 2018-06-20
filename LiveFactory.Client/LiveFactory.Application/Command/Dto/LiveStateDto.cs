using LiveCommand.Common;
using System;
using System.Collections.Generic;
using System.Text;
using JFJT.Framework.Extensions;

namespace LiveFactory.Application.Command.Dto
{
    public class LiveStateDto
    {
        public Guid Id { get; set; }
        public LiveStateType State { get; set; }

        public string StateName
        {
            get
            {
                return State.ToEnumDesc();
            }
        }
    }
}
