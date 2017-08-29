using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogBAU(ActivityOrganiser fk02, Curriculum BAU)
        {


            var MA1 = new CurriculumModule()
            {
                ShortName = "MA1",
                Name = "Mathematik 1",
                ModuleId = "BAU1",
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

            MA1.Groups.Add(GetGroup(BAU, "1"));
            BAU.Modules.Add(MA1);
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

            MA2.Groups.Add(GetGroup(BAU, "2"));
            BAU.Modules.Add(MA2);
            _db.CurriculumModules.Add(MA2);

            var BS1 = new CurriculumModule()
            {
                ShortName = "BS1",
                Name = "Baustatik 1",
                ModuleId = "BAU2",
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

            BS1.Groups.Add(GetGroup(BAU, "1"));
            BAU.Modules.Add(BS1);
            _db.CurriculumModules.Add(BS1);


            var BS2 = new CurriculumModule()
            {
                ShortName = "BS2",
                Name = "Baustatik 2",
                ModuleId = "BAU2",
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

            BS2.Groups.Add(GetGroup(BAU, "2"));
            BAU.Modules.Add(BS2);
            _db.CurriculumModules.Add(BS2);

            var BA1 = new CurriculumModule()
            {
                ShortName = "BA1",
                Name = "Baustoffe 1",
                ModuleId = "BAU3",
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

            BA1.Groups.Add(GetGroup(BAU, "1"));
            BAU.Modules.Add(BA1);
            _db.CurriculumModules.Add(BA1);

            var BA2 = new CurriculumModule()
            {
                ShortName = "BA2",
                Name = "Baustoffe 2",
                ModuleId = "BAU3",
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

            BA2.Groups.Add(GetGroup(BAU, "2"));
            BAU.Modules.Add(BA2);
            _db.CurriculumModules.Add(BA2);

            var BC1 = new CurriculumModule()
            {
                ShortName = "BC1",
                Name = "Bauchemie 1",
                ModuleId = "BAU4",
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

            BC1.Groups.Add(GetGroup(BAU, "1"));
            BAU.Modules.Add(BC1);
            _db.CurriculumModules.Add(BC1);

            var BC2 = new CurriculumModule()
            {
                ShortName = "BC2",
                Name = "Bauchemie 2",
                ModuleId = "BAU4",
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

            BC2.Groups.Add(GetGroup(BAU, "2"));
            BAU.Modules.Add(BC2);
            _db.CurriculumModules.Add(BC2);

            var BP1 = new CurriculumModule()
            {
                ShortName = "BP1",
                Name = "Bauphysik 1",
                ModuleId = "BAU5",
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

            BP1.Groups.Add(GetGroup(BAU, "1"));
            BAU.Modules.Add(BP1);
            _db.CurriculumModules.Add(BP1);

            var BP2 = new CurriculumModule()
            {
                ShortName = "BP2",
                Name = "Bauphysik 2",
                ModuleId = "BAU5",
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

            BP2.Groups.Add(GetGroup(BAU, "2"));
            BAU.Modules.Add(BP2);
            _db.CurriculumModules.Add(BP2);

            var HK = new CurriculumModule()
            {
                ShortName = "HK",
                Name = "Hochbaukonstruktion",
                ModuleId = "BAU6",
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

            HK.Groups.Add(GetGroup(BAU, "1"));
            BAU.Modules.Add(HK);
            _db.CurriculumModules.Add(HK);

            var GD1 = new CurriculumModule()
            {
                ShortName = "GD1",
                Name = "Grundlagen der Darstellung 1",
                ModuleId = "BAU7",
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

            GD1.Groups.Add(GetGroup(BAU, "1"));
            BAU.Modules.Add(GD1);
            _db.CurriculumModules.Add(GD1);

            var GD2 = new CurriculumModule()
            {
                ShortName = "GD2",
                Name = "Grundlagen der Darstellung 2",
                ModuleId = "BAU7",
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

            GD2.Groups.Add(GetGroup(BAU, "2"));
            BAU.Modules.Add(GD2);
            _db.CurriculumModules.Add(GD2);

            var KZ = new CurriculumModule()
            {
                ShortName = "KZ",
                Name = "Konstruktives Zeichnen",
                ModuleId = "BAU7.1",
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

            KZ.Groups.Add(GetGroup(BAU, "N.N."));
            BAU.Modules.Add(KZ);
            _db.CurriculumModules.Add(KZ);

            var CAD = new CurriculumModule()
            {
                ShortName = "CAD",
                Name = "CAD",
                ModuleId = "BAU7.2",
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

            CAD.Groups.Add(GetGroup(BAU, "6"));
            BAU.Modules.Add(CAD);
            _db.CurriculumModules.Add(CAD);

            var DG = new CurriculumModule()
            {
                ShortName = "DG",
                Name = "Darstellende Geometrie",
                ModuleId = "BAU7.3",
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

            DG.Groups.Add(GetGroup(BAU, "N.N."));
            BAU.Modules.Add(DG);
            _db.CurriculumModules.Add(DG);

            var BI = new CurriculumModule()
            {
                ShortName = "BI",
                Name = "Bauinformatik -Grundlagen",
                ModuleId = "BAU8",
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

            BI.Groups.Add(GetGroup(BAU, "1"));
            BAU.Modules.Add(BI);
            _db.CurriculumModules.Add(BI);

            var BAU2 = new CurriculumModule()
            {
                ShortName = "BAU2",
                Name = "Baustatik 2",
                ModuleId = "BAU101",
                Description = "Berechnung der Verformungen und Schnittgrößen statisch unbestimmter ebener und räumlicher Tragwerke",
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

            BAU2.Groups.Add(GetGroup(BAU, "3"));
            BAU.Modules.Add(BAU2);
            _db.CurriculumModules.Add(BAU2);

            var MB1 = new CurriculumModule()
            {
                ShortName = "MB1",
                Name = "Massivbau I Grundlagen",
                ModuleId = "BAU102",
                Description = "Kenntnis über die wichtigsten Berechnungsverfahren und Konstruktionsregeln von Bauteilen aus Stahlbeton und Mauerwerk",
                ECTS = 5,
                MV = GetHost(fk02, "GEB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Massivbau I Grundlagen"
                    },
                }
            };

            MB1.Groups.Add(GetGroup(BAU, "3"));
            BAU.Modules.Add(MB1);
            _db.CurriculumModules.Add(MB1);

            var MB2 = new CurriculumModule()
            {
                ShortName = "MB2",
                Name = "Massivbau 2 Grundlagen",
                ModuleId = "BAU102",
                Description = "Kenntnis über die wichtigsten Berechnungsverfahren und Konstruktionsregeln von Bauteilen aus Stahlbeton und Mauerwerk",
                ECTS = 5,
                MV = GetHost(fk02, "GEB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Massivbau 2 Grundlagen"
                    },
                }
            };

            MB2.Groups.Add(GetGroup(BAU, "4"));
            BAU.Modules.Add(MB2);
            _db.CurriculumModules.Add(MB2);

            var SH = new CurriculumModule()
            {
                ShortName = "SH",
                Name = "Stahl- und Holzbau",
                ModuleId = "BAU103",
                Description = "Ermittlung der Beanspruchungen",
                ECTS = 8,
                MV = GetHost(fk02, "HAU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Stahl- und Holzbau"
                    },
                }
            };

            SH.Groups.Add(GetGroup(BAU, "4"));
            BAU.Modules.Add(SH);
            _db.CurriculumModules.Add(SH);

            var BM = new CurriculumModule()
            {
                ShortName = "BM",
                Name = "Bodenmechanik mit Praktikum",
                ModuleId = "BAU104",
                Description = "Methoden der Baugrunderkundung",
                ECTS = 5,
                MV = GetHost(fk02, "SLO"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bodenmechanik mit Praktikum"
                    },
                }
            };

            BM.Groups.Add(GetGroup(BAU, "3"));
            BAU.Modules.Add(BM);
            _db.CurriculumModules.Add(BM);

            var GB = new CurriculumModule()
            {
                ShortName = "GB",
                Name = "Grundbau",
                ModuleId = "BAU105",
                Description = "Stützkonstruktionen und weitere geotechnische Bauwerke zu entwerfen,",
                ECTS = 5,
                MV = GetHost(fk02, "SLO"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Grundbau"
                    },
                }
            };

            GB.Groups.Add(GetGroup(BAU, "4"));
            BAU.Modules.Add(GB);
            _db.CurriculumModules.Add(GB);

            var LVW = new CurriculumModule()
            {
                ShortName = "LVW",
                Name = "Landverkehrswegebau",
                ModuleId = "BAU105",
                Description = "Landverkehrswegebau",
                ECTS = 6,
                MV = GetHost(fk02, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Landverkehrswegebau"
                    },
                }
            };

            LVW.Groups.Add(GetGroup(BAU, "3"));
            BAU.Modules.Add(LVW);
            _db.CurriculumModules.Add(LVW);

            var WB1 = new CurriculumModule()
            {
                ShortName = "WB1",
                Name = "Wasserbau I",
                ModuleId = "BAU107",
                Description = "Grundlagen der Hydraulik, des Wasserbaus und der Wasserwirtschaft vertraut werden",
                ECTS = 6,
                MV = GetHost(fk02, "FRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wasserbau I"
                    },
                }
            };

            WB1.Groups.Add(GetGroup(BAU, "3"));
            BAU.Modules.Add(WB1);
            _db.CurriculumModules.Add(WB1);

            var SWW = new CurriculumModule()
            {
                ShortName = "SWW",
                Name = "Siedlungswasserwirtschaft",
                ModuleId = "BAU108",
                Description = "Die Studierenden sollen mit den Grundlagen der Siedlungswasserwirtschaft vertraut gemacht und befähigt werden, kleine einfache Anlagen zu planen und zu bemessen.",
                ECTS = 6,
                MV = GetHost(fk02, "FRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Siedlungswasserwirtschaft"
                    },
                }
            };

            SWW.Groups.Add(GetGroup(BAU, "4"));
            BAU.Modules.Add(SWW);
            _db.CurriculumModules.Add(SWW);


            var BPS1 = new CurriculumModule()
            {
                ShortName = "BPS1",
                Name = "Bauproduktionsplanung und –steuerung 1 (Grundlagen)",
                ModuleId = "BAU109",
                Description = "Besonderheiten der Bauwirtschaft mit Projektbeteiligten",
                ECTS = 4,
                MV = GetHost(fk02, "FRI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bauproduktionsplanung und –steuerung 1 (Grundlagen)"
                    },
                }
            };

            BPS1.Groups.Add(GetGroup(BAU, "3"));
            BAU.Modules.Add(BPS1);
            _db.CurriculumModules.Add(BPS1);


            var BPS2 = new CurriculumModule()
            {
                ShortName = "BPS2",
                Name = "Bauproduktionsplanung und –steuerung 2 (Grundlagen)",
                ModuleId = "BAU109",
                Description = "Besonderheiten der Bauwirtschaft mit Projektbeteiligten",
                ECTS = 4,
                MV = GetHost(fk02, "FRI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bauproduktionsplanung und –steuerung 2 (Grundlagen)"
                    },
                }
            };

            BPS2.Groups.Add(GetGroup(BAU, "4"));
            BAU.Modules.Add(BPS2);
            _db.CurriculumModules.Add(BPS2);

            var ST = new CurriculumModule()
            {
                ShortName = "ST",
                Name = "Sicherheitstechnik",
                ModuleId = "BAU111",
                Description = "Ermittlung und Bewertung von Risiken in der Arbeitssicherheit",
                ECTS = 5,
                MV = GetHost(fk02, "HAR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Sicherheitstechnik"
                    },
                }
            };

            ST.Groups.Add(GetGroup(BAU, "5"));
            BAU.Modules.Add(ST);
            _db.CurriculumModules.Add(ST);


            var PS = new CurriculumModule()
            {
                ShortName = "PS",
                Name = "Praxisseminar",
                ModuleId = "BAU112",
                Description = "Einführung in das praktische Studiensemester",
                ECTS = 5,
                MV = GetHost(fk02, "BIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Praxisseminar"
                    },
                }
            };

            PS.Groups.Add(GetGroup(BAU, "5"));
            BAU.Modules.Add(PS);
            _db.CurriculumModules.Add(PS);

            var PB = new CurriculumModule()
            {
                ShortName = "PB",
                Name = "Praktikum, Praktikumsbericht",
                ModuleId = "BAU113",
                Description = "Kennenlernen von einem oder mehreren Berufsbildern des Bauingenieurs aus dem Bereich Verwaltung, Planung, Bauabwicklung, Produktion, Kosten - und Ausführungskontrolle.",
                ECTS = 15,
                MV = GetHost(fk02, "ANS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Praktikum, Praktikumsbericht"
                    },
                }
            };

            PB.Groups.Add(GetGroup(BAU, "5"));
            BAU.Modules.Add(PB);
            _db.CurriculumModules.Add(PB);

            var CB = new CurriculumModule()
            {
                ShortName = "CB",
                Name = "Computerunterstützte Berechnung Tragwerke des Ingenieurbaus",
                ModuleId = "BAU122",
                Description = "Durch exemplarische Behandlung typischer Tragwerke aus Stahl- und Spannbeton sollen die Studierenden Kenntnis erlangen über den Einsatz von EDV Programmen in der Praxis",
                ECTS = 15,
                MV = GetHost(fk02, "SEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Computerunterstützte Berechnung Tragwerke des Ingenieurbaus"
                    },
                }
            };

            CB.Groups.Add(GetGroup(BAU, "7"));
            BAU.Modules.Add(CB);
            _db.CurriculumModules.Add(CB);

            var VS = new CurriculumModule()
            {
                ShortName = "VS",
                Name = "Praktikum Vermessung und Straßenabsteckung",
                ModuleId = "BAU110.2",
                Description = "Die Studierenden sollen befähigt werden, vermessungstechnische Verfahren zur Berechnung und praktischen Absteckung von Trassierungselementen anzuwenden.",
                ECTS = 0,
                MV = GetHost(fk02, "EGE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Praktikum Vermessung und Straßenabsteckung"
                    },
                }
            };

            VS.Groups.Add(GetGroup(BAU, "5"));
            BAU.Modules.Add(VS);
            _db.CurriculumModules.Add(VS);


            var TH = new CurriculumModule()
            {
                ShortName = "TH",
                Name = "Tragwerke des Hochbaus",
                ModuleId = "BAU201",
                Description = "Durch exemplarische Behandlung typischer Tragwerke des Hochbaus aus unterschiedlichen Baustoffen sollen die Studierenden Kenntnis erlangen über die wichtigsten Elemente zur Abtragung von Horizontalund Vertikallasten sowie deren Bemessung und Konstruktion",
                ECTS = 5,
                MV = GetHost(fk02, "SEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Tragwerke des Hochbaus"
                    },
                }
            };

            TH.Groups.Add(GetGroup(BAU, "6"));
            BAU.Modules.Add(TH);
            _db.CurriculumModules.Add(TH);

            var BB1 = new CurriculumModule()
            {
                ShortName = "BB1",
                Name = "Bauordnungs- und Bauvertragsrecht I",
                ModuleId = "BAU202/302",
                Description = "Die Studierenden sollen einen Überblick über das Bauordnungsrecht erhalten",
                ECTS = 5,
                MV = GetHost(fk02, "JUN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bauordnungs- und Bauvertragsrecht I"
                    },
                }
            };

            BB1.Groups.Add(GetGroup(BAU, "6"));
            BAU.Modules.Add(BB1);
            _db.CurriculumModules.Add(BB1);

            var IP1 = new CurriculumModule()
            {
                ShortName = "IP1",
                Name = "Integrierte Planungsmethoden 1",
                ModuleId = "BAU203/303",
                Description = "Integrierte Planungsmethoden",
                ECTS = 4,
                MV = GetHost(fk02, "STE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Integrierte Planungsmethoden 1"
                    },
                }
            };

            IP1.Groups.Add(GetGroup(BAU, "6"));
            BAU.Modules.Add(IP1);
            _db.CurriculumModules.Add(IP1);


            var IP2 = new CurriculumModule()
            {
                ShortName = "IP2",
                Name = "Integrierte Planungsmethoden 2",
                ModuleId = "BAU203/303",
                Description = "Integrierte Planungsmethoden",
                ECTS = 4,
                MV = GetHost(fk02, "STE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Integrierte Planungsmethoden 2"
                    },
                }
            };

            IP2.Groups.Add(GetGroup(BAU, "7"));
            BAU.Modules.Add(IP2);
            _db.CurriculumModules.Add(IP2);

            var SS = new CurriculumModule()
            {
                ShortName = "SS",
                Name = "Stahlbau und-Stabilitätslehre",
                ModuleId = "BAU305/362",
                Description = "Die Studierenden sollen vertiefte Kenntnisse der Berechnungs- und Bemessungsverfahren des Stahlbaus erhalten",
                ECTS = 5,
                MV = GetHost(fk02, "ANS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Stahlbau und-Stabilitätslehre"
                    },
                }
            };

            SS.Groups.Add(GetGroup(BAU, "6"));
            BAU.Modules.Add(SS);
            _db.CurriculumModules.Add(SS);

            var WS = new CurriculumModule()
            {
                ShortName = "WS",
                Name = "Werkstoff- und Schweißtechnik Grundlagen",
                ModuleId = "BAU306/373",
                Description = "Die Studierenden sollen mit den Eigenschaften verschiedener im Bauwesen und im Anlagenbau hauptsächlich verwendeter Stähle",
                ECTS = 5,
                MV = GetHost(fk02, "ENG"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Werkstoff- und Schweißtechnik Grundlagen"
                    },
                }
            };

            WS.Groups.Add(GetGroup(BAU, "6"));
            BAU.Modules.Add(WS);
            _db.CurriculumModules.Add(WS);

            var SHO = new CurriculumModule()
            {
                ShortName = "SHO",
                Name = "Stahlhochbau",
                ModuleId = "BAU307/374",
                Description = "Die Studierenden sollen mit grundlegenden Bauwerken, Konstruktions - und Berechnungsverfahren des Stahlhochbaus vertraut gemacht werden",
                ECTS = 5,
                MV = GetHost(fk02, "ANS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Stahlhochbau"
                    },
                }
            };

            SHO.Groups.Add(GetGroup(BAU, "6"));
            BAU.Modules.Add(SHO);
            _db.CurriculumModules.Add(SHO);

            var SG = new CurriculumModule()
            {
                ShortName = "SG",
                Name = "Stahlbrückenbau Grundlagen",
                ModuleId = "BAU307/374",
                Description = "Die Studierenden sollen mit den grundlegenden Konstruktions- und Berechnungsverfahren des Stahlbrückenbaus vertraut gemacht werden",
                ECTS = 5,
                MV = GetHost(fk02, "HAU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Stahlbrückenbau Grundlagen"
                    },
                }
            };

            SG.Groups.Add(GetGroup(BAU, "6"));
            BAU.Modules.Add(SG);
            _db.CurriculumModules.Add(SG);


            var AKS = new CurriculumModule()
            {
                ShortName = "AKS",
                Name = "Ausgewählte Kapitel aus dem Stahlbau",
                ModuleId = "BAU309/376",
                Description = "Die Studierenden sollen Torsionsprobleme im Stahlbau als solche erfassen, korrekt berechnen und die betroffenen Bauteile nach Eurocode Normen nachweisen können",
                ECTS = 5,
                MV = GetHost(fk02, "HAU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ausgewählte Kapitel aus dem Stahlbau"
                    },
                }
            };

            AKS.Groups.Add(GetGroup(BAU, "7"));
            BAU.Modules.Add(AKS);
            _db.CurriculumModules.Add(AKS);


            var BA = new CurriculumModule()
            {
                ShortName = "BA",
                Name = "Bachelorarbeit (Abschlussarbeit zum Studium)",
                ModuleId = "BAU350",
                Description = "Abschlussarbeit",
                ECTS = 12,
                MV = GetHost(fk02, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bachelorarbeit (Abschlussarbeit zum Studium)"
                    },
                }
            };

            BA.Groups.Add(GetGroup(BAU, "7"));
            BAU.Modules.Add(BA);
            _db.CurriculumModules.Add(BA);


            var BK = new CurriculumModule()
            {
                ShortName = "BK",
                Name = "Bauphysik und Konstruktiver Brandschutz",
                ModuleId = "BAU355",
                Description = "Die Studierenden sollen die Fähigkeiten erlangen, hinsichtlich des Brandschutzes konstruieren zu können, rechnerische Nachweise für die Bemessung im Brandfall zu führen und die Industriebaurichtlinie anzuwenden.",
                ECTS = 5,
                MV = GetHost(fk02, "SUL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bauphysik und Konstruktiver Brandschutz"
                    },
                }
            };

            BK.Groups.Add(GetGroup(BAU, "7"));
            BAU.Modules.Add(BK);
            _db.CurriculumModules.Add(BK);

            var BT = new CurriculumModule()
            {
                ShortName = "BT",
                Name = "Betontechnologie",
                ModuleId = "BAU356",
                Description = "Die Studierenden erhalten erweiterte Kenntnisse in der Betontechnologie",
                ECTS = 5,
                MV = GetHost(fk02, "DAU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Betontechnologie"
                    },
                }
            };

            BT.Groups.Add(GetGroup(BAU, "6"));
            BAU.Modules.Add(BT);
            _db.CurriculumModules.Add(BT);

            var EO = new CurriculumModule()
            {
                ShortName = "EO",
                Name = "Erd- und Oberbau bei Landverkehrswegen",
                ModuleId = "BAU356",
                Description = "Die Studierenden sollen Kenntnis des Oberbaus, des Erdbaus und der Entwässerung von Landverkehrswegen gewinnen.",
                ECTS = 5,
                MV = GetHost(fk02, "EGE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Erd- und Oberbau bei Landverkehrswegen"
                    },
                }
            };

            EO.Groups.Add(GetGroup(BAU, "7"));
            BAU.Modules.Add(EO);
            _db.CurriculumModules.Add(EO);

            var FG = new CurriculumModule()
            {
                ShortName = "FG",
                Name = "Grundlagen Fassadentechnik und Glasbau",
                ModuleId = "BAU366",
                Description = "Die Studierenden sollen Kenntnis des Oberbaus, des Erdbaus und der Entwässerung von Landverkehrswegen gewinnen.",
                ECTS = 5,
                MV = GetHost(fk02, "OEM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Grundlagen Fassadentechnik und Glasbau"
                    },
                }
            };

            FG.Groups.Add(GetGroup(BAU, "6"));
            BAU.Modules.Add(FG);
            _db.CurriculumModules.Add(FG);

            var SBWL = new CurriculumModule()
            {
                ShortName = "SBWL",
                Name = "Spezielle Betriebswirtschaftslehre und betriebliches Controlling im Bauwesen",
                ModuleId = "BAU368",
                Description = "Kenntnis von den betriebswirtschaftlichen Grundlagen des Baubetriebs und der Unternehmens - und Kostenrechnung in Bauunternehmen",
                ECTS = 5,
                MV = GetHost(fk02, "CLA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Spezielle Betriebswirtschaftslehre und betriebliches Controlling im Bauwesen"
                    },
                }
            };

            SBWL.Groups.Add(GetGroup(BAU, "7"));
            BAU.Modules.Add(SBWL);
            _db.CurriculumModules.Add(SBWL);


            var SB = new CurriculumModule()
            {
                ShortName = "SB",
                Name = "Schlüsselfertiges Bauen",
                ModuleId = "BAU371",
                Description = "erlernen die Grundlagen der Bauordnungen und deren Anwendung bei der Baurealisierung sowie Anforderungen des Brandschutzes und der Bauproduktenrichtlinie",
                ECTS = 5,
                MV = GetHost(fk02, "CLA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Schlüsselfertiges Bauen"
                    },
                }
            };

            SB.Groups.Add(GetGroup(BAU, "6"));
            BAU.Modules.Add(SB);
            _db.CurriculumModules.Add(SB);

            var KL = new CurriculumModule()
            {
                ShortName = "KL",
                Name = "Kosten- und Leistungsrechung",
                ModuleId = "BAU372",
                Description = "Die Studierenden werden befähigt zur Erstellung von Angeboten, erkennen Kostenabhängigkeiten, können eine Auftragskalkulation nach Zuschlagserteilung erstellen",
                ECTS = 5,
                MV = GetHost(fk02, "CLA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kosten- und Leistungsrechung"
                    },
                }
            };

            KL.Groups.Add(GetGroup(BAU, "6"));
            BAU.Modules.Add(KL);
            _db.CurriculumModules.Add(KL);








            _db.SaveChanges();
        }

    }
}




