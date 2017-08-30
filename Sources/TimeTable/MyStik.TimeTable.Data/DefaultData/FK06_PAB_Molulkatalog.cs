using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogPAB_GS(ActivityOrganiser fk06, Curriculum pht)
        {

            var Mathe1 = new CurriculumModule()
            {
                ShortName = "Mathe1",
                Name = "Mathematik 1",
                ModuleId = "PNB110",
                Description = "Fundamentale naturwissenschaftliche Sachverhalte zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 7,
                MV = GetHost(fk06, "HOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathe1"
                    },
                }

            };

            Mathe1.Groups.Add(GetGroup(pht, "1"));
            pht.Modules.Add(Mathe1);
            _db.CurriculumModules.Add(Mathe1);

            var Phy1 = new CurriculumModule()
            {
                ShortName = "Phy 1",
                Name = "Physik 1",
                ModuleId = "PNB120",
                Description = "Fundamentale physikalische und physikalisch-technische Sachverhalte zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 5,
                MV = GetHost(fk06, "LIB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Physik1 Vorl"
                    },
                    new ModuleCourse
                    {
                        CourseType = CourseType.Practice,
                        ExternalId = "PhysikÜB"
                    },
                }

            };

            Phy1.Groups.Add(GetGroup(pht, "1"));
            pht.Modules.Add(Phy1);
            _db.CurriculumModules.Add(Phy1);

            var ET1 = new CurriculumModule()
            {
                ShortName = "ET1",
                Name = "Elektrotechnik 1",
                ModuleId = "PNB130",
                Description = "	Fundamentale elektrotechnische Sachverhalte bezüglich physikalisch-technischer Geräte und Systeme zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 4,
                MV = GetHost(fk06, "PAR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ET1"
                    },
                }
            };

            ET1.Groups.Add(GetGroup(pht, "1"));
            pht.Modules.Add(ET1);
            _db.CurriculumModules.Add(ET1);

            var tm = new CurriculumModule()
            {
                ShortName = "TM",
                Name = "Technische Mechanik",
                ModuleId = "PNB140",
                Description = "Maschinen berechnen",
                ECTS = 4,
                MV = GetHost(fk06, "WIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TM"
                    },
                }
            };

            tm.Groups.Add(GetGroup(pht, "1"));
            pht.Modules.Add(tm);
            _db.CurriculumModules.Add(tm);


            var WT1 = new CurriculumModule()
            {
                ShortName = "WT1",
                Name = "Werkstofftechnik 1",
                ModuleId = "PNB150",
                Description = "Fundamentale werkstofftechnische Sachverhalte bezüglich physikalisch-technischer Geräte und Prozesse zu verstehen, zu beschreiben und anzuwenden",
                ECTS = 4,
                MV = GetHost(fk06, "KOC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wertstofftechnik 1"
                    },
                }

            };
            WT1.Groups.Add(GetGroup(pht, "1"));
            pht.Modules.Add(WT1);
            _db.CurriculumModules.Add(WT1);

            var CAD1 = new CurriculumModule()
            {
                ShortName = "CAD1",
                Name = "Konstruktion/CAD 1",
                ModuleId = "PHB160",
                Description = "Das Fach vermittelt die Grundlagen der rechnergestützen Konstruktion Mithilfe von Computer Aided Design Systemen und dessen Methoden. Des Weiteren sollen 3D und 2D Geometrien erzeugt und gestaltet werden. ",
                ECTS = 4,
                MV = GetHost(fk06, "SLU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Konstruktion/CAD 1"
                    },
                }

            };

            CAD1.Groups.Add(GetGroup(pht, "1"));
            pht.Modules.Add(CAD1);
            _db.CurriculumModules.Add(CAD1);

            var Mathe2 = new CurriculumModule()
            {
                ShortName = "Mathe2",
                Name = "Mathematik 2",
                ModuleId = "PNB210",
                Description = "Fundamentale naturwissenschaftliche Sachverhalte zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 6,
                MV = GetHost(fk06, "HOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathe2"
                    },
                }

            };

            Mathe2.Groups.Add(GetGroup(pht, "2"));
            pht.Modules.Add(Mathe2);
            _db.CurriculumModules.Add(Mathe2);

            var Physik2 = new CurriculumModule()
            {
                ShortName = "PHY2",
                Name = "Physik 2",
                ModuleId = "PNB220",
                Description = "Fundamentale physikalische Sachverhalte zu verstehen, zu beschreiben und anzuwenden Grundlagen von Wärmelehre, Elektrizität und Magnetismus",
                ECTS = 4,
                MV = GetHost(fk06, "LIB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik 2"
                    },
                }

            };

            Physik2.Groups.Add(GetGroup(pht, "2"));
            pht.Modules.Add(Physik2);
            _db.CurriculumModules.Add(Physik2);


            var ET2 = new CurriculumModule()
            {
                ShortName = "ET2",
                Name = "Elektrotechnik 2/Elektrische Antriebe",
                ModuleId = "PNB230",
                Description = "Aktives, d.h. zum Entwurf befähigendes Verständnis passiver Schaltungen bis Filter 1. Ordnung und fundamentale elektrotechnische Sachverhalte bezüglich physikalisch - technischer Geräte und Systeme zu verstehen,zu beschreiben und anzuwenden",
                ECTS = 4,
                MV = GetHost(fk06, "MEN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Elektrotechnik 2/Elektrische Antriebe"
                    },
                }
            };

            ET2.Groups.Add(GetGroup(pht, "2"));
            pht.Modules.Add(ET2);
            _db.CurriculumModules.Add(ET2);


            var tm2 = new CurriculumModule()
            {
                ShortName = "TM2",
                Name = "Technische Mechanik 2",
                ModuleId = "PNB240",
                Description = "Maschinen berechnen",
                ECTS = 5,
                MV = GetHost(fk06, "WIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TM2"
                    },
                }
            };

            tm2.Groups.Add(GetGroup(pht, "2"));
            pht.Modules.Add(tm2);
            _db.CurriculumModules.Add(tm2);

            var CAD2 = new CurriculumModule()
            {
                ShortName = "CAD2",
                Name = "Konstruktion/CAD 2",
                ModuleId = "PNB250",
                Description = "Das Fach vermittelt die Grundlagen der rechnergestützen Konstruktion Mithilfe von Computer Aided Design Systemen und dessen Methoden. Des Weiteren sollen 3D und 2D Geometrien erzeugt und gestaltet werden. ",
                ECTS = 5,
                MV = GetHost(fk06, "SLU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Konstruktion/CAD 2"
                    },
                }

            };

            CAD2.Groups.Add(GetGroup(pht, "2"));
            pht.Modules.Add(CAD2);
            _db.CurriculumModules.Add(CAD2);

            var Info = new CurriculumModule()
            {
                ShortName = "Info",
                Name = "Informatik ",
                ModuleId = "PNB260",
                Description = "Besitz eines grundlegenden Verständnisses über den Aufbau und die Funktion moderner Computer und Betriebssysteme ",
                ECTS = 4,
                MV = GetHost(fk06, "SWA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Informatik"
                    },
                }

            };

            Info.Groups.Add(GetGroup(pht, "2"));
            pht.Modules.Add(Info);
            _db.CurriculumModules.Add(Info);

            var IP1 = new CurriculumModule()
            {
                ShortName = "IP1",
                Name = "Industriepraktikum 1",
                ModuleId = "PNB310.1",
                Description = "die Arbeitsmethodik des Physik-Ingenieurs im betrieblichen Umfeld anhand konkreter Aufgabenstellungen",
                ECTS = 13,
                MV = GetHost(fk06, "LIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Industriepraktikum 1"
                    },
                }

            };
            IP1.Groups.Add(GetGroup(pht, "3"));
            pht.Modules.Add(IP1);
            _db.CurriculumModules.Add(IP1);

            var IP2 = new CurriculumModule()
            {
                ShortName = "IP2",
                Name = "Industriepraktikum 2",
                ModuleId = "PNB310.2",
                Description = "die Arbeitsmethodik des Physik-Ingenieurs im betrieblichen Umfeld anhand konkreter Aufgabenstellungen",
                ECTS = 17,
                MV = GetHost(fk06, "LIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Industriepraktikum 2"
                    },
                }

            };
            IP2.Groups.Add(GetGroup(pht, "5"));
            pht.Modules.Add(IP2);
            _db.CurriculumModules.Add(IP2);

            var SS1 = new CurriculumModule()
            {
                ShortName = "SS1",
                Name = "Signale und Systeme 1",
                ModuleId = "PNB320.1",
                Description = "Bezogen auf die übergeordneten Lernziele des Studiengangs trägt dieses Modul ins-besondere bei zum Kompetenzbereich: Grundlagen der Natur - und Ingenieurwissenschaften ",
                ECTS = 4,
                MV = GetHost(fk06, "HOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = " 	Signale und Systeme 1"
                    },
                }

            };
            SS1.Groups.Add(GetGroup(pht, "3"));
            pht.Modules.Add(SS1);
            _db.CurriculumModules.Add(SS1);

            var SS2 = new CurriculumModule()
            {
                ShortName = "SS2",
                Name = "Signale und Systeme 2",
                ModuleId = "PNB320.2",
                Description = "Bezogen auf die übergeordneten Lernziele des Studiengangs trägt dieses Modul ins-besondere bei zum Kompetenzbereich: Grundlagen der Natur - und Ingenieurwissenschaften ",
                ECTS = 3,
                MV = GetHost(fk06, "HOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = " 	Signale und Systeme 2"
                    },
                }

            };
            SS2.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(SS2);
            _db.CurriculumModules.Add(SS2);

            var WT2 = new CurriculumModule()
            {
                ShortName = "WT2",
                Name = "Werkstofftechnik 2/Chemie in der Produktion",
                ModuleId = "PNB330",
                Description = "Bezogen auf die übergeordneten Lernziele des Studienganges trägt dieses Modul insbesondere bei zu den Kompetenzbereichen:Grundlagen der Natur - und Ingenieurwissenschaften ",
                ECTS = 4,
                MV = GetHost(fk06, "KOC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wertstofftechnik 2/Chemie in der Produktion"
                    },
                }

            };
            WT2.Groups.Add(GetGroup(pht, "3"));
            pht.Modules.Add(WT2);
            _db.CurriculumModules.Add(WT2);

            var IS1 = new CurriculumModule()
            {
                ShortName = "IS1",
                Name = "Industrieseminar 1",
                ModuleId = "PNB340.1",
                Description = "Bezogen auf die übergeordneten Lernziele des Studienganges trägt dieses Modul insbesondere bei zu den Kompetenzbereichen:Grundlagen der Natur - und Ingenieurwissenschaften ",
                ECTS = 3,
                MV = GetHost(fk06, "LIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Industrieseminar 1"
                    },
                }

            };
            IS1.Groups.Add(GetGroup(pht, "3"));
            pht.Modules.Add(IS1);
            _db.CurriculumModules.Add(IS1);

            var IS2 = new CurriculumModule()
            {
                ShortName = "IS2",
                Name = "Industrieseminar 2",
                ModuleId = "PNB340.2",
                Description = "Bezogen auf die übergeordneten Lernziele des Studienganges trägt dieses Modul insbesondere bei zu den Kompetenzbereichen:Grundlagen der Natur - und Ingenieurwissenschaften ",
                ECTS = 3,
                MV = GetHost(fk06, "LIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Industrieseminar 2"
                    },
                }

            };
            IS2.Groups.Add(GetGroup(pht, "6"));
            pht.Modules.Add(IS2);
            _db.CurriculumModules.Add(IS2);

            var TE = new CurriculumModule()
            {
                ShortName = "TE",
                Name = "Technisches Englisch",
                ModuleId = "PNB370.1",
                Description = "Bezogen auf die übergeordneten Lernziele des Studienganges trägt dieses Modul insbesondere bei zu den Kompetenzbereichen:Grundlagen der Natur - und Ingenieurwissenschaften ",
                ECTS = 2,
                MV = GetHost(fk06, "CHA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Technisches Englisch"
                    },
                }

            };
            TE.Groups.Add(GetGroup(pht, "3"));
            pht.Modules.Add(TE);
            _db.CurriculumModules.Add(TE);

            var MS = new CurriculumModule()
            {
                ShortName = "MS",
                Name = "Messtechnik/Sensorik",
                ModuleId = "PNB340.1",
                Description = "	Bezogen auf die übergeordneten Lernziele des Studiengangs trägt dieses Modul insbesondere bei zu den Kompetenzbereichen:Produktionstechnik Automatisierungstechnik SoftSkills ",
                ECTS = 4,
                MV = GetHost(fk06, "FIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Messtechnik/Sensorik"
                    },
                }

            };
            MS.Groups.Add(GetGroup(pht, "3"));
            pht.Modules.Add(MS);
            _db.CurriculumModules.Add(MS);

            var EA = new CurriculumModule()
            {
                ShortName = "EA",
                Name = "Ergonomie und Arbeitsgesteltung",
                ModuleId = "PNB 410",
                Description = "	Bezogen auf die übergeordneten Lernziele des Studienganges trägt dieses Modul insbesondere bei zum Kompetenzbereich: •Produktionstechnik ",
                ECTS = 4,
                MV = GetHost(fk06, "LIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ergonomie und Arbeitsplanung"
                    },
                }

            };
            EA.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(EA);
            _db.CurriculumModules.Add(EA);

            var FT1 = new CurriculumModule()
            {
                ShortName = "FT1",
                Name = "Fertigungstechnik 1",
                ModuleId = "PNB 420",
                Description = "	Bezogen auf die übergeordneten Lernziele des Studienganges trägt dieses Modul insbesondere bei zum Kompetenzbereich: •Produktionstechnik ",
                ECTS = 4,
                MV = GetHost(fk06, "FUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fertigungstechnik 1"
                    },
                }

            };
            FT1.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(FT1);
            _db.CurriculumModules.Add(FT1);

            var RT = new CurriculumModule()
            {
                ShortName = "RT",
                Name = "Regelungstechnik",
                ModuleId = "PNB 430",
                Description = "	Bezogen auf die übergeordneten Lernziele des Studienganges trägt dieses Modul insbesondere bei zum Kompetenzbereich: Grundlagen der Natur- und Ingenieurwissenschaften Automatisierungstechnik",
                ECTS = 5,
                MV = GetHost(fk06, "WEB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Regelungstechnik"
                    },
                }

            };
            RT.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(RT);
            _db.CurriculumModules.Add(RT);

            var CAD = new CurriculumModule()
            {
                ShortName = "CAD",
                Name = "Konstruktion/CAD/FEM",
                ModuleId = "PNB 440",
                Description = "	Bezogen auf die übergeordneten Lernziele des Studienganges trägt dieses Modul insbesondere bei zu den Kompetenzbereichen: Grundlagen der Natur- und Ingenieurwissenschaften Produktionstechnik Sprachliche und interkulturelle Kompetenzen",
                ECTS = 5,
                MV = GetHost(fk06, "SLU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Konstruktion/CAD/FEM "
                    },
                }

            };
            CAD.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(CAD);
            _db.CurriculumModules.Add(CAD);

            var PA1 = new CurriculumModule()
            {
                ShortName = "PA1",
                Name = "Prozessautomatisierung I ",
                ModuleId = "PNB 450",
                Description = "	Bezogen auf die übergeordneten Lernziele des Studiengangs trägt dieses Modul insbesondere bei zu den Kompetenzbereichen: Produktionstechnik Automatisierungstechnik SoftSkills",
                ECTS = 4,
                MV = GetHost(fk06, "FIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Prozessautomatisierung I  "
                    },
                }

            };
            PA1.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(PA1);
            _db.CurriculumModules.Add(PA1);

            var DE = new CurriculumModule()
            {
                ShortName = "DE",
                Name = "Digitalelektronik",
                ModuleId = "PNB 450",
                Description = "Steuerungen für Produktionsanlagen konzipieren, entwickeln und anpassen können. Der Student soll nach Besuch der Lehrveranstaltung sowie der Absolvierung des Praktikums",
                ECTS = 4,
                MV = GetHost(fk06, "HER"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Digitalelektronik"
                    },
                }

            };
            DE.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(DE);
            _db.CurriculumModules.Add(DE);


            var DF = new CurriculumModule()
            {
                ShortName = "DF",
                Name = "Digitale Fabrik",
                ModuleId = "PNB 510",
                Description = "IT/Informationsmanagement ",
                ECTS = 4,
                MV = GetHost(fk06, "LIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Digitale Fabrik"
                    },
                }

            };
            DF.Groups.Add(GetGroup(pht, "5"));
            pht.Modules.Add(DF);
            _db.CurriculumModules.Add(DF);


            var AO = new CurriculumModule()
            {
                ShortName = "AO",
                Name = "Angewandte Optik ",
                ModuleId = "PNB 510",
                Description = "Dualismus des Lichtes (Welle, Teilchen, Strahl), Optische Materialien und Werkstoffe, Reflexion und Brechung, Fresnelsche Formeln",
                ECTS = 4,
                MV = GetHost(fk06, "FIC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Angewandte Optik "
                    },
                }

            };
            AO.Groups.Add(GetGroup(pht, "5"));
            pht.Modules.Add(AO);
            _db.CurriculumModules.Add(AO);

            var BWL = new CurriculumModule()
            {
                ShortName = "BWL",
                Name = "Betriebswirtschaftliche Grundlagen/Kostenrechnung ",
                ModuleId = "PNB 520",
                Description = "Betriebswirtschaftliche Kenntnisse ",
                ECTS = 6,
                MV = GetHost(fk06, "ZAN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Betriebswirtschaftliche Grundlagen/Kostenrechnung"
                    },
                }

            };
            BWL.Groups.Add(GetGroup(pht, "5"));
            pht.Modules.Add(BWL);
            _db.CurriculumModules.Add(BWL);

            var II1 = new CurriculumModule()
            {
                ShortName = "II1",
                Name = "Industrielle Informatik I ",
                ModuleId = "PNB 530",
                Description = "Automatisierung, IT / Informationsmanagement",
                ECTS = 6,
                MV = GetHost(fk06, "SEN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Industrielle Informatik I"
                    },
                }

            };
            II1.Groups.Add(GetGroup(pht, "5"));
            pht.Modules.Add(II1);
            _db.CurriculumModules.Add(II1);

            var FT2 = new CurriculumModule()
            {
                ShortName = "FT2",
                Name = "Fertigungstechnik 2 ",
                ModuleId = "PNB 550",
                Description = "Produktionstechnik, Automatisierungstechnik",
                ECTS = 5,
                MV = GetHost(fk06, "FUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fertigungstechnik 2 "
                    },
                }

            };
            FT2.Groups.Add(GetGroup(pht, "5"));
            pht.Modules.Add(FT2);
            _db.CurriculumModules.Add(FT2);


            var AF = new CurriculumModule()
            {
                ShortName = "AF",
                Name = "Arbeits- und Fabrikplanung ",
                ModuleId = "PNB 560",
                Description = "Produktionstechnik",
                ECTS = 4,
                MV = GetHost(fk06, "LIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Arbeits- und Fabrikplanung "
                    },
                }

            };
            AF.Groups.Add(GetGroup(pht, "5"));
            pht.Modules.Add(AF);
            _db.CurriculumModules.Add(AF);

            var SPM = new CurriculumModule()
            {
                ShortName = "SPM",
                Name = "Simulation Produktion und Materialfluss  ",
                ModuleId = "PNB 590",
                Description = "IT/Informationsmanagement ",
                ECTS = 5,
                MV = GetHost(fk06, "LIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Simulation Produktion und Materialfluss "
                    },
                }

            };
            SPM.Groups.Add(GetGroup(pht, "5"));
            pht.Modules.Add(SPM);
            _db.CurriculumModules.Add(SPM);

            var QS = new CurriculumModule()
            {
                ShortName = "QS",
                Name = "Qualitätsmanagement und Statistik ",
                ModuleId = "PNB 620",
                Description = "Produktionstechnik, Automatisierungstechnik",
                ECTS = 6,
                MV = GetHost(fk06, "SAC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Qualitätsmanagement und Statistik "
                    },
                }

            };
            QS.Groups.Add(GetGroup(pht, "6"));
            pht.Modules.Add(QS);
            _db.CurriculumModules.Add(QS);

            var EP = new CurriculumModule()
            {
                ShortName = "EP",
                Name = "Engineering Project ",
                ModuleId = "PNB 630",
                Description = "Grundlagen der Natur- und Ingenieurwissenschaften Produktionstechnik Soft Skills",
                ECTS = 6,
                MV = GetHost(fk06, "SLU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Engineering Project "
                    },
                }

            };
            EP.Groups.Add(GetGroup(pht, "6"));
            pht.Modules.Add(EP);
            _db.CurriculumModules.Add(EP);

            var BA = new CurriculumModule()
            {
                ShortName = "BA",
                Name = "Bachelorarbeit",
                ModuleId = "PNB 710",
                Description = "Abschlussarbeit",
                ECTS = 12,
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
            BA.Groups.Add(GetGroup(pht, "7"));
            pht.Modules.Add(BA);
            _db.CurriculumModules.Add(BA);

            var HR = new CurriculumModule()
            {
                ShortName = "HR",
                Name = "Handhabungstechnik/Robotik I ",
                ModuleId = "PNB 720",
                Description = "Produktionstechnik, BWL",
                ECTS = 6,
                MV = GetHost(fk06, "LIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Handhabungstechnik/Robotik I  "
                    },
                }

            };
            HR.Groups.Add(GetGroup(pht, "7"));
            pht.Modules.Add(HR);
            _db.CurriculumModules.Add(HR);

            var AP = new CurriculumModule()
            {
                ShortName = "AP",
                Name = "Arbeitssystem- und Prozessgestaltung",
                ModuleId = "PNB 730",
                Description = "Produktionstechnik, Automatisierungstechnik, SoftSkills",
                ECTS = 4,
                MV = GetHost(fk06, "FIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Arbeitssystem- und Prozessgestaltung"
                    },
                }

            };
            AP.Groups.Add(GetGroup(pht, "7"));
            pht.Modules.Add(AP);
            _db.CurriculumModules.Add(AP);

            var PL = new CurriculumModule()
            {
                ShortName = "PL",
                Name = "Produktionsplanung und -steuerung / Logistik ",
                ModuleId = "PNB 740",
                Description = "Produktionstechnik, BWL",
                ECTS = 4,
                MV = GetHost(fk06, "ZAN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Produktionsplanung und -steuerung / Logistik "
                    },
                }

            };
            PL.Groups.Add(GetGroup(pht, "7"));
            pht.Modules.Add(PL);
            _db.CurriculumModules.Add(PL);

            _db.SaveChanges();
        }

    }
}



















