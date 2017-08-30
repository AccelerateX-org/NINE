using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogMFB_GS(ActivityOrganiser fk06, Curriculum MFB)
        {
            var Allgemeinwissenschaften1 = new CurriculumModule()
            {
                ShortName = "Allgemeinwissenschaften1",
                Name = "Allgemeinwissenschaften1",
                ModuleId = "MFB100.1",
                Description = "Gemäß den Ankündigungen der FK Allgemeinwissenschaften ",
                ECTS = 2,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Allgemeinwissenschaften1"
                    },
                   
                }

            };

            Allgemeinwissenschaften1.Groups.Add(GetGroup(MFB, "1"));
            MFB.Modules.Add(Allgemeinwissenschaften1);
            _db.CurriculumModules.Add(Allgemeinwissenschaften1);

            var Allgemeinwissenschaften2 = new CurriculumModule()
            {
                ShortName = "Allgemeinwissenschaften2",
                Name = "Allgemeinwissenschaften2",
                ModuleId = "MFB100.2",
                Description = "Gemäß den Ankündigungen der FK Allgemeinwissenschaften ",
                ECTS = 2,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Allgemeinwissenschaften2"
                    },
                }
            };

            Allgemeinwissenschaften2.Groups.Add(GetGroup(MFB, "2"));
            MFB.Modules.Add(Allgemeinwissenschaften2);
            _db.CurriculumModules.Add(Allgemeinwissenschaften2);

            var Mathematik1 = new CurriculumModule()
            {
                ShortName = "Mathematik1",
                Name = "Mathematik1",
                ModuleId = "MFB110",
                Description = "Grundlagen, Analysis 1, Vektorrechnung, Analysis 2, Begleitende Übungen mit praktischen Anwendungen ",
                ECTS = 7,
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

            Mathematik1.Groups.Add(GetGroup(MFB, "1"));
            MFB.Modules.Add(Mathematik1);
            _db.CurriculumModules.Add(Mathematik1);


            var Physik1 = new CurriculumModule()
            {
                ShortName = "Physik1",
                Name = "Physik1",
                ModuleId = "MFB120",
                Description = "Grundlagen der Natur- und Ingenieurwissenschaften ",
                ECTS = 5,
                MV = GetHost(fk06, "ALT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik1"
                    },
                }

            };

            Physik1.Groups.Add(GetGroup(MFB, "1"));
            MFB.Modules.Add(Physik1);
            _db.CurriculumModules.Add(Physik1);

            var Elektrotechnik1 = new CurriculumModule()
            {
                ShortName = "Elektrotechnik1",
                Name = "Elektrotechnik1",
                ModuleId = "MFB130",
                Description = "Grundkenntnisse der Zusammenhänge in der Elektrotechnik",
                ECTS = 4,
                MV = GetHost(fk06, "FSR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Elektrotechnik1"
                    },
                }

            };

            Elektrotechnik1.Groups.Add(GetGroup(MFB, "1"));
            MFB.Modules.Add(Elektrotechnik1);
            _db.CurriculumModules.Add(Elektrotechnik1);

            var TechnischeMechanik1 = new CurriculumModule()
            {
                ShortName = "TechnischeMechanik1",
                Name = "TechnischeMechanik1",
                ModuleId = "MFB140",
                Description = "Grundlagen der Natur- und Ingenieurwissenschaften.",
                ECTS = 4,
                MV = GetHost(fk06, "NIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeMechanik1"
                    },
                }

            };

            TechnischeMechanik1.Groups.Add(GetGroup(MFB, "1"));
            MFB.Modules.Add(TechnischeMechanik1);
            _db.CurriculumModules.Add(TechnischeMechanik1);

            var Werkstofftechnik1 = new CurriculumModule()
            {
                ShortName = "Werkstofftechnik1",
                Name = "Werkstofftechnik1",
                ModuleId = "MFB150",
                Description = "Werkstofftechnik 1 und Chemie",
                ECTS = 5,
                MV = GetHost(fk06, "Pol"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Werkstofftechnik1"
                    },
                }
            };

            Werkstofftechnik1.Groups.Add(GetGroup(MFB, "1"));
            MFB.Modules.Add(Werkstofftechnik1);
            _db.CurriculumModules.Add(Werkstofftechnik1);

            var TechnischesZeichnen1 = new CurriculumModule()
            {
                ShortName = "TechnischesZeichnen1",
                Name = "TechnischesZeichnen1",
                ModuleId = "MFB160.1",
                Description = "Grundlagen des technischen Zeichnens/CAD",
                ECTS = 2,
                MV = GetHost(fk06, "SAL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischesZeichnen1"
                    },
                }

            };

            TechnischesZeichnen1.Groups.Add(GetGroup(MFB, "1"));
            MFB.Modules.Add(TechnischesZeichnen1);
            _db.CurriculumModules.Add(TechnischesZeichnen1);

            var TechnischesZeichnen2 = new CurriculumModule()
            {
                ShortName = "TechnischesZeichnen2",
                Name = "TechnischesZeichnen2",
                ModuleId = "MFB160.2",
                Description = "Grundlagen des technischen Zeichnens/CAD",
                ECTS = 3,
                MV = GetHost(fk06, "SAL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischesZeichnen2"
                    },
                }

            };

            TechnischesZeichnen2.Groups.Add(GetGroup(MFB, "2"));
            MFB.Modules.Add(TechnischesZeichnen2);
            _db.CurriculumModules.Add(TechnischesZeichnen2);

            var Mathematik2 = new CurriculumModule()
            {
                ShortName = "Mathematik2",
                Name = "Mathematik2",
                ModuleId = "MFB210",
                Description = "Mathematik2",
                ECTS = 6,
                MV = GetHost(fk06, "HOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathematik2"
                    },
                }

            };

            Mathematik2.Groups.Add(GetGroup(MFB, "2"));
            MFB.Modules.Add(Mathematik2);
            _db.CurriculumModules.Add(Mathematik2);

            var Physik2 = new CurriculumModule()
            {
                ShortName = "Physik2",
                Name = "Physik2",
                ModuleId = "MFB220",
                Description = "Grundlagen der Natur- und Ingenieurwissenschaften ",
                ECTS = 4,
                MV = GetHost(fk06, "LIB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik2"
                    },
                }

            };

            Physik2.Groups.Add(GetGroup(MFB, "2"));
            MFB.Modules.Add(Physik2);
            _db.CurriculumModules.Add(Physik2);

            var Elektrotechnik2 = new CurriculumModule()
            {
                ShortName = "Elektrotechnik2",
                Name = "Elektrotechnik2",
                ModuleId = "MFB230",
                Description = "Grundkenntnisse der Zusammenhänge in der Elektrotechnik",
                ECTS = 4,
                MV = GetHost(fk06, "FSR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Elektrotechnik2"
                    },
                }

            };

            Elektrotechnik2.Groups.Add(GetGroup(MFB, "2"));
            MFB.Modules.Add(Elektrotechnik2);
            _db.CurriculumModules.Add(Elektrotechnik2);




            var TechnischeMechanik2 = new CurriculumModule()
            {
                ShortName = "TechnischeMechanik2",
                Name = "TechnischeMechanik2",
                ModuleId = "MFB240",
                Description = "Grundlagen der Natur- und Ingenieurwissenschaften.",
                ECTS = 4,
                MV = GetHost(fk06, "NIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeMechanik2"
                    },
                }

            };

            TechnischeMechanik2.Groups.Add(GetGroup(MFB, "2"));
            MFB.Modules.Add(TechnischeMechanik2);
            _db.CurriculumModules.Add(TechnischeMechanik2);


            var TechnischeOptik1 = new CurriculumModule()
            {
                ShortName = "TechnischeOptik1",
                Name = "TechnischeOptik1",
                ModuleId = "MFB250",
                Description = "Es wird ein fundamentales ingenieurwissenschaftliches Grundwissen der Optik in der Feinwerktechnik und Mechatronik angelegt.",
                ECTS = 4,
                MV = GetHost(fk06, "FIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeOptik1"
                    },
                }

            };

            TechnischeOptik1.Groups.Add(GetGroup(MFB, "2"));
            MFB.Modules.Add(TechnischeOptik1);
            _db.CurriculumModules.Add(TechnischeOptik1);


            var Konstruktionselemente = new CurriculumModule()
            {
                ShortName = " Konstruktionselemente",
                Name = " Konstruktionselemente",
                ModuleId = "MFB270",
                Description = "Es wird ein fundamentales ingenieurwissenschaftliches Grundwissen der Optik in der Feinwerktechnik und Mechatronik angelegt.",
                ECTS = 4,
                MV = GetHost(fk06, "WAG"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = " Konstruktionselemente"
                    },
                }

            };

            Konstruktionselemente.Groups.Add(GetGroup(MFB, "2"));
            MFB.Modules.Add(Konstruktionselemente);
            _db.CurriculumModules.Add(Konstruktionselemente);


            var Informatik = new CurriculumModule()
            {
                ShortName = " Informatik",
                Name = " Informatik",
                ModuleId = "MFB310",
                Description = "Kenntnisse Aufbau und Funktionsweise von Rechnern",
                ECTS = 4,
                MV = GetHost(fk06, "SHE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = " Informatik"
                    },
                }

            };

            Informatik.Groups.Add(GetGroup(MFB, "3"));
            MFB.Modules.Add(Informatik);
            _db.CurriculumModules.Add(Informatik);

            var SignaleSysteme = new CurriculumModule()
            {
                ShortName = "SignaleSysteme",
                Name = "SignaleSysteme",
                ModuleId = "MFB320",
                Description = "Erkennen, wie mit den Modellvorstellungen von Signale und Systeme Vorgänge in mechatronischen Geräten beschrieben werden können",
                ECTS = 6,
                MV = GetHost(fk06, "SHE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SignaleSysteme"
                    },
                }

            };

            SignaleSysteme.Groups.Add(GetGroup(MFB, "3"));
            MFB.Modules.Add(SignaleSysteme);
            _db.CurriculumModules.Add(SignaleSysteme);


            var Elektronik = new CurriculumModule()
            {
                ShortName = "Elektronik",
                Name = "Elektronik",
                ModuleId = "MFB330",
                Description = "Es wird ein fundamentales ingenieurwissenschaftliches Grundwissen der Elektronik in der Mechatronik angelegt.",
                ECTS = 4,
                MV = GetHost(fk06, "FIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Elektronik"
                    },
                }

            };

            Elektronik.Groups.Add(GetGroup(MFB, "3"));
            MFB.Modules.Add(Elektronik);
            _db.CurriculumModules.Add(Elektronik);


            var TechnischeOptik2 = new CurriculumModule()
            {
                ShortName = "TechnischeOptik2",
                Name = "TechnischeOptik2",
                ModuleId = "MFB340",
                Description = "Es wird ein fundamentales ingenieurwissenschaftliches Grundwissen der Lichttechnik, des optischen Gerätebaus und der optischen Messtechnik in der Feinwerktechnik und Mechatronik angelegt.",
                ECTS = 4,
                MV = GetHost(fk06, "FIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeOptik2"
                    },
                }

            };

            TechnischeOptik2.Groups.Add(GetGroup(MFB, "3"));
            MFB.Modules.Add(TechnischeOptik2);
            _db.CurriculumModules.Add(TechnischeOptik2);


            var Werkstofftechnik2 = new CurriculumModule()
            {
                ShortName = "Werkstofftechnik2",
                Name = "Werkstofftechnik2",
                ModuleId = "MFB350",
                Description = "Vermittlung eines fundamentalen ingenieurwissenschaftlichen Grundwissens der Werkstofftechnik in der Mechatronik.",
                ECTS = 4,
                MV = GetHost(fk06, "STE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Werkstofftechnik2"
                    },
                }

            };

            Werkstofftechnik2.Groups.Add(GetGroup(MFB, "3"));
            MFB.Modules.Add(Werkstofftechnik2);
            _db.CurriculumModules.Add(Werkstofftechnik2);


            var Fertigungstechnik1 = new CurriculumModule()
            {
                ShortName = "Fertigungstechnik1",
                Name = "Fertigungstechnik1",
                ModuleId = "MFB360",
                Description = "Erlangung grundlegender Voraussetzungen für den Einsatz in Konstruktion, Arbeitsvorbereitung, Produktion und Qualitätssicherung. ",
                ECTS = 4,
                MV = GetHost(fk06, "FUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fertigungstechnik1"
                    },
                }

            };

            Fertigungstechnik1.Groups.Add(GetGroup(MFB, "3"));
            MFB.Modules.Add(Fertigungstechnik1);
            _db.CurriculumModules.Add(Fertigungstechnik1);


            var Ergonomie = new CurriculumModule()
            {
                ShortName = "Ergonomie",
                Name = "Ergonomie",
                ModuleId = "MFB360",
                Description = "Die Studierenden lernen Arbeitssysteme systematisch zu analysieren und zu beschreiben.",
                ECTS = 4,
                MV = GetHost(fk06, "LIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ergonomie"
                    },
                }

            };

            Ergonomie.Groups.Add(GetGroup(MFB, "3"));
            MFB.Modules.Add(Ergonomie);
            _db.CurriculumModules.Add(Ergonomie);


            var Konstruktionstechnik1 = new CurriculumModule()
            {
                ShortName = "Konstruktionstechnik1",
                Name = "Konstruktionstechnik1",
                ModuleId = "MFB410",
                Description = "In diesem Modul werden die grundlegenden Fähigkeiten vermittelt, mechatronische Bauteile und Systeme seitens der Aufgabenstellung zu definieren und Lösungen für die technische Umsetzung zu finden. ",
                ECTS = 5,
                MV = GetHost(fk06, "LEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Konstruktionstechnik1"
                    },
                }

            };

            Konstruktionstechnik1.Groups.Add(GetGroup(MFB, "4"));
            MFB.Modules.Add(Konstruktionstechnik1);
            _db.CurriculumModules.Add(Konstruktionstechnik1);

            var ModelbildungSimulation = new CurriculumModule()
            {
                ShortName = "ModelbildungSimulation",
                Name = "ModelbildungSimulation",
                ModuleId = "MFB420",
                Description = "Aufstellen von Modellen für Prozesse mit elektrischen, mechanischen und informationstechnischen Anteilen",
                ECTS = 4,
                MV = GetHost(fk06, "WEB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ModelbildungSimulation"
                    },
                }

            };

            ModelbildungSimulation.Groups.Add(GetGroup(MFB, "4"));
            MFB.Modules.Add(ModelbildungSimulation);
            _db.CurriculumModules.Add(ModelbildungSimulation);

            var Reglungstechnik = new CurriculumModule()
            {
                ShortName = "Reglungstechnik",
                Name = "Reglungstechnik",
                ModuleId = "MFB430",
                Description = "Erkennen, wie mit Steuerungen und Regelungen Vorgänge in realen Systemen gezielt beeinflusst werden können.",
                ECTS = 5,
                MV = GetHost(fk06, "WEB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Reglungstechnik"
                    },
                }

            };

            Reglungstechnik.Groups.Add(GetGroup(MFB, "4"));
            MFB.Modules.Add(Reglungstechnik);
            _db.CurriculumModules.Add(Reglungstechnik);

            var Signalverarbeitung = new CurriculumModule()
            {
                ShortName = "Signalverarbeitung",
                Name = "Signalverarbeitung",
                ModuleId = "MFB440",
                Description = "In Anknüpfung an das Modul „Signale und Systeme“ werden mathematische Modelle zur Digitalisierung von Daten und zu deren Weiterverarbeitung betrachtet und praktisch angewendet.",
                ECTS = 4,
                MV = GetHost(fk06, "EGR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Signalverarbeitung"
                    },
                }

            };

            Signalverarbeitung.Groups.Add(GetGroup(MFB, "4"));
            MFB.Modules.Add(Signalverarbeitung);
            _db.CurriculumModules.Add(Signalverarbeitung);


            var Mikroprozessor = new CurriculumModule()
            {
                ShortName = "Mikroprozessor",
                Name = "Mikroprozessor",
                ModuleId = "MFB450",
                Description = "Befähigung, einen Mikrocontroller als Kernstück einer mechatronischen Komponente einzusetzen",
                ECTS = 4,
                MV = GetHost(fk06, "HER"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mikroprozessor"
                    },
                }

            };

            Mikroprozessor.Groups.Add(GetGroup(MFB, "4"));
            MFB.Modules.Add(Mikroprozessor);
            _db.CurriculumModules.Add(Mikroprozessor);

            var Sensorik = new CurriculumModule()
            {
                ShortName = "Sensorik",
                Name = "Sensorik",
                ModuleId = "MFB460",
                Description = "Das Modul bietet eine Einführung in die physikalischen Funktionsmechanismen verschiedener grundlegender Sensorgruppen und in deren praktische Nutzung.",
                ECTS = 4,
                MV = GetHost(fk06, "EGR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Sensorik"
                    },
                }

            };

            Sensorik.Groups.Add(GetGroup(MFB, "4"));
            MFB.Modules.Add(Sensorik);
            _db.CurriculumModules.Add(Sensorik);

            var MechanismDesignAnalysis = new CurriculumModule()
            {
                ShortName = "MechanismDesignAnalysis",
                Name = "MechanismDesignAnalysis",
                ModuleId = "MFB470",
                Description = "Einführung in die Grundbegriffe der (ebenen) Getriebelehre: Analyse existierender Mechanismen und Entwurf neuer Mechanismen mit dem Ziel, gewünschte Bewegungsvorgänge zu erzeugen.",
                ECTS = 4,
                MV = GetHost(fk06, "WIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MechanismDesignAnalysis"
                    },
                }

            };

            MechanismDesignAnalysis.Groups.Add(GetGroup(MFB, "4"));
            MFB.Modules.Add(MechanismDesignAnalysis);
            _db.CurriculumModules.Add(MechanismDesignAnalysis);

            var MedizinischeMesstechnik = new CurriculumModule()
            {
                ShortName = "MedizinischeMesstechnik",
                Name = "MedizinischeMesstechnik",
                ModuleId = "MFB480",
                Description = "Verständnis der Entstehung, Erfassung und Weiterverarbeitung von Signalen in biologischen Systemen ",
                ECTS = 4,
                MV = GetHost(fk06, "GIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MedizinischeMesstechnik"
                    },
                }

            };

            MedizinischeMesstechnik.Groups.Add(GetGroup(MFB, "4"));
            MFB.Modules.Add(MedizinischeMesstechnik);
            _db.CurriculumModules.Add(MedizinischeMesstechnik);

            var Humanbiologie = new CurriculumModule()
            {
                ShortName = "Humanbiologie",
                Name = "Humanbiologie",
                ModuleId = "MFB490",
                Description = "Erwerb von Grundkenntnissen der Terminologie, Anatomie und Pathophysiologie ",
                ECTS = 4,
                MV = GetHost(fk06, "GIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Humanbiologie"
                    },
                }

            };

            Humanbiologie.Groups.Add(GetGroup(MFB, "4"));
            MFB.Modules.Add(Humanbiologie);
            _db.CurriculumModules.Add(Humanbiologie);


            var Industriepraktikum = new CurriculumModule()
            {
                ShortName = "Industriepraktikum",
                Name = "Industriepraktikum",
                ModuleId = "MFB510",
                Description = "Kennenlernen der Ingenieurtätigkeit im gewählten Studiengang, in der gewählten Studienrichtung. ",
                ECTS = 22,
                MV = GetHost(fk06, "HER"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Industriepraktikum"
                    },
                }

            };

            Industriepraktikum.Groups.Add(GetGroup(MFB, "5"));
            MFB.Modules.Add(Industriepraktikum);
            _db.CurriculumModules.Add(Industriepraktikum);


            var Praxisseminar = new CurriculumModule()
            {
                ShortName = "Praxisseminar",
                Name = "Praxisseminar",
                ModuleId = "MFB520",
                Description = "Komplexe Sachverhalte strukturieren und in Präsentationen verständlich vermitteln. ",
                ECTS = 4,
                MV = GetHost(fk06, "LIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Praxisseminar"
                    },
                }

            };

            Praxisseminar.Groups.Add(GetGroup(MFB, "5"));
            MFB.Modules.Add(Praxisseminar);
            _db.CurriculumModules.Add(Praxisseminar);

            var BetriebswirtschaftlicheGrundlagen = new CurriculumModule()
            {
                ShortName = "BetriebswirtschaftlicheGrundlagen",
                Name = "BetriebswirtschaftlicheGrundlagen",
                ModuleId = "MFB530",
                Description = "Dieses Modul vermittelt grundlegende fachübergreifende, insbesondere betriebswirtschaftliche Kenntnisse, die Fähigkeit, technische Produkte und betriebliche Prozesse nach wirtschaftlichen Kriterien zu analysieren, zu bewerten und zu gestalten sowie die Kompetenz, fach- und disziplinübergreifend im Team zu arbeiten.",
                ECTS = 4,
                MV = GetHost(fk06, "LIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BetriebswirtschaftlicheGrundlagen"
                    },
                }

            };

            BetriebswirtschaftlicheGrundlagen.Groups.Add(GetGroup(MFB, "5"));
            MFB.Modules.Add(BetriebswirtschaftlicheGrundlagen);
            _db.CurriculumModules.Add(BetriebswirtschaftlicheGrundlagen);

            var Konstruktionstechnik2 = new CurriculumModule()
            {
                ShortName = "Konstruktionstechnik2",
                Name = "Konstruktionstechnik2",
                ModuleId = "MFB600",
                Description = "In diesem Modul werden die grundlegenden Fähigkeiten vermittelt, mechatronische Bauteile und Systeme von der prinzipiellen Lösung bis hin zur Produktdokumentation zu erarbeiten.",
                ECTS = 4,
                MV = GetHost(fk06, "LEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Konstruktionstechnik2"
                    },
                }

            };

            Konstruktionstechnik2.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(Konstruktionstechnik2);
            _db.CurriculumModules.Add(Konstruktionstechnik2);

            var Fertigungstechnik2 = new CurriculumModule()
            {
                ShortName = "Fertigungstechnik2",
                Name = "Fertigungstechnik2",
                ModuleId = "MFB610",
                Description = "Erlangung vertiefter Kenntnisse für den Einsatz in Konstruktion, Arbeitsvorbereitung, Produktion und Qualitätssicherung. ",
                ECTS = 4,
                MV = GetHost(fk06, "FUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fertigungstechnik2"
                    },
                }

            };

            Fertigungstechnik2.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(Fertigungstechnik2);
            _db.CurriculumModules.Add(Fertigungstechnik2);

            var EmbeddedSystems1 = new CurriculumModule()
            {
                ShortName = "EmbeddedSystems1",
                Name = "EmbeddedSystems1",
                ModuleId = "MFB620",
                Description = "Es sollen Grundkenntnisse vermittelt werden, die es dem Studenten ermöglichen, eine Aufgabenstellung aus dem Bereich der eingebetteten System von der Idee bis zur funktionstüchtigen Platine zu lösen.",
                ECTS = 4,
                MV = GetHost(fk06, "PAR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EmbeddedSystems1"
                    },
                }

            };

            EmbeddedSystems1.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(EmbeddedSystems1);
            _db.CurriculumModules.Add(EmbeddedSystems1);

            var LasertechnikOptoelektronik1 = new CurriculumModule()
            {
                ShortName = "LasertechnikOptoelektronik1 ",
                Name = "LasertechnikOptoelektronik1 ",
                ModuleId = "MFB630",
                Description = "Grundkenntnisse der lasergestützten Messtechnik ",
                ECTS = 4,
                MV = GetHost(fk06, "FIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "LasertechnikOptoelektronik1 "
                    },
                }

            };

            LasertechnikOptoelektronik1.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(LasertechnikOptoelektronik1);
            _db.CurriculumModules.Add(LasertechnikOptoelektronik1);

            var MechatronischeIntegration = new CurriculumModule()
            {
                ShortName = "MechatronischeIntegration ",
                Name = "MechatronischeIntegration",
                ModuleId = "MFB640",
                Description = "Das Modul vermittelt die Fähigkeit, mechatronische, medizinische Systeme zu kennen, zu verstehen und anzuwenden, zu entwerfen, auszulegen und zu simulieren, sowie die Kompetenz, diese Systeme zu analysieren, ihre Qualität zu bewerten und zu sichern.",
                ECTS = 6,
                MV = GetHost(fk06, "LEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MechatronischeIntegration"
                    },
                }

            };

            MechatronischeIntegration.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(MechatronischeIntegration);
            _db.CurriculumModules.Add(MechatronischeIntegration);

            var MedizinischeProduktentwicklung = new CurriculumModule()
            {
                ShortName = "MedizinischeProduktentwicklung",
                Name = "MedizinischeProduktentwicklung",
                ModuleId = "MFB650",
                Description = "Das Modul vermittelt die Fähigkeit, medizinische mechatronische Systeme zu kennen, zu verstehen und anzuwenden, zu entwerfen, auszulegen und zu simulieren, sowie die Kompetenz, diese Systeme zu analysieren, ihre Qualität zu bewerten und zu sichern.",
                ECTS = 4,
                MV = GetHost(fk06, "LEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MedizinischeProduktentwicklung"
                    },
                }

            };

            MedizinischeProduktentwicklung.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(MedizinischeProduktentwicklung);
            _db.CurriculumModules.Add(MedizinischeProduktentwicklung);


            var KlinischeTechnikundKommunikation = new CurriculumModule()
            {
                ShortName = "KlinischeTechnikundKommunikation",
                Name = "KlinischeTechnikundKommunikation",
                ModuleId = "MFB660",
                Description = "Verständnis für die besonderen Anforderungen an die Einrichtung und den Betrieb eines modernen Krankenhauses",
                ECTS = 4,
                MV = GetHost(fk06, "GIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "KlinischeTechnikundKommunikation"
                    },
                }

            };

            KlinischeTechnikundKommunikation.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(KlinischeTechnikundKommunikation);
            _db.CurriculumModules.Add(KlinischeTechnikundKommunikation);


            var MedizinischeSysteme = new CurriculumModule()
            {
                ShortName = "MedizinischeSysteme",
                Name = "MedizinischeSysteme",
                ModuleId = "MFB670",
                Description = "Verständnis medizintechnischer Systeme in ihren Möglichkeiten und Grenzen der Anwendung ",
                ECTS = 4,
                MV = GetHost(fk06, "GIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MedizinischeSysteme"
                    },
                }

            };

            MedizinischeSysteme.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(MedizinischeSysteme);
            _db.CurriculumModules.Add(MedizinischeSysteme);

            var MedizinischeOptik = new CurriculumModule()
            {
                ShortName = "MedizinischeOptik",
                Name = "MedizinischeOptik",
                ModuleId = "MFB680",
                Description = "Kenntnisse über Aufbau und Funktion der in der Medizin verwendeten optischen Geräte und grundlegendes Verständnis der Anwendungsverfahren",
                ECTS = 6,
                MV = GetHost(fk06, "WON"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MedizinischeOptik"
                    },
                }

            };

            MedizinischeOptik.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(MedizinischeOptik);
            _db.CurriculumModules.Add(MedizinischeOptik);



            var MedizinischeTechnik = new CurriculumModule()
            {
                ShortName = "MedizinischeTechnik",
                Name = "MedizinischeTechnik",
                ModuleId = "MFB690",
                Description = "Fähigkeit, Informationen zu einem Spezialgebiete der Medizintechnik zu recherchieren und die so erworbenen Kenntnisse in kompakt Fom darzustellen. ",
                ECTS = 4,
                MV = GetHost(fk06, "SHL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MedizinischeTechnik"
                    },
                }

            };

            MedizinischeTechnik.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(MedizinischeTechnik);
            _db.CurriculumModules.Add(MedizinischeTechnik);



            var Bachelorarbeit = new CurriculumModule()
            {
                ShortName = "Bachelorarbeit",
                Name = "Bachelorarbeit",
                ModuleId = "MFB700",
                Description = "Durchführen einer größeren Projektarbeit, Selbständigkeit, fachübergreifendes Anwenden des im Studium erlernten Stoffes nachweisen.",
                ECTS =12,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bachelorarbeit"
                    },
                }

            };

            Bachelorarbeit.Groups.Add(GetGroup(MFB, "7"));
            MFB.Modules.Add(Bachelorarbeit);
            _db.CurriculumModules.Add(Bachelorarbeit);

            var FiniteElementeMethode = new CurriculumModule()
            {
                ShortName = "FiniteElementeMethode",
                Name = "FiniteElementeMethode",
                ModuleId = "MFB710",
                Description = "Vertiefung des Verständnisses für mathematisch-physikalische Problemstellungen in der Mechatronik",
                ECTS = 5,
                MV = GetHost(fk06, "HAL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FiniteElementeMethode"
                    },
                }

            };

            FiniteElementeMethode.Groups.Add(GetGroup(MFB, "7"));
            MFB.Modules.Add(FiniteElementeMethode);
            _db.CurriculumModules.Add(FiniteElementeMethode);

            var QualitätsmanagementMechatronik = new CurriculumModule()
            {
                ShortName = "QualitätsmanagementMechatronik",
                Name = "QualitätsmanagementMechatronik",
                ModuleId = "MFB720",
                Description = "Wissensvermittlung von Qualtitätsmanagementsystemen, speziell von DIN EN ISO 9000.",
                ECTS = 4,
                MV = GetHost(fk06, "GIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "QualitätsmanagementMechatronik"
                    },
                }

            };

            QualitätsmanagementMechatronik.Groups.Add(GetGroup(MFB, "7"));
            MFB.Modules.Add(QualitätsmanagementMechatronik);
            _db.CurriculumModules.Add(QualitätsmanagementMechatronik);


            var MedizinischeBildgebung = new CurriculumModule()
            {
                ShortName = "MedizinischeBildgebung",
                Name = "MedizinischeBildgebung",
                ModuleId = "MFB730",
                Description = "Überblick über die gängigen bildgebenden Verfahren in der Medizintechnik",
                ECTS = 5,
                MV = GetHost(fk06, "WON"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MedizinischeBildgebung"
                    },
                }

            };

            MedizinischeBildgebung.Groups.Add(GetGroup(MFB, "7"));
            MFB.Modules.Add(MedizinischeBildgebung);
            _db.CurriculumModules.Add(MedizinischeBildgebung);

            var QualitätsmanagementMedizintechnik = new CurriculumModule()
            {
                ShortName = "QualitätsmanagementMedizintechnik",
                Name = "QualitätsmanagementMedizintechnik",
                ModuleId = "MFB740",
                Description = "In diesem Modul werden die Fähigkeiten vermittelt, mechatronische Systeme (Geräte und Apparate) und Prozesse zu kennen, zu verstehen und anzuwenden sowie als Einzelperson oder im Team Projekte durchzuführen und die Arbeitsergebnisse zu dokumentieren und zu präsentieren.",
                ECTS = 4,
                MV = GetHost(fk06, "FUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "QualitätsmanagementMedizintechnik"
                    },
                }

            };

            QualitätsmanagementMedizintechnik.Groups.Add(GetGroup(MFB, "7"));
            MFB.Modules.Add(QualitätsmanagementMedizintechnik);
            _db.CurriculumModules.Add(QualitätsmanagementMedizintechnik);


            var WahlpflichtmodulTechnik = new CurriculumModule()
            {
                ShortName = "WahlpflichtmodulTechnik",
                Name = "WahlpflichtmodulTechnik",
                ModuleId = "MFB800",
                Description = "Siehe Fachbeschreibung des ausgewählten Moduls",
                ECTS = 4,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WahlpflichtmodulTechnik"
                    },
                }

            };

            WahlpflichtmodulTechnik.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(WahlpflichtmodulTechnik);
            _db.CurriculumModules.Add(WahlpflichtmodulTechnik);



            var WahlpflichtmodulG1 = new CurriculumModule()
            {
                ShortName = "WahlpflichtmodulG1",
                Name = "WahlpflichtmodulG1",
                ModuleId = "MFB801",
                Description = "Siehe Fachbeschreibung des ausgewählten Moduls",
                ECTS = 4,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WahlpflichtmodulG1"
                    },
                }

            };

            WahlpflichtmodulG1.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(WahlpflichtmodulG1);
            _db.CurriculumModules.Add(WahlpflichtmodulG1);


            var WahlpflichtmodulG2 = new CurriculumModule()
            {
                ShortName = "WahlpflichtmodulG2",
                Name = "WahlpflichtmodulG2",
                ModuleId = "MFB802",
                Description = "Siehe Fachbeschreibung des ausgewählten Moduls",
                ECTS = 4,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WahlpflichtmodulG2"
                    },
                }

            };

            WahlpflichtmodulG2.Groups.Add(GetGroup(MFB, "7"));
            MFB.Modules.Add(WahlpflichtmodulG2);
            _db.CurriculumModules.Add(WahlpflichtmodulG2);


            var WahlpflichtmodulM1 = new CurriculumModule()
            {
                ShortName = "WahlpflichtmodulM1",
                Name = "WahlpflichtmodulM1",
                ModuleId = "MFB803",
                Description = "Siehe Fachbeschreibung des ausgewählten Moduls",
                ECTS = 4,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WahlpflichtmodulM1"
                    },
                }

            };

            WahlpflichtmodulM1.Groups.Add(GetGroup(MFB, "6"));
            MFB.Modules.Add(WahlpflichtmodulM1);
            _db.CurriculumModules.Add(WahlpflichtmodulM1);

            var WahlpflichtmodulM2 = new CurriculumModule()
            {
                ShortName = "WahlpflichtmodulM2",
                Name = "WahlpflichtmodulM2",
                ModuleId = "MFB804",
                Description = "Siehe Fachbeschreibung des ausgewählten Moduls",
                ECTS = 4,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WahlpflichtmodulM2"
                    },
                }

            };

            WahlpflichtmodulM2.Groups.Add(GetGroup(MFB, "7"));
            MFB.Modules.Add(WahlpflichtmodulM2);
            _db.CurriculumModules.Add(WahlpflichtmodulM2);



            _db.SaveChanges();
        }

    }
}
