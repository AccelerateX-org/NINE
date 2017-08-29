using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogSA(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "SA");

            InitCatalogSA_GS(fk11, sa);
        }

        public void InitCatalogSA_GS(ActivityOrganiser fk11, Curriculum SA)
        {
            var TA = new CurriculumModule()
            {
                ShortName = "TA",
                Name = "Träger und Arbeitsfelder der Sozialen Arbeit",
                ModuleId = "MB_O_1_1",
                Description = "Überblick über die Träger und Einrichtungen der SA",
                ECTS = 5,
                MV = GetHost(fk11, "KAU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TA"
                    },

                }

            };

            SA.Modules.Add(TA);
            _db.CurriculumModules.Add(TA);


            var OfG = new CurriculumModule()
            {
                ShortName = "OfG",
                Name = "Organisatorische und fachpolitische Grundlagen",
                ModuleId = "MB_O_2_1",
                Description = "Das Trägersystem in der Sozialen Arbeit",
                ECTS = 5,
                MV = GetHost(fk11, "KLO"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "OfG"
                    },

                }

            };

            SA.Modules.Add(OfG);
            _db.CurriculumModules.Add(OfG);

            var SP = new CurriculumModule()
            {
                ShortName = "SP",
                Name = "Sozialpolitik",
                ModuleId = "MB_O_3_1",
                Description = "Grundlagen der Sozialpolitik",
                ECTS = 5,
                MV = GetHost(fk11, "YOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SP"
                    },

                }

            };
            SA.Modules.Add(SP);
            _db.CurriculumModules.Add(SP);

            var SOW = new CurriculumModule()
            {
                ShortName = "SOW",
                Name = "Soziale Organisation im Wandel",
                ModuleId = "MB_O_4_1",
                Description = "Organisation der Sozialen Arbeit und aktuelle ökonomische Herausforderungen",
                ECTS = 5,
                MV = GetHost(fk11, "STE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SOW"
                    },

                }

            };
            SA.Modules.Add(SOW);
            _db.CurriculumModules.Add(SOW);

            var WSA1 = new CurriculumModule()
            {
                ShortName = "WSA1",
                Name = "Wissenschaft Soziale Arbeit I",
                ModuleId = "MB_W_1_1",
                Description = "Geschichte/Entstehung der Sozialen Arbeit",
                ECTS = 5,
                MV = GetHost(fk11, "POE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WSA1"
                    },

                }
            
            };
            SA.Modules.Add(WSA1);
            _db.CurriculumModules.Add(WSA1);

            var BW1 = new CurriculumModule()
            {
                ShortName = "BW1",
                Name = "Bezugswissenschaften 1",
                ModuleId = "MB_W_1_2",
                Description = "Erziehungswissenschaften 1 / Psychologie 1",
                ECTS = 5,
                MV = GetHost(fk11, "LIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BW1"
                    },
                 }           
            };
            SA.Modules.Add(BW1);
            _db.CurriculumModules.Add(BW1);


            var WSA2 = new CurriculumModule()
            {
                ShortName = "WSA2",
                Name = "Wissenschaft Soziale Arbeit II",
                ModuleId = "MB_W_2_1",
                Description = "Klassische Theorien der Sozialen Arbeit",
                ECTS = 5,
                MV = GetHost(fk11, "SOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WSA2"
                    },
                    }
            };
            SA.Modules.Add(WSA2);
            _db.CurriculumModules.Add(WSA2);


            var BW2 = new CurriculumModule()
            {
                ShortName = "BW2",
                Name = "Bezugswissenschaften II",
                ModuleId = "MB_W_2_2",
                Description = "Thermenspektrum der Erziehungswissenschaft",
                ECTS = 5,
                MV = GetHost(fk11, "LIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BW2"
                    },
                    }
            };
            SA.Modules.Add(BW2);
            _db.CurriculumModules.Add(BW2);

            var KuK = new CurriculumModule()
            {
                ShortName = "KuK",
                Name = "Kommunikations- und Kulturwissenschaften",
                ModuleId = "MB_W_2_2",
                Description = "Grundlagen der Kommunikations- und Kulturwissenschaften",
                ECTS = 5,
                MV = GetHost(fk11, "HIL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "KuK"
                    },
                    }
            };
            SA.Modules.Add(KuK);
            _db.CurriculumModules.Add(KuK);

            var OEK = new CurriculumModule()
            {
                ShortName = "OEK",
                Name = "Ökonomie",
                ModuleId = "MB_W_2_2",
                Description = "Grundlagen der Ökonomie",
                ECTS = 5,
                MV = GetHost(fk11, "MUT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "OEK"
                    },
                    }
            };
            SA.Modules.Add(OEK);
            _db.CurriculumModules.Add(OEK);

            var GW = new CurriculumModule()
            {
                ShortName = "GW",
                Name = "Gesundheitswissenschaften",
                ModuleId = "MB_W_2_2",
                Description = "Grundlagen der Gesundheitswissenschaften und Sozialemedizin",
                ECTS = 5,
                MV = GetHost(fk11, "JAN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GW"
                    },
                    }
            };
            SA.Modules.Add(GW);
            _db.CurriculumModules.Add(GW);

            var WSA3 = new CurriculumModule()
            {
                ShortName = "WSA3",
                Name = "Wissenschaft Soziale Arbeit III",
                ModuleId = "MB_W_3_1",
                Description = "Klassische Theorien der Sozialen Arbeit",
                ECTS = 5,
                MV = GetHost(fk11, "SAG"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WSA3"
                    },
                    }
            };
            SA.Modules.Add(WSA3);
            _db.CurriculumModules.Add(WSA3);

            var BW3 = new CurriculumModule()
            {
                ShortName = "BW3",
                Name = "Bezugswissenschaften III",
                ModuleId = "MB_W_3_2",
                Description = "Theorien Sozialer Probleme",
                ECTS = 5,
                MV = GetHost(fk11, "MUT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BW3"
                    },
                    }
            };
            SA.Modules.Add(BW3);
            _db.CurriculumModules.Add(BW3);


            var WSA4 = new CurriculumModule()
            {
                ShortName = "WSA4",
                Name = "Wissenschaft Soziale Arbeit IV",
                ModuleId = "MB_W_4_1",
                Description = "Planung und Durchführung eines Forschungs- und Evaluationsprojektes",
                ECTS = 5,
                MV = GetHost(fk11, "SOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WSA4"
                    },
                    }
            };
            SA.Modules.Add(WSA4);
            _db.CurriculumModules.Add(WSA4);

            var EuR = new CurriculumModule()
            {
                ShortName = "EuR",
                Name = "Einführung in Ethik und Recht der Sozialen Arbeits",
                ModuleId = "MB_WN_1_1",
                Description = "Ethische und normative Grundfragen Sozialer Arbeit",
                ECTS = 5,
                MV = GetHost(fk11, "STI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EuR"
                    },
                    }
            };
            SA.Modules.Add(EuR);
            _db.CurriculumModules.Add(EuR);

            var RG1 = new CurriculumModule()
            {
                ShortName = "RG1",
                Name = "Rechtliche Grundlagen I",
                ModuleId = "MB_WN_2_1",
                Description = "Kindschafts- und Jugendhilfrecht",
                ECTS = 5,
                MV = GetHost(fk11, "STI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "RG1"
                    },
                    }
            };
            SA.Modules.Add(RG1);
            _db.CurriculumModules.Add(RG1);

            var RG2 = new CurriculumModule()
            {
                ShortName = "RG2",
                Name = "Rechtliche Grundlagen II",
                ModuleId = "MB_WN_3_1",
                Description = "Sozialrecht II / Ausgewählte Rechtsgebiete",
                ECTS = 5,
                MV = GetHost(fk11, "STI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "RG2"
                    },
                    }
            };
            SA.Modules.Add(RG2);
            _db.CurriculumModules.Add(RG2);

            var EuR3 = new CurriculumModule()
            {
                ShortName = "EuR3",
                Name = "Angewandte Ethik und Rechtliche Grundlagen III",
                ModuleId = "MB_WN_4_1",
                Description = "Sozialrecht 3 / Angewandte Ethik",
                ECTS = 5,
                MV = GetHost(fk11, "STI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EuR3"
                    },
                    }
            };
            SA.Modules.Add(EuR3);
            _db.CurriculumModules.Add(EuR3);

            var MET1 = new CurriculumModule()
            {
                ShortName = "MET1",
                Name = "Methoden I",
                ModuleId = "MB_H_1_1",
                Description = "Einführung in die Methoden der Sozialen Arbeit",
                ECTS = 5,
                MV = GetHost(fk11, "STR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MET1"
                    },
                }
            };
            SA.Modules.Add(MET1);
            _db.CurriculumModules.Add(MET1);

            var MET2 = new CurriculumModule()
            {
                ShortName = "MET2",
                Name = "Methoden II",
                ModuleId = "MB_H_1_2",
                Description = "Mentorat I / Kreativ Medien",
                ECTS = 5,
                MV = GetHost(fk11, "BRU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MET2"
                    },
                    }
            };
            SA.Modules.Add(MET2);
            _db.CurriculumModules.Add(MET2);

            var BH1 = new CurriculumModule()
            {
                ShortName = "BH1",
                Name = "Berufliches Handeln I",
                ModuleId = "MB_H_2_1",
                Description = "Theorie-Praxis-Seminar I",
                ECTS = 5,
                MV = GetHost(fk11, "SIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BH1"
                    },
                }
            };
            SA.Modules.Add(BH1);
            _db.CurriculumModules.Add(BH1);

            var BH2 = new CurriculumModule()
            {
                ShortName = "BH2",
                Name = "Berufliches Handeln II",
                ModuleId = "MB_H_3_1",
                Description = "Theorie-Praxis-Seminar II",
                ECTS = 5,
                MV = GetHost(fk11, "SIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BH2"
                    },
                    }
            };
            SA.Modules.Add(BH2);
            _db.CurriculumModules.Add(BH2);

            var BH3 = new CurriculumModule()
            {
                ShortName = "BH3",
                Name = "Berufliches Handeln III",
                ModuleId = "MB_H_4_1",
                Description = "Theorie-Praxis-Seminar III",
                ECTS = 5,
                MV = GetHost(fk11, "SIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BH3"
                    },
                    }
            };
            SA.Modules.Add(BH3);
            _db.CurriculumModules.Add(BH3);

            var MET3 = new CurriculumModule()
            {
                ShortName = "MET3",
                Name = "Methoden III",
                ModuleId = "MB_H_3_2",
                Description = "Mentorat 3",
                ECTS = 5,
                MV = GetHost(fk11, "GOS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MET3"
                    },
                }
            };
            SA.Modules.Add(MET3);
            _db.CurriculumModules.Add(MET3);


            var MET4 = new CurriculumModule()
            {
                ShortName = "MET4",
                Name = "Methoden IV",
                ModuleId = "MB_H_4_2",
                Description = "Systematische Auswertung",
                ECTS = 5,
                MV = GetHost(fk11, "KLO"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MET4"
                    },
                }
            };
            SA.Modules.Add(MET4);
            _db.CurriculumModules.Add(MET4);


            var PM = new CurriculumModule()
            {
                ShortName = "PM",
                Name = "Praxismodul",
                ModuleId = "MB_H_5_1",
                Description = "Praxiserfahrung",
                ECTS = 25,
                MV = GetHost(fk11, "SIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PM"
                    },
                }
                };
            SA.Modules.Add(PM);
            _db.CurriculumModules.Add(PM);


            var PB = new CurriculumModule()
            {
                ShortName = "PB",
                Name = "Praxisbegleitung",
                ModuleId = "MB_H_5_2",
                Description = "Ablauforganisation und Prozessabläufe",
                ECTS = 5,
                MV = GetHost(fk11, "AMU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PB"
                    },
                }
            };
            SA.Modules.Add(PB);
            _db.CurriculumModules.Add(PB);


            var QO = new CurriculumModule()
            {
                ShortName = "QO",
                Name = "Oulifizierungsbereichsspezifische Organisationsfragen",
                ModuleId = "MB_O_6_1 BKM",
                Description = "Organisation von Kultur- und Medieneinrichtungen",
                ECTS = 5,
                MV = GetHost(fk11, "HIL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "QO"
                    },
                }
                };
            SA.Modules.Add(QO);
            _db.CurriculumModules.Add(QO);


            var QT = new CurriculumModule()
            {
                ShortName = "QT",
                Name = "Oulifizierungsbereichsspezifische Theroiefragen I",
                ModuleId = "MB_W_6_1 BKM",
                Description = "Kulturelle Bildung / Kulturpädagogik",
                ECTS = 5,
                MV = GetHost(fk11, "HIL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "QT"
                    },
                }
                };
            SA.Modules.Add(QT);
            _db.CurriculumModules.Add(QT);


            var QT2 = new CurriculumModule()
            {
                ShortName = "QT2",
                Name = "Oulifizierungsbereichsspezifische Theroiefragen II",
                ModuleId = "MB_W_6_2 BKM",
                Description = "Kommunikationstheorie / Medientheorie",
                ECTS = 5,
                MV = GetHost(fk11, "HIL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "QT2"
                    },

                }
            };
            SA.Modules.Add(QT2);
            _db.CurriculumModules.Add(QT2);

            var QW = new CurriculumModule()
            {
                ShortName = "QW",
                Name = "Oulifizierungsbereichsspezifische Wertefragen",
                ModuleId = "MB_WN_6_1 BKM",
                Description = "Ästhetische Phänomene",
                ECTS = 5,
                MV = GetHost(fk11, "HIL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "QW"
                    },
                }
                };
            SA.Modules.Add(QW);
            _db.CurriculumModules.Add(QW);


            var QH = new CurriculumModule()
            {
                ShortName = "QH",
                Name = "Oulifizierungsbereichsspezifische Handlungsansätze",
                ModuleId = "MB_H_6_1 BKM",
                Description = "Bedarfsermittlung / Projektmanagment",
                ECTS = 5,
                MV = GetHost(fk11, "HIL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "QH"
                    },
                    }
            };
            SA.Modules.Add(QH);
            _db.CurriculumModules.Add(QH);


            var QT3 = new CurriculumModule()
            {
                ShortName = "QT3",
                Name = "Oulifizierungsbereichsspezifische Theroiefragen III",
                ModuleId = "MB_W_7_2 BKM",
                Description = "Mediensozialisation",
                ECTS = 5,
                MV = GetHost(fk11, "HIL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "QT3"
                    },
}
            };
            SA.Modules.Add(QT3);
            _db.CurriculumModules.Add(QT3);


            var QOcs = new CurriculumModule()
            {
                ShortName = "QO",
                Name = "Oulifizierungsbereichsspezifische Organisationsfragen",
                ModuleId = "MB_O_6_1 cs",
                Description = "Kommunikationstheorie / Medientheorie",
                ECTS = 5,
                MV = GetHost(fk11, "KLO"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "QO"
                    },
                  }
            };
            SA.Modules.Add(QOcs);
            _db.CurriculumModules.Add(QOcs);


            var MET = new CurriculumModule()
            {
                ShortName = "MET4.1",
                Name = "Methode V: Vertiefende Methodenangebote",
                ModuleId = "MB_W_6_2",
                Description = "analytische und handlungsorientierte Methoden",
                ECTS = 5,
                MV = GetHost(fk11, "SIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MET4.1"
                    },
}
            };
            SA.Modules.Add(MET);
            _db.CurriculumModules.Add(MET);

            var SGZ = new CurriculumModule()
            {
                ShortName = "SGZ",
                Name = "Sozialpolitische Gegenwarts- und Zukunftsfragen",
                ModuleId = "MB_WN_7_1",
                Description = "Diskurs / Diskussion",
                ECTS = 5,
                MV = GetHost(fk11, "YOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SGZ"
                    },
}
            };
            SA.Modules.Add(SGZ);
            _db.CurriculumModules.Add(SGZ);


            var WW = new CurriculumModule()
            {
                ShortName = "WW",
                Name = "Wissenschaftswerkstatt",
                ModuleId = "MB_W_7_2 ",
                Description = "Lektürekurs",
                ECTS = 5,
                MV = GetHost(fk11, "SO"),        
           ModuleCourses = new List<ModuleCourse>
      {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WW"
                    },
                    }
            };
            SA.Modules.Add(WW);
            _db.CurriculumModules.Add(WW);

            var BA = new CurriculumModule()
            {
                ShortName = "BA",
                Name = "Bachelorarbeit",
                ModuleId = "MB_W_7_2",
                Description = "Absprache mit Prüfungskommission",
                ECTS = 5,
                MV = GetHost(fk11, "N.n."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BA"
                    },
}
            };
            SA.Modules.Add(BA);
            _db.CurriculumModules.Add(BA);

            _db.SaveChanges();

        }
    }
}