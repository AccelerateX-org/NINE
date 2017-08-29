using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Curriculum.FK09_WI
{
    public class ModuleCatalogFK09WI : IModuleCatalog
    {
        public string Organiser
        {
            get { return "FK 09"; }
        }

        public string ShortName
        {
            get { return "WI"; }
        }


        private OrganiserMember GetHost(string name)
        {
            return new OrganiserMember
            {
                ShortName = name,
            };
        }

        private CurriculumGroup GetGroup(string name)
        {
            return new CurriculumGroup
            {
                Name = name
            };
        }


        public ICollection<Data.CurriculumModule> GetModules(string options)
        {
            var allModules = new List<CurriculumModule>();

            allModules.AddRange(GetModulesGrundstudium());
            allModules.AddRange(GetModulesIntegration());
            allModules.AddRange(GetModulesStudienrichtungen());
            allModules.AddRange(GetModulesAA());

            return allModules;
        }

        /*
        private IEnumerable<CurriculumModule> GetModules_Dummy()
        {
            var allModules = new List<CurriculumModule>
            {
            };
            return allModules;
        }
        */

        private IEnumerable<CurriculumModule> GetModulesGrundstudium()
        {
            var allModules = new List<CurriculumModule>
            {
                new CurriculumModule
                {
                    ShortName = "Mathe 1",
                    Name = "Mathematik 1",
                    ModuleId = "GS!1", // Geht ins Kriterium: Format [Kriterium]![Nummer o.ä.]
                    Description = "Rechnen",
                    ECTS = 6,
                    MV = GetHost("VOEL"), // wird bei der Akkreditierung übernommen
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Lecture,
                            ExternalId = "Mathe1 Vorl"
                        },
                        new ModuleCourse
                        {
                            CourseType = CourseType.Practice,
                            ExternalId = "MatheÜB",
                            CapacityCourses = new List<CapacityCourse>
                            {
                                new CapacityCourse
                                {
                                    ShortName = "MatheÜB-G1"
                                },
                                new CapacityCourse
                                {
                                    ShortName = "MatheÜB-G2"
                                }
                            }
                        }
                    },
                    Groups = new List<CurriculumGroup> // werden bei der Akkreditierung übernommen
                    {
                        GetGroup("1")
                    }
                },
                new CurriculumModule
                {
                    ShortName = "Mathe 2",
                    Name = "Mathematik 2",
                    ModuleId = "GS!2",
                    Description = "mehr Rechnen",
                    ECTS = 5,
                    MV = GetHost("VOEL"),
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Seminar,
                            ExternalId = "Mathe2"
                        }
                    },
                    Groups = new List<CurriculumGroup>
                    {
                        GetGroup("2")
                    }

                },
                new CurriculumModule
                {
                    ShortName = "TM",
                    Name = "Technische Mechanik",
                    ModuleId = "GS!3",
                    Description = "Maschinen berechnen",
                    ECTS = 5,
                    MV = GetHost("ANZ"),
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Seminar,
                            ExternalId = "TM"
                        }
                    },
                        Groups = new List<CurriculumGroup>
                        {
                            GetGroup("1")
                        }

                },
                new CurriculumModule()
                {
                    ShortName = "Physik",
                    Name = "Physik",
                    ModuleId = "GS!4",
                    Description = "Mechanik, Thermo",
                    ECTS = 5,
                    MV = GetHost("REB"),
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Seminar,
                            ExternalId = "Physik"
                        }
                    },
                            Groups = new List<CurriculumGroup>
                            {
                                GetGroup("2")
                            }

                },
                new CurriculumModule()
                {
                    ShortName = "Chemie/Wkst",
                    Name = "Chemie und Werkstoffe",
                    ModuleId = "GS!5",
                    Description = "organ./anorgan. Chemie, Werkstoffgrundlagen",
                    ECTS = 4,
                    MV = GetHost("RAB"),
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Seminar,
                            ExternalId = "Chemie"
                        }
                    },
                            Groups = new List<CurriculumGroup>
                            {
                                GetGroup("1")
                            }
                },
                new CurriculumModule()
                {
                    ShortName = "WT",
                    Name = "Werkstofftechnik",
                    ModuleId = "GS!6",
                    Description = "Werkstoffprüfung und Eigenschaften",
                    ECTS = 4,
                    MV = GetHost("RAB"),
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Seminar,
                            ExternalId = "Werkstoffe"
                        }
                    },
                            Groups = new List<CurriculumGroup>
                            {
                                GetGroup("2")
                            }

                },
                new CurriculumModule()
                {
                    ShortName = "E-Tech",
                    Name = "Elektrotechnik",
                    ModuleId = "GS!7",
                    Description = "Gleich- und Wechselstromlehre",
                    ECTS = 5,
                    MV = GetHost("PIR"),
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Seminar,
                            ExternalId = "ETech"
                        }
                    },
                            Groups = new List<CurriculumGroup>
                            {
                                GetGroup("2")
                            }
                },
                new CurriculumModule()
                {
                    ShortName = "TZ",
                    Name = "Technisches Zeichnen",
                    ModuleId = "GS!8",
                    Description = "Grundlagen des technischen Zeichnens",
                    ECTS = 4,
                    MV = GetHost("SCU"),
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Seminar,
                            ExternalId = "TZ"
                        }
                    },
                            Groups = new List<CurriculumGroup>
                            {
                                GetGroup("1")
                            }

                },
                new CurriculumModule()
                {
                    ShortName = "ME1",
                    Name = "Maschinenelemente 1",
                    ModuleId = "GS!9",
                    Description = "Schraubenberechnung, Verfahrenstechnik",
                    ECTS = 5,
                    MV = GetHost("DAE"),
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Seminar,
                            ExternalId = "ME1"
                        }
                    },
                            Groups = new List<CurriculumGroup>
                            {
                                GetGroup("2")
                            }

                },
                new CurriculumModule()
                {
                    ShortName = "BWL",
                    Name = "Betriebswirtschaftslehre",
                    ModuleId = "GS!10",
                    Description = "Grundlagen der BWL",
                    ECTS = 4,
                    MV = GetHost("SAC"),
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Seminar,
                            ExternalId = "BWL"
                        }
                    },
                            Groups = new List<CurriculumGroup>
                            {
                                GetGroup("1")
                            }
                },
                new CurriculumModule()
                {
                    ShortName = "BuBi",
                    Name = "Buchhaltung und Bilanzierung",
                    ModuleId = "GS!11",
                    Description = "Bilanzierungsgrundlagen",
                    ECTS = 4,
                    MV = GetHost("ENB"),
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Seminar,
                            ExternalId = "BuBi"
                        }
                    },
                            Groups = new List<CurriculumGroup>
                            {
                                GetGroup("2")
                            }
                },
                new CurriculumModule()
                {
                    ShortName = "GdI",
                    Name = "Grundlagen der Informatik",
                    ModuleId = "GS!12",
                    Description = "Programmieren lernen",
                    ECTS = 5,
                    MV = GetHost("HIN"),
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Seminar,
                            ExternalId = "Inform",
                            CapacityCourses = new List<CapacityCourse>
                            {
                                new CapacityCourse
                                {
                                    ShortName = "Inform-G1"
                                },
                                new CapacityCourse
                                {
                                    ShortName = "Inform-G2"
                                }
                            }

                        }
                    },
                            Groups = new List<CurriculumGroup>
                            {
                                GetGroup("1")
                            }

                },
                new CurriculumModule()
                {
                    ShortName = "VWL",
                    Name = "Volkswirtschaftslehre",
                    ModuleId = "GS!13",
                    Description = "Mikro- und Makroökonomische Zusammenhänge",
                    ECTS = 4,
                    MV = GetHost("WOL"),
                    ModuleCourses = new List<ModuleCourse>
                    {
                        new ModuleCourse
                        {
                            CourseType = CourseType.Seminar,
                            ExternalId = "VWL"
                        },
                    },
                            Groups = new List<CurriculumGroup>
                            {
                                GetGroup("2")
                            }

                }
            };

            return allModules;
        }

        private IEnumerable<CurriculumModule> GetModulesIntegration()
        {
            var allModules = new List<CurriculumModule>();

            var infosys = new CurriculumModule()
            {
                ShortName = "InfoSys",
                Name = "Informationssysteme",
                ModuleId = "I!1",
                Description = "Überblich über verwendete Inofsysteme",
                ECTS = 4,
                MV = GetHost("TEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "InfoSys"
                    },
                }
            };

            infosys.Groups.Add(GetGroup("3 BIO"));
            infosys.Groups.Add(GetGroup("3 TEC"));
            infosys.Groups.Add(GetGroup("3 INF"));

            allModules.Add(infosys);

            var ergo = new CurriculumModule()
            {
                ShortName = "Ergo",
                Name = "Ergonomie mit Praktikum",
                ModuleId = "I!2",
                Description = "....",
                ECTS = 3,
                MV = GetHost("BRB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ergo"
                    },
                }
            };

            ergo.Groups.Add(GetGroup("4 BIO"));
            ergo.Groups.Add(GetGroup("4 TEC"));
            ergo.Groups.Add(GetGroup("4 INF"));

            allModules.Add(ergo);

            var ppqm = new CurriculumModule()
            {
                ShortName = "PPQM",
                Name = "Projekt- und Qualitätsmanagmet",
                ModuleId = "I!3",
                Description = "...",
                ECTS = 5,
                MV = GetHost("SCU"),
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

            ppqm.Groups.Add(GetGroup("5 BIO"));
            ppqm.Groups.Add(GetGroup("5 TEC"));
            ppqm.Groups.Add(GetGroup("5 INF"));

            allModules.Add(ppqm);

            var persorgentw = new CurriculumModule()
            {
                ShortName = "PersOrgEntw",
                Name = "Personal- und Organisationsentwicklug",
                ModuleId = "I!4",
                Description = "...",
                ECTS = 4,
                MV = GetHost("OST"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PersOrgEntw"
                    },
                }

            };

            persorgentw.Groups.Add(GetGroup("7 BIO"));
            persorgentw.Groups.Add(GetGroup("7 TEC"));
            persorgentw.Groups.Add(GetGroup("7 INF"));

            allModules.Add(persorgentw);

            var fseng1 = new CurriculumModule()
            {
                ShortName = "FS Englisch 1",
                Name = "Fachsprache Englisch 1",
                ModuleId = "I!5",
                Description = "...",
                ECTS = 4,
                MV = GetHost("TMF"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FS Engl Hi1"
                    },
                }
            };

            fseng1.Groups.Add(GetGroup("4 BIO"));
            fseng1.Groups.Add(GetGroup("4 TEC"));
            fseng1.Groups.Add(GetGroup("4 INF"));

            allModules.Add(fseng1);

            var fseng2 = new CurriculumModule()
            {
                ShortName = "FS Englisch 2",
                Name = "Fachsprache Englisch 2",
                ModuleId = "I!6",
                Description = "...",
                ECTS = 4,
                MV = GetHost("TMF"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FS Engl Hi2"
                    },
                }
            };

            fseng2.Groups.Add(GetGroup("5 BIO"));
            fseng2.Groups.Add(GetGroup("5 TEC"));
            fseng2.Groups.Add(GetGroup("5 INF"));

            allModules.Add(fseng2);

            var fseng3 = new CurriculumModule()
            {
                ShortName = "FS Englisch 3",
                Name = "Fachsprache Englisch 3",
                ModuleId = "I!7",
                Description = "...",
                ECTS = 4,
                MV = GetHost("TMF"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FS Engl Van"
                    },
                }
            };

            fseng3.Groups.Add(GetGroup("6 BIO"));
            fseng3.Groups.Add(GetGroup("6 TEC"));
            fseng3.Groups.Add(GetGroup("6 INF"));

            allModules.Add(fseng3);

            var fsfran1 = new CurriculumModule()
            {
                ShortName = "FS Franz 1",
                Name = "Fachsprache Französisch 1",
                ModuleId = "I!8",
                Description = "...",
                ECTS = 4,
                MV = GetHost("KUN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FS Franz1"
                    },
                }
            };

            fsfran1.Groups.Add(GetGroup("4 BIO"));
            fsfran1.Groups.Add(GetGroup("4 TEC"));
            fsfran1.Groups.Add(GetGroup("4 INF"));

            allModules.Add(fsfran1);

            var fsfran2 = new CurriculumModule()
            {
                ShortName = "FS Franz 2",
                Name = "Fachsprache Französisch 2",
                ModuleId = "I!9",
                Description = "...",
                ECTS = 4,
                MV = GetHost("KUN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FS Franz2"
                    },
                }
            };

            fsfran2.Groups.Add(GetGroup("5 BIO"));
            fsfran2.Groups.Add(GetGroup("5 TEC"));
            fsfran2.Groups.Add(GetGroup("5 INF"));

            allModules.Add(fsfran2);

            var fsfran3 = new CurriculumModule()
            {
                ShortName = "FS Franz 3",
                Name = "Fachsprache Französisch 3",
                ModuleId = "I!10",
                Description = "...",
                ECTS = 4,
                MV = GetHost("KUN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FS Franz3"
                    },
                }
            };

            fsfran3.Groups.Add(GetGroup("6 BIO"));
            fsfran3.Groups.Add(GetGroup("6 TEC"));
            fsfran3.Groups.Add(GetGroup("6 INF"));

            allModules.Add(fsfran3);

            var wipro = new CurriculumModule()
            {
                ShortName = "Wipro",
                Name = "Wissenschaftliche Projektarbeit",
                ModuleId = "I!11",
                Description = "...",
                ECTS = 3,
                MV = GetHost("KUR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WisProj"
                    },
                }
            };

            wipro.Groups.Add(GetGroup("6 BIO"));
            wipro.Groups.Add(GetGroup("6 TEC"));
            wipro.Groups.Add(GetGroup("6 INF"));

            allModules.Add(wipro);

            var schlqual = new CurriculumModule()
            {
                ShortName = "SchQual",
                Name = "Schlüsselqualifikationen",
                ModuleId = "I!12",
                Description = "...",
                ECTS = 2,
                MV = GetHost("SCH"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SchlQual"
                    },
                }
            };

            schlqual.Groups.Add(GetGroup("6 BIO"));
            schlqual.Groups.Add(GetGroup("6 TEC"));
            schlqual.Groups.Add(GetGroup("6 INF"));

            allModules.Add(schlqual);


            //Technische Module

            var prod = new CurriculumModule()
            {
                ShortName = "Prod",
                Name = "Produktion",
                ModuleId = "T!1",
                Description = "...",
                ECTS = 4,
                MV = GetHost("KOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Prod"
                    },
                }
            };

            prod.Groups.Add(GetGroup("3 BIO"));
            prod.Groups.Add(GetGroup("3 TEC"));
            prod.Groups.Add(GetGroup("3 INF"));

            allModules.Add(prod);

            var me2 = new CurriculumModule()
            {
                ShortName = "ME2",
                Name = "Angewandte Technik",
                ModuleId = "T!2",
                Description = "...",
                ECTS = 5,
                MV = GetHost("ANZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ME2"
                    },
                }
            };

            me2.Groups.Add(GetGroup("3 BIO"));
            me2.Groups.Add(GetGroup("3 TEC"));
            me2.Groups.Add(GetGroup("3 INF"));

            allModules.Add(me2);

            var autundsens = new CurriculumModule()
            {
                ShortName = "Aut und Sens",
                Name = "Automatisierung und Sensorik",
                ModuleId = "T!3",
                Description = "...",
                ECTS = 4,
                MV = GetHost("GLS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Aut"
                    },
                }
            };

            autundsens.Groups.Add(GetGroup("4 BIO"));
            autundsens.Groups.Add(GetGroup("4 TEC"));
            autundsens.Groups.Add(GetGroup("4 INF"));

            allModules.Add(autundsens);

            var pml1 = new CurriculumModule()
            {
                ShortName = "PML 1",
                Name = "Produktionsmanagement und Logistik 1",
                ModuleId = "T!4",
                Description = "...",
                ECTS = 4,
                MV = GetHost("SPI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PML1"
                    },
                }
            };

            pml1.Groups.Add(GetGroup("5 BIO"));
            pml1.Groups.Add(GetGroup("5 TEC"));
            pml1.Groups.Add(GetGroup("5 INF"));

            allModules.Add(pml1);

            var pml2 = new CurriculumModule()
            {
                ShortName = "PML 2",
                Name = "Produktionsmanagement und Logistik 2",
                ModuleId = "T!5",
                Description = "...",
                ECTS = 4,
                MV = GetHost("MER"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PML2"
                    },
                }
            };

            pml2.Groups.Add(GetGroup("7 BIO"));
            pml2.Groups.Add(GetGroup("7 TEC"));
            pml2.Groups.Add(GetGroup("7 INF"));

            allModules.Add(pml2);

            //Betriebswirtschaftlische Module

            var kost = new CurriculumModule()
            {
                ShortName = "Kost",
                Name = "Kostenrechnung",
                ModuleId = "B!1",
                Description = "...",
                ECTS = 4,
                MV = GetHost("KRA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kost"
                    },
                }
            };

            kost.Groups.Add(GetGroup("3 BIO"));
            kost.Groups.Add(GetGroup("3 TEC"));
            kost.Groups.Add(GetGroup("3 INF"));

            allModules.Add(kost);

            var mark = new CurriculumModule()
            {
                ShortName = "Mark",
                Name = "Marketing",
                ModuleId = "B!2",
                Description = "...",
                ECTS = 4,
                MV = GetHost("COR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mark"
                    },
                }
            };

            mark.Groups.Add(GetGroup("4 BIO"));
            mark.Groups.Add(GetGroup("4 TEC"));
            mark.Groups.Add(GetGroup("4 INF"));

            allModules.Add(mark);

            var fiw = new CurriculumModule()
            {
                ShortName = "FIW",
                Name = "Finanz- und Investitionswirtschaft",
                ModuleId = "B!3",
                Description = "...",
                ECTS = 4,
                MV = GetHost("MCI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FIW-WI"
                    },
                }
            };

            fiw.Groups.Add(GetGroup("4 BIO"));
            fiw.Groups.Add(GetGroup("4 TEC"));
            fiw.Groups.Add(GetGroup("4 INF"));

            allModules.Add(fiw);

            var upo = new CurriculumModule()
            {
                ShortName = "UPO",
                Name = "Unternehmensplanung und Organisation",
                ModuleId = "B!4",
                Description = "...",
                ECTS = 4,
                MV = GetHost("ENG"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "UPO"
                    },
                }
            };

            upo.Groups.Add(GetGroup("5 BIO"));
            upo.Groups.Add(GetGroup("5 TEC"));
            upo.Groups.Add(GetGroup("5 INF"));

            allModules.Add(upo);

            var wiprecht = new CurriculumModule()
            {
                ShortName = "WiP-Recht",
                Name = "Wirtschaftsprivatrecht",
                ModuleId = "B!5",
                Description = "...",
                ECTS = 4,
                MV = GetHost("WLH"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WiP-Recht"
                    },
                }
            };

            wiprecht.Groups.Add(GetGroup("7 BIO"));
            wiprecht.Groups.Add(GetGroup("7 TEC"));
            wiprecht.Groups.Add(GetGroup("7 INF"));

            allModules.Add(wiprecht);

            var da = new CurriculumModule()
            {
                ShortName = "DA",
                Name = "Datenanalyse",
                ModuleId = "B!6",
                Description = "...",
                ECTS = 4,
                MV = GetHost("VOEL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DA"
                    },
                }
            };

            da.Groups.Add(GetGroup("3 BIO"));
            da.Groups.Add(GetGroup("3 TEC"));
            da.Groups.Add(GetGroup("3 INF"));

            allModules.Add(da);

            return allModules;
        }


        private IEnumerable<CurriculumModule> GetModulesStudienrichtungen()
        {
            var allModules = new List<CurriculumModule>();

            //Industrielle Technik Module

            var vut = new CurriculumModule()
            {
                ShortName = "VUT",
                Name = "Verfahrens- und Umwelttechnik",
                ModuleId = "IND!1",
                Description = "Verfahrens- und Umwelttechnik lernen",
                ECTS = 4,
                MV = GetHost("HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "VUT"
                    },
                }
            };

            vut.Groups.Add(GetGroup("3 TEC"));

            allModules.Add(vut);

            var entech = new CurriculumModule()
            {
                ShortName = "EnTech",
                Name = "Energietechnik",
                ModuleId = "IND!2",
                Description = "Energietechnik lernen",
                ECTS = 4,
                MV = GetHost("MAU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EnTech"
                    },
                }
            };

            entech.Groups.Add(GetGroup("3 TEC"));

            allModules.Add(entech);

            var entkon = new CurriculumModule()
            {
                ShortName = "EntKon",
                Name = "Entwicklung und Konstruktion mit CAD",
                ModuleId = "IND!3",
                Description = "Entwicklung und Konstruktion mit CAD lernen",
                ECTS = 4,
                MV = GetHost("ANZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EntKon"
                    },
                }
            };

            entkon.Groups.Add(GetGroup("4 TEC"));

            allModules.Add(entkon);

            var ferttech = new CurriculumModule()
            {
                ShortName = "FertTech",
                Name = "Fertigungstechnik",
                ModuleId = "IND!4",
                Description = "Fertigungstechnik lernen",
                ECTS = 4,
                MV = GetHost("KOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FertTech"
                    },
                }
            };

            ferttech.Groups.Add(GetGroup("4 TEC"));

            allModules.Add(ferttech);

            var fertautprak = new CurriculumModule()
            {
                ShortName = "FertAutPrak",
                Name = "Fertigungstechnik und Automatisierung mit Praktikum",
                ModuleId = "IND!5",
                Description = "Vorlesung und Prakitkum dazu",
                ECTS = 4,
                MV = GetHost("PIR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FertTech Prak"
                    },
                }
            };

            fertautprak.Groups.Add(GetGroup("5 TEC"));

            allModules.Add(fertautprak);

            var plm = new CurriculumModule()
            {
                ShortName = "PLM",
                Name = "Product Lifecycle Management",
                ModuleId = "IND!6",
                Description = "Product Lifecycle Management lernen",
                ECTS = 4,
                MV = GetHost("SCN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ProdLife"
                    },
                }
            };

            plm.Groups.Add(GetGroup("5 TEC"));

            allModules.Add(plm);

            //Informationstechnik Module

            var daba = new CurriculumModule()
            {
                ShortName = "DB",
                Name = "Datenbanken in Technik und Wirtschaft",
                ModuleId = "INF!1",
                Description = "Datenbanken lernen",
                ECTS = 3,
                MV = GetHost("TEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DB"
                    },
                }
            };

            daba.Groups.Add(GetGroup("3 INF"));

            allModules.Add(daba);

            var se1 = new CurriculumModule()
            {
                ShortName = "SE1",
                Name = "Software Engineering 1",
                ModuleId = "INF!2",
                Description = "Programmieren lernen",
                ECTS = 5,
                MV = GetHost("HIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SoftIng1"
                    },
                }
            };

            se1.Groups.Add(GetGroup("3 INF"));

            allModules.Add(se1);

            var se2 = new CurriculumModule()
            {
                ShortName = "SE2",
                Name = "Software Engineering 2",
                ModuleId = "INF!3",
                Description = "nochmal Programmieren lernen",
                ECTS = 4,
                MV = GetHost("SCN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SoftIng2"
                    },
                }
            };

            se2.Groups.Add(GetGroup("4 INF"));

            allModules.Add(se2);

            var itproj1 = new CurriculumModule()
            {
                ShortName = "ITProj1",
                Name = "IT-Projektseminar 1",
                ModuleId = "INF!4",
                Description = "Projektarbeit",
                ECTS = 4,
                MV = GetHost("HIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "IT-Proj-1"
                    },
                }
            };

            itproj1.Groups.Add(GetGroup("4 INF"));

            allModules.Add(itproj1);

            var itproj2 = new CurriculumModule()
            {
                ShortName = "ITProj2",
                Name = "IT-Projektseminar 2",
                ModuleId = "INF!5",
                Description = "nochmal Projektarbeit",
                ECTS = 4,
                MV = GetHost("HIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "IT-Proj-2"
                    },
                }
            };

            itproj2.Groups.Add(GetGroup("5 INF"));

            allModules.Add(itproj2);

            var embsys = new CurriculumModule()
            {
                ShortName = "EmbSys",
                Name = "Embedded Systems",
                ModuleId = "INF!6",
                Description = "Embedded Systems lernen",
                ECTS = 4,
                MV = GetHost("HIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EmbSys"
                    },
                }
            };

            embsys.Groups.Add(GetGroup("5 INF"));

            allModules.Add(embsys);

            //Biotechnologie Module

            var bioprak = new CurriculumModule()
            {
                ShortName = "BioPrak",
                Name = "Biotechnologisches Praktikum",
                ModuleId = "BIO!1",
                Description = "Praktikum",
                ECTS = 4,
                MV = GetHost("TRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Practice,
                        ExternalId = "Bio-Prakt"
                    },
                }
            };

            bioprak.Groups.Add(GetGroup("5 BIO"));

            allModules.Add(bioprak);

            var mobi = new CurriculumModule()
            {
                ShortName = "MoBi",
                Name = "Molekularbiologie",
                ModuleId = "BIO!2",
                Description = "Molekularbiologie lernen",
                ECTS = 4,
                MV = GetHost("TRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MoBi"
                    },
                }
            };

            mobi.Groups.Add(GetGroup("3 BIO"));

            allModules.Add(mobi);


            var indbio = new CurriculumModule()
            {
                ShortName = "IndBio",
                Name = "Industrielle Biotechnologie",
                ModuleId = "BIO!3",
                Description = "Industrielle Biotechnologie lernen",
                ECTS = 4,
                MV = GetHost("HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "IndBiotechn"
                    },
                }
            };

            indbio.Groups.Add(GetGroup("4 BIO"));

            allModules.Add(indbio);

            var biovt = new CurriculumModule()
            {
                ShortName = "BioVT",
                Name = "Bioverfahrenstechnik",
                ModuleId = "BIO!4",
                Description = "Bioverfahrenstechnik lernen",
                ECTS = 4,
                MV = GetHost("HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bio-VT"
                    },
                }
            };

            biovt.Groups.Add(GetGroup("4 BIO"));

            allModules.Add(biovt);

            var nawaro = new CurriculumModule()
            {
                ShortName = "NaWaRo",
                Name = "Nachwachsende Rohstoffe",
                ModuleId = "BIO!5",
                Description = "Rohstoffe lernen",
                ECTS = 4,
                MV = GetHost("HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "NaWaRo"
                    },
                }
            };

            nawaro.Groups.Add(GetGroup("5 BIO"));

            allModules.Add(nawaro);

            var techumw = new CurriculumModule()
            {
                ShortName = "TechUmw",
                Name = "Technischer Umweltschutz",
                ModuleId = "BIO!6",
                Description = "Technischer Umweltschutz lernen",
                ECTS = 4,
                MV = GetHost("TRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Tech Umw"
                    },
                }
            };

            techumw.Groups.Add(GetGroup("3 BIO"));

            allModules.Add(techumw);

            return allModules;
        }


        private IEnumerable<CurriculumModule> GetModulesAA()
        {
            var allModules = new List<CurriculumModule>
            {
                new CurriculumModule()
                {
                    ShortName = "BAC",
                    Name = "Abschlussarbeit",
                    ModuleId = "AA!1",
                    Description = "",
                    ECTS = 12,
                    MV = GetHost("KRA"),            // der PK-Vorsitzende
                    Groups = new List<CurriculumGroup>
                    {
                        GetGroup("7 BIO"),
                        GetGroup("7 INF"),
                        GetGroup("7 BIO")
                    }
                }
            };
            return allModules;
        }

    }
}
