using System.ComponentModel;

namespace LiveFactory.Core.Enums
{
    /// <summary>
    /// 播放类型
    /// </summary>
    public enum PlayType
    {
        [Description("video/mp4")]
        HttpMp4,
        [Description("video/x-flv")]
        HttpFlv,
        [Description("rtmp/flv")]
        RtmpFlv,
        [Description("application/x-mpegURL")]
        HttpHls
    }
}
