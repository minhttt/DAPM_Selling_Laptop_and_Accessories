using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DoAn_LapTrinhWeb.Models
{
    public class GGAuth
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Picture
        {
            get; set;
        }
        
    }
}