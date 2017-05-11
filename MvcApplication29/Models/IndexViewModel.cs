using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageShare.Data;

namespace MvcApplication29.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Image> MostRecent { get; set; }
        public IEnumerable<Image> MostPopular { get; set; }
        public IEnumerable<Image> MostLikedImages { get; set; } 
        public User User { get; set; }

    }
}