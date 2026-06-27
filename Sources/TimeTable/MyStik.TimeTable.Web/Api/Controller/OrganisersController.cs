using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.IO.Contracts;
using MyStik.TimeTable.Web.Api.Contracts;
using Org.BouncyCastle.Crypto.Prng;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace MyStik.TimeTable.Web.Api.Controller
{

    [RoutePrefix("api/v2/organisers")]
    public class OrganisersController : ApiBaseController
    {
        /// <summary>
        /// Liefert eine Liste aller Veranstalter
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        [ResponseType(typeof(List<OrganiserEntityApiContract>))]
        public IHttpActionResult GetOrganisers(string institution)
        {
            var orgs = Db.Organisers.Where(x =>
                    x.Institution.Tag.Equals(institution) &&
                    x.IsFaculty && !x.IsStudent).OrderBy(x => x.ShortName)
                .Include(activityOrganiser => activityOrganiser.Institution).ToList();

            var response = new List<OrganiserEntityApiContract>();
            foreach (var org in orgs)
            {
                response.Add(new OrganiserEntityApiContract
                {
                    Id = org.Id,
                    Title = org.Name,
                    Key = org.FullTag,
                    Context = new OrganiserContextApiContract
                    {
                        Institution = org.Institution.Tag,
                        Organiser = org.ShortName
                    }
                });
            }

            return Ok(response);
        }

        [Route("{key}/lecturers")]
        [HttpGet]
        [ResponseType(typeof(List<OrganiserLecturerApiContract>))]
        public IHttpActionResult GetLecturers(string key)
        {
            var words = key.Split('|');
            var institutionId = words.Length > 0 ? words[0] : string.Empty;
            var organiserId = words.Length > 1 ? words[1] : string.Empty;


            var org = Db.Organisers
                .Include(activityOrganiser => activityOrganiser.Members)
                .SingleOrDefault(x => x.Institution.Tag == institutionId && x.ShortName == organiserId);

            if (org == null)
                return NotFound();  

            var response = new List<OrganiserLecturerApiContract>();
            foreach (var member in org.Members)
            {
                response.Add(new OrganiserLecturerApiContract
                {
                    MemberKey = member.FullTag,
                    Name = member.PersonName
                });
            }

            return Ok(response);
        }

        [Route("{key}/rooms")]
        [HttpGet]
        [ResponseType(typeof(List<OrganiserRoomApiContract>))]
        public IHttpActionResult GetRooms(string key)
        {
            var words = key.Split('|');
            var institutionId = words.Length > 0 ? words[0] : string.Empty;
            var organiserId = words.Length > 1 ? words[1] : string.Empty;

            var org = Db.Organisers.Include(activityOrganiser =>
                    activityOrganiser.RoomAssignments.Select(roomAssignment => roomAssignment.Room))
                .SingleOrDefault(x => x.Institution.Tag == institutionId && x.ShortName == organiserId);

            if (org == null)
                return NotFound();

            var response = new List<OrganiserRoomApiContract>();
            foreach (var assignment in org.RoomAssignments)
            {
                response.Add(new OrganiserRoomApiContract
                {
                    RoomKey = assignment.Room.Number,
                    IsOwner = assignment.IsOwner
                });
            }

            return Ok(response);
        }


        [Route("{key}/curricula")]
        [HttpGet]
        [ResponseType(typeof(List<OrganiserCurriculumApiContract>))]
        public IHttpActionResult GetCurricula(string key)
        {
            var words = key.Split('|');
            var institutionId = words.Length > 0 ? words[0] : string.Empty;
            var organiserId = words.Length > 1 ? words[1] : string.Empty;

            var org = Db.Organisers.Include(activityOrganiser => activityOrganiser.Curricula)
                .SingleOrDefault(x => x.Institution.Tag == institutionId && x.ShortName == organiserId);

            if (org == null)
                return NotFound();

            var response = new List<OrganiserCurriculumApiContract>();
            foreach (var curr in org.Curricula.Where(x => !x.IsDeprecated).ToList())
            {
                response.Add(new OrganiserCurriculumApiContract
                {
                    CurriculumKey = curr.FullTag,
                    Name = curr.Title,
                    Alias = curr.Alias
                });
            }

            return Ok(response);
        }

        [Route("{key}/modules")]
        [HttpGet]
        [ResponseType(typeof(List<OrganiserModuleApiContract>))]
        public IHttpActionResult GetModules(string key)
        {
            var words = key.Split('|');
            var institutionId = words.Length > 0 ? words[0] : string.Empty;
            var organiserId = words.Length > 1 ? words[1] : string.Empty;


            var org = Db.Organisers.Include(activityOrganiser =>
                    activityOrganiser.ModuleCatalogs.Select(curriculumModuleCatalog => curriculumModuleCatalog.Modules))
                .SingleOrDefault(x => x.Institution.Tag == institutionId && x.ShortName == organiserId);

            if (org == null)
                return NotFound();

            var response = new List<OrganiserModuleApiContract>();
            foreach (var cat in org.ModuleCatalogs.OrderBy(x => x.Tag))
            {
                foreach (var module in cat.Modules)
                {
                    response.Add(new OrganiserModuleApiContract
                    {
                        ModuleKey= module.FullTag,
                        Title = module.Name,
                    });
                }
            }

            return Ok(response);
        }


        [Route("{key}/courses/{semester}")]
        [HttpGet]
        [ResponseType(typeof(List<OrganiserCourseApiContract>))]
        public IHttpActionResult GetCourses(string key, string semester)
        {
            var words = key.Split('|');
            var institutionId = words.Length > 0 ? words[0] : string.Empty;
            var organiserId = words.Length > 1 ? words[1] : string.Empty;

            var org = Db.Organisers
                .SingleOrDefault(x => x.Institution.Tag == institutionId && x.ShortName == organiserId);

            if (org == null)
                return NotFound();

            var sem = Db.Semesters.SingleOrDefault(x => x.Name == semester);

            if (sem == null)
                return NotFound();

            var response = new List<OrganiserCourseApiContract>();

            // Eigenes Angebot + Export
            var courses = Db.Activities.OfType<Course>()
                .Where(x =>
                    x.Semester.Id == sem.Id &&
                    x.Organiser.Id == org.Id)
                .ToList();

            // Import: das was andere FKs anbieten
            foreach (var curr in org.Curricula.Where(x => !x.IsDeprecated).OrderBy(x => x.Name).ToList())
            {
                foreach (var label in curr.LabelSet.ItemLabels.ToList())
                {
                    var labeledCourses = Db.Activities.OfType<Course>()
                        .Where(x =>
                            x.Organiser.Id != org.Id &&         // Angebote anderer FKs
                            x.Semester.Id == sem.Id &&
                            x.LabelSet != null &&
                            x.LabelSet.ItemLabels.Any(l => l.Id == label.Id)) // in eigenem Curriculum gelabelt
                        .ToList();

                    courses.AddRange(labeledCourses);
                }
            }

            courses = courses.Distinct().ToList();

            foreach (var course in courses)
            {
                response.Add(new OrganiserCourseApiContract
                {
                    CourseKey = course.FullTag,
                    Title = course.Name
                });
            }


            return Ok(response);
        }

    }
}