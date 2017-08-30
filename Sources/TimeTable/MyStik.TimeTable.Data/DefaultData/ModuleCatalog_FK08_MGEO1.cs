using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogMGEO_Studiengänge(ActivityOrganiser fk06, Curriculum MGEO)
        {


            var Unternehmensmanagement = new CurriculumModule()
            {
                ShortName = "Unternehmensmanagement",
                Name = "Unternehmensmanagement",
                ModuleId = "6101",
                Description = "Unternehmen zu führen und zu steuern",
                ECTS = 5,
                MV = GetHost(fk06, "ZOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Unternehmensmanagement"
                    }
                }

            };

            Unternehmensmanagement.Groups.Add(GetGroup(MGEO, "1"));
            MGEO.Modules.Add(Unternehmensmanagement);
            _db.CurriculumModules.Add(Unternehmensmanagement);

            var Fernerkundung = new CurriculumModule()
            {
                ShortName = "Fernerkundung",
                Name = "Vertiefung Fernerkundung",
                ModuleId = "6102",
                Description = "Die Studierenden erlangen Kenntnis über neue Entwicklungen im Bereich der Fernerkundung",
                ECTS = 5,
                MV = GetHost(fk06, "KAM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Fernerkundung"
                    }
                }

            };

            Fernerkundung.Groups.Add(GetGroup(MGEO, "1"));
            MGEO.Modules.Add(Fernerkundung);
            _db.CurriculumModules.Add(Fernerkundung);

            var Visualisierung = new CurriculumModule()
            {
                ShortName = "3DVisualisierung",
                Name = "Interaktive 3D-Visualisierung",
                ModuleId = "6111",
                Description = "hochdetaillierte Gelände- und 3D-Gebäudemodelle für einen Landschaftsausschnitt zu modellieren",
                ECTS = 5,
                MV = GetHost(fk06, "OST"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Visualisierung"
                    }
                }

            };

            Visualisierung.Groups.Add(GetGroup(MGEO, "1"));
            MGEO.Modules.Add(Visualisierung);
            _db.CurriculumModules.Add(Visualisierung);

            var GISProgrammierung = new CurriculumModule()
            {
                ShortName = "GISProgrammierung",
                Name = "GIS-Programmierung",
                ModuleId = "6121",
                Description = "hochdetaillierte Gelände- und 3D-Gebäudemodelle für einen Landschaftsausschnitt zu modellieren",
                ECTS = 5,
                MV = GetHost(fk06, "WIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "GISProgrammierung"
                    }
                }

            };

            GISProgrammierung.Groups.Add(GetGroup(MGEO, "1"));
            MGEO.Modules.Add(GISProgrammierung);
            _db.CurriculumModules.Add(GISProgrammierung);

            var Messtechnik = new CurriculumModule()
            {
                ShortName = "3D-Messtechnik",
                Name = "3D-Messtechnik",
                ModuleId = "6122",
                Description = "Die Studierenden kennen die Instrumente und Methoden der 3D-Messtechnik. Sie sind in der Lage Messkonzepte zu erstellen und Projekte der 3D - Messtechnik in Teamarbeit durchzuführen",
                ECTS = 5,
                MV = GetHost(fk06, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "3D-Messtechnik"
                    }
                }

            };

            Messtechnik.Groups.Add(GetGroup(MGEO, "1"));
            MGEO.Modules.Add(Messtechnik);
            _db.CurriculumModules.Add(Messtechnik);

            var InNavigation = new CurriculumModule()
            {
                ShortName = "Indoor Navigation",
                Name = "Indoor Navigation",
                ModuleId = "6131",
                Description = "Die Studierenden erlangen Kenntnisse im Umgang mit ausgewählter indoor-fähiger Navigationssensorik.",
                ECTS = 5,
                MV = GetHost(fk06, "TIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "InNavigation"
                    }
                }

            };

            InNavigation.Groups.Add(GetGroup(MGEO, "1"));
            MGEO.Modules.Add(InNavigation);
            _db.CurriculumModules.Add(InNavigation);

            var MobileMapping = new CurriculumModule()
            {
                ShortName = "Mobile Mapping",
                Name = "Mobile Mapping",
                ModuleId = "6132",
                Description = "Die Studierenden kennen Methoden und Algorithmen zum Lokalisieren und Navigieren eines mobilen Systems und sind in der Lage ein solches System zu modellieren.",
                ECTS = 5,
                MV = GetHost(fk06, "ABM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "MobileMapping"
                    }
                }

            };

            MobileMapping.Groups.Add(GetGroup(MGEO, "1"));
            MGEO.Modules.Add(MobileMapping);
            _db.CurriculumModules.Add(MobileMapping);

            var KartographInformationsvisualisierung = new CurriculumModule()
            {
                ShortName = "Mobile KartographInformationsvisualisierung",
                Name = "Kartographische Informationsvisualisierung",
                ModuleId = "6221",
                Description = "Die Studierenden erlangen Kenntnisse über die modernen visuellen und technischen Methoden der Geovisualisierung.",
                ECTS = 5,
                MV = GetHost(fk06, "KIR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "KartographInformationsvisualisierung"
                    }
                }

            };

            KartographInformationsvisualisierung.Groups.Add(GetGroup(MGEO, "1"));
            MGEO.Modules.Add(KartographInformationsvisualisierung);
            _db.CurriculumModules.Add(KartographInformationsvisualisierung);

            var MobileNetze = new CurriculumModule()
            {
                ShortName = "MobileNetze",
                Name = "Mobile Netze",
                ModuleId = "6221",
                Description = "die Anwendung von Sensoren und Messmethoden der geodätischen Objektüberwachung",
                ECTS = 5,
                MV = GetHost(fk06, "CZA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "MobileNetze"
                    }
                }

            };

            MobileNetze.Groups.Add(GetGroup(MGEO, "1"));
            MGEO.Modules.Add(MobileNetze);
            _db.CurriculumModules.Add(MobileNetze);

            //2.SEMESTER

            var KatUmweltmanagement = new CurriculumModule()
            {
                ShortName = "3DVisualisierung",
                Name = "Katastrophen- und Umweltmanagement",
                ModuleId = "6112",
                Description = "Die Studierenden kennen die Diversität und Definitionsmerkmale von Natur- und Umweltkatastrophen",
                ECTS = 5,
                MV = GetHost(fk06, "HAN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "KatUmweltmanagement"
                    }
                }

            };

            KatUmweltmanagement.Groups.Add(GetGroup(MGEO, "2"));
            MGEO.Modules.Add(KatUmweltmanagement);
            _db.CurriculumModules.Add(KatUmweltmanagement);

            var ProjektInformationsmanagement = new CurriculumModule()
            {
                ShortName = "ProjektInformationsmanagement",
                Name = "Projekt- und Informationsmanagement",
                ModuleId = "6201",
                Description = "adäquate Methoden und Technologien zur Lösung von Aufgaben anzuwenden und Problemstellungen und Fragen aus dem Informations - und Projektmanagement zu bearbeiten",
                ECTS = 5,
                MV = GetHost(fk06, "ZOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "ProjektInformationsmanagement"
                    }
                }

            };

            ProjektInformationsmanagement.Groups.Add(GetGroup(MGEO, "2"));
            MGEO.Modules.Add(ProjektInformationsmanagement);
            _db.CurriculumModules.Add(ProjektInformationsmanagement);

            var Geodateninfrastruktur = new CurriculumModule()
            {
                ShortName = "Geodateninfrastruktur",
                Name = "Geodateninfrastruktur",
                ModuleId = "6202",
                Description = "wichtigste Standards und Normen in der Geoinformatik",
                ECTS = 5,
                MV = GetHost(fk06, "KAM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Geodateninfrastruktur"
                    }
                }

            };

            Geodateninfrastruktur.Groups.Add(GetGroup(MGEO, "2"));
            MGEO.Modules.Add(Geodateninfrastruktur);
            _db.CurriculumModules.Add(Geodateninfrastruktur);

            var MobileKartographie = new CurriculumModule()
            {
                ShortName = "MobileKartographie",
                Name = "Mobile Kartographie",
                ModuleId = "6212",
                Description = "Die Studierenden erlangen Kenntnisse über kartographische Konzepte und Visualisierungstechniken für mobile Applikationen.",
                ECTS = 5,
                MV = GetHost(fk06, "KIR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "MobileKartographie"
                    }
                }

            };

            MobileKartographie.Groups.Add(GetGroup(MGEO, "2"));
            MGEO.Modules.Add(MobileKartographie);
            _db.CurriculumModules.Add(MobileKartographie);

            var GeoMonitoring = new CurriculumModule()
            {
                ShortName = "GeoMonitoring",
                Name = "Geo-Monitoring",
                ModuleId = "6221",
                Description = "die Anwendung von Sensoren und Messmethoden der geodätischen Objektüberwachung",
                ECTS = 5,
                MV = GetHost(fk06, "CZA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "GeoMonitoring"
                    }
                }

            };

            GeoMonitoring.Groups.Add(GetGroup(MGEO, "2"));
            MGEO.Modules.Add(GeoMonitoring);
            _db.CurriculumModules.Add(GeoMonitoring);

            var AdvancedRemoteSensingMethods = new CurriculumModule()
            {
                ShortName = "GeoMonitoring",
                Name = "Advanced Remote Sensing Methods",
                ModuleId = "6231",
                Description = "Die Studierenden erlangen Kenntnisse über fortgeschrittene Methoden und Algorithmen der Mustererkennung und Fernerkundung.Sie sind in der Lage, die Methoden und Algorithmen in einer Programmiersprache umzusetzen.",
                ECTS = 5,
                MV = GetHost(fk06, "KRZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Advanced Remote Sensing Methods"
                    }
                }

            };

            AdvancedRemoteSensingMethods.Groups.Add(GetGroup(MGEO, "2"));
            MGEO.Modules.Add(AdvancedRemoteSensingMethods);
            _db.CurriculumModules.Add(AdvancedRemoteSensingMethods);

            var RaumanalysPlanungsprozesse = new CurriculumModule()
            {
                ShortName = "RaumanalysPlanungsprozesse",
                Name = "Raumanalysen und regionale Planungsprozesse",
                ModuleId = "6222",
                Description = "Operationalisierung von Planungsentwicklungen für die Datenauswahl und Implementierung in ein GIS-System",
                ECTS = 5,
                MV = GetHost(fk06, "CZA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "RaumanalysPlanungsprozesse"
                    }
                }

            };

            RaumanalysPlanungsprozesse.Groups.Add(GetGroup(MGEO, "2"));
            MGEO.Modules.Add(RaumanalysPlanungsprozesse);
            _db.CurriculumModules.Add(RaumanalysPlanungsprozesse);


            //3.SEMESTER


            var Masterkolloquium = new CurriculumModule()
            {
                ShortName = "3DVisualisierung",
                Name = "Interaktive 3D-Visualisierung",
                ModuleId = "6301",
                Description = "Die Studierenden sind in der Lage: Inhalte selbständig auszuarbeiten Ergebnisse adäquat vor einem Fachpublikum zu präsentieren",
                ECTS = 3,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Masterkolloquium"
                    }
                }

            };

            Masterkolloquium.Groups.Add(GetGroup(MGEO, "3"));
            MGEO.Modules.Add(Masterkolloquium);
            _db.CurriculumModules.Add(Masterkolloquium);


            var Masterarbeit = new CurriculumModule()
            {
                ShortName = "Masterarbeit",
                Name = "Masterarbeit",
                ModuleId = "6302",
                Description = "Masterarbeit",
                ECTS = 27,
                MV = GetHost(fk06, "OST"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Visualisierung"
                    }
                }

            };
            Masterkolloquium.Groups.Add(GetGroup(MGEO, "3"));
            MGEO.Modules.Add(Masterkolloquium);
            _db.CurriculumModules.Add(Masterkolloquium);

        }

    }
}

