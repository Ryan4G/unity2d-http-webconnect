using HttpConnectServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HttpConnectServer.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class StorageDataController : ControllerBase
    {
        private readonly ILogger<StorageDataController> _logger;

        public StorageDataController(ILogger<StorageDataController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string GetData(string name)
        {
            var result = $"Hello, {name}";

            return result;
        }

        [HttpPost]
        public string PostData([FromForm]GameData data)
        {
            var result = "";

            if (data.Name == "admin" && data.Password == "123")
            {
                result = $"Welcome {data.Name}, login successfuly!";
            }
            else
            {
                result = $"Opppps! {data.Name} might check your password!";
            }

            return result;
        }

        [HttpPost]
        public ActionResult RequestImage([FromForm]IFormFile image)
        {
            var now = DateTime.Now;
            
            //文件存储路径
            var filePath = string.Format("/Uploads/{0}/{1}/{2}/", now.ToString("yyyy"), now.ToString("yyyyMM"), now.ToString("yyyyMMdd"));
            
            //获取当前web目录
            var webRootPath = System.Environment.CurrentDirectory;
           
            if (!Directory.Exists(webRootPath + filePath))
            {
                Directory.CreateDirectory(webRootPath + filePath);
            }

            try
            {
                if (image != null)
                {
                    #region  图片文件的条件判断
                    //文件后缀
                    var fileExtension = Path.GetExtension(image.FileName);

                    //判断后缀是否是图片
                    const string fileFilt = ".gif|.jpg|.jpeg|.png";
                    if (fileExtension == null)
                    {
                        //return Error("上传的文件没有后缀");
                        throw new Exception("Upload File Type Missing");
                    }

                    if (fileFilt.IndexOf(fileExtension.ToLower(), StringComparison.Ordinal) <= -1)
                    {
                        //return Error("请上传jpg、png、gif格式的图片");
                        throw new Exception("Upload File Type Uncorrect");
                    }

                    //判断文件大小    
                    long length = image.Length;
                    if (length > 1024 * 1024 * 2) //2M
                    {
                        //return Error("上传的文件不能大于2M");
                        throw new Exception("Upload File Over Size");
                    }

                    #endregion

                    var strDateTime = DateTime.Now.ToString("yyMMddhhmmssfff"); //取得时间字符串
                    var strRan = Convert.ToString(new Random().Next(100, 999)); //生成三位随机数
                    var saveName = strDateTime + strRan + fileExtension;

                    //插入图片数据                 
                    using (FileStream fs = System.IO.File.Create(webRootPath + filePath + saveName))
                    {
                        image.CopyTo(fs);
                        fs.Flush();
                    }

                    using (Stream stream = image.OpenReadStream())
                    {
                        byte[] bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        stream.Seek(0, SeekOrigin.Begin);

                        return File(bytes, image.ContentType);
                    }
                }
            }
            catch (Exception ex)
            {
                //这边增加日志，记录错误的原因
                //ex.ToString();
            }

            throw new Exception("Upload Fail");
        }

        [HttpGet]
        public ActionResult GetSounds()
        {
            var soundUrl = @$"{System.Environment.CurrentDirectory}/Uploads/Sounds/Chest Creak.wav";

            try
            {
                var bytes = System.IO.File.ReadAllBytes(soundUrl);

                return File(bytes, "audio/x-wav");
            }
            catch (Exception ex)
            {
                //这边增加日志，记录错误的原因
                //ex.ToString();
            }

            throw new Exception("Download Fail");
        }
    }
}
