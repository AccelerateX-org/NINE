using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MyStik.TimeTable.Web.Api.DTOs;

namespace MyStik.TimeTable.Web.Api.Controller
{

    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/v2/modules")]
    public class ModulesController : ApiBaseController
    {
        /// <summary>
        /// Liste aller Studiengänge
        /// </summary>
        [Route("")]
        public IQueryable<NamedDto> GetCurricula()
        {
            // Alle Studiengänge mit Packages
            // Da besteht die Vermutung, dass es auch Module gibt
            var currs = Db.Curricula.Where(x => x.Packages.Any())
                .Select(x => new NamedDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    ShortName = x.ShortName
                }).AsQueryable();

            return currs;
        }

        /// <summary>
        /// Liste aller Versionen eines Studiengangs
        /// </summary>
        [Route("{name}")]
        public IQueryable<NamedDto> GetCurriculumVersions(string name)
        {
            return new List<NamedDto>().AsQueryable();
        }

        /// <summary>
        /// Liste aller Semester einer Studiengangsversion jeweils mit
        /// Angabe aller Module
        /// </summary>
        [Route("{name}/{version}")]
        public IQueryable<NamedDto> GetCurriculumModules(string name, string version, string search)
        {
            // Alle Semester
            // Pro Semester alle Module
            // pro Module dann weitere Informationen
            var curr = Db.Curricula.FirstOrDefault(x => x.ShortName.Equals(name));






            return new List<NamedDto>().AsQueryable();
        }


    }
}
