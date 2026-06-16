using Antlr.Runtime.Misc;
using Microsoft.AspNet.SignalR.Messaging;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.IO.Contracts;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Services;
using RazorEngine.Compilation.ImpromptuInterface.Optimization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace MyStik.TimeTable.Web.Api.Controller
{

    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/v2/modules")]
    public class ModulesController : ApiBaseController
    {

        [Route("")]
        [ResponseType(typeof(List<ModuleEntityApiContract>))]

        public IHttpActionResult GetModules(string institution, string organiser = "", string catalog = "", string topic = "")
        {
            var modules = Db.CurriculumModules.Where(x =>
                x.Catalog != null && x.Catalog.Organiser.Institution.Tag.Equals(institution) &&
                (string.IsNullOrEmpty(organiser) || x.Catalog.Organiser.ShortName.Equals(organiser)) &&
                (string.IsNullOrEmpty(catalog) || x.Catalog.Tag.Equals(catalog)) && 
                (string.IsNullOrEmpty(topic) || x.Tag.Equals(topic))).Include(curriculumModule => curriculumModule.Catalog.Organiser.Institution)
                .ToList();

            var list = new List<ModuleEntityApiContract>();

            foreach (var module in modules)
            {
                var context = new ModuleContextApiContract
                {
                    Institution = module.Catalog.Organiser.Institution.Tag,
                    Organiser = module.Catalog.Organiser.ShortName,
                    Catalog = module.Catalog.Tag,
                    Topic = module.Tag
                };


                list.Add(new ModuleEntityApiContract
                {
                    Id = module.Id,
                    Key = module.FullTag,
                    Context = context
                });
            }

            return Ok(list);
        }

        [Route("{key}/blueprint")]
        [ResponseType(typeof(List<ModuleDetailsApiContract>))]

        public IHttpActionResult GetModuleByContext(string key)
        {
            var words = key.Split('|');
            var institutionId = words.Length > 0 ? words[0] : string.Empty;
            var organiserId = words.Length > 1 ? words[1] : string.Empty;
            var catalogId = words.Length > 2 ? words[2] : string.Empty;
            var topicId = words.Length > 3 ? words[3] : string.Empty;

            var dbModules = Db.CurriculumModules.Where(x =>
                x.Catalog != null && x.Catalog.Organiser.Institution.Tag.Equals(institutionId) &&
                (string.IsNullOrEmpty(organiserId) || x.Catalog.Organiser.ShortName.Equals(organiserId)) &&
                (string.IsNullOrEmpty(catalogId) || x.Catalog.Tag.Equals(catalogId)) &&
                (string.IsNullOrEmpty(topicId) || x.Tag.Equals(topicId))).Include(curriculumModule =>
                curriculumModule.Catalog.Organiser.Institution).ToList();

            var list = new List<ModuleDetailsApiContract>();

            foreach (var dbModule in dbModules)
            {
                var context = new ModuleContextApiContract
                {
                    Institution = dbModule.Catalog.Organiser.Institution.Tag,
                    Organiser = dbModule.Catalog.Organiser.ShortName,
                    Catalog = dbModule.Catalog.Tag,
                    Topic = dbModule.Tag
                };

                var module = TransformModule(dbModule, context);

                list.Add(module);
            }


            return Ok(list);
        }


        /// <summary>
        /// Modulbeschreibung nach Semester
        /// </summary>
        [Route("{key}/instance/{semester}")]
        [ResponseType(typeof(List<ModuleDescriptionApiContract>))]

        public IHttpActionResult GetModuleImplementation(string key, string semester)
        {
            var sem = Db.Semesters.SingleOrDefault(x => x.Name.ToLower().Equals(semester.ToLower()));
            if (sem == null)
            {
                return NotFound();
            }

            var words = key.Split('|');
            var institutionId = words.Length > 0 ? words[0] : string.Empty;
            var organiserId = words.Length > 1 ? words[1] : string.Empty;
            var catalogId = words.Length > 2 ? words[2] : string.Empty;
            var topicId = words.Length > 3 ? words[3] : string.Empty;

            var dbModules = Db.CurriculumModules.Where(x =>
                x.Catalog != null && x.Catalog.Organiser.Institution.Tag.Equals(institutionId) &&
                (string.IsNullOrEmpty(organiserId) || x.Catalog.Organiser.ShortName.Equals(organiserId)) &&
                (string.IsNullOrEmpty(catalogId) || x.Catalog.Tag.Equals(catalogId)) &&
                (string.IsNullOrEmpty(topicId) || x.Tag.Equals(topicId))).Include(curriculumModule =>
                curriculumModule.Catalog.Organiser.Institution).Include(curriculumModule1 => curriculumModule1.Descriptions.Select(moduleDescription => moduleDescription.Semester)).Include(curriculumModule => curriculumModule.Descriptions.Select(moduleDescription1 => moduleDescription1.ChangeLog)).Include(curriculumModule1 => curriculumModule1.ModuleSubjects.Select(moduleSubject => moduleSubject.SubjectTeachings.Select(subjectTeaching =>
                subjectTeaching.Course.Semester))).ToList();

            var list = new List<ModuleDescriptionApiContract>();

            foreach (var dbModule in dbModules)
            {
                var context = new ModuleContextApiContract
                {
                    Institution = dbModule.Catalog.Organiser.Institution.Tag,
                    Organiser = dbModule.Catalog.Organiser.ShortName,
                    Catalog = dbModule.Catalog.Tag,
                    Topic = dbModule.Tag
                };


                var model = new ModuleDescriptionApiContract
                {
                    ModuleKey = dbModule.FullTag,
                    SemesterKey = sem.Name,
                    Teachings = new List<ModuleTeachingApiContract>(),
                    Examinations = new List<ModuleExaminationApiContract>(),
                    Achievements = new List<ModuleAchievementApiContract>()
                };

                // Kurse
                foreach (var subject in dbModule.ModuleSubjects.ToList())
                {
                    var teachings = subject.SubjectTeachings.Where(x => x.Course.Semester.Id == sem.Id).ToList();

                    foreach (var teaching in teachings)
                    {
                        model.Teachings.Add(new ModuleTeachingApiContract
                        {
                            InstructionKey = subject.FullTag,
                            CourseKey = teaching.Course.FullTag
                        });
                    }
                }


                // Prüfungen
                var exams = Db.ExaminationDescriptions.Where(x => x.Semester.Id == sem.Id &&
                                                                  x.ExaminationOption.Module.Id == dbModule.Id)
                    .Include(examinationDescription => examinationDescription.ExaminationOption)
                    .Include(examinationDescription1 => examinationDescription1.FirstExminer)
                    .Include(examinationDescription2 => examinationDescription2.SecondExaminer).ToList();

                foreach (var examinationDescription in exams)
                {
                    var firstExaminer = examinationDescription.FirstExminer != null ? examinationDescription.FirstExminer.FullName : string.Empty;
                    var secondExaminer = examinationDescription.SecondExaminer != null ? examinationDescription.SecondExaminer.FullName : string.Empty;

                    var examiners = $"{firstExaminer} | {secondExaminer}";

                    model.Examinations.Add(new ModuleExaminationApiContract
                    {
                        ChallengeKey = examinationDescription.ExaminationOption.FullTag,
                        ExamKey = "Exxx",
                        Description = $"{examinationDescription.Description} | {examinationDescription.Conditions} | {examinationDescription.Utilities} | {examiners}"
                    });

                }

                // Beschreibung
                var desc = dbModule.Descriptions
                    .Where(x => x.Semester.Id == sem.Id && x.ChangeLog != null && x.ChangeLog.Approved != null)
                    .OrderByDescending(x => x.ChangeLog.Approved.Value)
                    .FirstOrDefault();

                if (desc != null)
                {
                    model.Achievements.Add(new ModuleAchievementApiContract
                    {
                        Description = desc.Description,
                        MaterialKey = "Mxxx",
                        ObjectiveKey = "Oxxx"
                    });
                }


                list.Add(model);
            }
            
            return Ok(list);
        }


        private ModuleDetailsApiContract TransformModule(CurriculumModule dbModule, ModuleContextApiContract context)
        {
            var module = new ModuleDetailsApiContract
            {
                Id = dbModule.Id,
                Key = dbModule.FullTag,
                Context = context,
                Identifiers = new List<ModuleIdentifierApiContract>
                {
                    new ModuleIdentifierApiContract
                    {
                        Language = "de",
                        Title = dbModule.Name,
                        Summary = string.Empty
                    },
                    new ModuleIdentifierApiContract
                    {
                        Language = "en",
                        Title = dbModule.NameEn,
                        Summary = string.Empty
                    }
                },
                Editors = new List<ModuleEditorApiContract>(),
                Usages = new List<ModuleUsageApiContract>(),
                Instructions = new List<ModuleInstructionApiContract>(),
                Challenges = new List<ModuleChallengeApiContract>(),
                Objectives = new List<ModuleObjectiveApiContract>()
            };

            foreach (var moduleResponsibility in dbModule.ModuleResponsibilities.ToList())
            {
                module.Editors.Add(new ModuleEditorApiContract
                {
                    Name = moduleResponsibility.Member.PersonName,
                    MemberKey = moduleResponsibility.Member.FullTag
                });
            }

            // todo: das Einhängen in den chips
            var slots = (from subject in dbModule.ModuleSubjects.ToList() from subjectAccreditation in subject.SubjectAccreditations.ToList() select subjectAccreditation.Slot)
                .Distinct()
                .ToList();

            foreach (var slot in slots)
            {
                module.Usages.Add(new ModuleUsageApiContract
                {
                    ChipKey = slot.FullTag,
                    InstructionKey = ""
                });
            }


            foreach (var moduleSubject in dbModule.ModuleSubjects.ToList())
            {
                module.Instructions.Add(new ModuleInstructionApiContract
                {
                    InstructionKey = moduleSubject.FullTag,
                    Title = moduleSubject.Name,
                    Format = moduleSubject.TeachingFormat.Tag,
                    ContactHours = moduleSubject.SWS
                });
            }

            foreach (var examinationOption in dbModule.ExaminationOptions.ToList())
            {
                var option = new ModuleChallengeApiContract
                {
                    ChallengeKey = examinationOption.FullTag,
                    Title = examinationOption.Name,
                    Fractions = new List<ModuleChallengeFractionApiContract>()
                };

                foreach (var examinationFraction in examinationOption.Fractions.ToList())
                {
                    var fraction = new ModuleChallengeFractionApiContract
                    {
                        Weight = examinationFraction.Weight,
                        Format = examinationFraction.Form.ShortName
                    };
                    option.Fractions.Add(fraction);
                }
                module.Challenges.Add(option);
            }

            // Dummy für die Objectives
            module.Objectives.Add(new ModuleObjectiveApiContract
            {
                ObjectiveKey = $"{dbModule.FullTag}|O1",
                Target = "Ziel 1",
                Description = "Beschreibung zum Ziel 1",
                Competence = "Kompetenz 1",
                Taxonomy = "Taxonomie 1",
                Level = 1
            });

            return module;
        }



        /// <summary>
        /// Suche nache Modulesn
        /// </summary>
        [Route("{id}")]
        [ResponseType(typeof(ModuleDetailsApiContract))]
        public IHttpActionResult GetModuleById(Guid id)
        {
            var dbModule = Db.CurriculumModules
                .Include(curriculumModule =>
                    curriculumModule.ModuleResponsibilities.Select(moduleResponsibility => moduleResponsibility.Member)).Include(curriculumModule1 =>
                    curriculumModule1.ModuleSubjects.Select(moduleSubject => moduleSubject.TeachingFormat)).Include(curriculumModule => curriculumModule.ExaminationOptions.Select(examinationOption =>
                    examinationOption.Fractions.Select(examinationFraction => examinationFraction.Form))).Include(curriculumModule1 =>
                    curriculumModule1.Catalog.Organiser.Institution)
                .SingleOrDefault(x => x.Id == id);

            if (dbModule == null)
                return NotFound();

            var context = new ModuleContextApiContract
            {
                Institution = dbModule.Catalog.Organiser.Institution.Tag,
                Organiser = dbModule.Catalog.Organiser.ShortName,
                Catalog = dbModule.Catalog.Tag,
                Topic = dbModule.Tag
            };


            var module = TransformModule(dbModule, context);

            return Ok(module);
        }

    }
}
