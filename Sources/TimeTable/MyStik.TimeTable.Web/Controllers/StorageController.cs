using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{
    public class StorageController : Controller
    {
        //
        // GET: /Storage/
        public ActionResult GetRessource(Guid id)
        {
            var db = new TimeTableDbContext();

            var file = db.Storages.SingleOrDefault(img => img.Id == id);

            return File(file.BinaryData, file.FileType);
        }

        public ActionResult GetDocument(Guid id)
        {
            var db = new TimeTableDbContext();

            var file = db.Storages.SingleOrDefault(img => img.Id == id);

            return File(file.BinaryData, file.FileType, file.Name);
        }
    }
}