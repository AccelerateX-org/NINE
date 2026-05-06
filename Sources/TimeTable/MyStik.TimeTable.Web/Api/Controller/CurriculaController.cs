using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using MyStik.TimeTable.DataServices.IO.Contracts;

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
        [ResponseType(typeof(List<CurriculumDto>))]

        public IHttpActionResult GetCurricula(string curriculum_id = "")
        {
            var list = new List<CurriculumDto>();

            if (string.IsNullOrEmpty(curriculum_id))
            {
                var curricula = Db.Curricula.Where(x => !x.IsDeprecated).ToList();
                foreach (var curriculum in curricula)
                {
                    var dto = new CurriculumDto
                    {
                        Title = curriculum.Name,
                        Curriculum_alias = curriculum.ShortName,
                        Curriculum_id = curriculum.ID,
                        Organiser_id = curriculum.Organiser?.ShortName
                    };
                    list.Add(dto);
                }
                return Ok(list);
            }
            else
            {
                var curriculum = Db.Curricula.FirstOrDefault(x => x.ShortName == curriculum_id);
                if (curriculum == null)
                    return NotFound();

                var dto = new CurriculumDto
                {
                    Title = curriculum.Name,
                    Curriculum_alias = curriculum.ShortName,
                    Curriculum_id = curriculum.ID,
                    Organiser_id = curriculum.Organiser?.ShortName  
                };

                list.Add(dto);

                return Ok(list);
            }
        }


        [HttpGet]
        [Route("{curriculum_id}/moduleplan")]
        [ResponseType(typeof(List<CurriculumSlotDto>))]

        public IHttpActionResult GetCurriculumPlan(string curriculum_id)
        {
            var list = new List<CurriculumSlotDto>();

            // TODO: split curriculum_id into tag and date
            var curr = Db.Curricula.FirstOrDefault(x => x.Tag == curriculum_id);
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


        [HttpGet]
        [Route("{curriculum_id}/cohortes")]
        [ResponseType(typeof(List<CourseApiCohortContract>))]

        public IHttpActionResult GetCohortes(string curriculum_id)
        {
            var list = new List<CourseApiCohortContract>();

            // TODO: split curriculum_id into tag and date
            var curr = Db.Curricula.FirstOrDefault(x => x.Tag == curriculum_id);
            if (curr == null)
                return NotFound();

            if (curr.LabelSet != null)
            {
                foreach (var label in curr.LabelSet.ItemLabels)
                {
                    var cohort = new CourseApiCohortContract
                    {
                        CurriculumId = curr.ID,
                        CurriculumAlias = curr.ShortName,
                        Label = label.Name,
                    };
                    list.Add(cohort);
                }
            }

            if (curr.Organiser.LabelSet != null)
            {
                foreach (var label in curr.Organiser.LabelSet.ItemLabels)
                {
                    var cohort = new CourseApiCohortContract
                    {
                        OrganiserId = curr.Organiser.ShortName,
                        Label = label.Name,
                    };
                    list.Add(cohort);
                }
            }

            if (curr.Organiser.Institution.LabelSet != null)
            {
                foreach (var label in curr.Organiser.Institution.LabelSet.ItemLabels)
                {
                    var cohort = new CourseApiCohortContract
                    {
                        InstitutionId = curr.Organiser.Institution.Tag,
                        Label = label.Name,
                    };
                    list.Add(cohort);
                }
            }


            return Ok(list);
        }
    }
}
