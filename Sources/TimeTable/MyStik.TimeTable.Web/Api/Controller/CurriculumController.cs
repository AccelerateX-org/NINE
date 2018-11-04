using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/v2/curriculum")]

    public class CurriculumController : ApiBaseController
    {
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
                        Name = module.Key.Name
                    };

                    foreach (var subject in module)
                    {
                        var subjectDto = new CurriculumSchemeSubjectDto()
                        {
                            Name = subject.Name,
                            ECTS = subject.Ects
                        };

                        foreach (var contentModule in subject.ContentModules)
                        {
                            var optionDto = new CurriculumSchemeOptionDto()
                            {
                                Id = contentModule.TeachingBuildingBlock?.Id ?? Guid.Empty,
                                Name = contentModule.TeachingBuildingBlock?.Name ?? string.Empty,
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
