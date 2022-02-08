using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.DTOs;

namespace MyStik.TimeTable.Web.Api.Services
{
    public class ModuleConverter
    {
        private TimeTableDbContext _db;

        /// <summary>
        /// 
        /// </summary>
        public ModuleConverter(TimeTableDbContext db)
        {
            _db = db;
        }


        public CertificateModuleDto ConvertCertificateModule(Guid id)
        {
            var dto = new CertificateModuleDto();

            /*
            var module = _db.CertificateModules.SingleOrDefault(x => x.Id == id);

            dto.Id = module.Id;
            dto.Name = module.Name;

            foreach (var subject in module.Subjects)
            {
                // Ebene Modulbeschreibung
                foreach (var accreditation in subject.ContentModules)
                {
                    dto.Subjects.Add(ConvertAccreditatedModule(accreditation.Id));
                }
            }
            */

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        public AccreditatedModuleDto ConvertAccreditatedModule(Guid id)
        {
            var dto = new AccreditatedModuleDto();

            /*
            var module = _db.Accreditations.SingleOrDefault(x => x.Id == id);

            dto.Id = module.Id;
            dto.Number = module.Number;
            dto.isMandatory = module.IsMandatory;

            // Ebene Subjects
            dto.Ects = module.CertificateSubject.Ects;
            dto.Term = module.CertificateSubject.Term;


            if (module.TeachingBuildingBlock != null)
            {
                dto.Name = module.TeachingBuildingBlock.Name;

                foreach (var lecturer in module.TeachingBuildingBlock.Lecturers)
                {
                    dto.Lecturers.Add(new LecturerDto
                    {
                        FirstName = lecturer.Member.FirstName,
                        LastName = lecturer.Member.Name,
                        Title = lecturer.Member.Title,
                        Faculty = lecturer.Member.Organiser.ShortName
                    });

                    if (lecturer.IsAdmin)
                    {
                        dto.ModuleAccounts.Add(new LecturerDto
                        {
                            FirstName = lecturer.Member.FirstName,
                            LastName = lecturer.Member.Name,
                            Title = lecturer.Member.Title,
                            Faculty = lecturer.Member.Organiser.ShortName
                        });
                    }
                }

                dto.Language = "Deutsch";

                var sb = new StringBuilder();

                if (!string.IsNullOrEmpty(module.TeachingBuildingBlock.Description))
                {
                    sb.AppendFormat("<h5>{0}</h5>", module.TeachingBuildingBlock.Name);
                    sb.AppendFormat("<h5>{0}</h5>", module.TeachingBuildingBlock.Description);
                }

                foreach (var teachingUnit in module.TeachingBuildingBlock.TeachingUnits)
                {
                    sb.AppendFormat("<h5>{0}</h5>", teachingUnit.Name2);
                    sb.AppendFormat("<h5>{0}</h5>", teachingUnit.Description2);
                }

                dto.Content = sb.ToString();


                foreach (var teachingUnit in module.TeachingBuildingBlock.TeachingUnits)
                {
                    dto.TeachingMethods.Add(new TeachingDto
                    {
                        Category = teachingUnit.Form.Name,
                    });
                }

                foreach (var examinationUnit in module.TeachingBuildingBlock.ExaminationUnits)
                {
                    dto.Exams.Add(new ExamDto
                    {
                        Category = examinationUnit.Form.Name,
                        CalcFactor = examinationUnit.Weight,
                        Info = examinationUnit.Duration.HasValue ? $"{examinationUnit.Duration.Value} min" : string.Empty
                    });

                    foreach (var aid in examinationUnit.ExaminationAids)
                    {
                        dto.Resources.Add(new ExamAidDto
                        {
                            Category = aid.Name,
                            Info = aid.Description
                        });
                    }
                }
            }
            else
            {
                dto.Name = "Keine Modulbeschreibung vorhandem";
            }
            */

            return dto;
        }
    }
}