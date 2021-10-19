using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpConnectServer.Models
{
    public class ImageData
    {
        public IFormFile image { get; set; }
    }
}
