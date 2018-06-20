using LiveFactory.Core.Models.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Qiniu.Common;
using Qiniu.Http;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.RS;
using Qiniu.RS.Model;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LiveFactory.Core
{
    public class QiniuManage
    {
        private readonly IOptions<AppSettings> _appSettings;
        public QiniuManage()
        {
        }
        public QiniuManage(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }
        //private readonly string AccessKey= _appSettings.Value.Qiniu.AccessKey;;
        public string AccessKey
        {
            get
            {
                return _appSettings.Value.Qiniu.AccessKey;
            }
        }
        public string SecretKey
        {
            get
            {
                return _appSettings.Value.Qiniu.SecretKey;
            }
        }
        public string SpaceName
        {
            get
            {
                return _appSettings.Value.Qiniu.SpaceName;
            }
        }
        /// <summary>
        /// 简单上传-上传字节数据
        /// </summary>
        public HttpResult UploadData(string fileName, string fileAddress)
        {
            // 生成(上传)凭证时需要使用此Mac
            // 这个示例单独使用了一个Settings类，其中包含AccessKey和SecretKey
            // 实际应用中，请自行设置您的AccessKey和SecretKey
            var mac = new Mac(AccessKey, SecretKey);
            var bucket = SpaceName;
            var saveKey = fileName;
            var data = File.ReadAllBytes(fileAddress);
            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy
            {
                // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
                Scope = bucket + ":" + saveKey
            };
            //putPolicy.Scope = bucket;
            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            // putPolicy.DeleteAfterDays = 1;
            // 生成上传凭证，参见
            // https://developer.qiniu.com/kodo/manual/upload-token            


            var jstr = putPolicy.ToJsonString();
            var token = Auth.CreateUploadToken(mac, jstr);

            ZoneID zoneId = ZoneID.CN_South;
            //[CN_East: 华东]
            //[CN_South: 华南]
            //[CN_North: 华北]
            //[US_North: 北美]
            var USE_HTTPS = true; //是否使用HTTPS
            Qiniu.Common.Config.SetZone(zoneId, USE_HTTPS);

            var fu = new FormUploader();
            var result = fu.UploadData(data, saveKey, token);
            return result;
            //Console.WriteLine(result);
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        public HttpResult DelFile(string fileName)
        {
            // 空间名
            string Bucket = SpaceName;
            // 文件名
            string Key = fileName;
            Mac mac = new Mac(AccessKey, SecretKey);
            BucketManager bucketManager = new BucketManager(mac);
            HttpResult deleteRet = bucketManager.Delete(Bucket, Key);
            return deleteRet;
            //if (deleteRet.Code != (int)HttpCode.OK)
            //{
            //    Console.WriteLine("delete error: " + deleteRet.ToString());
            //}
        }

        public HttpResult GetInfo(string fileName)
        {
            Mac mac = new Mac(AccessKey, SecretKey);
            string bucket = SpaceName;
            string key = fileName;
            BucketManager bm = new BucketManager(mac);
            StatResult result = bm.Stat(bucket, key);
            return result;
        }
    }
}