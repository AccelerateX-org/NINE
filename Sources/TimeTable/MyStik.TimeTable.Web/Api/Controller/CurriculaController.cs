using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.IO.Contracts;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(List<CurriculumEntityApiContract>))]

        public IHttpActionResult GetCurricula(string institution, string organiser = "", string program = "", string amendment = "")
        {
            var list = new List<CurriculumEntityApiContract>();
            var curricula = Db.Curricula.Where(x =>
                x.Organiser.Institution.Tag.Equals(institution) &&
                (string.IsNullOrEmpty(organiser) || x.Organiser.ShortName.Equals(organiser)) &&
                (string.IsNullOrEmpty(program) || x.Tag.Equals(program)) &&
                !x.IsDeprecated).Include(curriculum => curriculum.Degree).Include(curriculum1 =>
                curriculum1.Organiser.Institution).ToList();

            foreach (var curriculum in curricula)
            {
                var am = curriculum.StatuteTakeEffect?.ToString("yyyy-MM-dd") ?? string.Empty;

                if (!string.IsNullOrEmpty(am) || !string.IsNullOrEmpty(amendment) || am.Equals(amendment)) 
                    continue;
                
                var dto = new CurriculumEntityApiContract
                {
                    Id = curriculum.Id,
                    Key = curriculum.FullTag,
                    Context = new CurriculumContextApiContract
                    {
                        Institution = curriculum.Organiser.Institution.Tag,
                        Organiser = curriculum.Organiser.ShortName,
                        Program = curriculum.Tag,
                        Amendment = curriculum.StatuteTakeEffect?.ToString("yyyy-MM-dd")
                    }
                };

                list.Add(dto);
            }

            return Ok(list);
        }


        [HttpGet]
        [Route("{key}/structure")]
        [ResponseType(typeof(List<CurriculumDetailsApiContract>))]

        public IHttpActionResult GetCurriculumStructure(string key)
        {
            var words = key.Split('|');
            var institution = words.Length > 0 ? words[0] : string.Empty;
            var organiser = words.Length > 1 ? words[1] : string.Empty;
            var program = words.Length > 2 ? words[2] : string.Empty;
            var amendment = words.Length > 3 ? words[3] : string.Empty;

            var curricula = Db.Curricula.Where(x =>
                x.Organiser.Institution.Tag.Equals(institution) &&
                (string.IsNullOrEmpty(organiser) || x.Organiser.ShortName.Equals(organiser)) &&
                (string.IsNullOrEmpty(program) || x.Tag.Equals(program)) &&
                !x.IsDeprecated).Include(curriculum => curriculum.Degree).Include(curriculum1 =>
                curriculum1.Organiser.Institution).ToList();


            var list = new List<CurriculumDetailsApiContract>();

            foreach (var curriculum in curricula)
            {
                var am = curriculum.StatuteTakeEffect?.ToString("yyyy-MM-dd") ?? string.Empty;

                if (!string.IsNullOrEmpty(am) || !string.IsNullOrEmpty(amendment) || am.Equals(amendment))
                    continue;

                var context = new CurriculumContextApiContract
                {
                    Institution = curriculum.Organiser.Institution.Tag,
                    Organiser = curriculum.Organiser.ShortName,
                    Program = curriculum.Tag,
                    Amendment = curriculum.StatuteTakeEffect?.ToString("yyyy-MM-dd")
                };


                var dto = TransformCurriculum(curriculum, context);

                list.Add(dto);
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("{key}/cohorts")]
        [ResponseType(typeof(List<CurriculumDetailsApiContract>))]

        public IHttpActionResult GetCurriculumCohorts(string key)
        {
            var words = key.Split('|');
            var institution = words.Length > 0 ? words[0] : string.Empty;
            var organiser = words.Length > 1 ? words[1] : string.Empty;
            var program = words.Length > 2 ? words[2] : string.Empty;
            var amendment = words.Length > 3 ? words[3] : string.Empty;

            var curricula = Db.Curricula.Where(x =>
                x.Organiser.Institution.Tag.Equals(institution) &&
                (string.IsNullOrEmpty(organiser) || x.Organiser.ShortName.Equals(organiser)) &&
                (string.IsNullOrEmpty(program) || x.Tag.Equals(program)) &&
                !x.IsDeprecated).Include(curriculum => curriculum.Degree).Include(curriculum1 =>
                curriculum1.Organiser.Institution).ToList();


            var list = new List<CurriculumDetailsApiContract>();

            foreach (var curriculum in curricula)
            {
                var am = curriculum.StatuteTakeEffect?.ToString("yyyy-MM-dd") ?? string.Empty;

                if (!string.IsNullOrEmpty(am) || !string.IsNullOrEmpty(amendment) || am.Equals(amendment))
                    continue;

                var context = new CurriculumContextApiContract
                {
                    Institution = curriculum.Organiser.Institution.Tag,
                    Organiser = curriculum.Organiser.ShortName,
                    Program = curriculum.Tag,
                    Amendment = curriculum.StatuteTakeEffect?.ToString("yyyy-MM-dd")
                };


                var dto = TransformCurriculum(curriculum, context);

                list.Add(dto);
            }

            return Ok(list);
        }


        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(CurriculumDetailsApiContract))]

        public IHttpActionResult GetCurriculumById(Guid id)
        {
            var curriculum = Db.Curricula.Include(curriculum1 => curriculum1.Degree).Include(curriculum2 =>
                curriculum2.Organiser.Institution).SingleOrDefault(x => x.Id == id);

            if (curriculum == null)
            {
                return NotFound();
            }

            var context = new CurriculumContextApiContract
            {
                Institution = curriculum.Organiser.Institution.Tag,
                Organiser = curriculum.Organiser.ShortName,
                Program = curriculum.Tag,
                Amendment = curriculum.StatuteTakeEffect?.ToString("yyyy-MM-dd")
            };

            var dto = TransformCurriculum(curriculum, context);

            return Ok(dto);
        }


        [HttpGet]
        [Route("{key}/alternatives/{slot}")]
        [ResponseType(typeof(CurriculumUnitDetailsApiContract))]

        public IHttpActionResult GetCurriculumUnitById(Guid curriculumId, Guid unitId)
        {
            var curriculum = Db.Curricula.Include(curriculum1 => curriculum1.Degree).Include(curriculum2 =>
                curriculum2.Organiser.Institution).SingleOrDefault(x => x.Id == curriculumId);

            if (curriculum == null)
            {
                return NotFound();
            }

            var slot = Db.CurriculumSlots.Include(curriculumSlot =>
                    curriculumSlot.SubjectAccreditations.Select(subjectAccreditation => subjectAccreditation.Subject))
                .SingleOrDefault(x => x.Id == unitId && x.AreaOption.Area.Curriculum.Id == curriculumId);

            if (slot == null)
            {
                return NotFound();
            }

            var dto = new CurriculumUnitDetailsApiContract
            {
                Id = slot.Id,
                Key = slot.FullTag,
                Title = slot.Name,
                Description = slot.Description,
                Alternatives = new List<CurriculumChipApiContract>()
            };

            // old
            foreach (var accr in slot.SubjectAccreditations)
            {
                var chip = new CurriculumChipApiContract
                {
                    CreditPoints = slot.ECTS,
                    ChipKey = slot.FullTag,
                    SubjectKey = accr.Subject.FullTag
                };

                dto.Alternatives.Add(chip);
            }


            return Ok(dto);
        }

        [HttpGet]
        [Route("{key}/instances/{semester}")]
        [ResponseType(typeof(List<CurriculumInstanceApiContract>))]

        public IHttpActionResult GetCurriculumInstance(string key, string semester)
        {
            var words = key.Split('|');
            var institution = words.Length > 0 ? words[0] : string.Empty;
            var organiser = words.Length > 1 ? words[1] : string.Empty;
            var program = words.Length > 2 ? words[2] : string.Empty;
            var amendment = words.Length > 3 ? words[3] : string.Empty;

            var curricula = Db.Curricula.Where(x =>
                x.Organiser.Institution.Tag.Equals(institution) &&
                (string.IsNullOrEmpty(organiser) || x.Organiser.ShortName.Equals(organiser)) &&
                (string.IsNullOrEmpty(program) || x.Tag.Equals(program)) &&
                !x.IsDeprecated).Include(curriculum => curriculum.Degree).Include(curriculum1 =>
                curriculum1.Organiser.Institution).ToList();


            var sem = Db.Semesters.SingleOrDefault(x => x.Name.Equals(semester));
            if (sem == null)
            {
                return NotFound();
            }

            var list = new List<CurriculumInstanceApiContract>();

            foreach (var curriculum in curricula)
            {
                var am = curriculum.StatuteTakeEffect?.ToString("yyyy-MM-dd") ?? string.Empty;

                if (!string.IsNullOrEmpty(am) || !string.IsNullOrEmpty(amendment) || am.Equals(amendment))
                    continue;

                var context = new CurriculumContextApiContract
                {
                    Institution = curriculum.Organiser.Institution.Tag,
                    Organiser = curriculum.Organiser.ShortName,
                    Program = curriculum.Tag,
                    Amendment = curriculum.StatuteTakeEffect?.ToString("yyyy-MM-dd")
                };


                var subjects = Db.ModuleCourses.Where(x =>
                    x.SubjectAccreditations.Any(c =>
                        c.Slot != null &&
                        c.Slot.AreaOption != null &&
                        c.Slot.AreaOption.Area.Curriculum.Id == curriculum.Id)).Include(moduleSubject => moduleSubject.Module).ToList();

                var modules = subjects.Select(x => x.Module).Distinct().ToList();

                var dto = new CurriculumInstanceApiContract
                {
                    CurriculumKey = curriculum.FullTag,
                    SemesterKey = sem.Name,
                    Modules = new List<CurriculumModuleApiContract>()
                };

                foreach (var module in modules)
                {
                    dto.Modules.Add(new CurriculumModuleApiContract
                    {
                        ModuleKey = module.FullTag
                    });
                }

                list.Add(dto);
            }

            return Ok(list);
        }


        private CurriculumDetailsApiContract TransformCurriculum(Curriculum curriculum, CurriculumContextApiContract context)
        {
            var dto = new CurriculumDetailsApiContract()
            {
                Id = curriculum.Id,
                Key = curriculum.FullTag,
                Context = context,
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
                Language = "de", // curriculum.Language,
                Units = new List<CurriculumUnitApiContract>()
            };

            var allSlots = Db.CurriculumSlots
                .Where(x => x.AreaOption != null &&
                            x.AreaOption.Area.Curriculum.Id == curriculum.Id).Include(curriculumSlot =>
                    curriculumSlot.SubjectAccreditations.Select(subjectAccreditation => subjectAccreditation.Subject)).ToList();

            foreach (var slot in allSlots)
            {
                var slotDto = new CurriculumUnitApiContract
                {
                    Id = slot.Id,
                    Semester = slot.Semester,
                    CreditPoints = slot.ECTS,
                    Title = slot.Name,
                    SlotId = slot.FullTag,
                };

                dto.Units.Add(slotDto);
            }

            return dto;
        }




    }
}
