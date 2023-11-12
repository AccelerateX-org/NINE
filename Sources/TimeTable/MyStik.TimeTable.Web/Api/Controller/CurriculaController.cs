using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MyStik.TimeTable.Web.Api.DTOs;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// Abfrage von Studiengängen
    /// </summary>
    [RoutePrefix("api/v2/curricula")]
    public class CurriculaController : ApiBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        [Route("")]
        public IQueryable<CurriculumDto> GetCurricula(string org = "")
        {
            var curriculaWithPlan =
                string.IsNullOrEmpty(org)
                    ? Db.Curricula.Where(x => x.Areas.Any()) .ToList()
                    : Db.Curricula.Where(x => x.Areas.Any() && x.Organiser.ShortName.ToLower().Equals(org.ToLower())).ToList();

            var result = new List<CurriculumDto>();

            foreach (var curriculum in curriculaWithPlan)
            {
                var curr = new CurriculumDto
                {
                    Id = curriculum.Id,
                    Name = curriculum.Name,
                    ShortName = curriculum.ShortName,
                    
                    Organiser = new OrganiserDto
                    {
                        Name = curriculum.Organiser.Name,
                        ShortName = curriculum.Organiser.ShortName,
                        Color = curriculum.Organiser.HtmlColor,
                        Id = curriculum.Organiser.Id
                    }
                };

                result.Add(curr);
            }

            return result.AsQueryable();
        }

        /*
        [Route("{name}")]
        public NamedDto GetCurriculumInfo(string name)
        {
            return new NamedDto();
        }


        [Route("{name}/versions")]
        public IQueryable<NamedDto> GetCurriculumVersions(string name)
        {
            return new List<NamedDto>().AsQueryable();
        }

        [Route("{name}/{version}/modules/{semester}")]
        public IQueryable<CurriculumSchemeSemesterDto> GetCurriculumPlan(string name, string version, string semester)
        {
            var list = new List<CurriculumSchemeSemesterDto>();

            // semester und Version werden noch nicht ausgewertet
            var curr = Db.Curricula.SingleOrDefault(x => x.ShortName.ToUpper().Equals(name.Trim().ToUpper()));


            if (curr == null)
                return list.AsQueryable();

            var accreditions =
                Db.Accreditations.Where(x => x.Slot.CurriculumSection.Curriculum.Id == curr.Id)
                    .Distinct()
                    .OrderBy(x => x.Module.ShortName).ToList();


            foreach (var ac in accreditions)
            {
                var semDto = list.FirstOrDefault(x => x.Term == ac.Slot.CurriculumSection.Order);

                if (semDto == null)
                {
                    semDto = new CurriculumSchemeSemesterDto
                    {
                        Term = ac.Slot.CurriculumSection.Order
                    };

                    list.Add(semDto);
                }


                var moduleDto = new CurriculumSchemeModuleDto()
                {
                    Id = ac.Module.Id,
                    Name = ac.Module.Name,
                    Ects = ac.Slot.ECTS,
                };

                semDto.Modules.Add(moduleDto);
            }

            return list.AsQueryable();
        }
        */


        [Route("{id}/slots")]
        public IQueryable<CurriculumSlotDto> GetCurriculumPlan(Guid id)
        {
            var list = new List<CurriculumSlotDto>();

            var curr = Db.Curricula.FirstOrDefault(x => x.Id == id);
            if (curr == null)
                return list.AsQueryable();

            var allSlots = Db.CurriculumSlots
                .Where(x => x.AreaOption != null &&
                            x.AreaOption.Area.Curriculum.Id == curr.Id).ToList();

            foreach (var slot in allSlots)
            {
                var slotDto = new CurriculumSlotDto
                {
                    id = slot.Id,
                    semester = slot.Semester,
                    ects = slot.ECTS,
                    name = slot.Name,
                    tag = slot.FullTag,
                    modules = new List<ModuleDto>()
                };

                foreach (var accreditation in slot.ModuleAccreditations)
                {
                    var moduleDto = new ModuleDto
                    {
                        Id = accreditation.Module.Id,
                        Name = accreditation.Module.Name,
                        Tag = accreditation.Module.FullTag
                    };

                    slotDto.modules.Add(moduleDto);
                }

                list.Add(slotDto);
            }

            return list.AsQueryable();
        }
    }
}
