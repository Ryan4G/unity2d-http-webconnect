using HttpConnectServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpConnectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
    }
}
