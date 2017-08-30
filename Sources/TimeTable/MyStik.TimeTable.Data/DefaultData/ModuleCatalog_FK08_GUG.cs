using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogGUG_GS(ActivityOrganiser fk08, Curriculum GUG)
        {
            var m1 = new CurriculumModule()
            {
                ShortName = "Mathematik 1",
                Name = "Mathematik 1",
                ModuleId = "11",
                Description = "Ziel der Lehrveranstaltung ist es, ausgewählte Kapitel der Analysis zu wiederholen und zu vertiefen",
                ECTS = 5,
                MV = GetHost(fk08, "ZOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathematik 1"
                    },
                }

            };

            m1.Groups.Add(GetGroup(GUG, "1"));
            GUG.Modules.Add(m1);
            _db.CurriculumModules.Add(m1);

            var phy = new CurriculumModule()
            {
                ShortName = "Physik",
                Name = "Physik",
                ModuleId = "12",
                Description = "Einführung in die Grundlagen der Physik für Geodäten",
                ECTS = 4,
                MV = GetHost(fk08, "LEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik"
                    },
                }

            };

            phy.Groups.Add(GetGroup(GUG, "1"));
            GUG.Modules.Add(phy);
            _db.CurriculumModules.Add(phy);

            var info = new CurriculumModule()
            {
                ShortName = "Einführung in die Informatik",
                Name = "Einführung in die Informatik",
                ModuleId = "13",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage, Daten der Geoinformatik formal zu beschreiben und Inhalte zu strukturieren",
                ECTS = 5,
                MV = GetHost(fk08, "JOS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Einführung in die Informatik"
                    },
                }

            };

            info.Groups.Add(GetGroup(GUG, "1"));
            GUG.Modules.Add(info);
            _db.CurriculumModules.Add(info);

            var geoalg = new CurriculumModule()
            {
                ShortName = "Geodätische Algorithmen",
                Name = "Geodätische Algorithmen",
                ModuleId = "11",
                Description = "Nach der Teilnahme an den Modulveranstaltungen sind die Studierenden in der Lage, die grundlegenden geodätischen Algorithmen zu wissen und zu verstehen",
                ECTS = 5,
                MV = GetHost(fk08, "CZA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geodätische Algorithmen"
                    },
                }

            };

            geoalg.Groups.Add(GetGroup(GUG, "1"));
            GUG.Modules.Add(geoalg);
            _db.CurriculumModules.Add(geoalg);

            var geogru = new CurriculumModule()
            {
                ShortName = "Geodätische Grundlagen 1",
                Name = "Geodätische Grundlagen 1",
                ModuleId = "15",
                Description = "Nach der Teilnahme an den Modulveranstaltungen sind die Studierenden in der Lage, die grundlegenden geodätischen Messverfahren zur Lage - und Höhenbestimmung und die dazugehörigen Berechnungsmethoden zu wissen",
                ECTS = 5,
                MV = GetHost(fk08, "CZA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geodätische Grundlagen 1"
                    },
                }

            };

            geogru.Groups.Add(GetGroup(GUG, "1"));
            GUG.Modules.Add(geogru);
            _db.CurriculumModules.Add(geogru);

            var grupra = new CurriculumModule()
            {
                ShortName = "Grundpraktikum (Internship)",
                Name = "Grundpraktikum (Internship)",
                ModuleId = "16",
                Description = "Ziel der praktischen Ausbildung ist es, Einblick in das Berufsfeld der angewandten Geodäsie und Geoinformatik zu erhalten sowie gelerntes Grundlagenwissen über die Methoden der Geodatenerfassung,-auswertung und -visualisierung schrittweise anzuwenden und weiter zu vertiefen",
                ECTS = 4,
                MV = GetHost(fk08, "GD"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Grundpraktikum (Internship)"
                    },
                }

            };

            grupra.Groups.Add(GetGroup(GUG, "1"));
            GUG.Modules.Add(grupra);
            _db.CurriculumModules.Add(grupra);



            var aw1 = new CurriculumModule()
            {
                ShortName = "Allgemeinwissenschaften",
                Name = "Allgemeinwissenschaften",
                ModuleId = "17",
                Description = "Kulturelle Kompetenz, Schlüsselqualifikationen, Internationale Kompetenz",
                ECTS = 5,
                MV = GetHost(fk08, "AND"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Allgemeinwissenschaften"
                    },
                }

            };

            aw1.Groups.Add(GetGroup(GUG, "1"));
            GUG.Modules.Add(aw1);
            _db.CurriculumModules.Add(aw1);

            var m2 = new CurriculumModule()
            {
                ShortName = "Mathematik 2",
                Name = "Mathematik 2",
                ModuleId = "21",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage die Methoden der linearen Algebra und der mathematischenStatistik für die Lösung von geodätischen Aufgaben,insbesondre für die Ausgleichungsrechnung einzusetzen.",
                ECTS = 5,
                MV = GetHost(fk08, "LOT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathematik 2"
                    },
                }

            };

            m2.Groups.Add(GetGroup(GUG, "2"));
            GUG.Modules.Add(m2);
            _db.CurriculumModules.Add(m2);

            var compu = new CurriculumModule()
            {
                ShortName = "Computergrafik und Bildverarbeitung",
                Name = "Computergrafik und Bildverarbeitung",
                ModuleId = "22",
                Description = "Nach der Teilnahme an den Modulveranstaltungen sind die Studierenden in der Lage die Computergrafik einschließlich der Grafiksysteme zu kennen und zu verstehen.",
                ECTS = 4,
                MV = GetHost(fk08, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Computergrafik und Bildverarbeitung"
                    },
                }

            };

            compu.Groups.Add(GetGroup(GUG, "2"));
            GUG.Modules.Add(compu);
            _db.CurriculumModules.Add(compu);

            var oop = new CurriculumModule()
            {
                ShortName = "Objektorientierte Programmierung",
                Name = "Objektorientierte Programmierung",
                ModuleId = "23",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage, Aufgabenstellungen der Geodäsie und Geoinformatik zu analysieren und das resultierende Software - Design als modulare Komponenten in einer objekt - orientierten Programmiersprache zu implementieren und diese zu testen.",
                ECTS = 5,
                MV = GetHost(fk08, "JOS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Objektorientierte Programmierung"
                    },
                }

            };

            oop.Groups.Add(GetGroup(GUG, "2"));
            GUG.Modules.Add(oop);
            _db.CurriculumModules.Add(oop);

            var senso = new CurriculumModule()
            {
                ShortName = "Sensorik",
                Name = "Sensorik",
                ModuleId = "24",
                Description = "Nach der Teilnahme an den Modulveranstaltungen sind die Studierenden in der Lage den Aufbau und die Funktionsweise von geodätischen Instrumenten und Sensoren zu kennen und zu verstehen sowie diese fachgerecht anzuwenden",
                ECTS = 5,
                MV = GetHost(fk08, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Sensorik"
                    },
                }

            };

            senso.Groups.Add(GetGroup(GUG, "2"));
            GUG.Modules.Add(senso);
            _db.CurriculumModules.Add(senso);

            var geogru2 = new CurriculumModule()
            {
                ShortName = "Geodätische Grundlagen 2",
                Name = "Geodätische Grundlagen 2",
                ModuleId = "25",
                Description = "Ergänzend zu dem Modul „Geodätische Grundlagen 1“ sind die Studierenden nach der Teilnahme an den Modulveranstaltungen in der Lage, weitere geodätischen Messverfahren zur Lage - und Höhenbestimmung und diedazugehörigen Berechnungsmethoden zu wissen, die wichtigsten Problemstellungen der Verfahren zu bewertenund die Verfahren in praktischen Aufgabenstellungen anzuwenden.",
                ECTS = 5,
                MV = GetHost(fk08, "CZA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geodätische Grundlagen 2"
                    },
                }

            };

            geogru2.Groups.Add(GetGroup(GUG, "2"));
            GUG.Modules.Add(geogru2);
            _db.CurriculumModules.Add(geogru2);

            var liegrecht = new CurriculumModule()
            {
                ShortName = "Liegenschaftsrecht",
                Name = "Liegenschaftsrecht",
                ModuleId = "26",
                Description = "Verständnis für die Bedeutung des Liegenschafts- und Grundbuchrechts",
                ECTS = 4,
                MV = GetHost(fk08, "ZOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Liegenschaftsrecht"
                    },
                }

            };

            liegrecht.Groups.Add(GetGroup(GUG, "2"));
            GUG.Modules.Add(liegrecht);
            _db.CurriculumModules.Add(liegrecht);

            var aw2 = new CurriculumModule()
            {
                ShortName = "Allgemeinwissenschaften",
                Name = "Allgemeinwissenschaften",
                ModuleId = "27",
                Description = "General Science",
                ECTS = 2,
                MV = GetHost(fk08, "AND"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Allgemeinwissenschaften"
                    },
                }

            };

            aw2.Groups.Add(GetGroup(GUG, "2"));
            GUG.Modules.Add(aw2);
            _db.CurriculumModules.Add(aw2);

            var ausgl = new CurriculumModule()
            {
                ShortName = "Ausgleichungsrechnung",
                Name = "Ausgleichungsrechnung",
                ModuleId = "31",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage die Methode der kleinsten Quadrate zur Schätzung von bestmöglichen Parametern aus Beobachtungen und zur Qualitätssicherung für Messprozesse einzusetzen.",
                ECTS = 5,
                MV = GetHost(fk08, "LOT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ausgleichungsrechnung"
                    },
                }

            };

            ausgl.Groups.Add(GetGroup(GUG, "3"));
            GUG.Modules.Add(ausgl);
            _db.CurriculumModules.Add(ausgl);

            var cad = new CurriculumModule()
            {
                ShortName = "CAD",
                Name = "CAD",
                ModuleId = "32",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage, ein CAD-System auf fachspezifische 2D- und 3DFragestellungen anzuwenden",
                ECTS = 5,
                MV = GetHost(fk08, "WIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "CAD"
                    },
                }

            };

            cad.Groups.Add(GetGroup(GUG, "3"));
            GUG.Modules.Add(cad);
            _db.CurriculumModules.Add(cad);

            var geoinfo = new CurriculumModule()
            {
                ShortName = "Geoinformatik",
                Name = "Geoinformatik",
                ModuleId = "33",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage, Anwendungsschemata für Geoinformationssysteme zu modellieren und diese in einer GIS - Software zu realisieren",
                ECTS = 5,
                MV = GetHost(fk08, "JOS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geoinformatik"
                    },
                }

            };

            geoinfo.Groups.Add(GetGroup(GUG, "3"));
            GUG.Modules.Add(geoinfo);
            _db.CurriculumModules.Add(geoinfo);

            var geodb = new CurriculumModule()
            {
                ShortName = "Geodatenbanken",
                Name = "Geodatenbanken",
                ModuleId = "34",
                Description = "Grundverständnis und Aufgaben von Geodatenbanken",
                ECTS = 5,
                MV = GetHost(fk08, "KLU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geodatenbanken"
                    },
                }

            };

            geodb.Groups.Add(GetGroup(GUG, "3"));
            GUG.Modules.Add(geodb);
            _db.CurriculumModules.Add(geodb);

            var bezug = new CurriculumModule()
            {
                ShortName = "Geodätische Bezugssysteme",
                Name = "Geodätische Bezugssysteme",
                ModuleId = "35",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage, geodätische Berechnungen auf einer Referenzfläche in verschiedenen Koordinatenarten durchzuführen",
                ECTS = 5,
                MV = GetHost(fk08, "JOS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geodätische Bezugssysteme"
                    },
                }

            };

            bezug.Groups.Add(GetGroup(GUG, "3"));
            GUG.Modules.Add(bezug);
            _db.CurriculumModules.Add(bezug);

            var compvisio = new CurriculumModule()
            {
                ShortName = "Computervision",
                Name = "Computervision",
                ModuleId = "36",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage, die grundlegenden Methoden dreidimensionalen Rechnersehens zu verstehen",
                ECTS = 5,
                MV = GetHost(fk08, "KRZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Computervision"
                    },
                }

            };

            compvisio.Groups.Add(GetGroup(GUG, "3"));
            GUG.Modules.Add(compvisio);
            _db.CurriculumModules.Add(compvisio);

            var GNSS = new CurriculumModule()
            {
                ShortName = "Global Navigation Satellite Systems",
                Name = "Global Navigation Satellite Systems",
                ModuleId = "41",
                Description = "Nach der Teilnahme an den Modulveranstaltungen sind die Studierenden in der Lage GNSS-Systeme und deren Komponenten zu benennen sowie die Grundlagen und wichtigsten Problemstellungen der Positionsbestimmung mit Satelliten darzustellen und zu bewerten",
                ECTS = 5,
                MV = GetHost(fk08, "CZA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Global Navigation Satellite Systems"
                    },
                }

            };

            GNSS.Groups.Add(GetGroup(GUG, "4"));
            GUG.Modules.Add(GNSS);
            _db.CurriculumModules.Add(GNSS);

            var fern1 = new CurriculumModule()
            {
                ShortName = "Fernerkundung 1",
                Name = "Fernerkundung 1",
                ModuleId = "42",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage, die grundlegenden Methoden zur Klassifikation von Fernerkundungsdaten zu verstehen",
                ECTS = 5,
                MV = GetHost(fk08, "KRZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fernerkundung 1"
                    },
                }

            };

            fern1.Groups.Add(GetGroup(GUG, "4"));
            GUG.Modules.Add(fern1);
            _db.CurriculumModules.Add(fern1);

            var geoinfosys = new CurriculumModule()
            {
                ShortName = "Geoinformationssysteme",
                Name = "Geoinformationssysteme",
                ModuleId = "43",
                Description = "Nach der Teilnahme an diesem Modul sind die Studierenden in der Lage die Einsatzmöglichkeiten von Geoinformationssystemen auf andere Anwendungsfälle zu übertragen und damit eigenständig Lösungsansätze für Problemstellungen der Geoinformatik zu entwickeln.",
                ECTS = 5,
                MV = GetHost(fk08, "JOS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geoinformationssysteme"
                    },
                }

            };

            geoinfosys.Groups.Add(GetGroup(GUG, "4"));
            GUG.Modules.Add(geoinfosys);
            _db.CurriculumModules.Add(geoinfosys);

            var dreid = new CurriculumModule()
            {
                ShortName = "3D-Objekterfassung",
                Name = "3D-Objekterfassung",
                ModuleId = "44",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage Verfahren zur 3D-Objekterfassung und die dazugehörenden Berechnungsmethoden auszuwählen und für die Umsetzung in der Praxis weiter zu entwickeln, die wichtigsten Problemstellungen der Verfahren zu bewerten und ein Ausgleichungsprogramm für die Auswertung der Messungen anzuwenden",
                ECTS = 5,
                MV = GetHost(fk08, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "3D-Objekterfassung"
                    },
                }

            };

            dreid.Groups.Add(GetGroup(GUG, "4"));
            GUG.Modules.Add(dreid);
            _db.CurriculumModules.Add(dreid);

            var raumland = new CurriculumModule()
            {
                ShortName = "Raumplanung und Landmanagement",
                Name = "Raumplanung und Landmanagement",
                ModuleId = "45",
                Description = "Überblick über Ziele und Bedeutung von Raumordnung, Landesplanung und Regionalplanung",
                ECTS = 5,
                MV = GetHost(fk08, "ZOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Raumplanung und Landmanagement"
                    },
                }

            };

            raumland.Groups.Add(GetGroup(GUG, "4"));
            GUG.Modules.Add(raumland);
            _db.CurriculumModules.Add(raumland);

            var rechtimmo = new CurriculumModule()
            {
                ShortName = "Städtebaurecht und Immobilienwertermittlung",
                Name = "Städtebaurecht und Immobilienwertermittlung",
                ModuleId = "46",
                Description = "Kenntnis des Städtebaurechts, Einsicht in das Bauordnungsrecht und die Grundlagen der Immobilienwertermittlung, Fähigkeit zur Bearbeitung von Bodenordnungsverfahren.",
                ECTS = 5,
                MV = GetHost(fk08, "ZOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Städtebaurecht und Immobilienwertermittlung"
                    },
                }

            };

            rechtimmo.Groups.Add(GetGroup(GUG, "4"));
            GUG.Modules.Add(rechtimmo);
            _db.CurriculumModules.Add(rechtimmo);

            // Grenze

            var navi = new CurriculumModule()
            {
                ShortName = "Navigation",
                Name = "Navigation",
                ModuleId = "51",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage, grundlegende Navigationsverfahren und gängige Navigationssensorik zu verstehen bzw.die Verwendungsmöglichkeiten beurteilen zu können, Berechnungen durchzuführen und das Genauigkeitspotenzial verschiedener Navigationssensorik analysieren zu können.",
                ECTS = 5,
                MV = GetHost(fk08, "TIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Navigation"
                    },
                }

            };

            navi.Groups.Add(GetGroup(GUG, "5"));
            GUG.Modules.Add(navi);
            _db.CurriculumModules.Add(navi);

            var fernerk2 = new CurriculumModule()
            {
                ShortName = "Fernerkundung 2",
                Name = "Fernerkundung 2",
                ModuleId = "52",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage, die grundlegenden Methoden zur Klassifikation von speziellen Fernerkundungsdaten(z.B.flugzeugestütztes Laserscanning, Radar) zu verstehen",
                ECTS = 5,
                MV = GetHost(fk08, "KRZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fernerkundung 2"
                    },
                }

            };

            fernerk2.Groups.Add(GetGroup(GUG, "5"));
            GUG.Modules.Add(fernerk2);
            _db.CurriculumModules.Add(fernerk2);

            var geovi = new CurriculumModule()
            {
                ShortName = "Geovisualisierung",
                Name = "Geovisualisierung",
                ModuleId = "53",
                Description = "Geodaten nutzungs- und zielgruppenorientiert zu visualisieren",
                ECTS = 5,
                MV = GetHost(fk08, "OST"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geovisualisierung"
                    },
                }

            };

            geovi.Groups.Add(GetGroup(GUG, "5"));
            GUG.Modules.Add(geovi);
            _db.CurriculumModules.Add(geovi);

            var inggeo = new CurriculumModule()
            {
                ShortName = "Ingenieurgeodäsie",
                Name = "Ingenieurgeodäsie",
                ModuleId = "54",
                Description = "Nach der Teilnahme an den Modulveranstaltungen sind die Studierenden in der Lage Verfahren der Ingenieurgeodäsie und die dazugehörenden Berechnungsmethoden auszuwählen und für praktische Aufgabenstellungen weiter zu entwickeln, die wichtigsten Problemstellungen der Verfahren zu bewerten und mit einem Trassierungsprogramm zu arbeiten",
                ECTS = 5,
                MV = GetHost(fk08, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ingenieurgeodäsie"
                    },
                }

            };

            inggeo.Groups.Add(GetGroup(GUG, "5"));
            GUG.Modules.Add(inggeo);
            _db.CurriculumModules.Add(inggeo);

            var boden = new CurriculumModule()
            {
                ShortName = "Projekt Bodenmanagement und GIS",
                Name = "Projekt Bodenmanagement und GIS",
                ModuleId = "55",
                Description = "Fähigkeit zur planerischen und technischen Bearbeitung eines Bodenordnungsverfahrens",
                ECTS = 5,
                MV = GetHost(fk08, "ZOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Projekt Bodenmanagement und GIS"
                    },
                }

            };

            boden.Groups.Add(GetGroup(GUG, "5"));
            GUG.Modules.Add(boden);
            _db.CurriculumModules.Add(boden);

            var ppm = new CurriculumModule()
            {
                ShortName = "Personal- und Projektmanagement",
                Name = "Personal- und Projektmanagement",
                ModuleId = "56",
                Description = "Kenntnisse im Bereich der sozialen Kompetenz. Fähigkeit zum Erkennen verschiedener Kommunikationsweisen und Führungsverhalten.Kenntnisse in Arbeitsorganisation und Projektmanagement",
                ECTS = 5,
                MV = GetHost(fk08, "ZOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Personal- und Projektmanagement"
                    },
                }

            };

            ppm.Groups.Add(GetGroup(GUG, "5"));
            GUG.Modules.Add(ppm);
            _db.CurriculumModules.Add(ppm);

            var geogis = new CurriculumModule()
            {
                ShortName = "Projekt Geodäsie und GIS",
                Name = "Projekt Geodäsie und GIS",
                ModuleId = "61",
                Description = "Die Studierenden sind nach der Teilnahme an dem Modul in der Lage, eine 3D-Objektaufnahme mit unterschiedlicher geodätischer Messtechnik durchzuführen und diese rechnerisch und graphisch auszuwerten",
                ECTS = 6,
                MV = GetHost(fk08, "CZA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Projekt Geodäsie und GIS"
                    },
                }

            };

            geogis.Groups.Add(GetGroup(GUG, "6"));
            GUG.Modules.Add(geogis);
            _db.CurriculumModules.Add(geogis);

            var prakti = new CurriculumModule()
            {
                ShortName = "Praktikum",
                Name = "Praktikum (18 Wochen á 5 Tage)",
                ModuleId = "62",
                Description = "Ziel der praktischen Ausbildung ist es, Einsicht in den Ablauf von Projekten aus den verschiedenen Bereichen der angewandten Geodäsie und Geoinformatik zu erhalten, gelerntes Fachwissen anzuwenden und zu vertiefen",
                ECTS = 24,
                MV = GetHost(fk08, "GD"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Praktikum (18 Wochen á 5 Tage)"
                    },
                }

            };

            prakti.Groups.Add(GetGroup(GUG, "6"));
            GUG.Modules.Add(prakti);
            _db.CurriculumModules.Add(prakti);

            var wpm1 = new CurriculumModule()
            {
                ShortName = "Wahlpflichtmodul 1",
                Name = "Wahlpflichtmodul 1",
                ModuleId = "71",
                Description = "Das Wahlpflichtangebot wird seitens der Fakultät in jedem Jahr aktuell festgelegt. Lernziele gemäß der Modulbeschreibungen.",
                ECTS = 5,
                MV = GetHost(fk08, "WPM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wahlpflichtmodul 1"
                    },
                }

            };

            wpm1.Groups.Add(GetGroup(GUG, "7"));
            GUG.Modules.Add(wpm1);
            _db.CurriculumModules.Add(wpm1);

            var wpm2 = new CurriculumModule()
            {
                ShortName = "Wahlpflichtmodul 2",
                Name = "Wahlpflichtmodul 2",
                ModuleId = "72",
                Description = "Das Wahlpflichtangebot wird seitens der Fakultät in jedem Jahr aktuell festgelegt. Lernziele gemäß der Modulbeschreibungen.",
                ECTS = 5,
                MV = GetHost(fk08, "WPM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wahlpflichtmodul 2"
                    },
                }

            };

            wpm2.Groups.Add(GetGroup(GUG, "7"));
            GUG.Modules.Add(wpm2);
            _db.CurriculumModules.Add(wpm2);

            var wpm3 = new CurriculumModule()
            {
                ShortName = "Wahlpflichtmodul 3",
                Name = "Wahlpflichtmodul 3",
                ModuleId = "73",
                Description = "Das Wahlpflichtangebot wird seitens der Fakultät in jedem Jahr aktuell festgelegt. Lernziele gemäß der Modulbeschreibungen.",
                ECTS = 5,
                MV = GetHost(fk08, "WPM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wahlpflichtmodul 3"
                    },
                }

            };

            wpm3.Groups.Add(GetGroup(GUG, "7"));
            GUG.Modules.Add(wpm3);
            _db.CurriculumModules.Add(wpm3);

            var bacharbsem = new CurriculumModule()
            {
                ShortName = "Bachelorarbeit und Bachelorseminar",
                Name = "Bachelorarbeit und Bachelorseminar",
                ModuleId = "74",
                Description = "Fähigkeit, eine praxisbezogene Aufgabenstellung aus den Fachgebieten des Studiengangs und deren Anwendungen in der jeweiligen Fachdisziplin selbstständig zu bearbeiten.",
                ECTS = 15,
                MV = GetHost(fk08, "ALL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bachelorarbeit und Bachelorseminar"
                    },
                }

            };

            bacharbsem.Groups.Add(GetGroup(GUG, "7"));
            GUG.Modules.Add(bacharbsem);
            _db.CurriculumModules.Add(bacharbsem);






            _db.SaveChanges();
        }

    }
}