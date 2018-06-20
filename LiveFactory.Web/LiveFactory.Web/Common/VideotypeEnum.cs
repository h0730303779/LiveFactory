using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LiveFactory.Web.Common
{
    public class VideoType : TagHelper
    {
        public Enum Value { get; set; }

        public enum PlayerType
        {
            [Description("video/mp4")]
            mp4 = 1,
            [Description("video/x-flv")]
            x_flv = 2,
            [Description("rtmp/flv")]
            flv2 = 3,
            [Description("application/x-mpegURL")]
            mpegURL = 4
        }
        public static string description(Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }

        //public List<SelectListItem> GetEnumSelectListItem()
        //{
        //    var list = new List<SelectListItem>();
        //    var typeInfo = Value.GetType().GetTypeInfo();
        //    var enumValues = typeInfo.GetEnumValues();

        //    foreach (var value in enumValues)
        //    {

        //        MemberInfo memberInfo =
        //          typeInfo.GetMember(value.ToString()).First();

        //        var descriptionAttribute =
        //          memberInfo.GetCustomAttribute<DescriptionAttribute>();

        //        list.Add(new SelectListItem()
        //        {
        //            Text = descriptionAttribute.Description,
        //            Value = value.ToString()
        //        });
        //    }

        //    return list;
        //}

        //public override void Process(TagHelperContext context, TagHelperOutput output)
        //{
        //    var list = GetEnumSelectListItem();

        //    output.Content.AppendHtml("<select>");
        //    foreach (var item in list)
        //    {
        //        if (item.Value != null)
        //            output.Content.AppendHtml($"<option value='{item.Value}'>{item.Text}</option>");
        //        else
        //            output.Content.AppendHtml($"<option>{item.Text}</option>");
        //    }
        //    output.Content.AppendHtml("<select/>");
        //}
    }
}
