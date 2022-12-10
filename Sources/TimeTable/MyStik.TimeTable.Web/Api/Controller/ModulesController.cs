using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{

    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/v2/modules")]
    public class ModulesController : ApiBaseController
    {
        /// <summary>
        /// Suche nache Modulesn
        /// </summary>
        [Route("{id}/summary")]
        public ModuleDto GetModuleSummary(Guid id)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);
            if (module == null)
                return null;

            var model = new ModuleDto
            {
                Name = module.Name,
                ShortName = module.ShortName,
                Ects = module.Accreditations.First().Slot.ECTS,
            };

            //module.ModuleResponsibilities

            return model;
        }

        [Route("{id}/details/{semester}")]
        public IQueryable<NamedDto> GetModuleDetails(Guid id)
        {
            return new List<NamedDto>().AsQueryable();
        }

        [Route("{id}/courses/{semester}")]
        public IQueryable<NamedDto> GetModuleCourses(Guid id)
        {
            return new List<NamedDto>().AsQueryable();
        }


        /*
        [Route("{id}")]
        [ResponseType(typeof(AccreditatedModuleDto))]
        public async Task<IHttpActionResult> GetModule(Guid id)
        {
            if (!(await Db.CertificateModules.FindAsync(id) is CertificateModule module))
            {
                return NotFound();
            }

            var converter = new ModuleConverter(Db);
            var dto = converter.ConvertCertificateModule(id);

            return Ok(dto);
        }
        */

    }
}
