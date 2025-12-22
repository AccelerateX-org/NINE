using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

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
        [Route("{curriculum_id}")]
        [ResponseType(typeof(CurriculumDto))]

        public IHttpActionResult GetCurricula(string curriculum_id)
        {
            var curriculum = Db.Curricula.FirstOrDefault(x => x.ShortName == curriculum_id);
            if (curriculum == null)
                return NotFound();

            var dto = new CurriculumDto
            {
                Name = curriculum.Name,
                Curriculum_id = curriculum.ShortName,
            };

            return Ok(dto);
        }


        [HttpGet]
        [Route("{curriculum_id}/moduleplan")]
        [ResponseType(typeof(List<CurriculumSlotDto>))]

        public IHttpActionResult GetCurriculumPlan(string curriculum_id)
        {
            var list = new List<CurriculumSlotDto>();

            var curr = Db.Curricula.FirstOrDefault(x => x.ShortName == curriculum_id);
            if (curr == null)
                return NotFound();

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

                foreach (var accreditation in slot.SubjectAccreditations)
                {
                    var moduleDto = new ModuleDto
                    {
                        Id = accreditation.Subject.Id,
                        Name = accreditation.Subject.Name,
                        Tag = accreditation.Subject.Tag
                    };

                    slotDto.modules.Add(moduleDto);
                }

                list.Add(slotDto);
            }

            return Ok(list);
        }
    }
}
