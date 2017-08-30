using System;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class StorageController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetRessource(Guid id)
        {
            var db = new TimeTableDbContext();

            var file = db.Storages.SingleOrDefault(img => img.Id == id);

            return File(file.BinaryData, file.FileType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetDocument(Guid id)
        {
            var db = new TimeTableDbContext();

            var file = db.Storages.SingleOrDefault(img => img.Id == id);

            return File(file.BinaryData, file.FileType, file.Name);
        }
    }
}