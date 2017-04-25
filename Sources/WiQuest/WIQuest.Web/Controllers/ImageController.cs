using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WIQuest.Web.Data;

namespace WIQuest.Web.Controllers
{
    public class ImageController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: Image
        public ActionResult ShowImage(Guid? id)
        {
            var image = db.BinaryStorages.SingleOrDefault(img => img.Id == id);

            if (image != null)
            {
                return File(image.ImageData, image.ImageFileType);
            }

            return File("~/images/48px-Face-sad_svg.png", "image/png");
        }

    }
}