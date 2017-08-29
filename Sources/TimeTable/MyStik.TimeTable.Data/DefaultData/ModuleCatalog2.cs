using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogWI_Integration(ActivityOrganiser fk09, Curriculum wi)
        {
            //Integrationsmodule

            var infosys = new CurriculumModule()
            {
                ShortName = "InfoSys",
                Name = "Informationssysteme",
                ModuleId = "I1",
                Description = "Überblich über verwendete Inofsysteme",
                ECTS = 4,
                MV = GetHost(fk09, "TEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "InfoSys"
                    },
                }
            };

            infosys.Groups.Add(GetGroup(wi, "3 BIO"));
            infosys.Groups.Add(GetGroup(wi, "3 TEC"));
            infosys.Groups.Add(GetGroup(wi, "3 INF"));

            wi.Modules.Add(infosys);

            _db.CurriculumModules.Add(infosys);

            var ergo = new CurriculumModule()
            {
                ShortName = "Ergo",
                Name = "Ergonomie mit Praktikum",
                ModuleId = "I2",
                Description = "....",
                ECTS = 3,
                MV = GetHost(fk09, "BRB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ergo"
                    },
                }
            };

            ergo.Groups.Add(GetGroup(wi, "4 BIO"));
            ergo.Groups.Add(GetGroup(wi, "4 TEC"));
            ergo.Groups.Add(GetGroup(wi, "4 INF"));

            wi.Modules.Add(ergo);

            _db.CurriculumModules.Add(ergo);

            var ppqm = new CurriculumModule()
            {
                ShortName = "PPQM",
                Name = "Projekt- und Qualitätsmanagmet",
                ModuleId = "I3",
                Description = "...",
                ECTS = 5,
                MV = GetHost(fk09, "SCU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "PPQM-Vorl"
                    },
                    new ModuleCourse
                    {
                        CourseType = CourseType.Practice,
                        ExternalId = "PPQM-Proj"
                    },
                }

            };

            ppqm.Groups.Add(GetGroup(wi, "5 BIO"));
            ppqm.Groups.Add(GetGroup(wi, "5 TEC"));
            ppqm.Groups.Add(GetGroup(wi, "5 INF"));

            wi.Modules.Add(ppqm);

            _db.CurriculumModules.Add(ppqm);

            var persorgentw = new CurriculumModule()
            {
                ShortName = "PersOrgEntw",
                Name = "Personal- und Organisationsentwicklug",
                ModuleId = "I4",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "OST"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PersOrgEntw"
                    },
                }

            };

            persorgentw.Groups.Add(GetGroup(wi, "7 BIO"));
            persorgentw.Groups.Add(GetGroup(wi, "7 TEC"));
            persorgentw.Groups.Add(GetGroup(wi, "7 INF"));

            wi.Modules.Add(persorgentw);

            _db.CurriculumModules.Add(persorgentw);

            var fseng1 = new CurriculumModule()
            {
                ShortName = "FS Englisch 1",
                Name = "Fachsprache Englisch 1",
                ModuleId = "I5",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "TMF"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FS Engl Hi1"
                    },
                }
            };

            fseng1.Groups.Add(GetGroup(wi, "4 BIO"));
            fseng1.Groups.Add(GetGroup(wi, "4 TEC"));
            fseng1.Groups.Add(GetGroup(wi, "4 INF"));

            wi.Modules.Add(fseng1);

            _db.CurriculumModules.Add(fseng1);

            var fseng2 = new CurriculumModule()
            {
                ShortName = "FS Englisch 2",
                Name = "Fachsprache Englisch 2",
                ModuleId = "I6",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "TMF"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FS Engl Hi2"
                    },
                }
            };

            fseng2.Groups.Add(GetGroup(wi, "5 BIO"));
            fseng2.Groups.Add(GetGroup(wi, "5 TEC"));
            fseng2.Groups.Add(GetGroup(wi, "5 INF"));

            wi.Modules.Add(fseng2);

            _db.CurriculumModules.Add(fseng2);

            var fseng3 = new CurriculumModule()
            {
                ShortName = "FS Englisch 3",
                Name = "Fachsprache Englisch 3",
                ModuleId = "I7",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "TMF"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FS Engl Van"
                    },
                }
            };

            fseng3.Groups.Add(GetGroup(wi, "6 BIO"));
            fseng3.Groups.Add(GetGroup(wi, "6 TEC"));
            fseng3.Groups.Add(GetGroup(wi, "6 INF"));

            wi.Modules.Add(fseng3);

            _db.CurriculumModules.Add(fseng3);

            var fsfran1 = new CurriculumModule()
            {
                ShortName = "FS Franz 1",
                Name = "Fachsprache Französisch 1",
                ModuleId = "I8",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "KUN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FS Franz1"
                    },
                }
            };

            fsfran1.Groups.Add(GetGroup(wi, "4 BIO"));
            fsfran1.Groups.Add(GetGroup(wi, "4 TEC"));
            fsfran1.Groups.Add(GetGroup(wi, "4 INF"));

            wi.Modules.Add(fsfran1);

            _db.CurriculumModules.Add(fsfran1);

            var fsfran2 = new CurriculumModule()
            {
                ShortName = "FS Franz 2",
                Name = "Fachsprache Französisch 2",
                ModuleId = "I9",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "KUN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FS Franz2"
                    },
                }
            };

            fsfran2.Groups.Add(GetGroup(wi, "5 BIO"));
            fsfran2.Groups.Add(GetGroup(wi, "5 TEC"));
            fsfran2.Groups.Add(GetGroup(wi, "5 INF"));

            wi.Modules.Add(fsfran2);

            _db.CurriculumModules.Add(fsfran2);

            var fsfran3 = new CurriculumModule()
            {
                ShortName = "FS Franz 3",
                Name = "Fachsprache Französisch 3",
                ModuleId = "I10",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "KUN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FS Franz3"
                    },
                }
            };

            fsfran3.Groups.Add(GetGroup(wi, "6 BIO"));
            fsfran3.Groups.Add(GetGroup(wi, "6 TEC"));
            fsfran3.Groups.Add(GetGroup(wi, "6 INF"));

            wi.Modules.Add(fsfran3);

            _db.CurriculumModules.Add(fsfran3);

            var wipro = new CurriculumModule()
            {
                ShortName = "Wipro",
                Name = "Wissenschaftliche Projektarbeit",
                ModuleId = "I11",
                Description = "...",
                ECTS = 3,
                MV = GetHost(fk09, "KUR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WisProj"
                    },
                }
            };

            wipro.Groups.Add(GetGroup(wi, "6 BIO"));
            wipro.Groups.Add(GetGroup(wi, "6 TEC"));
            wipro.Groups.Add(GetGroup(wi, "6 INF"));

            wi.Modules.Add(wipro);

            _db.CurriculumModules.Add(wipro);

            var schlqual = new CurriculumModule()
            {
                ShortName = "SchQual",
                Name = "Schlüsselqualifikationen",
                ModuleId = "I12",
                Description = "...",
                ECTS = 2,
                MV = GetHost(fk09, "SCH"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SchlQual"
                    },
                }
            };

            schlqual.Groups.Add(GetGroup(wi, "6 BIO"));
            schlqual.Groups.Add(GetGroup(wi, "6 TEC"));
            schlqual.Groups.Add(GetGroup(wi, "6 INF"));

            wi.Modules.Add(schlqual);

            _db.CurriculumModules.Add(schlqual);


            //Technische Module

            var prod = new CurriculumModule()
            {
                ShortName = "Prod",
                Name = "Produktion",
                ModuleId = "T1",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "KOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Prod"
                    },
                }
            };

            prod.Groups.Add(GetGroup(wi, "3 BIO"));
            prod.Groups.Add(GetGroup(wi, "3 TEC"));
            prod.Groups.Add(GetGroup(wi, "3 INF"));

            wi.Modules.Add(prod);

            _db.CurriculumModules.Add(prod);

            var me2 = new CurriculumModule()
            {
                ShortName = "ME2",
                Name = "Angewandte Technik",
                ModuleId = "T2",
                Description = "...",
                ECTS = 5,
                MV = GetHost(fk09, "ANZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ME2"
                    },
                }
            };

            me2.Groups.Add(GetGroup(wi, "3 BIO"));
            me2.Groups.Add(GetGroup(wi, "3 TEC"));
            me2.Groups.Add(GetGroup(wi, "3 INF"));

            wi.Modules.Add(me2);

            _db.CurriculumModules.Add(me2);

            var autundsens = new CurriculumModule()
            {
                ShortName = "Aut und Sens",
                Name = "Automatisierung und Sensorik",
                ModuleId = "T3",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "GLS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Aut"
                    },
                }
            };

            autundsens.Groups.Add(GetGroup(wi, "4 BIO"));
            autundsens.Groups.Add(GetGroup(wi, "4 TEC"));
            autundsens.Groups.Add(GetGroup(wi, "4 INF"));

            wi.Modules.Add(autundsens);

            _db.CurriculumModules.Add(autundsens);

            var pml1 = new CurriculumModule()
            {
                ShortName = "PML 1",
                Name = "Produktionsmanagement und Logistik 1",
                ModuleId = "T4",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "SPI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PML1"
                    },
                }
            };

            pml1.Groups.Add(GetGroup(wi, "5 BIO"));
            pml1.Groups.Add(GetGroup(wi, "5 TEC"));
            pml1.Groups.Add(GetGroup(wi, "5 INF"));

            wi.Modules.Add(pml1);

            _db.CurriculumModules.Add(pml1);

            var pml2 = new CurriculumModule()
            {
                ShortName = "PML 2",
                Name = "Produktionsmanagement und Logistik 2",
                ModuleId = "T5",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "MER"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PML2"
                    },
                }
            };

            pml2.Groups.Add(GetGroup(wi, "7 BIO"));
            pml2.Groups.Add(GetGroup(wi, "7 TEC"));
            pml2.Groups.Add(GetGroup(wi, "7 INF"));

            wi.Modules.Add(pml2);

            _db.CurriculumModules.Add(pml2);


            //Betriebswirtschaftlische Module

            var kost = new CurriculumModule()
            {
                ShortName = "Kost",
                Name = "Kostenrechnung",
                ModuleId = "B1",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "KRA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kost"
                    },
                }
            };

            kost.Groups.Add(GetGroup(wi, "3 BIO"));
            kost.Groups.Add(GetGroup(wi, "3 TEC"));
            kost.Groups.Add(GetGroup(wi, "3 INF"));

            wi.Modules.Add(kost);

            _db.CurriculumModules.Add(kost);

            var mark = new CurriculumModule()
            {
                ShortName = "Mark",
                Name = "Marketing",
                ModuleId = "B2",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "COR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mark"
                    },
                }
            };

            mark.Groups.Add(GetGroup(wi, "4 BIO"));
            mark.Groups.Add(GetGroup(wi, "4 TEC"));
            mark.Groups.Add(GetGroup(wi, "4 INF"));

            wi.Modules.Add(mark);

            _db.CurriculumModules.Add(mark);


            var fiw = new CurriculumModule()
            {
                ShortName = "FIW",
                Name = "Finanz- und Investitionswirtschaft",
                ModuleId = "B3",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "MCI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FIW-WI"
                    },
                }
            };

            fiw.Groups.Add(GetGroup(wi, "4 BIO"));
            fiw.Groups.Add(GetGroup(wi, "4 TEC"));
            fiw.Groups.Add(GetGroup(wi, "4 INF"));

            wi.Modules.Add(fiw);

            _db.CurriculumModules.Add(fiw);

            var upo = new CurriculumModule()
            {
                ShortName = "UPO",
                Name = "Unternehmensplanung und Organisation",
                ModuleId = "B4",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "ENG"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "UPO"
                    },
                }
            };

            upo.Groups.Add(GetGroup(wi, "5 BIO"));
            upo.Groups.Add(GetGroup(wi, "5 TEC"));
            upo.Groups.Add(GetGroup(wi, "5 INF"));

            wi.Modules.Add(upo);

            _db.CurriculumModules.Add(upo);

            var wiprecht = new CurriculumModule()
            {
                ShortName = "WiP-Recht",
                Name = "Wirtschaftsprivatrecht",
                ModuleId = "B5",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "WLH"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WiP-Recht"
                    },
                }
            };

            wiprecht.Groups.Add(GetGroup(wi, "7 BIO"));
            wiprecht.Groups.Add(GetGroup(wi, "7 TEC"));
            wiprecht.Groups.Add(GetGroup(wi, "7 INF"));

            wi.Modules.Add(wiprecht);

            _db.CurriculumModules.Add(wiprecht);

            var da = new CurriculumModule()
            {
                ShortName = "DA",
                Name = "Datenanalyse",
                ModuleId = "B6",
                Description = "...",
                ECTS = 4,
                MV = GetHost(fk09, "VOEL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DA"
                    },
                }
            };

            da.Groups.Add(GetGroup(wi, "3 BIO"));
            da.Groups.Add(GetGroup(wi, "3 TEC"));
            da.Groups.Add(GetGroup(wi, "3 INF"));

            wi.Modules.Add(da);

            _db.CurriculumModules.Add(da);



            _db.SaveChanges();
        }


    }
}
