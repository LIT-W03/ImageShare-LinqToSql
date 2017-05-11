using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageShare.Data;

namespace MvcApplication29.Models
{
    public class ShowImageViewModel
    {
        public Image Image { get; set; }
        public int Likes { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool HasUserLiked { get; set; }
    }
}