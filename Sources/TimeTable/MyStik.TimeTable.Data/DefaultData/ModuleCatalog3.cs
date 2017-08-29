using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogWI_Studienrichtungen(ActivityOrganiser fk09, Curriculum wi)
        {

            //Industrielle Technik Module

            var vut = new CurriculumModule()
            {
                ShortName = "VUT",
                Name = "Verfahrens- und Umwelttechnik",
                ModuleId = "IND1",
                Description = "Verfahrens- und Umwelttechnik lernen",
                ECTS = 4,
                MV = GetHost(fk09, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "VUT"
                    },
                }
            };

            vut.Groups.Add(GetGroup(wi,"3 TEC"));

            wi.Modules.Add(vut);

            _db.CurriculumModules.Add(vut);

            var entech = new CurriculumModule()
            {
                ShortName = "EnTech",
                Name = "Energietechnik",
                ModuleId = "IND2",
                Description = "Energietechnik lernen",
                ECTS = 4,
                MV = GetHost(fk09, "MAU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EnTech"
                    },
                }
            };

            entech.Groups.Add(GetGroup(wi,"3 TEC"));

            wi.Modules.Add(entech);

            _db.CurriculumModules.Add(entech);

            var entkon = new CurriculumModule()
            {
                ShortName = "EntKon",
                Name = "Entwicklung und Konstruktion mit CAD",
                ModuleId = "IND3",
                Description = "Entwicklung und Konstruktion mit CAD lernen",
                ECTS = 4,
                MV = GetHost(fk09, "ANZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EntKon"
                    },
                }
            };

            entkon.Groups.Add(GetGroup(wi,"4 TEC"));

            wi.Modules.Add(entkon);

            _db.CurriculumModules.Add(entkon);

            var ferttech = new CurriculumModule()
            {
                ShortName = "FertTech",
                Name = "Fertigungstechnik",
                ModuleId = "IND4",
                Description = "Fertigungstechnik lernen",
                ECTS = 4,
                MV = GetHost(fk09, "KOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FertTech"
                    },
                }
            };

            ferttech.Groups.Add(GetGroup(wi,"4 TEC"));

            wi.Modules.Add(ferttech);

            _db.CurriculumModules.Add(ferttech);

            var fertautprak = new CurriculumModule()
            {
                ShortName = "FertAutPrak",
                Name = "Fertigungstechnik und Automatisierung mit Praktikum",
                ModuleId = "IND5",
                Description = "Vorlesung und Prakitkum dazu",
                ECTS = 4,
                MV = GetHost(fk09, "PIR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FertTech Prak"
                    },
                }
            };

            fertautprak.Groups.Add(GetGroup(wi,"5 TEC"));

            wi.Modules.Add(fertautprak);

            _db.CurriculumModules.Add(fertautprak);

            var plm = new CurriculumModule()
            {
                ShortName = "PLM",
                Name = "Product Lifecycle Management",
                ModuleId = "IND6",
                Description = "Product Lifecycle Management lernen",
                ECTS = 4,
                MV = GetHost(fk09, "SCN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ProdLife"
                    },
                }
            };

            plm.Groups.Add(GetGroup(wi,"5 TEC"));

            wi.Modules.Add(plm);

            _db.CurriculumModules.Add(plm);

            //Informationstechnik Module

            var daba = new CurriculumModule()
            {
                ShortName = "DB",
                Name = "Datenbanken in Technik und Wirtschaft",
                ModuleId = "INF1",
                Description = "Datenbanken lernen",
                ECTS = 3,
                MV = GetHost(fk09, "TEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DB"
                    },
                }
            };

            daba.Groups.Add(GetGroup(wi,"3 INF"));

            wi.Modules.Add(daba);

            _db.CurriculumModules.Add(daba);

            var se1 = new CurriculumModule()
            {
                ShortName = "SE1",
                Name = "Software Engineering 1",
                ModuleId = "INF2",
                Description = "Programmieren lernen",
                ECTS = 5,
                MV = GetHost(fk09, "HIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SoftIng1"
                    },
                }
            };

            se1.Groups.Add(GetGroup(wi,"3 INF"));

            wi.Modules.Add(se1);

            _db.CurriculumModules.Add(se1);

            var se2 = new CurriculumModule()
            {
                ShortName = "SE2",
                Name = "Software Engineering 2",
                ModuleId = "INF3",
                Description = "nochmal Programmieren lernen",
                ECTS = 4,
                MV = GetHost(fk09, "SCN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SoftIng2"
                    },
                }
            };

            se2.Groups.Add(GetGroup(wi,"4 INF"));

            wi.Modules.Add(se2);

            _db.CurriculumModules.Add(se2);

            var itproj1 = new CurriculumModule()
            {
                ShortName = "ITProj1",
                Name = "IT-Projektseminar 1",
                ModuleId = "INF4",
                Description = "Projektarbeit",
                ECTS = 4,
                MV = GetHost(fk09, "HIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "IT-Proj-1"
                    },
                }
            };

            itproj1.Groups.Add(GetGroup(wi,"4 INF"));

            wi.Modules.Add(itproj1);

            _db.CurriculumModules.Add(itproj1);

            var itproj2 = new CurriculumModule()
            {
                ShortName = "ITProj2",
                Name = "IT-Projektseminar 2",
                ModuleId = "INF5",
                Description = "nochmal Projektarbeit",
                ECTS = 4,
                MV = GetHost(fk09, "HIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "IT-Proj-2"
                    },
                }
            };

            itproj2.Groups.Add(GetGroup(wi,"5 INF"));

            wi.Modules.Add(itproj2);

            _db.CurriculumModules.Add(itproj2);

            var embsys = new CurriculumModule()
            {
                ShortName = "EmbSys",
                Name = "Embedded Systems",
                ModuleId = "INF6",
                Description = "Embedded Systems lernen",
                ECTS = 4,
                MV = GetHost(fk09, "HIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EmbSys"
                    },
                }
            };

            embsys.Groups.Add(GetGroup(wi,"5 INF"));

            wi.Modules.Add(embsys);

            _db.CurriculumModules.Add(embsys);

            //Biotechnologie Module

            var bioprak = new CurriculumModule()
            {
                ShortName = "BioPrak",
                Name = "Biotechnologisches Praktikum",
                ModuleId = "BIO1",
                Description = "Praktikum",
                ECTS = 4,
                MV = GetHost(fk09, "TRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Practice,
                        ExternalId = "Bio-Prakt"
                    },
                }
            };

            bioprak.Groups.Add(GetGroup(wi,"5 BIO"));

            wi.Modules.Add(bioprak);

            _db.CurriculumModules.Add(bioprak);

            var mobi = new CurriculumModule()
            {
                ShortName = "MoBi",
                Name = "Molekularbiologie",
                ModuleId = "BIO2",
                Description = "Molekularbiologie lernen",
                ECTS = 4,
                MV = GetHost(fk09, "TRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MoBi"
                    },
                }
            };

            mobi.Groups.Add(GetGroup(wi,"3 BIO"));

            wi.Modules.Add(mobi);

            _db.CurriculumModules.Add(mobi);

            var indbio = new CurriculumModule()
            {
                ShortName = "IndBio",
                Name = "Industrielle Biotechnologie",
                ModuleId = "BIO3",
                Description = "Industrielle Biotechnologie lernen",
                ECTS = 4,
                MV = GetHost(fk09, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "IndBiotechn"
                    },
                }
            };

            indbio.Groups.Add(GetGroup(wi,"4 BIO"));

            wi.Modules.Add(indbio);

            _db.CurriculumModules.Add(indbio);

            var biovt = new CurriculumModule()
            {
                ShortName = "BioVT",
                Name = "Bioverfahrenstechnik",
                ModuleId = "BIO4",
                Description = "Bioverfahrenstechnik lernen",
                ECTS = 4,
                MV = GetHost(fk09, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bio-VT"
                    },
                }
            };

            biovt.Groups.Add(GetGroup(wi,"4 BIO"));

            wi.Modules.Add(biovt);

            _db.CurriculumModules.Add(biovt);

            var nawaro = new CurriculumModule()
            {
                ShortName = "NaWaRo",
                Name = "Nachwachsende Rohstoffe",
                ModuleId = "BIO5",
                Description = "Rohstoffe lernen",
                ECTS = 4,
                MV = GetHost(fk09, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "NaWaRo"
                    },
                }
            };

            nawaro.Groups.Add(GetGroup(wi,"5 BIO"));

            wi.Modules.Add(nawaro);

            _db.CurriculumModules.Add(nawaro);

            var techumw = new CurriculumModule()
            {
                ShortName = "TechUmw",
                Name = "Technischer Umweltschutz",
                ModuleId = "BIO6",
                Description = "Technischer Umweltschutz lernen",
                ECTS = 4,
                MV = GetHost(fk09, "TRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Tech Umw"
                    },
                }
            };

            techumw.Groups.Add(GetGroup(wi,"3 BIO"));

            wi.Modules.Add(techumw);

            _db.CurriculumModules.Add(techumw);

            

            _db.SaveChanges();

        }


    }
}
