using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageShare.Data;

namespace MvcApplication29.Models
{
    public class UploadViewModel
    {
        public Image Image { get; set; }
        public string HostName { get; set; }
    }

}