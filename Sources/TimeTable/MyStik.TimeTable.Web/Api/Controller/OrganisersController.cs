using MyStik.TimeTable.Web.Api.Contracts;
using System;
using System.Collections.Generic;
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
        [ResponseType(typeof(List<OrganiserApiContract>))]
        public IHttpActionResult GetOrganisers()
        {
            var orgs = Db.Organisers.Where(x => x.IsFaculty && !x.IsStudent).OrderBy(x => x.ShortName).ToList();

            var response = new List<OrganiserApiContract>();
            foreach (var org in orgs)
            {
                response.Add(new OrganiserApiContract
                {
                    Name = org.Name,
                    Organiser_Id = org.ShortName
                });
            }

            return Ok(response);
        }

        [Route("{organiser_id}/lecturers")]
        [HttpGet]
        [ResponseType(typeof(List<OrganiserLecturerApiContract>))]
        public IHttpActionResult GetLecturers(string organiser_id, string pattern="")
        {
            var org = Db.Organisers.SingleOrDefault(x => x.ShortName == organiser_id);

            if (org == null)
                return NotFound();  

            var response = new List<OrganiserLecturerApiContract>();
            foreach (var member in org.Members)
            {
                response.Add(new OrganiserLecturerApiContract
                {
                    Lecturer_Id = member.ShortName,
                    Name = member.FullName
                });
            }

            return Ok(response);
        }

        [Route("{organiser_id}/curricula")]
        [HttpGet]
        [ResponseType(typeof(List<OrganiserCurriculumApiContract>))]
        public IHttpActionResult GetCurricula(string organiser_id, string pattern = "")
        {
            var org = Db.Organisers.SingleOrDefault(x => x.ShortName == organiser_id);

            if (org == null)
                return NotFound();

            var response = new List<OrganiserCurriculumApiContract>();
            foreach (var curr in org.Curricula.Where(x => x.IsDeprecated == false))
            {
                response.Add(new OrganiserCurriculumApiContract
                {
                    Curriculum_Id = curr.ShortName,
                    Name = curr.Name
                });
            }

            return Ok(response);
        }

        [Route("{organiser_id}/modules")]
        [HttpGet]
        [ResponseType(typeof(List<OrganiserModuleApiContract>))]
        public IHttpActionResult GetModules(string organiser_id, string pattern = "")
        {
            var org = Db.Organisers.SingleOrDefault(x => x.ShortName == organiser_id);

            if (org == null)
                return NotFound();

            var response = new List<OrganiserModuleApiContract>();
            foreach (var cat in org.ModuleCatalogs.OrderBy(x => x.Tag))
            {
                foreach (var module in cat.Modules)
                {
                    response.Add(new OrganiserModuleApiContract
                    {
                        Module_Id = $"{org.ShortName}#{cat.Tag}#{module.Tag}",
                        Name_de = module.Name,
                        Name_en = module.NameEn
                    });
                }
            }

            return Ok(response);
        }


    }
}