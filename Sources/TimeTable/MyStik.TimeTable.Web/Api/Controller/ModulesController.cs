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
        [Route("")]
        public IQueryable<NamedDto> GetModules()
        {
            return new List<NamedDto>().AsQueryable();
        }

        [Route("{id}")]
        [ResponseType(typeof(AccreditatedModuleDto))]
        public async Task<IHttpActionResult> GetModule(Guid id)
        {
            if (!(await Db.CertificateModules.FindAsync(id) is CertificateModule module))
            {
                return NotFound();
            }

            // unvollständige Modulbeschreibungen werden gelten als nicht gefunden
            if (module.Subjects.Any(x => x.ContentModules.Any(y => y.TeachingBuildingBlock == null)))
            {
                return NotFound();
            }

            var converter = new ModuleConverter(Db);
            var dto = converter.ConvertCertificateModule(id);

            return Ok(dto);
        }


    }
}
