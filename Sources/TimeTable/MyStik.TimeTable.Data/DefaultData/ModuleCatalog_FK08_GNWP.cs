using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogGNWP_GS(ActivityOrganiser fk08, Curriculum GNWP)
        {
            var Analysis = new CurriculumModule()
            {
                ShortName = "Analysis",
                Name = "Analysis",
                ModuleId = "10107",
                Description = "Grundlegende Konzepte, Methoden und numerische Verfahren der eindimensionalen Analyse ",
                ECTS = 5,
                MV = GetHost(fk08, "EIC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Analysis"
                    },

                }

            };

            Analysis.Groups.Add(GetGroup(GNWP, "1"));
            GNWP.Modules.Add(Analysis);
            _db.CurriculumModules.Add(Analysis);





            var LineareAlgebra = new CurriculumModule()
            {
                ShortName = "LineareAlgebra",
                Name = "LineareAlgebra",
                ModuleId = "10207",
                Description = "Die Studierenden beherrschen die wichtigsten Begriffe, Methoden und Resultate der linearen Algebra und sind in der Lage diese auf praktische Beispiele anzuwenden.",
                ECTS = 5,
                MV = GetHost(fk08, "HOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "LineareAlgebra"
                    },

                }

            };

            LineareAlgebra.Groups.Add(GetGroup(GNWP, "1"));
            GNWP.Modules.Add(LineareAlgebra);
            _db.CurriculumModules.Add(LineareAlgebra);




            var Physik = new CurriculumModule()
            {
                ShortName = "Physik",
                Name = "Physik",
                ModuleId = "10308",
                Description = "Zentrale Grundbegriffe und Erhaltungssätze der Physik",
                ECTS = 5,
                MV = GetHost(fk08, "TOR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik"
                    },

                }

            };

            Physik.Groups.Add(GetGroup(GNWP, "1"));
            GNWP.Modules.Add(Physik);
            _db.CurriculumModules.Add(Physik);


            var Softwareentwicklung1 = new CurriculumModule()
            {
                ShortName = "Softwareentwicklung1",
                Name = "Softwareentwicklung1",
                ModuleId = "10407",
                Description = "Grundlegende Konzepte der prozeduralen und objektorientierten Programmierung auf der Basis einer aktuellen, allgemein verfügbaren Programmiersprache",
                ECTS = 8,
                MV = GetHost(fk08, "SCI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Softwareentwicklung1"
                    },

                }

            };

            Softwareentwicklung1.Groups.Add(GetGroup(GNWP, "1"));
            GNWP.Modules.Add(Softwareentwicklung1);
            _db.CurriculumModules.Add(Softwareentwicklung1);



            var Wahlpflicht1 = new CurriculumModule()
            {
                ShortName = "Wahlpflicht1",
                Name = "Wahlpflicht1",
                ModuleId = "10504",
                Description = "Lernziele gemäß der Modulbeschreibungen.",
                ECTS = 2,
                MV = GetHost(fk08, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wahlpflicht1"
                    },

                }

            };

            Wahlpflicht1.Groups.Add(GetGroup(GNWP, "1"));
            GNWP.Modules.Add(Wahlpflicht1);
            _db.CurriculumModules.Add(Wahlpflicht1);





            var EinführungGeotelematik = new CurriculumModule()
            {
                ShortName = "Wahlpflicht1",
                Name = "Wahlpflicht1",
                ModuleId = "10608",
                Description = "Kenntnisse über grundlegende Navigations- und Telematikansätze, die in aktuellen Projekten realisiert sind",
                ECTS = 3,
                MV = GetHost(fk08, "TIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EinführungGeotelematik"
                    },

                }

            };

            EinführungGeotelematik.Groups.Add(GetGroup(GNWP, "1"));
            GNWP.Modules.Add(EinführungGeotelematik);
            _db.CurriculumModules.Add(EinführungGeotelematik);



            var DiskreteMathematik = new CurriculumModule()
            {
                ShortName = "DiskreteMathematik",
                Name = "DiskreteMathematik",
                ModuleId = "20108",
                Description = "Die Studierenden beherrschen die Standardthemen, wichtigste Begriffe und Sätze aus dem Bereich der diskreten Mathematik und sind in der Lage dieses Wissen auf praktische Probleme anzuwenden",
                ECTS = 5,
                MV = GetHost(fk08, "TOR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DiskreteMathematik"
                    },

                }

            };

            DiskreteMathematik.Groups.Add(GetGroup(GNWP, "2"));
            GNWP.Modules.Add(DiskreteMathematik);
            _db.CurriculumModules.Add(DiskreteMathematik);






            var ParameterschätzungGeobezugssysteme = new CurriculumModule()
            {
                ShortName = "ParameterschätzungGeobezugssysteme",
                Name = "ParameterschätzungGeobezugssysteme",
                ModuleId = "20208",
                Description = "Die Studierenden sind in der Lage die Methode der kleinsten Quadrate zur Schätzung von Parametern von funktionalen Modellen und überschüssigen Beobachtungen einzusetzen",
                ECTS = 9,
                MV = GetHost(fk08, "TIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ParameterschätzungGeobezugssysteme"
                    },

                }

            };

            ParameterschätzungGeobezugssysteme.Groups.Add(GetGroup(GNWP, "2"));
            GNWP.Modules.Add(ParameterschätzungGeobezugssysteme);
            _db.CurriculumModules.Add(ParameterschätzungGeobezugssysteme);





            var Softwareentwicklung2 = new CurriculumModule()
            {
                ShortName = "Softwareentwicklung2",
                Name = "Softwareentwicklung2",
                ModuleId = "20307",
                Description = "Die Studierenden sind in der Lage theoretisch erfasste Verfahren, Methoden und Algorithmen in lauffähige und effiziente Software umzusetzen, die Lösungen angemessen zu testen, sowie strukturelle Schwachstellen zu erkennen und zu beseitigen. ",
                ECTS = 8,
                MV = GetHost(fk08, "SCI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Softwareentwicklung2"
                    },

                }

            };

            Softwareentwicklung2.Groups.Add(GetGroup(GNWP, "2"));
            GNWP.Modules.Add(Softwareentwicklung2);
            _db.CurriculumModules.Add(Softwareentwicklung2);



            var Statistik = new CurriculumModule()
            {
                ShortName = "Statistik",
                Name = "Statistik",
                ModuleId = "20408",
                Description = "Die Studierenden erlangen Kenntnis des Begriffes der Zufallsvariable für geodätische Messgrößen",
                ECTS = 3,
                MV = GetHost(fk08, "LOT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Statistik"
                    },

                }

            };

            Statistik.Groups.Add(GetGroup(GNWP, "2"));
            GNWP.Modules.Add(Statistik);
            _db.CurriculumModules.Add(Statistik);




            var Allgemeinwissenschaften = new CurriculumModule()
            {
                ShortName = "Allgemeinwissenschaften",
                Name = "Allgemeinwissenschaften",
                ModuleId = "205013",
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

            Allgemeinwissenschaften.Groups.Add(GetGroup(GNWP, "2"));
            GNWP.Modules.Add(Allgemeinwissenschaften);
            _db.CurriculumModules.Add(Allgemeinwissenschaften);



            var AlgorithmenDatenstrukturen1 = new CurriculumModule()
            {
                ShortName = "AlgorithmenDatenstrukturen1",
                Name = "AlgorithmenDatenstrukturen1",
                ModuleId = "30107",
                Description = "Die Studierenden sind in der Lage die Qualität von Datenstrukturen und Algorithmen einzuschätzen und ihre Implementierung in einem Programm umzusetzen. ",
                ECTS = 5,
                MV = GetHost(fk08, "FIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "AlgorithmenDatenstrukturen1"
                    },

                }

            };

            AlgorithmenDatenstrukturen1.Groups.Add(GetGroup(GNWP, "3"));
            GNWP.Modules.Add(AlgorithmenDatenstrukturen1);
            _db.CurriculumModules.Add(AlgorithmenDatenstrukturen1);






            var Geodatenbanken = new CurriculumModule()
            {
                ShortName = "Geodatenbanken",
                Name = "Geodatenbanken",
                ModuleId = "30208",
                Description = "Die Studierenden sind in der Lage die Prinzipien eines relationalen Datenbanksystems mit raumbezogenen Inhalten zu erkennen und zu verstehen, um damit für konkrete Anwendungen, unter Einsatz von gängigen Datenbankzugriffen, einen Datenbankentwurf zu erstellen. ",
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

            Geodatenbanken.Groups.Add(GetGroup(GNWP, "3"));
            GNWP.Modules.Add(Geodatenbanken);
            _db.CurriculumModules.Add(Geodatenbanken);



            var Geoinformatik = new CurriculumModule()
            {
                ShortName = "Geoinformatik",
                Name = "Geoinformatik",
                ModuleId = "30308",
                Description = "Die Studierenden sind in der Lage IT - Grundlagen, Datenstrukturen und Methoden zum Aufbau von GIS und zur Modellierung von Geoinformation in IT - Systemen einzusetze",
                ECTS = 5,
                MV = GetHost(fk08, "LOT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geoinformatik"
                    },

                }

            };

            Geoinformatik.Groups.Add(GetGroup(GNWP, "3"));
            GNWP.Modules.Add(Geoinformatik);
            _db.CurriculumModules.Add(Geoinformatik);




            var Satellitenpositionierung1 = new CurriculumModule()
            {
                ShortName = "Satellitenpositionierung1",
                Name = "Satellitenpositionierung1",
                ModuleId = "30408",
                Description = "Die Studierenden sind in der Lage Messungen mit Satellitenempfängern zu planen, durchzuführen und auszuwerten und beherrschen die Grundlagen und Fehlerquellen von Satellitenmessverfahren.",
                ECTS = 5,
                MV = GetHost(fk08, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Satellitenpositionierung1"
                    },

                }

            };

            Satellitenpositionierung1.Groups.Add(GetGroup(GNWP, "3"));
            GNWP.Modules.Add(Satellitenpositionierung1);
            _db.CurriculumModules.Add(Satellitenpositionierung1);



            var Navigation = new CurriculumModule()
            {
                ShortName = "Navigation",
                Name = "Navigation",
                ModuleId = "30508",
                Description = "Die Studierenden erlangen Kenntnis über grundlegende Navigationsverfahren, Berechnungsansätze in der Navigation und gängige Navigationssensorik und sind in der Lage die Verwendungsmöglichkeiten und das Genauigkeitspotenzial verschiedener Navigationssensorik zu beurteilen",
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

            Navigation.Groups.Add(GetGroup(GNWP, "3"));
            GNWP.Modules.Add(Navigation);
            _db.CurriculumModules.Add(Navigation);





            var Netzwerke = new CurriculumModule()
            {
                ShortName = "Netzwerke",
                Name = "Netzwerke",
                ModuleId = "30607",
                Description = "Die Studierenden beherrschen die Fachbegriffe und Prinzipien der Netzwerktechnik und Protokolle",
                ECTS = 5,
                MV = GetHost(fk08, "ZUG"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Navigation"
                    },

                }

            };

            Netzwerke.Groups.Add(GetGroup(GNWP, "3"));
            GNWP.Modules.Add(Netzwerke);
            _db.CurriculumModules.Add(Netzwerke);



            var GrundlagenNachrichtenübertragung = new CurriculumModule()
            {
                ShortName = "GrundlagenNachrichtenübertragung",
                Name = "GrundlagenNachrichtenübertragung",
                ModuleId = "40104",
                Description = "Die Studierenden erlangen ein grundlegendes Verständnis nachrichtentechnischer analoger und digitaler Signale und Systeme",
                ECTS = 8,
                MV = GetHost(fk08, "MIC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GrundlagenNachrichtenübertragung"
                    },

                }

            };

            GrundlagenNachrichtenübertragung.Groups.Add(GetGroup(GNWP, "4"));
            GNWP.Modules.Add(GrundlagenNachrichtenübertragung);
            _db.CurriculumModules.Add(GrundlagenNachrichtenübertragung);




            var Geosensorennetzwerke = new CurriculumModule()
            {
                ShortName = "Geosensorennetzwerke",
                Name = "Geosensorennetzwerke",
                ModuleId = "40208",
                Description = "Die Studierenden beherrschen die Fachbegriffe und haben Kenntnis über die Prinzipien von Geosensornetzwerke",
                ECTS = 5,
                MV = GetHost(fk08, "CZA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Geosensorennetzwerke"
                    },

                }

            };

            Geosensorennetzwerke.Groups.Add(GetGroup(GNWP, "4"));
            GNWP.Modules.Add(Geosensorennetzwerke);
            _db.CurriculumModules.Add(Geosensorennetzwerke);


            var Fernerkundung = new CurriculumModule()
            {
                ShortName = "Fernerkundung",
                Name = "Fernerkundung",
                ModuleId = "40308",
                Description = "Die Studierenden beherrschen die physikalischen Grundlagen und Methoden zur Klassifikation von Fernerkundungsdaten. ",
                ECTS = 5,
                MV = GetHost(fk08, "KRZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fernerkundung"
                    },

                }

            };

            Fernerkundung.Groups.Add(GetGroup(GNWP, "4"));
            GNWP.Modules.Add(Fernerkundung);
            _db.CurriculumModules.Add(Fernerkundung);





            var Satellitenpositionierung2 = new CurriculumModule()
            {
                ShortName = "Satellitenpositionierung2",
                Name = "Satellitenpositionierung2",
                ModuleId = "40408",
                Description = "Die Studierenden erlangen Kenntnis über die Referenzsysteme und Anwendungen von globalen Navigationssystemen mit Satelliten(GNSS).",
                ECTS = 5,
                MV = GetHost(fk08, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Satellitenpositionierung2"
                    },

                }

            };

            Satellitenpositionierung2.Groups.Add(GetGroup(GNWP, "4"));
            GNWP.Modules.Add(Satellitenpositionierung2);
            _db.CurriculumModules.Add(Satellitenpositionierung2);





            var VertiefungNavigation = new CurriculumModule()
            {
                ShortName = "VertiefungNavigation",
                Name = "VertiefungNavigation",
                ModuleId = "40508",
                Description = "Die Studierenden beherrschen Auswerteverfahren in der Navigation, in der Inertialnavigation",
                ECTS = 5,
                MV = GetHost(fk08, "TIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "VertiefungNavigation"
                    },

                }

            };

            VertiefungNavigation.Groups.Add(GetGroup(GNWP, "4"));
            GNWP.Modules.Add(VertiefungNavigation);
            _db.CurriculumModules.Add(VertiefungNavigation);





            var Wahlpflichtfach2 = new CurriculumModule()
            {
                ShortName = "Wahlpflichtfach2",
                Name = "Wahlpflichtfach2",
                ModuleId = "40607",
                Description = "Lernziele gemäß der Modulbeschreibungen",
                ECTS = 5,
                MV = GetHost(fk08, "WPM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wahlpflichtfach2"
                    },

                }

            };

            Wahlpflichtfach2.Groups.Add(GetGroup(GNWP, "4"));
            GNWP.Modules.Add(Wahlpflichtfach2);
            _db.CurriculumModules.Add(Wahlpflichtfach2);



            var Kolloquium = new CurriculumModule()
            {
                ShortName = "Kolloquium",
                Name = "Kolloquium",
                ModuleId = "5010478",
                Description = "Die Studierenden sind in der Lage Inhalte selbständig auszuarbeiten Ergebnisse adäquat vor einem Fachpublikum zu präsentieren",
                ECTS = 3,
                MV = GetHost(fk08, "GD"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kolloquium"
                    },

                }

            };

            Kolloquium.Groups.Add(GetGroup(GNWP, "5"));
            GNWP.Modules.Add(Kolloquium);
            _db.CurriculumModules.Add(Kolloquium);



            var ProjektstudiumNavigation = new CurriculumModule()
            {
                ShortName = "ProjektstudiumNavigation",
                Name = "ProjektstudiumNavigation",
                ModuleId = "50208",
                Description = "In wechselnden Themenstellungen werden kleine Projekte aus dem Bereich der mobilen Robotik und Navigation realisiert.",
                ECTS = 5,
                MV = GetHost(fk08, "ABM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ProjektstudiumNavigation"
                    },

                }

            };

            ProjektstudiumNavigation.Groups.Add(GetGroup(GNWP, "5"));
            GNWP.Modules.Add(ProjektstudiumNavigation);
            _db.CurriculumModules.Add(ProjektstudiumNavigation);




            var Praxissemester = new CurriculumModule()
            {
                ShortName = "Praxissemester",
                Name = "Praxissemester",
                ModuleId = "50308",
                Description = "Die Studierenden erlangen Einblick in den Ablauf von Projekten aus den Bereichen Telematik und Navigation",
                ECTS = 22,
                MV = GetHost(fk08, "GD"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Praxissemester"
                    },

                }

            };

            Praxissemester.Groups.Add(GetGroup(GNWP, "5"));
            GNWP.Modules.Add(Praxissemester);
            _db.CurriculumModules.Add(Praxissemester);





            var ComputergraphikBildverarbeitung = new CurriculumModule()
            {
                ShortName = "ComputergraphikBildverarbeitung",
                Name = "ComputergraphikBildverarbeitung",
                ModuleId = "60107",
                Description = "Die Studierenden beherrschen die grundlegenden Abläufe und Algorithmen in der Rendering Pipeline der Computergrafik und in der Verarbeitungskette der Bildverarbeitung",
                ECTS = 5,
                MV = GetHost(fk08, "FIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ComputergraphikBildverarbeitung"
                    },

                }

            };

            ComputergraphikBildverarbeitung.Groups.Add(GetGroup(GNWP, "6"));
            GNWP.Modules.Add(ComputergraphikBildverarbeitung);
            _db.CurriculumModules.Add(ComputergraphikBildverarbeitung);




            var DigitalSignalProcessing = new CurriculumModule()
            {
                ShortName = "DigitalSignalProcessing",
                Name = "DigitalSignalProcessing",
                ModuleId = "60204",
                Description = "Students will master the basic analytical methods of digital signal processing, in particular the analysis and design of discrete - time systems(filters) in the time and frequency domain and the application of the Discrete Fourier Transform(FFT / DFT).",
                ECTS = 5,
                MV = GetHost(fk08, "FIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DigitalSignalProcessing"
                    },

                }

            };

            DigitalSignalProcessing.Groups.Add(GetGroup(GNWP, "6"));
            GNWP.Modules.Add(DigitalSignalProcessing);
            _db.CurriculumModules.Add(DigitalSignalProcessing);




            var Nachrichtensatellitensysteme = new CurriculumModule()
            {
                ShortName = "Nachrichtensatellitensysteme",
                Name = "Nachrichtensatellitensysteme",
                ModuleId = "60304",
                Description = "Die Studierenden erlangen Kenntnis über Aufbau, Funktion und Betrieb von Nachrichtensatelliten.",
                ECTS = 5,
                MV = GetHost(fk08, "STR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Nachrichtensatellitensysteme"
                    },

                }

            };

            Nachrichtensatellitensysteme.Groups.Add(GetGroup(GNWP, "6"));
            GNWP.Modules.Add(Nachrichtensatellitensysteme);
            _db.CurriculumModules.Add(Nachrichtensatellitensysteme);




            var MultisensorNavigation = new CurriculumModule()
            {
                ShortName = "MultisensorNavigation",
                Name = "MultisensorNavigation",
                ModuleId = "60408",
                Description = "Die Studierenden kennen die unterschiedlichen Sensoren zur Multisensor Navigation",
                ECTS = 5,
                MV = GetHost(fk08, "ABM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Nachrichtensatellitensysteme"
                    },

                }

            };

            Nachrichtensatellitensysteme.Groups.Add(GetGroup(GNWP, "6"));
            GNWP.Modules.Add(Nachrichtensatellitensysteme);
            _db.CurriculumModules.Add(Nachrichtensatellitensysteme);


            var MobileAnwendungTelematik = new CurriculumModule()
            {
                ShortName = "MobileAnwendungTelematik",
                Name = "MobileAnwendungTelematik",
                ModuleId = "60507",
                Description = "Die Studierenden verfügen über ein grundlegendes Verständnis der Funktionsweise von Anwendungen auf mobilen Endgeräten. ",
                ECTS = 5,
                MV = GetHost(fk08, "SOC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MobileAnwendungTelematik"
                    },

                }

            };

            MobileAnwendungTelematik.Groups.Add(GetGroup(GNWP, "6"));
            GNWP.Modules.Add(MobileAnwendungTelematik);
            _db.CurriculumModules.Add(MobileAnwendungTelematik);





            var Wahlpflichtmodul1Technik = new CurriculumModule()
            {
                ShortName = "Wahlpflichtmodul1Technik",
                Name = "Wahlpflichtmodul1Technik",
                ModuleId = "6060478",
                Description = "Lernziele gemäß der Modulbeschreibungen. ",
                ECTS = 5,
                MV = GetHost(fk08, "WPM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wahlpflichtmodul1Technik"
                    },

                }

            };

            Wahlpflichtmodul1Technik.Groups.Add(GetGroup(GNWP, "6"));
            GNWP.Modules.Add(Wahlpflichtmodul1Technik);
            _db.CurriculumModules.Add(Wahlpflichtmodul1Technik);



            var LocationBasedServices = new CurriculumModule()
            {
                ShortName = "LocationBasedServices",
                Name = "LocationBasedServices",
                ModuleId = "70108",
                Description = "Die Studierenden erlangen vertiefte Kenntnisse über Datenstrukturen und Algorithmen aus dem Bereich Location Based Services.",
                ECTS = 5,
                MV = GetHost(fk08, "ABM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "LocationBasedServices"
                    },

                }

            };

            LocationBasedServices.Groups.Add(GetGroup(GNWP, "7"));
            GNWP.Modules.Add(LocationBasedServices);
            _db.CurriculumModules.Add(LocationBasedServices);





            var Wahlpflichtmodul2Technik = new CurriculumModule()
            {
                ShortName = "Wahlpflichtmodul2Technik",
                Name = "Wahlpflichtmodul2Technik",
                ModuleId = "7020478",
                Description = "Lernziele gemäß der Modulbeschreibungen.",
                ECTS = 5,
                MV = GetHost(fk08, "WPM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wahlpflichtmodul2Technik"
                    },

                }

            };

            Wahlpflichtmodul2Technik.Groups.Add(GetGroup(GNWP, "7"));
            GNWP.Modules.Add(Wahlpflichtmodul2Technik);
            _db.CurriculumModules.Add(Wahlpflichtmodul2Technik);


            var Wahlpflichtmodul3Technik = new CurriculumModule()
            {
                ShortName = "Wahlpflichtmodul3Technik",
                Name = "Wahlpflichtmodul3Technik",
                ModuleId = "7030478",
                Description = "Lernziele gemäß der Modulbeschreibungen.",
                ECTS = 5,
                MV = GetHost(fk08, "WPM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wahlpflichtmodul3Technik"
                    },

                }

            };

            Wahlpflichtmodul3Technik.Groups.Add(GetGroup(GNWP, "7"));
            GNWP.Modules.Add(Wahlpflichtmodul3Technik);
            _db.CurriculumModules.Add(Wahlpflichtmodul3Technik);



            var Bachelorseminar = new CurriculumModule()
            {
                ShortName = "Bachelorseminar",
                Name = "Bachelorseminar",
                ModuleId = "7040478",
                Description = "Die Studierenden sind in der Lage Inhalt und Ergebnisse der Bachelorarbeit adäquat vor einem Fachpublikum zu präsentieren",
                ECTS = 3,
                MV = GetHost(fk08, "ALL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bachelorseminar"
                    },

                }

            };

            Bachelorseminar.Groups.Add(GetGroup(GNWP, "7"));
            GNWP.Modules.Add(Bachelorseminar);
            _db.CurriculumModules.Add(Bachelorseminar);



            var Bachelorarbeit = new CurriculumModule()
            {
                ShortName = "Bachelorarbeit",
                Name = "Bachelorarbeit",
                ModuleId = "7050478",
                Description = "Die Studierenden sind in der Lage eine praxisbezogene Aufgabenstellung aus dem Gebiet des Studiengangs und seiner Anwendung in benachbarten Disziplinen selbstständig auf wissenschaftlicher Grundlage methodisch zu bearbeiten",
                ECTS = 12,
                MV = GetHost(fk08, "ALL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bachelorarbeit"
                    },

                }

            };

            Bachelorarbeit.Groups.Add(GetGroup(GNWP, "7"));
            GNWP.Modules.Add(Bachelorarbeit);
            _db.CurriculumModules.Add(Bachelorarbeit);

            _db.SaveChanges();
        }

    }
}
