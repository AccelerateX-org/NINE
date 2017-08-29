using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogFAB_GS(ActivityOrganiser fk03, Curriculum FAB)
        {
            var Ingenieurmathematik1 = new CurriculumModule()
            {
                ShortName = "Ingenieurmathematik1",
                Name = "Ingenieurmathematik1",
                ModuleId = "F1010",
                Description = "In der Modulgruppe werden gründliche Kenntnisse und vertieftes Verständnis für mathematische Begriffe und Methoden sowie analytische Denkweisen vermittelt",
                ECTS = 6,
                MV = GetHost(fk03, "SCL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ingenieurmathematik1"
                    },

                }

            };

             Ingenieurmathematik1.Groups.Add(GetGroup(FAB, "1"));
             FAB.Modules.Add(Ingenieurmathematik1);
            _db.CurriculumModules.Add(Ingenieurmathematik1);


            var TechnischeMechanik1 = new CurriculumModule()
            {
                ShortName = "TechnischeMechanik1",
                Name = "TechnischeMechanik1",
                ModuleId = "F1020",
                Description = "Die Studierenden sollen in der Lage sein, statische Probleme an Systemen starrer Körper selbständig zu lösen.",
                ECTS = 5,
                MV = GetHost(fk03, "MID"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeMechanik1"
                    },

                }

            };

            TechnischeMechanik1.Groups.Add(GetGroup(FAB, "1"));
            FAB.Modules.Add(TechnischeMechanik1);
            _db.CurriculumModules.Add(TechnischeMechanik1);



            var Produktentwicklung1 = new CurriculumModule()
            {
                ShortName = "Produktentwicklung1",
                Name = "Produktentwicklung1",
                ModuleId = "F1030",
                Description = "Die Lehrveranstaltung dient dem Erlernen der Grundlagen der Konstruktion, der Verbesserung der dreidimensionalen Vorstellungskraft sowie der Erlernung eines modernen 3D-CAD Systems.",
                ECTS = 7,
                MV = GetHost(fk03, "AMF"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Produktentwicklung1"
                    },

                }

            };

            Produktentwicklung1.Groups.Add(GetGroup(FAB, "1"));
            FAB.Modules.Add(Produktentwicklung1);
            _db.CurriculumModules.Add(Produktentwicklung1);




            var WerkstofftechnikMetalle = new CurriculumModule()
            {
                ShortName = "WerkstofftechnikMetalle",
                Name = "WerkstofftechnikMetalle",
                ModuleId = "F1100",
                Description = "Die Studierenden sollen in der Lage sein, Werkstoffstrukturen und Gebrauchseigenschaften in Berechnung, Konstruktion, Fertigung und betrieblicher Anwendung zu verknüpfen. ",
                ECTS = 5,
                MV = GetHost(fk03, "SCR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WerkstofftechnikMetalle"
                    },

                }

            };

            WerkstofftechnikMetalle.Groups.Add(GetGroup(FAB, "1"));
            FAB.Modules.Add(WerkstofftechnikMetalle);
            _db.CurriculumModules.Add(WerkstofftechnikMetalle);






            var Elektrotechnik = new CurriculumModule()
            {
                ShortName = "Elektrotechnik",
                Name = "Elektrotechnik",
                ModuleId = "F1150",
                Description = "Kenntnis der Grundbegriffe und Grundgesetze der Elektrotechnik und des Magnetismus sowie der zugrundeliegenden physikalischen Ursachen",
                ECTS = 5,
                MV = GetHost(fk03, "PAL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Elektrotechnik"
                    },

                }

            };

            Elektrotechnik.Groups.Add(GetGroup(FAB, "1"));
            FAB.Modules.Add(Elektrotechnik);
            _db.CurriculumModules.Add(Elektrotechnik);


            var Allgemeinwissenschaften1 = new CurriculumModule()
            {
                ShortName = "Allgemeinwissenschaften1",
                Name = "Allgemeinwissenschaften1",
                ModuleId = "F2150",
                Description = "Dem Modulhandbuch zu entnehmen",
                ECTS = 2,
                MV = GetHost(fk03, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Allgemeinwissenschaften1"
                    },

                }

            };

            Allgemeinwissenschaften1.Groups.Add(GetGroup(FAB, "1"));
            FAB.Modules.Add(Allgemeinwissenschaften1);
            _db.CurriculumModules.Add(Allgemeinwissenschaften1);


            var Allgemeinwissenschaften2 = new CurriculumModule()
            {
                ShortName = "Allgemeinwissenschaften2",
                Name = "Allgemeinwissenschaften2",
                ModuleId = "F2160",
                Description = "Dem Modulhandbuch zu entnehmen",
                ECTS = 2,
                MV = GetHost(fk03, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Allgemeinwissenschaften2"
                    },

                }

            };

            Allgemeinwissenschaften2.Groups.Add(GetGroup(FAB, "2"));
            FAB.Modules.Add(Allgemeinwissenschaften2);
            _db.CurriculumModules.Add(Allgemeinwissenschaften2);



            var Ingenieurmathematik2 = new CurriculumModule()
            {
                ShortName = "Ingenieurmathematik2",
                Name = "Ingenieurmathematik2",
                ModuleId = "F1060",
                Description = "In der Modulgruppe werden gründliche Kenntnisse und vertieftes Verständnis für mathematische Begriffe und Methoden sowie analytische Denkweisen vermittelt, deren Anwendungen in der Fahrzeugtechnik notwendig sind. ",
                ECTS = 6,
                MV = GetHost(fk03, "WAR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ingenieurmathematik2"
                    },

                }

            };

            Ingenieurmathematik2.Groups.Add(GetGroup(FAB, "2"));
            FAB.Modules.Add(Ingenieurmathematik2);
            _db.CurriculumModules.Add(Ingenieurmathematik2);




            var TechnischeMechanik2 = new CurriculumModule()
            {
                ShortName = "TechnischeMechanik2",
                Name = "TechnischeMechanik2",
                ModuleId = "F1070",
                Description = "Die Studierenden sollen in der Lage sein, elastostatische Probleme an Systemen aus Balken und Stäben selbständig zulösen. ",
                ECTS = 5,
                MV = GetHost(fk03, "MID"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeMechanik2"
                    },

                }

            };

            TechnischeMechanik2.Groups.Add(GetGroup(FAB, "2"));
            FAB.Modules.Add(TechnischeMechanik2);
            _db.CurriculumModules.Add(TechnischeMechanik2);



            var SpanloseFertigung = new CurriculumModule()
            {
                ShortName = "SpanloseFertigung",
                Name = "SpanloseFertigung",
                ModuleId = "F2010",
                Description = "Lernziel des Moduls ist die Fähigkeit zur Auswahl, Planung und Durchführung spanloser Fertigungsverfahren unter Berücksichtigung des Zusammenwirkens von Werkstoff, Konstruktion und Fertigung. ",
                ECTS = 5,
                MV = GetHost(fk03, "SCR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SpanloseFertigung"
                    },

                }

            };

            SpanloseFertigung.Groups.Add(GetGroup(FAB, "2"));
            FAB.Modules.Add(SpanloseFertigung);
            _db.CurriculumModules.Add(SpanloseFertigung);



            var Produktentwicklung2 = new CurriculumModule()
            {
                ShortName = "Produktentwicklung2",
                Name = "Produktentwicklung2",
                ModuleId = "F1090",
                Description = "Die Lehrveranstaltung dient dem Erlernen der Grundlagen der methodischen Produktentwicklung und der Vertiefung eines modernen 3D-CAD Systems.  ",
                ECTS = 5,
                MV = GetHost(fk03, "SCW"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Produktentwicklung2"
                    },

                }

            };

            Produktentwicklung2.Groups.Add(GetGroup(FAB, "2"));
            FAB.Modules.Add(Produktentwicklung2);
            _db.CurriculumModules.Add(Produktentwicklung2);


            var Maschinenelemente1 = new CurriculumModule()
            {
                ShortName = "Maschinenelemente1",
                Name = "Maschinenelemente1",
                ModuleId = "F1080",
                Description = "Grundlegendes Dimensionieren von Maschinenelementen und deren Verbindungen unter Berücksichtigung von beanspruchungs-und fertigungsgerechter Gestaltung für den Fahrzeugbau ",
                ECTS = 5,
                MV = GetHost(fk03, "ROH"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Maschinenelemente1"
                    },

                }

            };

            Maschinenelemente1.Groups.Add(GetGroup(FAB, "2"));
            FAB.Modules.Add(Maschinenelemente1);
            _db.CurriculumModules.Add(Maschinenelemente1);



            var Ingenieurinformatik = new CurriculumModule()
            {
                ShortName = "Ingenieurinformatik",
                Name = "Ingenieurinformatik",
                ModuleId = "F1160",
                Description = "Nach der Teilnahme an dieser Lehrveranstaltung können die Studierenden   technisch-wissenschaftliche   Programme   in   einer geeigneten    Programmierumgebung    neu    entwickeln    sowie bestehende  Programme  beurteilen  und  ggf.erweitern.  ",
                ECTS = 5,
                MV = GetHost(fk03, "REI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ingenieurinformatik"
                    },

                }

            };

            Ingenieurinformatik.Groups.Add(GetGroup(FAB, "3"));
            FAB.Modules.Add(Ingenieurinformatik);
            _db.CurriculumModules.Add(Ingenieurinformatik);



            var TechnischeMechanik3 = new CurriculumModule()
            {
                ShortName = "TechnischeMechanik3",
                Name = "TechnischeMechanik3",
                ModuleId = "F2030",
                Description = "Zentrales Lernziel ist das Verständnis des Zusammenhangs zwischen Kräften und Bewegungen an Systemen starrer Körper. ",
                ECTS = 5,
                MV = GetHost(fk03, "MID"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeMechanik3"
                    },

                }

            };

            TechnischeMechanik3.Groups.Add(GetGroup(FAB, "3"));
            FAB.Modules.Add(TechnischeMechanik3);
            _db.CurriculumModules.Add(TechnischeMechanik3);


            var ChemieKunststofftechnik = new CurriculumModule()
            {
                ShortName = "ChemieKunststofftechnik",
                Name = "ChemieKunststofftechnik",
                ModuleId = "F2020",
                Description = "Überblick über die chemischen Grundlagen der Polymer-Chemie",
                ECTS = 6,
                MV = GetHost(fk03, "HOS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ChemieKunststofftechnik"
                    },

                }

            };

            ChemieKunststofftechnik.Groups.Add(GetGroup(FAB, "3"));
            FAB.Modules.Add(ChemieKunststofftechnik);
            _db.CurriculumModules.Add(ChemieKunststofftechnik);



            var Fluidmechanik = new CurriculumModule()
            {
                ShortName = "Fluidmechanik",
                Name = "Fluidmechanik",
                ModuleId = "F2040",
                Description = "Die Studierenden kennen die wichtigsten Begriffe und Modellbildungen der technischen Strömungslehre",
                ECTS = 5,
                MV = GetHost(fk03, "HAK"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fluidmechanik"
                    },

                }

            };

            Fluidmechanik.Groups.Add(GetGroup(FAB, "3"));
            FAB.Modules.Add(Fluidmechanik);
            _db.CurriculumModules.Add(Fluidmechanik);



            var Thermodynamik1Wärmeübertragung = new CurriculumModule()
            {
                ShortName = "Thermodynamik1Wärmeübertragung",
                Name = "Thermodynamik1Wärmeübertragung",
                ModuleId = "F2050",
                Description = "Dieses Modul vermittelt die methodischen und fachlichen Qualifikationen zur thermodynamischen Analyse technischer Systeme.",
                ECTS = 6,
                MV = GetHost(fk03, "GUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Thermodynamik1Wärmeübertragung"
                    },

                }

            };

            Thermodynamik1Wärmeübertragung.Groups.Add(GetGroup(FAB, "3"));
            FAB.Modules.Add(Thermodynamik1Wärmeübertragung);
            _db.CurriculumModules.Add(Thermodynamik1Wärmeübertragung);


            var GrundlagenBetriebswirtschafWirtschaftsrecht = new CurriculumModule()
            {
                ShortName = "GrundlagenBetriebswirtschafWirtschaftsrecht",
                Name = "GrundlagenBetriebswirtschafWirtschaftsrecht",
                ModuleId = "F1140",
                Description = "Die Studierenden können die wesentlichen betriebswirtschaftlichen Prozesse in Zusammenhang mit der Leistungserstellung und -verwertung nachvollziehen",
                ECTS = 4,
                MV = GetHost(fk03, "EIC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GrundlagenBetriebswirtschafWirtschaftsrecht"
                    },

                }

            };

            GrundlagenBetriebswirtschafWirtschaftsrecht.Groups.Add(GetGroup(FAB, "3"));
            FAB.Modules.Add(GrundlagenBetriebswirtschafWirtschaftsrecht);
            _db.CurriculumModules.Add(GrundlagenBetriebswirtschafWirtschaftsrecht);


            var TechnischeDynamik = new CurriculumModule()
            {
                ShortName = "TechnischeDynamik",
                Name = "TechnischeDynamik",
                ModuleId = "F2060",
                Description = "Die Studierenden sind in der Lage, dynamische Systeme mit einem oder mehreren Freiheitsgraden mittels analytischer Methoden zu modellieren und ggf.zu  linearisieren.",
                ECTS = 5,
                MV = GetHost(fk03, "YUA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeDynamik"
                    },

                }

            };

            TechnischeDynamik.Groups.Add(GetGroup(FAB, "4"));
            FAB.Modules.Add(TechnischeDynamik);
            _db.CurriculumModules.Add(TechnischeDynamik);




            var SpanendeFertigungBetriebsorganisation = new CurriculumModule()
            {
                ShortName = "SpanendeFertigungBetriebsorganisation",
                Name = "SpanendeFertigungBetriebsorganisation",
                ModuleId = "F2070",
                Description = "Grundkenntnisse der spanenden Fertigung.",
                ECTS = 5,
                MV = GetHost(fk03, "RAS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SpanendeFertigungBetriebsorganisation"
                    },

                }

            };

            SpanendeFertigungBetriebsorganisation.Groups.Add(GetGroup(FAB, "4"));
            FAB.Modules.Add(SpanendeFertigungBetriebsorganisation);
            _db.CurriculumModules.Add(SpanendeFertigungBetriebsorganisation);


            var RegelungsMesstechnik = new CurriculumModule()
            {
                ShortName = "RegelungsMesstechnik",
                Name = "RegelungsMesstechnik",
                ModuleId = "F2080",
                Description = "Verständnis und Anwendung der Grundbegriffe der Messtechnik",
                ECTS = 6,
                MV = GetHost(fk03, "THI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "RegelungsMesstechnik"
                    },

                }

            };

            RegelungsMesstechnik.Groups.Add(GetGroup(FAB, "4"));
            FAB.Modules.Add(RegelungsMesstechnik);
            _db.CurriculumModules.Add(RegelungsMesstechnik);


            var Fahrzeugmechatronik1 = new CurriculumModule()
            {
                ShortName = "Fahrzeugmechatronik1",
                Name = "Fahrzeugmechatronik1",
                ModuleId = "F3010",
                Description = "Die Studierenden sollen in der Lage sein, die wesentlichen Komponenten des elektrischen Bordnetzes eines Kraftfahrzeugs zu verstehen",
                ECTS = 4,
                MV = GetHost(fk03, "BUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fahrzeugmechatronik1"
                    },

                }

            };

            Fahrzeugmechatronik1.Groups.Add(GetGroup(FAB, "4"));
            FAB.Modules.Add(Fahrzeugmechatronik1);
            _db.CurriculumModules.Add(Fahrzeugmechatronik1);


            var Verbrennungsmotoren1 = new CurriculumModule()
            {
                ShortName = "Verbrennungsmotoren1",
                Name = "Verbrennungsmotoren1",
                ModuleId = "F3020",
                Description = "Dieses Modul vermittelt die methodischen und fachlichen Qualifikationen, die für Einsatz und Entwicklung von Verbrennungsmotoren erforderlich sind.",
                ECTS = 4,
                MV = GetHost(fk03, "DOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Verbrennungsmotoren1"
                    },

                }

            };

            Verbrennungsmotoren1.Groups.Add(GetGroup(FAB, "4"));
            FAB.Modules.Add(Verbrennungsmotoren1);
            _db.CurriculumModules.Add(Verbrennungsmotoren1);


            var Fahrzeugtechnik = new CurriculumModule()
            {
                ShortName = "Fahrzeugtechnik",
                Name = "Fahrzeugtechnik",
                ModuleId = "F3030",
                Description = "Die Studierenden verstehen die Anforderungen für Fahrzeuge und deren Baugruppen ",
                ECTS = 6,
                MV = GetHost(fk03, "MIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fahrzeugtechnik"
                    },

                }

            };

            Fahrzeugtechnik.Groups.Add(GetGroup(FAB, "4"));
            FAB.Modules.Add(Fahrzeugtechnik);
            _db.CurriculumModules.Add(Fahrzeugtechnik);

            _db.SaveChanges();
        }

    }
}
