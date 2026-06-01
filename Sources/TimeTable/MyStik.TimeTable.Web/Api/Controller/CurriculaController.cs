using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(List<CurriculumApiContract>))]

        public IHttpActionResult GetCurricula(string institutionId, string organiserId = "", string curriculumId = "", string amendmentId = "")
        {
            var list = new List<CurriculumApiContract>();
            var curricula = Db.Curricula.Where(x =>
                x.Organiser.Institution.Tag.Equals(institutionId) &&
                (string.IsNullOrEmpty(organiserId) || x.Organiser.ShortName.Equals(organiserId)) &&
                (string.IsNullOrEmpty(curriculumId) || x.Tag.Equals(curriculumId)) &&
                !x.IsDeprecated).Include(curriculum => curriculum.Degree).Include(curriculum1 =>
                curriculum1.Organiser.Institution).ToList();

            foreach (var curriculum in curricula)
            {
                // TO-DO: nur die jeweils aktuelle version

                var dto = new CurriculumApiContract
                {
                    Id = curriculum.Id,
                    InstitutionId = curriculum.Organiser?.Institution?.Tag,
                    OrganiserId = curriculum.Organiser?.ShortName,
                    CurriculumId = curriculum.Tag,
                    AmendmentId = curriculum.StatuteTakeEffect?.ToString("yyyy-MM-dd"),
                    Alias = curriculum.ShortName,
                    Name = curriculum.Name,
                    Degree = curriculum.Degree?.Name,
                    Level = curriculum.Degree?.Level.ToString(),
                    CreditPoints = curriculum.EctsTarget,
                    Duration = 3.5, // curriculum.DurationTarget,
                    AsDual = curriculum.AsDual,
                    AsPartTime = curriculum.AsPartTime,
                    IsQualification = curriculum.IsQualification,
                    InSummerTerm = curriculum.InSummerTerm,
                    InWinterTerm = curriculum.InWinterTerm,
                    Language = "de" // curriculum.Language,
                };

                list.Add(dto);
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("{institutionId}/{organiserId}/{curriculumId}")]
        [ResponseType(typeof(List<CurriculumApiContract>))]

        public IHttpActionResult GetCurriculaVersions(string institutionId, string organiserId, string curriculumId)
        {
            var list = new List<CurriculumApiContract>();
            var curricula = Db.Curricula.Where(x =>
                x.Organiser.Institution.Tag.Equals(institutionId) &&
                (string.IsNullOrEmpty(organiserId) || x.Organiser.ShortName.Equals(organiserId)) &&
                (string.IsNullOrEmpty(curriculumId) || x.Tag.Equals(curriculumId)) &&
                !x.IsDeprecated).Include(curriculum => curriculum.Degree).Include(curriculum1 =>
                curriculum1.Organiser.Institution).ToList();

            foreach (var curriculum in curricula)
            {
                var dto = new CurriculumApiContract
                {
                    Id = curriculum.Id,
                    InstitutionId = curriculum.Organiser?.Institution?.Tag,
                    OrganiserId = curriculum.Organiser?.ShortName,
                    CurriculumId = curriculum.Tag,
                    AmendmentId = curriculum.StatuteTakeEffect?.ToString("yyyy-MM-dd"),
                    Alias = curriculum.ShortName,
                    Name = curriculum.Name,
                    Degree = curriculum.Degree?.Name,
                    Level = curriculum.Degree?.Level.ToString(),
                    CreditPoints = curriculum.EctsTarget,
                    Duration = 3.5, // curriculum.DurationTarget,
                    AsDual = curriculum.AsDual,
                    AsPartTime = curriculum.AsPartTime,
                    IsQualification = curriculum.IsQualification,
                    InSummerTerm = curriculum.InSummerTerm,
                    InWinterTerm = curriculum.InWinterTerm,
                    Language = "de" // curriculum.Language,
                };

                list.Add(dto);
            }

            return Ok(list);
        }


        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(CurriculumApiContract))]

        public IHttpActionResult GetCurriculum(Guid id)
        {
            var curriculum = Db.Curricula.Include(curriculum1 => curriculum1.Degree).Include(curriculum2 =>
                curriculum2.Organiser.Institution).SingleOrDefault(x => x.Id == id);

            if (curriculum == null)
            {
                return NotFound();
            }


            var dto = new CurriculumApiContract
            {
                Id = curriculum.Id,
                InstitutionId = curriculum.Organiser?.Institution?.Tag,
                OrganiserId = curriculum.Organiser?.ShortName,
                CurriculumId = curriculum.Tag,
                AmendmentId = curriculum.StatuteTakeEffect?.ToString("yyyy-MM-dd"),
                Alias = curriculum.ShortName,
                Name = curriculum.Name,
                Degree = curriculum.Degree?.Name,
                Level = curriculum.Degree?.Level.ToString(),
                CreditPoints = curriculum.EctsTarget,
                Duration = 3.5, // curriculum.DurationTarget,
                AsDual = curriculum.AsDual,
                AsPartTime = curriculum.AsPartTime,
                IsQualification = curriculum.IsQualification,
                InSummerTerm = curriculum.InSummerTerm,
                InWinterTerm = curriculum.InWinterTerm,
                Language = "de" // curriculum.Language,
            };

            return Ok(dto);
        }



        [HttpGet]
        [Route("{institutionId}/{organiserId}/{curriculumId}/{version}/modules")]
        [ResponseType(typeof(List<CurriculumSlotDto>))]

        public IHttpActionResult GetCurriculumPlan(string institutionId, string organiserId, string curriculumId, DateTime? version = null)
        {
            var list = new List<CurriculumSlotDto>();

            // TODO: split curriculum_id into tag and date
            var curr = Db.Curricula.FirstOrDefault(x => x.Tag == curriculumId);
            if (curr == null)
                return NotFound();

            var allSlots = Db.CurriculumSlots
                .Where(x => x.AreaOption != null &&
                            x.AreaOption.Area.Curriculum.Id == curr.Id).Include(curriculumSlot =>
                    curriculumSlot.SubjectAccreditations.Select(subjectAccreditation => subjectAccreditation.Subject)).ToList();

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
