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
        /// <summary>
        /// 
        /// </summary>
        [Route("")]
        public IQueryable<CurriculumDto> GetCurricula(string org="")
        {
            var curriculaWithPlan = 
                string.IsNullOrEmpty(org) ?
                    Db.Curricula.ToList() :
                    Db.Curricula.Where(x => x.Organiser.ShortName.ToLower().Equals(org.ToLower())).ToList();

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
                        Color = curriculum.Organiser.HtmlColor
                    }
                };

                result.Add(curr);
            }

            return result.AsQueryable();
        }

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

        [Route("{name}/{version}/scheme")]
        public IQueryable<CurriculumSchemeSemesterDto> GetCurriculumPlan(string name, string version)
        {
            var list = new List<CurriculumSchemeSemesterDto>();

            var curr = Db.Curricula.SingleOrDefault(x => x.ShortName.ToUpper().Equals(name.Trim().ToUpper()));

            if (curr == null)
                return list.AsQueryable();

            var semesterSubjects = Db.CertificateSubjects.Where(x => x.CertificateModule.Curriculum.Id == curr.Id).GroupBy(x => x.Term).ToList();

            foreach (var semester in semesterSubjects)
            {
                var semDto = new CurriculumSchemeSemesterDto()
                {
                    Term = semester.Key
                };

                var modules = semester.GroupBy(x => x.CertificateModule);
                foreach (var module in modules)
                {
                    var moduleDto = new CurriculumSchemeModuleDto()
                    {
                        Name = module.Key.Name,
                        TotalEcts = 0
                    };

                    foreach (var subject in module)
                    {
                        var subjectDto = new CurriculumSchemeSubjectDto()
                        {
                            Name = subject.Name,
                            ECTS = subject.Ects
                        };
                        moduleDto.TotalEcts += subject.Ects;

                        foreach (var contentModule in subject.ContentModules)
                        {
                            var optionDto = new CurriculumSchemeOptionDto()
                            {
                                Id = contentModule.Id,
                                Number = contentModule.Number,
                                IsMandatory = contentModule.IsMandatory
                            };

                            subjectDto.Options.Add(optionDto);
                        }

                        moduleDto.Subjects.Add(subjectDto);
                    }

                    semDto.Modules.Add(moduleDto);
                }

                list.Add(semDto);
            }


            return list.AsQueryable();
        }

    }
}

