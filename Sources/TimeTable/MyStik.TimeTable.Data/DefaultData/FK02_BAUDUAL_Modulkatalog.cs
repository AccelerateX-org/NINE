using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogBAUDUAL(ActivityOrganiser fk02, Curriculum baudual)
        {

            var MA1 = new CurriculumModule()
            {
                ShortName = "MA1",
                Name = "Mathematik 1",
                ModuleId = "BAUDUAL401",
                Description = "Die Studierenden sollen die mathematischen Methoden und grundlegenden Verfahren beherrschen lernen, die zur Lösung von technischen Problemen im Bauwesen erforderlich sind",
                ECTS = 5,
                MV = GetHost(fk02, "FRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathematik 1"
                    },
                }
            };

            MA1.Groups.Add(GetGroup(baudual, "3"));
            baudual.Modules.Add(MA1);
            _db.CurriculumModules.Add(MA1);

            var MA2 = new CurriculumModule()
            {
                ShortName = "MA2",
                Name = "Mathematik 2",
                ModuleId = "BAU1",
                Description = "Die Studierenden sollen die mathematischen Methoden und grundlegenden Verfahren beherrschen lernen, die zur Lösung von technischen Problemen im Bauwesen erforderlich sind",
                ECTS = 5,
                MV = GetHost(fk02, "FRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathematik 2"
                    },
                }
            };

            MA2.Groups.Add(GetGroup(baudual, "3"));
            baudual.Modules.Add(MA2);
            _db.CurriculumModules.Add(MA2);


            var BS1 = new CurriculumModule()
            {
                ShortName = "BS1",
                Name = "Baustatik 1",
                ModuleId = "BAUDUAL402",
                Description = "Die Studierenden sollen mit grundlegenden Elementen der Baustatik (inkl.der Festigkeitslehre) vertraut gemacht werden.Sie sollen die Fertigkeit besitzen, grundlegende Verfahren zur Lösung baustatischer Aufgaben bei statisch bestimmten Stabtragwerken anwenden zu können.",
                ECTS = 6,
                MV = GetHost(fk02, "KNE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Baustatik 1"
                    },
                }
            };

            BS1.Groups.Add(GetGroup(baudual, "1"));
            baudual.Modules.Add(BS1);
            _db.CurriculumModules.Add(BS1);


            var BS2 = new CurriculumModule()
            {
                ShortName = "BS2",
                Name = "Baustatik 2",
                ModuleId = "BAUDUAL402",
                Description = "Die Studierenden sollen mit grundlegenden Elementen der Baustatik (inkl.der Festigkeitslehre) vertraut gemacht werden.Sie sollen die Fertigkeit besitzen, grundlegende Verfahren zur Lösung baustatischer Aufgaben bei statisch bestimmten Stabtragwerken anwenden zu können.",
                ECTS = 6,
                MV = GetHost(fk02, "KNE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Baustatik 2"
                    },
                }
            };

            BS2.Groups.Add(GetGroup(baudual, "4"));
            baudual.Modules.Add(BS2);
            _db.CurriculumModules.Add(BS2);

            var BA1 = new CurriculumModule()
            {
                ShortName = "BA1",
                Name = "Baustoffe 1",
                ModuleId = "BAUDUAL403",
                Description = "Die Studierenden sollen mit den Eigenschaften sowie deren messtechnischen Bestimmung der wichtigsten Baustoffe vertraut gemacht werden",
                ECTS = 3,
                MV = GetHost(fk02, "DAU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Baustoffe 1"
                    },
                }
            };

            BA1.Groups.Add(GetGroup(baudual, "1"));
            baudual.Modules.Add(BA1);
            _db.CurriculumModules.Add(BA1);

            var BA2 = new CurriculumModule()
            {
                ShortName = "BA2",
                Name = "Baustoffe 2",
                ModuleId = "BAUDUAL403",
                Description = "Die Studierenden sollen mit den Eigenschaften sowie deren messtechnischen Bestimmung der wichtigsten Baustoffe vertraut gemacht werden",
                ECTS = 3,
                MV = GetHost(fk02, "DAU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Baustoffe 2"
                    },
                }
            };

            BA2.Groups.Add(GetGroup(baudual, "2"));
            baudual.Modules.Add(BA2);
            _db.CurriculumModules.Add(BA2);

            var BC1 = new CurriculumModule()
            {
                ShortName = "BC1",
                Name = "Bauchemie 1",
                ModuleId = "BAUDUAL404",
                Description = "chemischen Zusammenhängen bei der Herstellung und Erhärtung von Baustoffen",
                ECTS = 2,
                MV = GetHost(fk02, "KUS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bauchemie 1"
                    },
                }
            };

            BC1.Groups.Add(GetGroup(baudual, "1"));
            baudual.Modules.Add(BC1);
            _db.CurriculumModules.Add(BC1);

            var BC2 = new CurriculumModule()
            {
                ShortName = "BC2",
                Name = "Bauchemie 2",
                ModuleId = "BAUDUAL404",
                Description = "chemischen Zusammenhängen bei der Herstellung und Erhärtung von Baustoffen",
                ECTS = 2,
                MV = GetHost(fk02, "KUS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bauchemie 2"
                    },
                }
            };

            BC2.Groups.Add(GetGroup(baudual, "2"));
            baudual.Modules.Add(BC2);
            _db.CurriculumModules.Add(BC2);

            var BP1 = new CurriculumModule()
            {
                ShortName = "BP1",
                Name = "Bauphysik 1",
                ModuleId = "BAUDUAL405",
                Description = "bauphysikalische Grundlagen des Wärme-Feuchte und Schallschutzes kennen lernen.",
                ECTS = 1,
                MV = GetHost(fk02, "HOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bauphysik 1"
                    },
                }
            };

            BP1.Groups.Add(GetGroup(baudual, "3"));
            baudual.Modules.Add(BP1);
            _db.CurriculumModules.Add(BP1);

            var BP2 = new CurriculumModule()
            {
                ShortName = "BP2",
                Name = "Bauphysik 2",
                ModuleId = "BAUDUAL405",
                Description = "bauphysikalische Grundlagen des Wärme-Feuchte und Schallschutzes kennen lernen.",
                ECTS = 1,
                MV = GetHost(fk02, "HOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bauphysik 2"
                    },
                }
            };

            BP2.Groups.Add(GetGroup(baudual, "4"));
            baudual.Modules.Add(BP2);
            _db.CurriculumModules.Add(BP2);

            var HK = new CurriculumModule()
            {
                ShortName = "HK",
                Name = "Hochbaukonstruktion",
                ModuleId = "BAUDUAL406",
                Description = "Fähigkeit zur Entwicklung und zur grafischen Darstellung eines Gebäudekonzeptes erlangen",
                ECTS = 5,
                MV = GetHost(fk02, "HEN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Hochbaukonstruktion"
                    },
                }
            };

            HK.Groups.Add(GetGroup(baudual, "2"));
            baudual.Modules.Add(HK);
            _db.CurriculumModules.Add(HK);

            var GD1 = new CurriculumModule()
            {
                ShortName = "GD1",
                Name = "Grundlagen der Darstellung 1",
                ModuleId = "BAUDUAL407",
                Description = "Grundlagen der Darstellung",
                ECTS = 4,
                MV = GetHost(fk02, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Grundlagen der Darstellung 1"
                    },
                }
            };

            GD1.Groups.Add(GetGroup(baudual, "1"));
            baudual.Modules.Add(GD1);
            _db.CurriculumModules.Add(GD1);

            var GD2 = new CurriculumModule()
            {
                ShortName = "GD2",
                Name = "Grundlagen der Darstellung 2",
                ModuleId = "BAUDUAL407",
                Description = "Grundlagen der Darstellung",
                ECTS = 4,
                MV = GetHost(fk02, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Grundlagen der Darstellung 2"
                    },
                }
            };

            GD2.Groups.Add(GetGroup(baudual, "2"));
            baudual.Modules.Add(GD2);
            _db.CurriculumModules.Add(GD2);

            var KZ = new CurriculumModule()
            {
                ShortName = "KZ",
                Name = "Konstruktives Zeichnen",
                ModuleId = "BAUDUAL407",
                Description = "Einblick in die Planarten des Bauingenieurwesens und der Architektur gewinnen",
                ECTS = 0,
                MV = GetHost(fk02, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Konstruktives Zeichnen"
                    },
                }
            };

            KZ.Groups.Add(GetGroup(baudual, "1"));
            baudual.Modules.Add(KZ);
            _db.CurriculumModules.Add(KZ);

            var CAD = new CurriculumModule()
            {
                ShortName = "CAD",
                Name = "CAD",
                ModuleId = "BAUDUAL407.2",
                Description = "Fähigkeit erlangen, am Computer mit grundlegenden Funktionen eines bauspezifischen CAD - Systems zu arbeiten und sich weiterführende Funktionalitäten eigenständig erschließen zu können.",
                ECTS = 5,
                MV = GetHost(fk02, "SUL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "CAD"
                    },
                }
            };

            CAD.Groups.Add(GetGroup(baudual, "6"));
            baudual.Modules.Add(CAD);
            _db.CurriculumModules.Add(CAD);

            var DG = new CurriculumModule()
            {
                ShortName = "DG",
                Name = "Darstellende Geometrie",
                ModuleId = "BAUDUAL407.3",
                Description = "räumliche Vorstellungsvermögen und das Denken im Raum",
                ECTS = 0,
                MV = GetHost(fk02, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Darstellende Geometrie"
                    },
                }
            };

            DG.Groups.Add(GetGroup(baudual, "3"));
            baudual.Modules.Add(DG);
            _db.CurriculumModules.Add(DG);

            var BI = new CurriculumModule()
            {
                ShortName = "BI",
                Name = "Bauinformatik -Grundlagen",
                ModuleId = "BAUDUAL408",
                Description = "objektorientierten Programmiersprache analytische Lösungen zu technischen Problemstellungen zu erarbeiten",
                ECTS = 5,
                MV = GetHost(fk02, "STE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bauinformatik -Grundlagen"
                    },
                }
            };

            BI.Groups.Add(GetGroup(baudual, "3"));
            baudual.Modules.Add(BI);
            _db.CurriculumModules.Add(BI);


            _db.SaveChanges();
        }

    }
}


