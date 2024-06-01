using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
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
        [System.Web.Http.Route("")]
        public IQueryable<ModuleDtoVersion2> GetModules()
        {

            var list = new List<ModuleDtoVersion2>();

            var modules = Db.CurriculumModules.Where(x => x.Catalog != null).ToList();

            foreach (var curriculumModule in modules)
            {

                var dto = new ModuleDtoVersion2
                {
                    CatalogId = curriculumModule.FullTag,
                    Title = curriculumModule.Name,
                    UrlDescription = $"/ModuleDescription/Details/{curriculumModule.Id}",
                    Teachings = new List<TeachingDtoVersion2>()
                };

                foreach (var subject in curriculumModule.ModuleSubjects)
                {
                    foreach (var accreditation in subject.SubjectAccreditations)
                    {
                        var teachingDto = new TeachingDtoVersion2
                        {
                            Semester = accreditation.Slot.Semester,
                            CurriculumId = accreditation.Slot.AreaOption.Area.Curriculum.ShortName,
                            CurriculumTitle = accreditation.Slot.AreaOption.Area.Curriculum.Name,
                            UrlCurriculumPlan = $"/Curriculum/Details/{accreditation.Slot.AreaOption.Area.Curriculum.Id}",
                            SubjectTitle = subject.Name,
                            SubjectTeachingFormat = subject.TeachingFormat.Tag
                        };

                        dto.Teachings.Add(teachingDto);
                    }

                }

                list.Add(dto);
            }

            return list.AsQueryable();
        }

        /// <summary>
        /// Suche nache Modulesn
        /// </summary>
        [Route("{id}/description/{semester}")]
        public ModuleDescriptionDto GetModuleDescription(Guid id, string semester)
        {
            var model = new ModuleDescriptionDto();


            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);
            if (module == null)
            {
                model.DescriptionDe = "Modul nicht gefunden";
                return model;
            }

            var sem = Db.Semesters.SingleOrDefault(x => x.Name.ToLower().Equals(semester.ToLower()));
            if (sem == null)
            {
                model.DescriptionDe = "Semester nicht gefunden";
                return model;
            }

            var desc = module.Descriptions
                .Where(x => x.Semester.Id == sem.Id && x.ChangeLog != null && x.ChangeLog.Approved != null)
                .OrderByDescending(x => x.ChangeLog.Approved.Value)
                .FirstOrDefault();
            if (desc == null)
            {
                model.DescriptionDe = "Beschreibung nicht gefunden";
                return model;
            }

            model.Id = desc.Id;
            model.DescriptionDe = desc.Description;
            model.DescriptionEn = desc.DescriptionEn;

            /*
            foreach (var accreditation in module.Accreditations)
            {
                var exams = accreditation.ExaminationDescriptions
                    .Where(x => x.Semester.Id == sem.Id && x.ChangeLog.Approved != null).ToList();


            }
            */
            




            return model;
        }

        [Route("{curriculum}/{stage}/{semester}")]
        public List<ModuleSlotDto> GetModuleByStages(string curriculum, int stage, string semester)
        {
            var curr = Db.Curricula.Include(curriculum1 => curriculum1.Areas)
                .FirstOrDefault(x => x.ShortName.Equals(curriculum));
            var sem = new SemesterService().GetSemester(semester);

            if (curr == null || sem == null || !curr.Areas.Any())
            {
                return new List<ModuleSlotDto>();
            }

            var converter = new CourseConverter(Db);
            var model = new List<ModuleSlotDto>();

            var semSlots = Db.CurriculumSlots.Where(x =>
                x.AreaOption.Area.Curriculum.Id == curr.Id && x.Semester == stage).OrderBy(x => x.Tag).ToList();

            foreach (var slot in semSlots)
            {
                foreach (var module in slot.SubjectAccreditations.Select(x => x.Subject.Module).Distinct().ToList())
                {
                    foreach (var subject in module.ModuleSubjects.ToList())
                    {
                        var slotModel = new ModuleSlotDto();
                        slotModel.ModuleTag = module.Tag;
                        slotModel.ModuleName = module.Name;
                        slotModel.SubjectTag = subject.Tag;
                        slotModel.SubjectName = subject.Name;
                        slotModel.Courses = new List<CourseSummaryDto>();

                        var teachings = subject.SubjectTeachings.Where(x => x.Course.Semester.Id == sem.Id).ToList();
                        foreach (var teaching in teachings)
                        {
                            slotModel.Courses.Add(converter.ConvertSummary(teaching.Course));
                        }

                        model.Add(slotModel);
                    }
                }
            }

            return model;
        }


        /*
        [Route("{id}/details/{semester}")]
        public ModuleDto GetModuleDetails(Guid id, string semester)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);
            if (module == null)
                return null;

            var model = new ModuleDto
            {
                Name = module.Name,
                ShortName = module.ShortName,
            };

            //module.ModuleResponsibilities

            return model;
        }

        [Route("{id}/courses/{semester}")]
        public ModuleDto GetModuleCourses(Guid id, string semester)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);
            if (module == null)
                return null;

            var model = new ModuleDto
            {
                Name = module.Name,
                ShortName = module.ShortName,
            };

            //module.ModuleResponsibilities

            return model;
        }

        [Route("{tag}")]
        public IQueryable<ModuleDescriptionDto> GetModuleDescriptions(string tag)
        {
            var list = new List<ModuleDescriptionDto>();

            var orgTag = "";
            var catTag = "";
            var moduleTag = "";

            if (!tag.Contains('#'))
            {
                orgTag = tag;
            }
            else
            {
                var words = tag.Split('#');
                orgTag = words[0];
                if (words.Length > 1)
                    catTag = words[1];
                if (words.Length > 2)
                    moduleTag = words[2];
            }

            var org = Db.Organisers.FirstOrDefault(x =>
                !string.IsNullOrEmpty(x.Tag) &&
                x.Tag.ToUpper().Equals(orgTag.ToUpper()));

            List<CurriculumModule> modules = new List<CurriculumModule>();
            if (!string.IsNullOrEmpty(moduleTag))
            {
                var cat = org.ModuleCatalogs.FirstOrDefault(x => x.Tag.Equals(catTag));
                // nur das eine Modul
                if (cat != null)
                {
                    var m = cat.Modules.FirstOrDefault(x => x.Tag.Equals(moduleTag));

                    modules.Add(m);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(catTag))
                {
                    // alle Module des Katalogs
                    var cat = org.ModuleCatalogs.FirstOrDefault(x => x.Tag.Equals(catTag));
                    if (cat != null)
                    {
                        modules.AddRange(cat.Modules.ToList());
                    }

                }
                else
                {
                    // alle Kataloge der org
                    modules.AddRange(Db.CurriculumModules.Where(x => x.Catalog.Organiser.Id == org.Id).ToList());
                }
            }

            // das Semester, um das es geht
            var semesterService = new SemesterService(Db);

            var semesterList = new List<Semester>();
            var currentSemester = semesterService.GetSemester(DateTime.Today);
            semesterList.Add(currentSemester);
            semesterList.Add(semesterService.GetNextSemester(currentSemester));


            foreach (var module in modules)
            {
                var moduleDto = new ModuleDescriptionDto
                {
                    tag = module.FullTag,
                    name = module.Name,
                    responsible = new List<ModuleDescriptionRespDto>(),
                    slots = new List<ModuleDescriptionSlotDto>(),
                    instances = new List<ModuleDescriptionInstanceDto>()
                };

                foreach (var responsibility in module.ModuleResponsibilities)
                {
                    var respDto = new ModuleDescriptionRespDto
                    {
                        tag = responsibility.Member.FullTag
                    };

                    moduleDto.responsible.Add(respDto);
                }

                foreach (var accreditation in module.Accreditations)
                {
                    var slotDto = new ModuleDescriptionSlotDto
                    {
                        tag = accreditation.Slot.FullTag,
                    };

                    moduleDto.slots.Add(slotDto);
                }

                foreach (var semester in semesterList)
                {
                    var instanceDto = new ModuleDescriptionInstanceDto
                    {
                        semster = semester.Name,
                        description = "",
                        exams = new List<ModuleDescriptionExamDto>(),
                        teaching = new List<ModuleDescriptionTeachingDto>()
                    };

                    var description = module.Descriptions.FirstOrDefault(x => x.Semester.Id == semester.Id);

                    if (description != null)
                    {
                        instanceDto.description = description.Description;
                    }

                    foreach (var moduleAccreditation in module.Accreditations)
                    {
                        var exam = moduleAccreditation.ExaminationDescriptions.FirstOrDefault(x =>
                            x.Semester.Id == semester.Id);

                        if (exam != null)
                        {
                            var examDto = new ModuleDescriptionExamDto
                            {
                                first = exam.FirstExminer != null ? exam.FirstExminer.FullTag : string.Empty,
                                second = exam.SecondExaminer != null ? exam.SecondExaminer.FullTag : string.Empty,
                                conditions = exam.Conditions,
                                utilities = exam.Utilities,
                                fractions = new List<ModuleDescriptionExamFractionDto>()
                            };

                            foreach (var examinationFraction in exam.ExaminationOption.Fractions)
                            {
                                var fractDto = new ModuleDescriptionExamFractionDto
                                {
                                    tag = examinationFraction.Form.ShortName,
                                    weight = examinationFraction.Weight
                                };

                                examDto.fractions.Add(fractDto);
                            }

                            instanceDto.exams.Add(examDto);
                        }
                    }

                    moduleDto.instances.Add(instanceDto);
                }

                list.Add(moduleDto);
            }

            return list.AsQueryable();
        }

        [Route("{id}/details")]
        public ModuleDescriptionDto GetModuleDescriptions(Guid id)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            // das Semester, um das es geht
            var semesterService = new SemesterService(Db);

            var semesterList = new List<Semester>();
            var currentSemester = semesterService.GetSemester(DateTime.Today);
            semesterList.Add(currentSemester);
            semesterList.Add(semesterService.GetNextSemester(currentSemester));


            var moduleDto = new ModuleDescriptionDto
                {
                    id = module.Id,
                    tag = module.FullTag,
                    name = module.Name,
                    responsible = new List<ModuleDescriptionRespDto>(),
                    slots = new List<ModuleDescriptionSlotDto>(),
                    instances = new List<ModuleDescriptionInstanceDto>()
                };

                foreach (var responsibility in module.ModuleResponsibilities)
                {
                    var respDto = new ModuleDescriptionRespDto
                    {
                        tag = responsibility.Member.FullTag
                    };

                    moduleDto.responsible.Add(respDto);
                }

                foreach (var accreditation in module.Accreditations)
                {
                    var slotDto = new ModuleDescriptionSlotDto
                    {
                        tag = accreditation.Slot.FullTag,
                    };

                    moduleDto.slots.Add(slotDto);
                }

                foreach (var semester in semesterList)
                {
                    var instanceDto = new ModuleDescriptionInstanceDto
                    {
                        semster = semester.Name,
                        description = "",
                        exams = new List<ModuleDescriptionExamDto>(),
                        teaching = new List<ModuleDescriptionTeachingDto>()
                    };

                    var description = module.Descriptions.FirstOrDefault(x => x.Semester.Id == semester.Id);

                    if (description != null)
                    {
                        instanceDto.description = description.Description;
                    }

                    foreach (var moduleAccreditation in module.Accreditations)
                    {
                        var exam = moduleAccreditation.ExaminationDescriptions.FirstOrDefault(x =>
                            x.Semester.Id == semester.Id);

                        if (exam != null)
                        {
                            var examDto = new ModuleDescriptionExamDto
                            {
                                first = exam.FirstExminer != null ? exam.FirstExminer.FullTag : string.Empty,
                                second = exam.SecondExaminer != null ? exam.SecondExaminer.FullTag : string.Empty,
                                conditions = exam.Conditions,
                                utilities = exam.Utilities,
                                fractions = new List<ModuleDescriptionExamFractionDto>()
                            };

                            foreach (var examinationFraction in exam.ExaminationOption.Fractions)
                            {
                                var fractDto = new ModuleDescriptionExamFractionDto
                                {
                                    tag = examinationFraction.Form.ShortName,
                                    weight = examinationFraction.Weight
                                };

                                examDto.fractions.Add(fractDto);
                            }

                            instanceDto.exams.Add(examDto);
                        }
                    }

                    moduleDto.instances.Add(instanceDto);
                }


            return moduleDto;
        }
*/

    }
    }
