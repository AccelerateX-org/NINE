using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class CampusRoom
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int capacity { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class CampusController : ApiBaseController
    {

        /// <summary>
        /// 
        /// </summary>
        public IQueryable<CampusRoom> Get()
        {
            var org = Db.Rooms.Where(x => x.Number.StartsWith("R")).ToList();
            var fac = new List<CampusRoom>();
            foreach (var organiser in org)
            {
                var f = new CampusRoom
                {
                    id = organiser.Id.ToString(),
                    name = organiser.Name,
                    number = organiser.Number,
                    capacity = organiser.Capacity
                };

                fac.Add(f);
            }
            return fac.AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        [ResponseType(typeof(CampusRoom))]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            Room organiser = await Db.Rooms.FindAsync(id);
            if (organiser == null)
            {
                return NotFound();
            }

            var f = new CampusRoom()
            {
                id = organiser.Id.ToString(),
                name = organiser.Name,
                number = organiser.Number,
                capacity = organiser.Capacity
            };

            return Ok(f);
        }




        /// <summary>
        /// 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ActivityOrganiserExists(Guid id)
        {
            return Db.Organisers.Count(e => e.Id == id) > 0;
        }
    }
}