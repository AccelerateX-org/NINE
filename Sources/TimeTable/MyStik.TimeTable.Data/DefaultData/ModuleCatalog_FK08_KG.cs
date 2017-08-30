using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogKG_GS(ActivityOrganiser fk08, Curriculum KG)
        {
            var GrundlagenMathematik1 = new CurriculumModule()
            {
                ShortName = "GrundlagenMathematik1",
                Name = "GrundlagenMathematik1",
                ModuleId = "1108",
                Description = "Grundlage für alle mathematisch orientierten Module ",
                ECTS = 5,
                MV = GetHost(fk08, "ZOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GrundlagenMathematik1"
                    },

                }

            };

            GrundlagenMathematik1.Groups.Add(GetGroup(KG, "1"));
            KG.Modules.Add(GrundlagenMathematik1);
            _db.CurriculumModules.Add(GrundlagenMathematik1);


            var Geomedientechnik1 = new CurriculumModule()
            {
                ShortName = "Geomedientechnik1",
                Name = "Geomedientechnik1",
                ModuleId = "1208",
                Description = "Die Studierenden sind in der Lage, die grundlegenden Workflows der digitalen Fotografie von der Aufnahme über die Bildbearbeitung bis zum digitalen Endprodukt qualitätsorientiert anzuwenden",
                ECTS = 5,
                MV = GetHost(fk08, "OST"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geomedientechnik1"
                    },

                }

            };

            Geomedientechnik1.Groups.Add(GetGroup(KG, "1"));
            KG.Modules.Add(Geomedientechnik1);
            _db.CurriculumModules.Add(Geomedientechnik1);



            var Kartiographie1 = new CurriculumModule()
            {
                ShortName = "Kartiographie1",
                Name = "Kartiographie1",
                ModuleId = "1308",
                Description = "Umfassende Kenntnisse der topographischen Karten verschiedener Maßstäbe, der Darstellungsmethoden und der verwendeten Koordinatensysteme",
                ECTS = 5,
                MV = GetHost(fk08, "FOR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kartiographie1"
                    },

                }

            };

            Kartiographie1.Groups.Add(GetGroup(KG, "1"));
            KG.Modules.Add(Kartiographie1);
            _db.CurriculumModules.Add(Kartiographie1);


            var Kartendesign1 = new CurriculumModule()
            {
                ShortName = "Kartendesign1",
                Name = "Kartendesign1",
                ModuleId = "1408",
                Description = " Vermittlung von Kenntnisse der wichtigsten Theorien, Prinzipien und Methoden der kartografische Gestaltung.",
                ECTS = 5,
                MV = GetHost(fk08, "BUZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kartendesign1"
                    },

                }

            };

            Kartendesign1.Groups.Add(GetGroup(KG, "1"));
            KG.Modules.Add(Kartendesign1);
            _db.CurriculumModules.Add(Kartendesign1);




            var Geowissenschaften1 = new CurriculumModule()
            {
                ShortName = "Geowissenschaften1",
                Name = "Geowissenschaften1",
                ModuleId = "1508",
                Description = " Kenntnis der grundlegenden Typen, Formen und Prozesse der Geologie und Geomorphologie.",
                ECTS = 5,
                MV = GetHost(fk08, "HAN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geowissenschaften1"
                    },

                }

            };

            Geowissenschaften1.Groups.Add(GetGroup(KG, "1"));
            KG.Modules.Add(Geowissenschaften1);
            _db.CurriculumModules.Add(Geowissenschaften1);


            var Informatik1 = new CurriculumModule()
            {
                ShortName = "Informatik1",
                Name = "Informatik1",
                ModuleId = "1608",
                Description = " TeilnehmerInnen verstehen folgende wesentlichen Konzepte der Informatik auf einem grundlegenden, praxisorientierten, aber wissenschaftlichen Niveau.",
                ECTS = 5,
                MV = GetHost(fk08, "KLU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Informatik1"
                    },

                }

            };

            Informatik1.Groups.Add(GetGroup(KG, "1"));
            KG.Modules.Add(Informatik1);
            _db.CurriculumModules.Add(Informatik1);


            var GrundlagenMathematik2 = new CurriculumModule()
            {
                ShortName = "GrundlagenMathematik2",
                Name = "GrundlagenMathematik2",
                ModuleId = "2108",
                Description = " Verstehen des Nutzens mathematischer Methoden anhand konkreter praxisorientierter Anwendungsfälle",
                ECTS = 6,
                MV = GetHost(fk08, "KLU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GrundlagenMathematik2"
                    },

                }

            };

            GrundlagenMathematik2.Groups.Add(GetGroup(KG, "2"));
            KG.Modules.Add(GrundlagenMathematik2);
            _db.CurriculumModules.Add(GrundlagenMathematik2);






            var Geomedientechnik2 = new CurriculumModule()
            {
                ShortName = "Geomedientechnik2",
                Name = "Geomedientechnik2",
                ModuleId = "2208",
                Description = "Die Studierenden sind in der Lage, die wichtigsten Theorien, Prinzipien und Methoden der digitalen Vorstufe für Printmedien sowie der grundlegenden Druckverfahren zu verstehen und anzuwenden",
                ECTS = 5,
                MV = GetHost(fk08, "OST"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geomedientechnik2"
                    },

                }

            };

            Geomedientechnik2.Groups.Add(GetGroup(KG, "2"));
            KG.Modules.Add(Geomedientechnik2);
            _db.CurriculumModules.Add(Geomedientechnik2);




            var Kartographie2 = new CurriculumModule()
            {
                ShortName = "Kartographie2",
                Name = "Kartographie2",
                ModuleId = "2308",
                Description = "Die Studentinnen und Studenten sind nach der Vorlesung in der Lage theoretische Modelle der kartographischen Kommunikation zu verstehen und anzuwenden",
                ECTS = 4,
                MV = GetHost(fk08, "BUZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kartographie2"
                    },

                }

            };

            Kartographie2.Groups.Add(GetGroup(KG, "2"));
            KG.Modules.Add(Kartographie2);
            _db.CurriculumModules.Add(Kartographie2);







            var Kartendesign2 = new CurriculumModule()
            {
                ShortName = "Kartendesign2",
                Name = "Kartendesign2",
                ModuleId = "2408",
                Description = "Die Studentinnen und Studenten sind nach der Vorlesung in der Lage, Vermittlung von Kenntnissen der wichtigsten Theorien, Prinzipien und Methoden der kartografischen Generalisierung",
                ECTS = 5,
                MV = GetHost(fk08, "BUZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kartendesign2"
                    },

                }

            };

            Kartendesign2.Groups.Add(GetGroup(KG, "2"));
            KG.Modules.Add(Kartendesign2);
            _db.CurriculumModules.Add(Kartendesign2);


            var Informatik2 = new CurriculumModule()
            {
                ShortName = "Informatik2",
                Name = "Informatik2",
                ModuleId = "2608",
                Description = "Grundlegende Konzepte der prozeduralen und objektorientierten Programmierung auf der Basis einer aktuellen, allgemein verfügbaren Programmiersprache",
                ECTS = 5,
                MV = GetHost(fk08, "ABM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Informatik2"
                    },

                }

            };

            Informatik2.Groups.Add(GetGroup(KG, "2"));
            KG.Modules.Add(Informatik2);
            _db.CurriculumModules.Add(Informatik2);





            var Allgemeinwissenschaften = new CurriculumModule()
            {
                ShortName = "Allgemeinwissenschaften",
                Name = "Allgemeinwissenschaften",
                ModuleId = "3108",
                Description = "Lernziele gemäß Modulkatalog der Fakultät für Studium Generale und Interdisziplinäre Studien",
                ECTS = 4,
                MV = GetHost(fk08, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Allgemeinwissenschaften"
                    },

                }

            };

            Allgemeinwissenschaften.Groups.Add(GetGroup(KG, "3"));
            KG.Modules.Add(Allgemeinwissenschaften);
            _db.CurriculumModules.Add(Allgemeinwissenschaften);





            var Geomedientechnik3 = new CurriculumModule()
            {
                ShortName = "Geomedientechnik3",
                Name = "Geomedientechnik3",
                ModuleId = "3208",
                Description = "Die Studierenden sind in der Lage, WebMapping und E-Publishing Anwendungen auf Grundlage der Prinzipien des Cross-Media-Publishings zu konzipieren und umzusetzen",
                ECTS = 5,
                MV = GetHost(fk08, "OST"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geomedientechnik3"
                    },

                }

            };

            Geomedientechnik3.Groups.Add(GetGroup(KG, "3"));
            KG.Modules.Add(Geomedientechnik3);
            _db.CurriculumModules.Add(Geomedientechnik3);




            var Geomedientechnik4 = new CurriculumModule()
            {
                ShortName = "Geomedientechnik4",
                Name = "Geomedientechnik4",
                ModuleId = "3308",
                Description = "Die Studierenden sind in der Lage, mittels fotografischer 3D-Scantechniken 3D-Geodaten zu produzieren und diese zu verarbeiten.",
                ECTS = 5,
                MV = GetHost(fk08, "OST"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geomedientechnik4"
                    },

                }

            };

            Geomedientechnik4.Groups.Add(GetGroup(KG, "3"));
            KG.Modules.Add(Geomedientechnik4);
            _db.CurriculumModules.Add(Geomedientechnik4);







            var Kartendesign3 = new CurriculumModule()
            {
                ShortName = "Kartendesign3",
                Name = "Kartendesign3",
                ModuleId = "3408",
                Description = "Die Studentinnen und Studenten sind in der Lage, ein komplexes kartographisches Produkt zu konzipieren, redaktionell zu bearbeiten, zu gestalten und schließlich zu erstellen",
                ECTS = 5,
                MV = GetHost(fk08, "KIR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kartendesign3"
                    },

                }

            };

            Kartendesign3.Groups.Add(GetGroup(KG, "3"));
            KG.Modules.Add(Kartendesign3);
            _db.CurriculumModules.Add(Kartendesign3);





            var Fernerkundung1 = new CurriculumModule()
            {
                ShortName = "Fernerkundung1",
                Name = "Fernerkundung1",
                ModuleId = "3508",
                Description = "Nach der Teilnahme sind die Studierenden in der Lage, die grundlegenden Methoden der Optimierung und Klassifikation von Fernerkundungsdaten zu verstehen und anzuwende",
                ECTS = 6,
                MV = GetHost(fk08, "KAM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fernerkundung1"
                    },

                }

            };

            Fernerkundung1.Groups.Add(GetGroup(KG, "3"));
            KG.Modules.Add(Fernerkundung1);
            _db.CurriculumModules.Add(Fernerkundung1);







            var Geoinformatik1 = new CurriculumModule()
            {
                ShortName = "Geoinformatik1",
                Name = "Geoinformatik1",
                ModuleId = "3608",
                Description = "Die Studierenden erlangen ein Grundverständnis für Aufgaben und Rollen von(Geo-)Datenbanksystemen in komplexen Informationssystemen",
                ECTS = 5,
                MV = GetHost(fk08, "KLU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geoinformatik1"
                    },

                }

            };

            Geoinformatik1.Groups.Add(GetGroup(KG, "3"));
            KG.Modules.Add(Geoinformatik1);
            _db.CurriculumModules.Add(Geoinformatik1);




             var Geoinformatik2 = new CurriculumModule()
                            {
                                ShortName = "Geoinformatik2",
                                Name = "Geoinformatik2",
                                ModuleId = "4108",
                                Description = "Die Studierenden sind in der Lage IT-Grundlagen, Datenstrukturen und Methoden zum Aufbau von GIS und zur Modellierung von Geoinformation in IT-Systemen einzusetzen",
                                ECTS = 5,
                                MV = GetHost(fk08, "KLU"),
                                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geoinformatik2"
                    },

                }

                            };

            Geoinformatik2.Groups.Add(GetGroup(KG, "4"));
            KG.Modules.Add(Geoinformatik2);
            _db.CurriculumModules.Add(Geoinformatik2);





            var GrundlagenGeodätischeObjekterfassung3 = new CurriculumModule()
            {
                ShortName = "GrundlagenGeodätischeObjekterfassung3",
                Name = "GrundlagenGeodätischeObjekterfassung3",
                ModuleId = "4208",
                Description = "Nach der Teilnahme an den Modulveranstaltungen sind die Studierenden in der Lage die Grundlagen von  geodätischen Messverfahren zur 3D-Punktbestimmung wie Tachymetrie, GNSS und terrestrisches Laserscanning(TLS)  und die dazugehörigen Berechnungsmethoden und Problemstellungen zu kennen",
                ECTS = 6,
                MV = GetHost(fk08, "CZA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GrundlagenGeodätischeObjekterfassung3"
                    },

                }

            };

            GrundlagenGeodätischeObjekterfassung3.Groups.Add(GetGroup(KG, "4"));
            KG.Modules.Add(GrundlagenGeodätischeObjekterfassung3);
            _db.CurriculumModules.Add(GrundlagenGeodätischeObjekterfassung3);







            var Geomedientechnik5 = new CurriculumModule()
            {
                ShortName = "Geomedientechnik5",
                Name = "Geomedientechnik5",
                ModuleId = "4308",
                Description = "Die Studierenden sind in der Lage, 3D-Modelle mit realistischen Materialien auszustatten und beherrschen die hierzu notwendigen Shading- und Texturierungstechniken.",
                ECTS = 5,
                MV = GetHost(fk08, "OST"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geomedientechnik5"
                    },

                }

            };

            Geomedientechnik5.Groups.Add(GetGroup(KG, "4"));
            KG.Modules.Add(Geomedientechnik5);
            _db.CurriculumModules.Add(Geomedientechnik5);



             var ThematischeKartographie1 = new CurriculumModule()
             {
                 ShortName = "ThematischeKartographie1",
                 Name = "ThematischeKartographie1",
                 ModuleId = "4408",
                 Description = "Die Studentinnen und Studenten sind nach erfolgreichem Abschluss des Moduls in der Lage, analytische und komplexe thematische Karten auf Basis statistischer Daten zu konzipieren",
                 ECTS = 5,
                 MV = GetHost(fk08, "KIR"),
                 ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ThematischeKartographie1"
                    },

                }

             };

            ThematischeKartographie1.Groups.Add(GetGroup(KG, "4"));
            KG.Modules.Add(ThematischeKartographie1);
            _db.CurriculumModules.Add(ThematischeKartographie1);



             var Geowissenschaften3 = new CurriculumModule()
             {
                 ShortName = "Geowissenschaften3",
                 Name = "Geowissenschaften3",
                 ModuleId = "4508",
                 Description = "Kenntnis über die Teildisziplinen, inhaltliche Grundlagen und Theorien der Anthropogeographie",
                 ECTS = 4,
                 MV = GetHost(fk08, "HAN"),
                 ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geowissenschaften3"
                    },

                }

             };

            Geowissenschaften3.Groups.Add(GetGroup(KG, "4"));
            KG.Modules.Add(Geowissenschaften3);
            _db.CurriculumModules.Add(Geowissenschaften3);



             var Kartographie3 = new CurriculumModule()
             {
                 ShortName = "Kartographie3",
                 Name = "Kartographie3",
                 ModuleId = "4608",
                 Description = "Einsicht in die Methoden zur Beschreibung des Raumbezugs von Geodaten.",
                 ECTS = 5,
                 MV = GetHost(fk08, "KLU"),
                 ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kartographie3"
                    },

                }

             };

            Kartographie3.Groups.Add(GetGroup(KG, "4"));
            KG.Modules.Add(Kartographie3);
            _db.CurriculumModules.Add(Kartographie3);


             var Praktikum = new CurriculumModule()
             {
                 ShortName = "Praktikum",
                 Name = "Praktikum",
                 ModuleId = "5108",
                 Description = "Fähigkeit zu selbständiger Tätigkeit durch Mitwirken bei Produktion/Projekten; Bereitschaft und Fähigkeit zur Teamarbeit.",
                 ECTS = 25,
                 MV = GetHost(fk08, "N.N."),
                 ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Praktikum"
                    },

                }

             };

            Praktikum.Groups.Add(GetGroup(KG, "5"));
            KG.Modules.Add(Praktikum);
            _db.CurriculumModules.Add(Praktikum);


             var GeländepraktikumExkursion = new CurriculumModule()
             {
                 ShortName = "GeländepraktikumExkursion",
                 Name = "GeländepraktikumExkursion",
                 ModuleId = "5208",
                 Description = "Kenntnisse der Konzeption und Planung von kartographischen Messkampagnen im alpinen Gebirge",
                 ECTS = 5,
                 MV = GetHost(fk08, "KIR"),
                 ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GeländepraktikumExkursion"
                    },

                }

             };

            GeländepraktikumExkursion.Groups.Add(GetGroup(KG, "5"));
            KG.Modules.Add(GeländepraktikumExkursion);
            _db.CurriculumModules.Add(GeländepraktikumExkursion);


             var Geomedientechnik6 = new CurriculumModule()
             {
                 ShortName = "Geomedientechnik6",
                 Name = "Geomedientechnik6",
                 ModuleId = "6108",
                 Description = "Die Studierenden sind vertraut mit Prinzipien und Methoden multimedialer Geo-Applikationen unter Berücksichtigung aller notwendigen technischen Spezifikationen für die jeweiligen Medien",
                 ECTS = 5,
                 MV = GetHost(fk08, "OST"),
                 ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geomedientechnik6"
                    },

                }

             };

            Geomedientechnik6.Groups.Add(GetGroup(KG, "6"));
            KG.Modules.Add(Geomedientechnik6);
            _db.CurriculumModules.Add(Geomedientechnik6);




             var GrundlagenBetriebswirtschaft5 = new CurriculumModule()
             {
                 ShortName = "GrundlagenBetriebswirtschaft5",
                 Name = "GrundlagenBetriebswirtschaft5",
                 ModuleId = "6208",
                 Description = "Nach der Teilnahme an der Modulveranstaltung sind die Studierenden in der Lage, betriebswirtschaftliche Vorgänge in Betrieben mit den Schwerpunkten Unternehmensorganisation, Bilanzierung und Kosten-/ Leistungsrechnung zu verstehen und anzuwenden.",
                 ECTS = 5,
                 MV = GetHost(fk08, "LEI"),
                 ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GrundlagenBetriebswirtschaft5"
                    },

                }

             };

            GrundlagenBetriebswirtschaft5.Groups.Add(GetGroup(KG, "6"));
            KG.Modules.Add(GrundlagenBetriebswirtschaft5);
            _db.CurriculumModules.Add(GrundlagenBetriebswirtschaft5);


             var ThematischeKartographie2 = new CurriculumModule()
             {
                 ShortName = "ThematischeKartographie2",
                 Name = "ThematischeKartographie2",
                 ModuleId = "6308",
                 Description = "Die Studentinnen und Studenten sind nach der Veranstaltung in der Lage, den gesamten Redaktionsprozess thematischer Karten nachzuvollziehen und selbstständig zu bewältigen",
                 ECTS = 5,
                 MV = GetHost(fk08, "KIR"),
                 ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ThematischeKartographie2"
                    },

                }

             };

            ThematischeKartographie2.Groups.Add(GetGroup(KG, "6"));
            KG.Modules.Add(ThematischeKartographie2);
            _db.CurriculumModules.Add(ThematischeKartographie2);



             var Geoinformatik3 = new CurriculumModule()
             {
                 ShortName = "Geoinformatik3",
                 Name = "Geoinformatik3",
                 ModuleId = "6408",
                 Description = "Die Studierenden sind in der Lage eine mobile, dynamische und web-basierte GeoApp unter Beachtung von Methoden der thematischen Kartographie und nach Kriterien der professionellen Softwareentwicklung zu entwerfen und zu implementieren.",
                 ECTS = 10,
                 MV = GetHost(fk08, "SUE"),
                 ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geoinformatik3"
                    },

                }

             };

            Geoinformatik3.Groups.Add(GetGroup(KG, "6"));
            KG.Modules.Add(Geoinformatik3);
            _db.CurriculumModules.Add(Geoinformatik3);




             var Fernerkundung2 = new CurriculumModule()
             {
                 ShortName = "Fernerkundung2",
                 Name = "Fernerkundung2",
                 ModuleId = "6508",
                 Description = "Nach der erfolgreichen Teilnahme sind die Studierenden in der Lage, die Methoden der Bildverarbeitung anzuwenden, um die Qualität von Fernerkundungsdaten zu optimieren",
                 ECTS = 5,
                 MV = GetHost(fk08, "KAM"),
                 ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fernerkundung2"
                    },

                }

             };

            Fernerkundung2.Groups.Add(GetGroup(KG, "6"));
            KG.Modules.Add(Fernerkundung2);
            _db.CurriculumModules.Add(Fernerkundung2);


             var WahlKompetenzfeld1 = new CurriculumModule()
             {
                 ShortName = "WahlKompetenzfeld1",
                 Name = "WahlKompetenzfeld1",
                 ModuleId = "7108",
                 Description = "Lernziele gemäß der Modulbeschreibungen.",
                 ECTS = 5,
                 MV = GetHost(fk08, "N.N."),
                 ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WahlKompetenzfeld1"
                    },

                }

             };

            WahlKompetenzfeld1.Groups.Add(GetGroup(KG, "7"));
            KG.Modules.Add(WahlKompetenzfeld1);
            _db.CurriculumModules.Add(WahlKompetenzfeld1);





            var WahlKompetenzfeld2 = new CurriculumModule()
            {
                ShortName = "WahlKompetenzfeld2",
                Name = "WahlKompetenzfeld2",
                ModuleId = "7208",
                Description = "Lernziele gemäß der Modulbeschreibungen.",
                ECTS = 5,
                MV = GetHost(fk08, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WahlKompetenzfeld2"
                    },

                }

            };

            WahlKompetenzfeld2.Groups.Add(GetGroup(KG, "7"));
            KG.Modules.Add(WahlKompetenzfeld2);
            _db.CurriculumModules.Add(WahlKompetenzfeld2);



            var WahlKompetenzfeld3 = new CurriculumModule()
            {
                ShortName = "WahlKompetenzfeld3",
                Name = "WahlKompetenzfeld3",
                ModuleId = "7308",
                Description = "Lernziele gemäß der Modulbeschreibungen.",
                ECTS = 5,
                MV = GetHost(fk08, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WahlKompetenzfeld3"
                    },

                }

            };

            WahlKompetenzfeld3.Groups.Add(GetGroup(KG, "7"));
            KG.Modules.Add(WahlKompetenzfeld3);
            _db.CurriculumModules.Add(WahlKompetenzfeld3);


            var Bachelorarbeit = new CurriculumModule()
            {
                ShortName = "Bachelorarbeit",
                Name = "Bachelorarbeit",
                ModuleId = "7408",
                Description = "Fähigkeit, eine praxisbezogene Aufgabenstellung aus dem Gebiet des Studiengangs und seiner Anwendung in benachbarten Disziplinen selbstständig auf wissenschaftlicher Grundlage methodisch zu bearbeite",
                ECTS = 15,
                MV = GetHost(fk08, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bachelorarbeit"
                    },

                }

            };

            Bachelorarbeit.Groups.Add(GetGroup(KG, "7"));
            KG.Modules.Add(Bachelorarbeit);
            _db.CurriculumModules.Add(Bachelorarbeit);


            _db.SaveChanges();
        }

    }
}
