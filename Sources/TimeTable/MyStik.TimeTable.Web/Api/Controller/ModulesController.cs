using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Models;
using Org.BouncyCastle.Crypto.Tls;

namespace MyStik.TimeTable.Web.Api.Controller
{

    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/v2/modules")]
    public class ModulesController : ApiBaseController
    {
        [System.Web.Http.Route("")]
        public IQueryable<ModuleDto> GetModules()
        {
            var list = new List<ModuleDto>();

            var modules = Db.CurriculumModules.Where(x => x.Catalog != null).ToList();

            foreach (var curriculumModule in modules)
            {
                var dto = new ModuleDto
                {
                    Id = curriculumModule.Id,
                    Name = curriculumModule.Name,
                    Tag = curriculumModule.FullTag,
                    Accreditions = new List<ModuleAccreditionDto>(),
                    Subjects = new List<SubjectDto>(),
                    ExamOptions = new List<ExamOptionDto>()
                };

                foreach (var moduleSubject in curriculumModule.ModuleSubjects)
                {
                    var subject = new SubjectDto
                    {
                        Name = moduleSubject.Name,
                        SWS = moduleSubject.SWS,
                        TeachingFormat = moduleSubject.TeachingFormat.Tag
                    };

                    dto.Subjects.Add(subject);
                }

                foreach (var examinationOption in curriculumModule.ExaminationOptions)
                {
                    var option = new ExamOptionDto
                    {
                        Name = examinationOption.Name,

                        Exams = new List<ExamDto>()
                    };

                    dto.ExamOptions.Add(option);

                    foreach (var fraction in examinationOption.Fractions)
                    {
                        var exam = new ExamDto
                        {
                            Name = fraction.Form.ShortName,
                            Weight = fraction.Weight
                        };

                        option.Exams.Add(exam);
                    }
                }

                foreach (var subject in curriculumModule.ModuleSubjects)
                {
                    foreach (var accreditation in subject.SubjectAccreditations)
                    {
                        try
                        {
                            if (accreditation.Slot != null && accreditation.Slot.AreaOption != null &&
                                accreditation.Slot.AreaOption.Area != null &&
                                accreditation.Slot.AreaOption.Area.Curriculum != null &&
                                accreditation.Slot.AreaOption.Area.Curriculum.Organiser != null)
                            {
                                var acc = new ModuleAccreditionDto
                                {
                                    Curriculum = accreditation.Slot.AreaOption.Area.Curriculum.Name,
                                    Faculty = accreditation.Slot.AreaOption.Area.Curriculum.Organiser.ShortName,
                                    Slot = accreditation.Slot.FullTag,
                                    Ects = accreditation.Slot.ECTS
                                };

                                dto.Accreditions.Add(acc);
                            }
                        }
                        catch (Exception e)
                        {
                            // ignored
                        }
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
