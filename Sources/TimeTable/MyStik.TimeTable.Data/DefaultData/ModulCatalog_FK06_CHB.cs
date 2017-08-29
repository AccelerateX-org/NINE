using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData 
    {
        public void InitCatalogCHB_GS(ActivityOrganiser fk06, Curriculum CHB)
    {
        var Chemie1 = new CurriculumModule()
        {
            ShortName = "Chemie 1",
            Name = "Chemie 1",
            ModuleId = "CHB1",
            Description = "Atombau, Chemische Bindung, Chemiches Gleichgewicht, Chemische Reaktion, Säure-Base-Reaktionen, Heterogene Systeme, Redoxreaktionen",
            ECTS = 5,
            MV = GetHost(fk06, "DIM"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Chemie"
                    },
                }

        };

        Chemie1.Groups.Add(GetGroup(CHB, "1"));
        CHB.Modules.Add(Chemie1);
        _db.CurriculumModules.Add(Chemie1);

        var Physik1 = new CurriculumModule()
        {
            ShortName = "Physik 1",
            Name = "Physik 1",
            ModuleId = "CHB2",
            Description = "Metrologie, Kinematik, Dynamik des Massepunktes, Energierhaltung, Dynamik starrer Körper, Schwingungen",
            ECTS = 4,
            MV = GetHost(fk06, "CLS"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik1"
                    },
                }
        };

        Physik1.Groups.Add(GetGroup(CHB, "1"));
        CHB.Modules.Add(Physik1);
        _db.CurriculumModules.Add(Physik1);

        var Mathematik1 = new CurriculumModule()
        {
            ShortName = "Mathematik1",
            Name = "Mathematik1",
            ModuleId = "CHB3",
            Description = "Funktionen einer Veränderlichen, ausgewälhlte Exponential-und Logarithmusfunktionen; Wachstumsprozesse;Komplexe Zahlen; Allg.Schwingungen; Differenzial-/Integral-Rechnung; Geometrische Deutung der Ableitung; Linearisieren; Newton-Verfahren; Analytische Geometrie",
            ECTS = 4,
            MV = GetHost(fk06, "HIR"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathematik1"
                    },
                }
        };

        Mathematik1.Groups.Add(GetGroup(CHB, "1"));
        CHB.Modules.Add(Mathematik1);
        _db.CurriculumModules.Add(Mathematik1);


        var Konstruktion = new CurriculumModule()
        {
            ShortName = "Konstruktion",
            Name = "Konstruktion",
            ModuleId = "CHB4",
            Description = "Konstruktion/CAD:Kenntnis mechanischer und elektromechanischer Feinwerkelemente.",
            ECTS = 6,
            MV = GetHost(fk06, "WAG"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Konstruktion"
                    },
                }

        };

        Konstruktion.Groups.Add(GetGroup(CHB, "1"));
        CHB.Modules.Add(Konstruktion);
        _db.CurriculumModules.Add(Konstruktion);

        var Informatik = new CurriculumModule()
        {
            ShortName = "Informatik",
            Name = "Informatik",
            ModuleId = "CHB5",
            Description = "Kenntnis und Verständnis für die in der Anwendung der Informatik erforderlichen Begriffe und Methoden; Fähigkeit, den Einsatz von Informatikmitteln zu analysieren, zu planen und kritisch zu beurteilen.",
            ECTS = 6,
            MV = GetHost(fk06, "Brau"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Chemie"
                    },
                }

        };

        Informatik.Groups.Add(GetGroup(CHB, "1"));
        CHB.Modules.Add(Informatik);
        _db.CurriculumModules.Add(Informatik);

        var Arbeitssicherheit = new CurriculumModule()
        {
            ShortName = "Arbeitssicherheit",
            Name = "Arbeitssicherheit",
            ModuleId = "CHB6",
            Description = "Arbeitssicherheit, Ergonomie, Chemierecht",
            ECTS = 4,
            MV = GetHost(fk06, "RAB"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Arbeitssicherheit"
                    },
                }

        };

        Arbeitssicherheit.Groups.Add(GetGroup(CHB, "1"));
        CHB.Modules.Add(Arbeitssicherheit);
        _db.CurriculumModules.Add(Arbeitssicherheit);

        var Chemie2 = new CurriculumModule()
        {
            ShortName = "Chemie2",
            Name = "Chemie2",
            ModuleId = "CHB7",
            Description = "Organische Chemie",
            ECTS = 4,
            MV = GetHost(fk06, "VAS"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Chemie2"
                    },
                }
        };

        Chemie2.Groups.Add(GetGroup(CHB, "2"));
        CHB.Modules.Add(Chemie2);
        _db.CurriculumModules.Add(Chemie2);

        var Physik2 = new CurriculumModule()
        {
            ShortName = "Physik2",
            Name = "Physik2",
            ModuleId = "CHB8",
            Description = "Überlagerung von Schwingungen; Wellen; Wärmekapazität von Festkörpern; Eigenschaften deformierbarer Körper: Flüssigkeiten und Gase; Optik; Grundzüge des Atombaus; Messwertbehandlung im Praktikum",
            ECTS = 4,
            MV = GetHost(fk06, "SCU"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik2"
                    },
                }

        };

        Physik2.Groups.Add(GetGroup(CHB, "2"));
        CHB.Modules.Add(Physik2);
        _db.CurriculumModules.Add(Physik2);

        var Mathematik2 = new CurriculumModule()
        {
            ShortName = "Mathematik2",
            Name = "Mathematik2",
            ModuleId = "CHB9",
            Description = "Polynome;Partialbruchzerlegung;Integralrechnung;Folgen und Reihen; Fouriertransformation; Potenzreihen; Taylorreihe; Funktionen mehrerer Veränderlichen; partielle Ableitungen; Richtungsableitung;Tangentialebene; Fehlerrechnung etc.; Matrix- und Determinantenrechnung; lineare Gleichungssysteme; Ausgleichsrechnung; Differentialgleichung",
            ECTS = 5,
            MV = GetHost(fk06, "HIR"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathematik2"
                    },
                }
        };

        Mathematik2.Groups.Add(GetGroup(CHB, "2"));
        CHB.Modules.Add(Mathematik2);
        _db.CurriculumModules.Add(Mathematik2);

        var Werkstofftechnik1 = new CurriculumModule()
        {
            ShortName = "Werkstofftechnik1",
            Name = "Werkstofftechnik1",
            ModuleId = "CHB10",
            Description = "Erlangung von grundlegenden Kenntnissen über den Aufbau der wichtigsten Konstruktionswerkstoffe (Metalle und Kunststoffe)",
            ECTS = 4,
            MV = GetHost(fk06, "KOC"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Werkstofftechnik1"
                    },
                }

        };

        Werkstofftechnik1.Groups.Add(GetGroup(CHB, "2"));
        CHB.Modules.Add(Werkstofftechnik1);
        _db.CurriculumModules.Add(Werkstofftechnik1);

        var EnergieWärmeTechnik = new CurriculumModule()
        {
            ShortName = "EnergieWärmeTechnik",
            Name = "EnergieWärmeTechnik",
            ModuleId = "CHB11",
            Description = "Fähigkeit zur Anwendung technisch-thermodynamischer Grundsätze im Bereich der Energieumwandlung und zur Beurteilung alternativer Techniken",
            ECTS = 4,
            MV = GetHost(fk06, "WON"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EnergieWärmeTechnik"
                    },
                }
        };

        EnergieWärmeTechnik.Groups.Add(GetGroup(CHB, "2"));
        CHB.Modules.Add(EnergieWärmeTechnik);
        _db.CurriculumModules.Add(EnergieWärmeTechnik);

        var TechnischeMechanik = new CurriculumModule()
        {
            ShortName = "TechnischeMechanik",
            Name = "TechnischeMechanik",
            ModuleId = "CHB12",
            Description = "Mikro- und Makroökonomische Zusammenhänge",
            ECTS = 4,
            MV = GetHost(fk06, "NIE"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeMechanik"
                    },
                }
        };

        TechnischeMechanik.Groups.Add(GetGroup(CHB, "2"));
        CHB.Modules.Add(TechnischeMechanik);
        _db.CurriculumModules.Add(TechnischeMechanik);


        var Fluidmechanik = new CurriculumModule()
        {
            ShortName = "Fluidmechanik",
            Name = "Fluidmechanik",
            ModuleId = "CHB13",
            Description = "Erweiterung der physikalischen Kenntnisse auf dem Gebiet der Fluidmechanik und Übertragung der Kenntnisse auf Anwendungen technischer Systeme. Fähigkeit zu praxisbezogenen Lösungen für den Chemieingenieur",
            ECTS = 4,
            MV = GetHost(fk06, "LIE"),
            ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fluidmechanik"
                    },
                }
        };

        Fluidmechanik.Groups.Add(GetGroup(CHB, "2"));
        CHB.Modules.Add(Fluidmechanik);
        _db.CurriculumModules.Add(Fluidmechanik);

            var AnalytischeChemie = new CurriculumModule()
            {
                ShortName = "AnalytischeChemie",
                Name = "AnalytischeChemie",
                ModuleId = "CHB14",
                Description = "Kenntnis der chemischen Reaktionen, Theorien und Arbeitsmethoden, auf denen die Erkennung und Quantifizierung chemischer Stoffe basieren",
                ECTS = 5,
                MV = GetHost(fk06, "VAS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "AnalytischeChemie"
                    },
                }
            };

            AnalytischeChemie.Groups.Add(GetGroup(CHB, "3"));
            CHB.Modules.Add(AnalytischeChemie);
            _db.CurriculumModules.Add(AnalytischeChemie);

            var Statistik = new CurriculumModule()
            {
                ShortName = "Statistik",
                Name = "Statistik",
                ModuleId = "CHB15",
                Description = "Kenntnis der einschlägigen Begriffe und Methoden der Wahrscheinlichkeitsrechnung sowie der beschreibenden und der schließenden Statistik",
                ECTS = 4,
                MV = GetHost(fk06, "SAS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Statistik"
                    },
                }
            };

            Statistik.Groups.Add(GetGroup(CHB, "3"));
            CHB.Modules.Add(Statistik);
            _db.CurriculumModules.Add(Statistik);

            var PhysikalischeChemie1 = new CurriculumModule()
            {
                ShortName = "PhysikalischeChemie1",
                Name = "PhysikalischeChemie1",
                ModuleId = "CHB16",
                Description = "Kenntnisse der chemischen Thermodynamik, molekularen/makroskopischen Eigenschaften der Materie, Phasengleich/Chemische Gleichgewichte, die für die stofftliche Umwandlungen und den damit zusammenhängenden energetischen Erscheinungen von Bedeutung sind",
                ECTS = 4,
                MV = GetHost(fk06, "ZEY"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PhysikalischeChemie1"
                    },
                }
            };

            PhysikalischeChemie1.Groups.Add(GetGroup(CHB, "3"));
            CHB.Modules.Add(PhysikalischeChemie1);
            _db.CurriculumModules.Add(PhysikalischeChemie1);

            var Werkstofftechnik2 = new CurriculumModule()
            {
                ShortName = "Werkstofftechnik2",
                Name = "Werkstofftechnik2",
                ModuleId = "CHB17",
                Description = "Erlangung von grundlegenden Kenntnissen über den Aufbau, den Einsatz und die Einsatzgrenzen, von Kunststoffen, Korrosionsmechanismen, -erscheinungsbilder und -schutz, Ermüdung und Bruchflächenanalyse",
                ECTS = 4,
                MV = GetHost(fk06, "KOC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Werkstofftechnik2"
                    },
                }
            };

            Werkstofftechnik2.Groups.Add(GetGroup(CHB, "3"));
            CHB.Modules.Add(Werkstofftechnik2);
            _db.CurriculumModules.Add(Werkstofftechnik2);

            var Elektronik = new CurriculumModule()
            {
                ShortName = "Elektronik",
                Name = "Elektronik",
                ModuleId = "CHB18",
                Description = "Vermittlung von Grundkenntnissen der Elektronik unter besonderer Berücksichtigung der Anwendungsmöglichkeiten in der chemischen Technik",
                ECTS = 4,
                MV = GetHost(fk06, "SAS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Elektronik"
                    },
                }
            };

            Elektronik.Groups.Add(GetGroup(CHB, "3"));
            CHB.Modules.Add(Elektronik);
            _db.CurriculumModules.Add(Elektronik);

            var AngewandteChemie = new CurriculumModule()
            {
                ShortName = "AngewandteChemie",
                Name = "AngewandteChemie",
                ModuleId = "CHB19",
                Description = "Edelgasverbindungen, Schwermetalle, Eigenschaften und Präparation von Organometallverbindungen, Höhere Bindungsordnungen, Organohalogene, Gewinnung, Verfügbarkeit und Anwendungen seltener Erden, Katalyse und Oberflächenchemie, Eigenschaften und Präparation von Luminophoren, Radiochemikalien, Präbiotik",
                ECTS = 4,
                MV = GetHost(fk06, "SCH"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "AngewandteChemie"
                    },
                }
            };

            AngewandteChemie.Groups.Add(GetGroup(CHB, "3"));
            CHB.Modules.Add(AngewandteChemie);
            _db.CurriculumModules.Add(AngewandteChemie);

            var MechanischeVerfahrentechnik = new CurriculumModule()
            {
                ShortName = "MechanischeVerfahrentechnik",
                Name = "MechanischeVerfahrentechnik",
                ModuleId = "CHB20",
                Description = "Kenntnis chemisch-technologischer Verfahren der Reaktionsführung. Fähigkeit zur analytischen Erfassung und Lösung von Problemen",
                ECTS = 5,
                MV = GetHost(fk06, "ZEY"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MechanischeVerfahrentechnik"
                    },
                }
            };

            MechanischeVerfahrentechnik.Groups.Add(GetGroup(CHB, "3"));
            CHB.Modules.Add(MechanischeVerfahrentechnik);
            _db.CurriculumModules.Add(MechanischeVerfahrentechnik);

            var PhysikalischeChemie2 = new CurriculumModule()
            {
                ShortName = "PhysikalischeChemie2",
                Name = "PhysikalischeChemie2",
                ModuleId = "CHB21",
                Description = "Kenntnis der Eigenschaften der Materie, die für stoffliche Umwandlungen und Energieänderungen wichtig sind",
                ECTS = 5,
                MV = GetHost(fk06, "VAS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PhysikalischeChemie2"
                    },
                }
            };

            PhysikalischeChemie2.Groups.Add(GetGroup(CHB, "4"));
            CHB.Modules.Add(PhysikalischeChemie2);
            _db.CurriculumModules.Add(PhysikalischeChemie2);

            var ThermischeVerfahrenstechnik = new CurriculumModule()
            {
                ShortName = "ThermischeVerfahrenstechnik",
                Name = "ThermischeVerfahrenstechnik",
                ModuleId = "CHB22",
                Description = "Kenntnis chemisch-technologischer Verfahren der Reaktionsführung. Fähigkeit zur analytischen Erfassung und Lösung von Problemen",
                ECTS = 5,
                MV = GetHost(fk06, "ZEY"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ThermischeVerfahrenstechnik"
                    },
                }
            };

            ThermischeVerfahrenstechnik.Groups.Add(GetGroup(CHB, "4"));
            CHB.Modules.Add(ThermischeVerfahrenstechnik);
            _db.CurriculumModules.Add(ThermischeVerfahrenstechnik);

            var TechnischeChemie = new CurriculumModule()
            {
                ShortName = "TechnischeChemie",
                Name = "TechnischeChemie",
                ModuleId = "CHB23",
                Description = "umweltfreundliche Produktion von Verbrauchsgütern (chemische Grundstoffe, Kunststoffe, Kunstdünger und pharmazeutische Produkte) und Beurteilung der Qualität, enge Verknüpfung von Technischer Chemie, chemischer Verfahrenstechnik und Umwelttechnik",
                ECTS = 4,
                MV = GetHost(fk06, "ZEY"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeChemie"
                    },
                }
            };

            TechnischeChemie.Groups.Add(GetGroup(CHB, "4"));
            CHB.Modules.Add(TechnischeChemie);
            _db.CurriculumModules.Add(TechnischeChemie);

            var Simulationstechnik = new CurriculumModule()
            {
                ShortName = "Simulationstechnik",
                Name = "Simulationstechnik",
                ModuleId = "CHB24",
                Description = "Erkennen, wie das Verhalten realer Systeme anhand geeigneter mathematischer Modelle simuliert werden kann. Fähigkeit, Modelle für verschiedene Medien und Kopplungen zu erstellen, in Matlab/Simulink zu programmieren, Simulationsstudien durchzuführen, Fehler und Grenzen zu erkennen",
                ECTS = 4,
                MV = GetHost(fk06, "FRO"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Simulationstechnik"
                    },
                }
            };

            Simulationstechnik.Groups.Add(GetGroup(CHB, "4"));
            CHB.Modules.Add(Simulationstechnik);
            _db.CurriculumModules.Add(Simulationstechnik);

            var Messtechnik = new CurriculumModule()
            {
                ShortName = "Messtechnik",
                Name = "Messtechnik",
                ModuleId = "CHB25",
                Description = "Verständnis grundlegender Messverfahren und -prinzipien",
                ECTS = 4,
                MV = GetHost(fk06, "FRO"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Messtechnik"
                    },
                }
            };

            Messtechnik.Groups.Add(GetGroup(CHB, "4"));
            CHB.Modules.Add(Messtechnik);
            _db.CurriculumModules.Add(Messtechnik);

            var Qualitätsmanagment = new CurriculumModule()
            {
                ShortName = "Qualitätsmanagment",
                Name = "Qualitätsmanagment",
                ModuleId = "CHB26",
                Description = "Kennen und anwenden des strategischen Qualitätsmangements. Beherrschung präventiver Qualitätssicherungsmethoden. Grundlagen der Qualitätssicherung in Entwicklung und Fertigung. Erarbeiten von Qualitätsrichtlinien und deren Umsetzung. Statistische Grundlagen und Verfahren des Qualitätsmanagements. Organisation des Qualitätsmanagements in der Produktion",
                ECTS = 4,
                MV = GetHost(fk06, "FUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Qualitätsmanagment"
                    },
                }
            };

            Qualitätsmanagment.Groups.Add(GetGroup(CHB, "4"));
            CHB.Modules.Add(Qualitätsmanagment);
            _db.CurriculumModules.Add(Qualitätsmanagment);


            var Regelungstechnik = new CurriculumModule()
            {
                ShortName = "Regelungstechnik",
                Name = "Regelungstechnik",
                ModuleId = "CHB27",
                Description = "Erkennen, wie mit Steuerungen und Regelungen Vorgänge in realen Systemen gezielt beeinflusst werden können. Fähigkeit, Steuerungen und Regelungen zu spezifizieren, modellgestützt zu entwickeln, in einem Laborprojekt in Betrieb zu nehmen und zu beurteilen. Zusammenhänge erkennen mit der chemischen Verfahrenstechnik",
                ECTS = 4,
                MV = GetHost(fk06, "FRO"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Regelungstechnik"
                    },
                }
            };

            Regelungstechnik.Groups.Add(GetGroup(CHB, "4"));
            CHB.Modules.Add(Regelungstechnik);
            _db.CurriculumModules.Add(Regelungstechnik);

            var Industriepraktikum = new CurriculumModule()
            {
                ShortName = "Industriepraktikum",
                Name = "Industriepraktikum",
                ModuleId = "CHB28",
                Description = "Vertiefung der in der bisherigen Ausbildung erworbenen Kenntnisse und Fähigkeiten. Kennenlernen der Tätigkeit und Arbeitsmethodik des Chemieingenieurs in Institutionen und im industriellen Umfeld anhand konkreter Aufgabenstellungen in Bereichen der angewandten Forschung und Entwicklung, Dauer:24 Wochen a 4 Tage oder 19 Wochen a 5 Tage",
                ECTS = 22,
                MV = GetHost(fk06, "MAI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Industriepraktikum"
                    },
                }
            };

            Industriepraktikum.Groups.Add(GetGroup(CHB, "5"));
            CHB.Modules.Add(Industriepraktikum);
            _db.CurriculumModules.Add(Industriepraktikum);

            var Praxisseminar = new CurriculumModule()
            {
                ShortName = "Praxisseminar",
                Name = "Praxisseminar",
                ModuleId = "CHB29",
                Description = "Die Studierenden lernen Präsentationen und Fachvorträge ausarbeiten und sie vor einem Publikum zu präsentieren. Sie erfahren ihre Stärken und Schwächen und können in einem 2. Vortrag gezielt an Verbesserungen arbeiten",
                ECTS = 4,
                MV = GetHost(fk06, "MAI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Praxisseminar"
                    },
                }
            };

            Praxisseminar.Groups.Add(GetGroup(CHB, "5"));
            CHB.Modules.Add(Praxisseminar);
            _db.CurriculumModules.Add(Praxisseminar);

            var BetriebswirtschaftlicheGrundlagen = new CurriculumModule()
            {
                ShortName = "BetriebswirtschaftlicheGrundlagen",
                Name = "BetriebswirtschaftlicheGrundlagen",
                ModuleId = "CHB30",
                Description = "Dieses Modul vermittelt grundlegende fachübergreifende, insbesondere betriebswirtschaftliche Kenntnisse, die Fähigkeit, technische Produkte und betriebliche Prozesse nach wirtschaftlichen Kriterien zu analysieren, zu bewerten und zu gestalten sowie die Kompetenz, fach- und disziplinübergreifend im Team zu arbeiten",
                ECTS = 4,
                MV = GetHost(fk06, "ZAN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BetriebswirtschaftlicheGrundlagen"
                    },
                }
            };

            BetriebswirtschaftlicheGrundlagen.Groups.Add(GetGroup(CHB, "5"));
            CHB.Modules.Add(BetriebswirtschaftlicheGrundlagen);
            _db.CurriculumModules.Add(BetriebswirtschaftlicheGrundlagen);

            var Apparatetechnik = new CurriculumModule()
            {
                ShortName = "Apparatetechnik",
                Name = "Apparatetechnik",
                ModuleId = "CHB31",
                Description = "Fähigkeit zur Planung und Auslegung von Apparaten und zugehörigen Rohrleitungssystemen für Anwendungen in der Chemischen Technik",
                ECTS = 5,
                MV = GetHost(fk06, "ZEY"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Apparatetechnik"
                    },
                }
            };

            Apparatetechnik.Groups.Add(GetGroup(CHB, "6"));
            CHB.Modules.Add(Apparatetechnik);
            _db.CurriculumModules.Add(Apparatetechnik);

            var AnalytischeChemie2 = new CurriculumModule()
            {
                ShortName = "AnalytischeChemie2",
                Name = "AnalytischeChemie2",
                ModuleId = "CHB32",
                Description = "Fähigkeit, chemische Reaktionen im Bereich der analytischen Chemie zu verstehen, bei unterschiedlichen Fragenstellungen anzuwenden und zu beschreiben",
                ECTS = 5,
                MV = GetHost(fk06, "VAS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "AnalytischeChemie2"
                    },
                }
            };

            AnalytischeChemie2.Groups.Add(GetGroup(CHB, "6"));
            CHB.Modules.Add(AnalytischeChemie2);
            _db.CurriculumModules.Add(AnalytischeChemie2);

            var InstrumentelleAnalytik1 = new CurriculumModule()
            {
                ShortName = "InstrumentelleAnalytik1",
                Name = "InstrumentelleAnalytik1",
                ModuleId = "CHB33",
                Description = "Grundlegende Kenntnisse der modernen Verfahren in der der instrumentellen Analytik von der Theorie bis zur praktischen Umsetzung. Dies beinhaltet sowohl die physikalischen Prinzipien als auch gerätetechnische Aspekte und praktische Anwendungen",
                ECTS = 5,
                MV = GetHost(fk06, "MAI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "InstrumentelleAnalytik1"
                    },
                }
            };

            InstrumentelleAnalytik1.Groups.Add(GetGroup(CHB, "6"));
            CHB.Modules.Add(InstrumentelleAnalytik1);
            _db.CurriculumModules.Add(InstrumentelleAnalytik1);

            var ChemieRadioÖkotoxizität = new CurriculumModule()
            {
                ShortName = "ChemieRadioÖkotoxizität",
                Name = "ChemieRadioÖkotoxizität",
                ModuleId = "CHB34",
                Description = "Vermittlung von Grundlagen der ökologischen, pharmakologischen, Radio- und Gewerbetoxikologie",
                ECTS = 4,
                MV = GetHost(fk06, "SCH"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ChemieRadioÖkotoxizität"
                    },
                }
            };

            ChemieRadioÖkotoxizität.Groups.Add(GetGroup(CHB, "7"));
            CHB.Modules.Add(ChemieRadioÖkotoxizität);
            _db.CurriculumModules.Add(ChemieRadioÖkotoxizität);


            var InstrumentelleAnalytik2 = new CurriculumModule()
            {
                ShortName = "InstrumentelleAnalytik2",
                Name = "InstrumentelleAnalytik2",
                ModuleId = "CHB35",
                Description = "Vertiefte Kenntnisse der modernen Verfahren in der der instrumentellen Analytik von der Theorie bis zur praktischen Umsetzung",
                ECTS = 4,
                MV = GetHost(fk06, "VAS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "InstrumentelleAnalytik2"
                    },
                }
            };

            InstrumentelleAnalytik2.Groups.Add(GetGroup(CHB, "7"));
            CHB.Modules.Add(InstrumentelleAnalytik2);
            _db.CurriculumModules.Add(InstrumentelleAnalytik2);


            var Bachelorarbeit = new CurriculumModule()
            {
                ShortName = "Bachelorarbeit",
                Name = "Bachelorarbeit",
                ModuleId = "CHB36",
                Description = "Bachelorarbeit",
                ECTS = 12,
                MV = GetHost(fk06, ""),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bachelorarbeit"
                    },
                }
            };

            Bachelorarbeit.Groups.Add(GetGroup(CHB, "7"));
            CHB.Modules.Add(Bachelorarbeit);
            _db.CurriculumModules.Add(Bachelorarbeit);




        }

    public void InitCatalogCHB_Wahl(ActivityOrganiser fk06, Curriculum wi)
    {

        // WPMs, AW und Abschlussarbeit
        var aw = new CurriculumModule()
        {
            ShortName = "AW",
            Name = "Allgemeinwissenschaften",
            ModuleId = "W1",
            Description = "",
            ECTS = 4,
        };

        // gehört zu allen Studiengruppen im 5. Semester
        aw.Groups.Add(GetGroup(wi, "3 BIO"));
        aw.Groups.Add(GetGroup(wi, "3 INF"));
        aw.Groups.Add(GetGroup(wi, "3 TEC"));
        wi.Modules.Add(aw);
        _db.CurriculumModules.Add(aw);


        var wpm1 = new CurriculumModule()
        {
            ShortName = "WPM1",
            Name = "Fachwissenschaftliches Wahlpflichtmodul I",
            ModuleId = "W2",
            Description = "",
            ECTS = 4,
        };

        // gehört zu allen Studiengruppen im 4. Semester
        wpm1.Groups.Add(GetGroup(wi, "4 BIO"));
        wpm1.Groups.Add(GetGroup(wi, "4 INF"));
        wpm1.Groups.Add(GetGroup(wi, "4 TEC"));
        wi.Modules.Add(wpm1);
        _db.CurriculumModules.Add(wpm1);

        var wpm2 = new CurriculumModule()
        {
            ShortName = "WPM2",
            Name = "Fachwissenschaftliches Wahlpflichtmodul II",
            ModuleId = "W3",
            Description = "",
            ECTS = 4,
        };

        // gehört zu allen Studiengruppen im 5. Semester
        wpm2.Groups.Add(GetGroup(wi, "5 BIO"));
        wpm2.Groups.Add(GetGroup(wi, "5 INF"));
        wpm2.Groups.Add(GetGroup(wi, "5 TEC"));
        wi.Modules.Add(wpm2);
        _db.CurriculumModules.Add(wpm2);

        var wpm3 = new CurriculumModule()
        {
            ShortName = "WPM3",
            Name = "Fachwissenschaftliches Wahlpflichtmodul III",
            ModuleId = "W4",
            Description = "",
            ECTS = 4,
        };

        // gehört zu allen Studiengruppen im 6. Semester
        wpm3.Groups.Add(GetGroup(wi, "6 BIO"));
        wpm3.Groups.Add(GetGroup(wi, "6 INF"));
        wpm3.Groups.Add(GetGroup(wi, "6 TEC"));
        wi.Modules.Add(wpm3);
        _db.CurriculumModules.Add(wpm3);

        var prak = new CurriculumModule()
        {
            ShortName = "PRAK",
            Name = "Industriepraktikum",
            ModuleId = "W5",
            Description = "",
            ECTS = 20,
        };

        // gehört zu allen Studiengruppen im 6. Semester
        prak.Groups.Add(GetGroup(wi, "6 BIO"));
        prak.Groups.Add(GetGroup(wi, "6 INF"));
        prak.Groups.Add(GetGroup(wi, "6 TEC"));
        wi.Modules.Add(prak);
        _db.CurriculumModules.Add(prak);



        var bac = new CurriculumModule()
        {
            ShortName = "BAC",
            Name = "Abschlussarbeit",
            ModuleId = "W6",
            Description = "",
            ECTS = 12,
        };

        // gehört zu allen Studiengruppen im 7. Semester
        bac.Groups.Add(GetGroup(wi, "7 BIO"));
        bac.Groups.Add(GetGroup(wi, "7 INF"));
        bac.Groups.Add(GetGroup(wi, "7 TEC"));
        wi.Modules.Add(bac);
        _db.CurriculumModules.Add(bac);


        _db.SaveChanges();
    }

}
}
