using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MyStik.TimeTable.Data;
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
        /// <summary>
        /// Suche nache Modulesn
        /// </summary>
        [Route("{id}/summary")]
        public ModuleDto GetModuleSummary(Guid id)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);
            if (module == null)
                return null;

            var model = new ModuleDto
            {
                Name = module.Name,
                ShortName = module.ShortName,
                Subjects = new List<SubjectDto>(),
                ExamOptions = new List<ExamOptionDto>()
            };

            foreach (var moduleSubject in module.ModuleSubjects)
            {
                var subject = new SubjectDto
                {
                    Name = moduleSubject.Name,
                    SWS = moduleSubject.SWS,
                    TeachingFormat = moduleSubject.TeachingFormat.Tag
                };

                model.Subjects.Add(subject);
            }

            foreach (var examinationOption in module.ExaminationOptions)
            {
                var option = new ExamOptionDto
                {
                    Name = examinationOption.Name,

                    Exams = new List<ExamDto>()
                };

                model.ExamOptions.Add(option);

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

            //module.ModuleResponsibilities

            return model;
        }

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
    }
}
