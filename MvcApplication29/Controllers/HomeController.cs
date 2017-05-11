using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageShare.Data;
using MvcApplication29.Models;

namespace MvcApplication29.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var manager = new ImageShareManager(Properties.Settings.Default.ConStr);
            var viewModel = new IndexViewModel();
            viewModel.MostRecent = manager.GetFiveMostRecent();
            viewModel.MostPopular = manager.GetFiveMostPopular();
            viewModel.MostLikedImages = manager.GetFiveMostLikedImages();
            if (User.Identity.IsAuthenticated)
            {
                var userManager = new UserManager(Properties.Settings.Default.ConStr);
                viewModel.User = userManager.GetUser(User.Identity.Name);
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase image, string firstName, string lastName)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            image.SaveAs(Server.MapPath("~/Images/") + fileName);
            Image newImage = new Image
            {
                FirstName = firstName,
                LastName = lastName,
                ImageFile = fileName,
            };
            var manager = new ImageShareManager(Properties.Settings.Default.ConStr);
            manager.AddImage(newImage);
            var viewModel = new UploadViewModel();
            viewModel.Image = newImage;
            viewModel.HostName = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            return View(viewModel);
        }

        public ActionResult ShowImage(int id)
        {
            var manager = new ImageShareManager(Properties.Settings.Default.ConStr);
            manager.IncrementCount(id);
            var image = manager.GetImage(id);
            var viewModel = new ShowImageViewModel();
            viewModel.Image = image;
            viewModel.Likes = manager.GetLikesCount(id);
            viewModel.IsAuthenticated = User.Identity.IsAuthenticated;
            if (User.Identity.IsAuthenticated)
            {
                viewModel.HasUserLiked = manager.HasUserLiked(User.Identity.Name, id);
            }
            return View(viewModel);
        }

        public ActionResult GetCount(int id)
        {
            var manager = new ImageShareManager(Properties.Settings.Default.ConStr);
            Image image = manager.GetImage(id);
            return Json(new { viewCount = image.ViewCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public void LikeImage(int imageId)
        {
            var userManager = new UserManager(Properties.Settings.Default.ConStr);
            var user = userManager.GetUser(User.Identity.Name);
            var imageManager = new ImageShareManager(Properties.Settings.Default.ConStr);
            imageManager.AddImageLike(user.Id, imageId);
        }

        public ActionResult GetLikes(int imageId)
        {
            var manager = new ImageShareManager(Properties.Settings.Default.ConStr);
            return Json(new { likes = manager.GetLikesCount(imageId) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserLikes(string email)
        {
            var userManager = new UserManager(Properties.Settings.Default.ConStr);
            var viewModel = new UserLikesViewModel();
            viewModel.User = userManager.GetUser(email);
            if (viewModel.User != null)
            {
                var imageManager = new ImageShareManager(Properties.Settings.Default.ConStr);
                viewModel.LikedImages = imageManager.GetUserLikedImages(viewModel.User.Id);
            }
            return View(viewModel);
        }

    }
}
