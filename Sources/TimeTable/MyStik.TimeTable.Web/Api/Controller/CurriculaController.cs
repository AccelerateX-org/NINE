using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
        [Route("")]
        public IQueryable<CurriculumDto> GetCurricula()
        {
            var curriculaWithPlan = Db.Curricula.Where(x => x.Packages.Any()).ToList();

            var result = new List<CurriculumDto>();

            foreach (var curriculum in curriculaWithPlan)
            {
                var curr = new CurriculumDto();

                curr.Name = curriculum.Name;
                curr.ShortName = curriculum.ShortName;

                curr.Organiser = new OrganiserDto();
                curr.Organiser.Name = curriculum.Organiser.Name;
                curr.Organiser.ShortName = curriculum.Organiser.ShortName;
                curr.Organiser.Color = curriculum.Organiser.HtmlColor;

                result.Add(curr);
            }

            return result.AsQueryable();
        }

        [Route("{org}/{name}/structure")]
        public IQueryable<CurriculumPackageDto> GetCurrciulumStructure(string org, string name)
        {
            var curr = Db.Curricula.FirstOrDefault(x => x.Organiser.ShortName.Equals(org) && x.ShortName.Equals(name));

            var result = new List<CurriculumPackageDto>();

            foreach (var package in curr.Packages)
            {
                var pck = new CurriculumPackageDto();

                pck.Name = package.Name;

                foreach (var option in package.Options)
                {
                    var pckOpt = new CurriculumOptionDto();
                    pckOpt.Name = option.Name;

                    foreach (var requirement in option.Requirements)
                    {
                        var pckModule = new CurriculumModuleDto();
                        pckModule.Name = requirement.Name;
                        pckModule.Ects = requirement.ECTS;

                        foreach (var criteria in requirement.Criterias)
                        {
                            var pckSection = new CurriculumModuleSectionDto();
                            pckSection.Name = criteria.Name;
                            pckSection.Semester = criteria.Term;

                            // Hier werden Akkreditierug und Fach zusammengeführt
                            foreach (var accreditation in criteria.Accreditations)
                            {
                                var pckSubject = new CurriculumSubjectDto();
                                pckSubject.Name = accreditation.Module.Name;
                                pckSubject.ShortName = accreditation.Module.ShortName;
                                pckSubject.IsMandatory = accreditation.IsMandatory;

                                pckSection.Subjects.Add(pckSubject);
                            }

                            pckModule.Sections.Add(pckSection);

                        }

                        pckOpt.Modules.Add(pckModule);
                    }

                    pck.Options.Add(pckOpt);
                }

                result.Add(pck);
            }

            return result.AsQueryable();
        }

        [Route("{org}/{name}/schedule")]
        public IQueryable<CurriculumTermDto> GetCurrciulumSchedule(string org, string name)
        {
            var curr = Db.Curricula.FirstOrDefault(x => x.Organiser.ShortName.Equals(org) && x.ShortName.Equals(name));

            var result = new List<CurriculumTermDto>();

            var terms = Db.Criterias.Where(x => x.Requirement.Option.Package.Curriculum.Id == curr.Id).GroupBy(x => x.Term).ToList();

            foreach (var term in terms)
            {
                var cTerm = new CurriculumTermDto();
                cTerm.Number = term.Key;

                foreach (var criteria in term)
                {
                    var cSection = new CurriculumTermSectionDto();

                    cSection.Name = criteria.Name;

                    // berechnete ECTS
                    // teile auf: ECTS werden gleichmäßig auf alle aufgeteilt
                    cSection.Ects = criteria.Requirement.ECTS / (double)criteria.Requirement.Criterias.Count; ;

                    // der Pfad
                    cSection.Package = new CurriculumPackageDto{ Name = criteria.Requirement.Option.Package.Name };
                    cSection.Option = new CurriculumOptionDto{ Name = criteria.Requirement.Option.Name };
                    cSection.Module = new CurriculumModuleDto{ Name = criteria.Requirement.Name };

                    // Liste aller möglichen Fächer
                    foreach (var accreditation in criteria.Accreditations)
                    {
                        var cSubject = new CurriculumSubjectDto();
                        cSubject.Name = accreditation.Module.Name;
                        cSubject.IsMandatory = accreditation.IsMandatory;

                        var sbC = new StringBuilder();
                        foreach (var course in accreditation.Module.ModuleCourses.ToList())
                        {
                            sbC.AppendFormat("{0}", course.Name);
                            if (course != accreditation.Module.ModuleCourses.Last())
                            {
                                sbC.AppendFormat(", ");
                            }
                        }
                        cSubject.CourseTypes = sbC.ToString();


                        var sbE = new StringBuilder();
                        foreach (var exam in accreditation.Module.ModuleExams.ToList())
                        {
                            sbE.AppendFormat("{0}", exam.ExternalId);
                            if (exam != accreditation.Module.ModuleExams.Last())
                            {
                                sbE.AppendFormat(", ");
                            }
                        }
                        cSubject.ExamTypes = sbE.ToString();

                        cSection.Subjects.Add(cSubject);
                    }

                    cTerm.Sections.Add(cSection);
                }


                result.Add(cTerm);
            }


            return result.AsQueryable();
        }
    }
}

