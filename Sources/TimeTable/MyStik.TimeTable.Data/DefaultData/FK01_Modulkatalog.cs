using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        private void InitModulkatalogAR(ActivityOrganiser fk01)
        {
            var ar = GetCurriculum(fk01, "AR");

            InitCatalogAR_GS(fk01, ar);
        }



        private void InitCatalogAR_GS(ActivityOrganiser fk01, Curriculum ar)
        {

            var entwurf1 = new CurriculumModule()
            {
                ShortName = "Entwurf 1",
                Name = "Entwurf 1",
                ModuleId = "BA01",
                Description = "Konstruktion und Technik",
                ECTS = 6,
                MV = GetHost(fk01, "BOT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Entwurf1"
                    },
                }

            };

            var wahrnehmenunddarstellung1 = new CurriculumModule()
            {
                ShortName = "Wahrnehmen und Darstellen 1",
                Name = "Wahrnehmen und Darstellen 1",
                ModuleId = "BA02",
                Description = "Gestaltung",
                ECTS = 5,
                MV = GetHost(fk01, "LAN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WuD1"
                    },
                }

            };

            var grundlagenkonstruktion1 = new CurriculumModule()
            {
                ShortName = "Grundlagen Konstruktion 1",
                Name = "Grundlagen Konstruktion 1",
                ModuleId = "BA03",
                Description = "Konstruktion + Technik",
                ECTS = 5,
                MV = GetHost(fk01, "SCH"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GK1"
                    },
                }

            };

            var entwurfsgrundlagen1 = new CurriculumModule()
            {
                ShortName = "Entwurfsgrundlagen 1",
                Name = "Entwurfsgrundlagen 1",
                ModuleId = "BA04",
                Description = "Architektur",
                ECTS = 5,
                MV = GetHost(fk01, "BOT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Entwurfgrd1"
                    },
                }

            };

            var dasphaenomenderstadt = new CurriculumModule()
            {
                ShortName = "Das Phänomen der Stadt",
                Name = "Das Phänomen der Stadt",
                ModuleId = "BA05",
                Description = "Städtebau",
                ECTS = 5,
                MV = GetHost(fk01, "KRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Phän Stadt"
                    },
                }

            };

            var entwurf2 = new CurriculumModule()
            {
                ShortName = "Entwurf 2",
                Name = "Entwurf 2",
                ModuleId = "BA06",
                Description = "Architektur",
                ECTS = 10,
                MV = GetHost(fk01, "MEC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Entwurf2"
                    },
                }

            };

            var wahrnehmenunddarstellung2 = new CurriculumModule()
            {
                ShortName = "Wahrnehmen und Darstellen 2",
                Name = "Wahrnehmen und Darstellen 2",
                ModuleId = "BA07",
                Description = "Gestaltung",
                ECTS = 5,
                MV = GetHost(fk01, "BEE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WuD2"
                    },
                }

            };

            var entwurfsgrundlagen2 = new CurriculumModule()
            {
                ShortName = "Entwurfsgrundlagen 2",
                Name = "Entwurfsgrundlagen 2",
                ModuleId = "BA09",
                Description = "Architektur",
                ECTS = 5,
                MV = GetHost(fk01, "MEC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Entwurfgrd2"
                    },
                }

            };

            var grundlagenkonstruktion2 = new CurriculumModule()
            {
                ShortName = "Grundlagen Konstruktion 2",
                Name = "Grundlagen Konstruktion 2",
                ModuleId = "BA08",
                Description = "Konstruktion + Technik",
                ECTS = 5,
                MV = GetHost(fk01, "ZOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GK1"
                    },
                }

            };

            var elementedesstädtebaus = new CurriculumModule()
            {
                ShortName = "Elemente des Städtebaus",
                Name = "Elemente des Städtebaus",
                ModuleId = "BA10",
                Description = "Städtebau",
                ECTS = 5,
                MV = GetHost(fk01, "KAP"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Elem. Städte"
                    },
                }

            };

            var entwurf3 = new CurriculumModule()
            {
                ShortName = "Entwurf 3 (Konstruktion)",
                Name = "Entwurf 2 (Konstruktion)",
                ModuleId = "BA11",
                Description = "Konstruktion + Technik",
                ECTS = 10,
                MV = GetHost(fk01, "WOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Entwurf3"
                    },
                }

            };

            var digitaleentwurfsmethoden = new CurriculumModule()
            {
                ShortName = "Digitale Entwurfsmethoden",
                Name = "Digitale Entwurfsmethoden",
                ModuleId = "BA12",
                Description = "Gestaltung",
                ECTS = 10,
                MV = GetHost(fk01, "BER"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Dig. Ent."
                    },
                }

            };

            var integrationkonstruktiversysteme = new CurriculumModule()
            {
                ShortName = "Integration konstruktiver Systeme",
                Name = "Integration konstruktiver Systeme",
                ModuleId = "BA13",
                Description = "Konstruktion + Technik",
                ECTS = 5,
                MV = GetHost(fk01, "WOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ik Systeme"
                    },
                }

            };

            var entwurfsgrundlagen3 = new CurriculumModule()
            {
                ShortName = "Entwurfsgrundlagen 3",
                Name = "Entwurfsgrundlagen 3",
                ModuleId = "BA14",
                Description = "Architektur",
                ECTS = 5,
                MV = GetHost(fk01, "KUE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Entwurfgrd3"
                    },
                }

            };

            var grundlagenderstadtplanung = new CurriculumModule()
            {
                ShortName = "Grundlagen der Stadtplanung",
                Name = "Grundlagen der Stadtplanung",
                ModuleId = "BA15",
                Description = "Städtebau",
                ECTS = 5,
                MV = GetHost(fk01, "KAP"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GdSp"
                    },
                }

            };

            var entwurf4 = new CurriculumModule()
            {
                ShortName = "Entwurf 4 (Städtebau)",
                Name = "Entwurf 4 (Städtebau)",
                ModuleId = "BA16",
                Description = "Städtebau",
                ECTS = 10,
                MV = GetHost(fk01, "KAP"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Entwurf4"
                    },
                }

            };

            var gestalten1 = new CurriculumModule()
            {
                ShortName = "Gestalten 1",
                Name = "Gestalten 1",
                ModuleId = "BA17",
                Description = "Gestaltung",
                ECTS = 5,
                MV = GetHost(fk01, "PAU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Gestalten1"
                    },
                }

            };

            var sonderthemenkonstruktion = new CurriculumModule()
            {
                ShortName = "Sonderthemen Konstruktion",
                Name = "Sonderthemen Konstruktion",
                ModuleId = "BA18",
                Description = "Konstruktion + Technik",
                ECTS = 5,
                MV = GetHost(fk01, "RIC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Son. Konstruktion"
                    },
                }

            };

            var bauenimhistorischenkontext = new CurriculumModule()
            {
                ShortName = "Bauen im historischen Kontext",
                Name = "Bauen im historischen Kontext",
                ModuleId = "BA19",
                Description = "Architektur",
                ECTS = 5,
                MV = GetHost(fk01, "LAN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BihK"
                    },
                }

            };

            var allgemeinwissenschaften = new CurriculumModule()
            {
                ShortName = "Allgemeinwissenschaften",
                Name = "Allgemeinwissenschaften",
                ModuleId = "BA20",
                Description = "Architektur",
                ECTS = 5,
                MV = GetHost(fk01, "MEC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "allg. wiss."
                    },
                }

            };

            var gestalten2 = new CurriculumModule()
            {
                ShortName = "GS2",
                Name = "Gestalten 2",
                ModuleId = "BA22",
                Description = "Gestaltung",
                ECTS = 5,
                MV = GetHost(fk01, "KEG"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GS2"
                    },
                }
            };

            var baukilmatik = new CurriculumModule()
            {
                ShortName = "BK",
                Name = "Bauklimatik",
                ModuleId = "BA23",
                Description = "Konstruktion + Technik",
                ECTS = 5,
                MV = GetHost(fk01, "ESS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BK"
                    },
                }
            };

            var sondertehmenentwurf = new CurriculumModule()
            {
                ShortName = "stE",
                Name = "Sonderthemen Entwurf",
                ModuleId = "BA24",
                Description = "Architektur",
                ECTS = 5,
                MV = GetHost(fk01, "BRU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "stE"
                    },
                }
            };

            var interdisziplinärekompetenzen1 = new CurriculumModule()
            {
                ShortName = "IK1",
                Name = "Interdisziplinäre Kompetenzen 1",
                ModuleId = "BA25",
                Description = "Gestaltung, Städtebau, Architektur",
                ECTS = 5,
                MV = GetHost(fk01, "BRU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "IK1"
                    },
                }
            };

            var projektorganisation = new CurriculumModule()
            {
                ShortName = "PO",
                Name = "Projektorganisation",
                ModuleId = "BA26",
                Description = "Konstruktion + Technik",
                ECTS = 5,
                MV = GetHost(fk01, "HEN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PO"
                    },
                }
            };

            var entwurfsgrundlagen4 = new CurriculumModule()
            {
                ShortName = "EG4",
                Name = "Entwurfsgrundlagen 4",
                ModuleId = "BA27",
                Description = "Architektur",
                ECTS = 5,
                MV = GetHost(fk01, "WEB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EG4"
                    },
                }
            };

            var interdisziplinärekompetenzen2 = new CurriculumModule()
            {
                ShortName = "IK2",
                Name = "Interdisziplinäre Kompetenzen 2",
                ModuleId = "BA28",
                Description = "Gestaltung, Städtebau, Architektur",
                ECTS = 5,
                MV = GetHost(fk01, "WOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "IK2"
                    },
                }
            };

            var bachelorseminar = new CurriculumModule()
            {
                ShortName = "BS",
                Name = "Bachelorseminar und Bachelorthesis",
                ModuleId = "BA29",
                Description = "Konstruktion, Technik, Städtebau, Architektur",
                ECTS = 10,
                MV = GetHost(fk01, "KUE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BS"
                    },
                }
            };
        }
    }
}
