using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageShare.Data
{
    public class ImageWithLikes
    {
        public Image Image { get; set; }
        public int LikesCount { get; set; }
    }
}
