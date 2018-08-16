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
    public class Faculty
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string shortname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Curriculum> curricula { get; set; }
            
    }

    /// <summary>
    /// 
    /// </summary>
    public class Curriculum
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string shortname { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class FacultyController : ApiBaseController
    {

        // GET: api/Faculty
        /// <summary>
        /// 
        /// </summary>
        public IQueryable<Faculty> Get()
        {
            var org = Db.Organisers.Where(x => x.IsFaculty).ToList();
            var fac = new List<Faculty>();
            foreach (var organiser in org)
            {
                var f = new Faculty
                {
                    id = organiser.Id.ToString(),
                    name = organiser.Name,
                    shortname = organiser.ShortName,
                    curricula = new List<Curriculum>()
                };

                foreach (var curriculum in organiser.Curricula)
                {
                    var c = new Curriculum
                    {
                        id = curriculum.Id.ToString(),
                        name = curriculum.Name,
                        shortname = curriculum.ShortName
                    };

                    f.curricula.Add(c);
                }

                fac.Add(f);
            }
            return fac.AsQueryable();
        }


        // GET: api/Faculty/5
        /// <summary>
        /// 
        /// </summary>
        [ResponseType(typeof(Faculty))]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            ActivityOrganiser organiser = await Db.Organisers.FindAsync(id);
            if (organiser == null)
            {
                return NotFound();
            }

            var f = new Faculty
            {
                id = organiser.Id.ToString(),
                name = organiser.Name,
                shortname = organiser.ShortName,
                curricula = new List<Curriculum>()
            };

            foreach (var curriculum in organiser.Curricula)
            {
                var c = new Curriculum
                {
                    id = curriculum.Id.ToString(),
                    name = curriculum.Name,
                    shortname = curriculum.ShortName
                };

                f.curricula.Add(c);
            }

            return Ok(f);
        }

        /*
        // PUT: api/Faculty/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutActivityOrganiser(Guid id, ActivityOrganiser activityOrganiser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != activityOrganiser.Id)
            {
                return BadRequest();
            }

            Db.Entry(activityOrganiser).State = EntityState.Modified;

            try
            {
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityOrganiserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Faculty
        [ResponseType(typeof(ActivityOrganiser))]
        public async Task<IHttpActionResult> PostActivityOrganiser(ActivityOrganiser activityOrganiser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Db.Organisers.Add(activityOrganiser);
            await Db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = activityOrganiser.Id }, activityOrganiser);
        }

        // DELETE: api/Faculty/5
        [ResponseType(typeof(ActivityOrganiser))]
        public async Task<IHttpActionResult> DeleteActivityOrganiser(Guid id)
        {
            ActivityOrganiser activityOrganiser = await Db.Organisers.FindAsync(id);
            if (activityOrganiser == null)
            {
                return NotFound();
            }

            Db.Organisers.Remove(activityOrganiser);
            await Db.SaveChangesAsync();

            return Ok(activityOrganiser);
        }
        */

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