using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogPHT_GS(ActivityOrganiser fk06, Curriculum pht)
        {
            var Phy1 = new CurriculumModule()
            {
                ShortName = "Phy 1",
                Name = "Physik 1",
                ModuleId = "PHB110",
                Description = "Fundamentale physikalische und physikalisch-technische Sachverhalte zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 6,
                MV = GetHost(fk06, "GAN"),
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

            var Chemie1 = new CurriculumModule()
            {
                ShortName = "Chemie1",
                Name = "Chemie1",
                ModuleId = "PHB120.1",
                Description = "Fundamentale naturwissenschaftliche Sachverhalte zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 4,
                MV = GetHost(fk06, "DIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Chemie1"
                    },
                }
            };

            Chemie1.Groups.Add(GetGroup(pht, "1"));
            pht.Modules.Add(Chemie1);
            _db.CurriculumModules.Add(Chemie1);

            var chemie2 = new CurriculumModule()
            {
                ShortName = "Chemie2",
                Name = "Chemie2",
                ModuleId = "PHB120.2",
                Description = "Erwerb von Grundkenntnissen im Fach Organische Chemie und im Umgang mit Chemikalien und chemischen Laborgeräten",
                ECTS = 2,
                MV = GetHost(fk06, "DIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Chemie2"
                    },
                }
            };

            chemie2.Groups.Add(GetGroup(pht, "2"));
            pht.Modules.Add(chemie2);
            _db.CurriculumModules.Add(chemie2);


            var Mathe1 = new CurriculumModule()
            {
                ShortName = "Mathe1",
                Name = "Mathematik 1",
                ModuleId = "PHB130",
                Description = "Fundamentale naturwissenschaftliche Sachverhalte zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 8,
                MV = GetHost(fk06, "SAC"),
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

            var Mathe2 = new CurriculumModule()
            {
                ShortName = "Mathe2",
                Name = "Mathematik 2",
                ModuleId = "PHB230",
                Description = "Fundamentale naturwissenschaftliche Sachverhalte zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 4,
                MV = GetHost(fk06, "SAC"),
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

            var tm = new CurriculumModule()
            {
                ShortName = "TM",
                Name = "Technische Mechanik",
                ModuleId = "PHB260",
                Description = "Maschinen berechnen",
                ECTS = 5,
                MV = GetHost(fk06, "NIE"),
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

            var ET1 = new CurriculumModule()
            {
                ShortName = "ET1",
                Name = "Elektrotechnik 1",
                ModuleId = "PHB140.1",
                Description = "	Fundamentale elektrotechnische Sachverhalte bezüglich physikalisch-technischer Geräte und Systeme zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 0,
                MV = GetHost(fk06, "MEN"),
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

            var ET2 = new CurriculumModule()
            {
                ShortName = "ET2",
                Name = "Elektrotechnik 2",
                ModuleId = "PHB140.1",
                Description = "Aktives, d.h. zum Entwurf befähigendes Verständnis passiver Schaltungen bis Filter 1. Ordnung und fundamentale elektrotechnische Sachverhalte bezüglich physikalisch - technischer Geräte und Systeme zu verstehen,zu beschreiben und anzuwenden",
                ECTS = 4,
                MV = GetHost(fk06, "MEN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ET2"
                    },
                }
            };

            ET2.Groups.Add(GetGroup(pht, "2"));
            pht.Modules.Add(ET2);
            _db.CurriculumModules.Add(ET2);


         var Info1 = new CurriculumModule()
            {
                ShortName = "Info1",
                Name = "Informatik 1",
                ModuleId = "PHB150",
                Description = "Besitz eines grundlegenden Verständnisses über den Aufbau und die Funktion moderner Computer und Betriebssysteme ",
                ECTS = 4,
                MV = GetHost(fk06, "BRA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Informatik 1"
                    },
                }

            };

            Info1.Groups.Add(GetGroup(pht, "1"));
            pht.Modules.Add(Info1);
            _db.CurriculumModules.Add(Info1);

            var Info2 = new CurriculumModule()
            {
                ShortName = "Info2",
                Name = "Informatik 2",
                ModuleId = "PHB250",
                Description = "Grundlegende Konzepte der datenflussorientierten Programmierung verstehen und anwenden Analysieren von Aufgabenstellungen der computer - basierten Messtechnik und Umsetzung in LabVIEW - Programmen ",
                ECTS = 4,
                MV = GetHost(fk06, "BRA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Informatik 2"
                    },
                }

            };

            Info2.Groups.Add(GetGroup(pht, "2"));
            pht.Modules.Add(Info2);
            _db.CurriculumModules.Add(Info2);

            var CAD = new CurriculumModule()
            {
                ShortName = "CAD",
                Name = "Konstruktion/CAD",
                ModuleId = "PHB160",
                Description = "Das Fach vermittelt die Grundlagen der rechnergestützen Konstruktion Mithilfe von Computer Aided Design Systemen und dessen Methoden. Des Weiteren sollen 3D und 2D Geometrien erzeugt und gestaltet werden. ",
                ECTS = 6,
                MV = GetHost(fk06, "SAL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Konstruktion/CAD"
                    },
                }

            };

            CAD.Groups.Add(GetGroup(pht, "2"));
            pht.Modules.Add(CAD);
            _db.CurriculumModules.Add(CAD);

            var Physik2 = new CurriculumModule()
            {
                ShortName = "PHY2",
                Name = "Physik 2",
                ModuleId = "PHB210",
                Description = "Fundamentale physikalische Sachverhalte zu verstehen, zu beschreiben und anzuwenden Grundlagen von Wärmelehre, Elektrizität und Magnetismus",
                ECTS = 6,
                MV = GetHost(fk06, "GUN"),
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


            var Physikpraktikum = new CurriculumModule()
            {
                ShortName = "PHY-P",
                Name = "Physikpraktikum",
                ModuleId = "PHB220",
                Description = "Fundamentale physikalisch-messtechnische Sachverhalte zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 4,
                MV = GetHost(fk06, "GUN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physikpraktikum"
                    },
                }

            };
            Physikpraktikum.Groups.Add(GetGroup(pht, "2"));
            pht.Modules.Add(Physikpraktikum);
            _db.CurriculumModules.Add(Physikpraktikum);

            var WT1 = new CurriculumModule()
            {
                ShortName = "WT1",
                Name = "Werkstofftechnik 1",
                ModuleId = "PHB270",
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
            WT1.Groups.Add(GetGroup(pht, "2"));
            pht.Modules.Add(WT1);
            _db.CurriculumModules.Add(WT1);

            var PHY3 = new CurriculumModule()
            {
                ShortName = "PHY3",
                Name = "Physik 3",
                ModuleId = "PHB310",
                Description = "	Fundamentale physikalisch-technische Sachverhalte zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 6,
                MV = GetHost(fk06, "GUN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik 3"
                    },
                }

            };
            PHY3.Groups.Add(GetGroup(pht, "3"));
            pht.Modules.Add(PHY3);
            _db.CurriculumModules.Add(PHY3);


            var PC = new CurriculumModule()
            {
                ShortName = "PC",
                Name = "Physikalische Chemie",
                ModuleId = "PHB320",
                Description = "	Am Beispiel der Thermodynamik wird ein zweiter Ansatz der Naturwissenschaft gelehrt, um phänomenologische Sachverhalte auf der Basis von Axiomen, hier Hauptsätze genannt, zu verstehen.",
                ECTS = 5,
                MV = GetHost(fk06, "FRA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Phsikalische Chemie"
                    },
                }

            };
            PC.Groups.Add(GetGroup(pht, "3"));
            pht.Modules.Add(PC);
            _db.CurriculumModules.Add(PC);

            var MA3 = new CurriculumModule()
            {
                ShortName = "MA3",
                Name = "Mathematik 3",
                ModuleId = "PHB 330.1",
                Description = "	Fähigkeit, die Verfahren aus den genannten Gebieten als Hilfsmittel bei der Lösung technischer und physikalischer Probleme einzusetzen",
                ECTS = 0,
                MV = GetHost(fk06, "SAC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathematik 3"
                    },
                }

            };
            MA3.Groups.Add(GetGroup(pht, "3"));
            pht.Modules.Add(MA3);
            _db.CurriculumModules.Add(MA3);

            var MA4 = new CurriculumModule()
            {
                ShortName = "MA4",
                Name = "Mathematik 4",
                ModuleId = "PHB 330.2",
                Description = "Fähigkeit, Integraltransformationen und Grundkenntnisse der Stochastik als Hilfsmittel bei der Lösung technischer und physikalischer Probleme einzusetzen",
                ECTS = 6,
                MV = GetHost(fk06, "SAC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathematik 4"
                    },
                }

            };
            MA4.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(MA4);
            _db.CurriculumModules.Add(MA4);

            var ET_a = new CurriculumModule()
            {
                ShortName = "ET_a",
                Name = "Elektronik analog",
                ModuleId = "PHB 340",
                Description = "Fundamentale elektronische Sachverhalte für physikalisch-technische Geräte und System zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 4,
                MV = GetHost(fk06, "MEN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Elektronik analog"
                    },
                }

            };
            ET_a.Groups.Add(GetGroup(pht, "3"));
            pht.Modules.Add(ET_a);
            _db.CurriculumModules.Add(ET_a);

            var MT1 = new CurriculumModule()
            {
                ShortName = "MT1",
                Name = "Messtechnik 1",
                ModuleId = "PHB 350",
                Description = "Grundlegende Prinzipien, Verfahren und Geräte der Messtechnik, insbesondere der Sensorik, zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 4,
                MV = GetHost(fk06, "HEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Messtechnik 1"
                    },
                }

            };
            MT1.Groups.Add(GetGroup(pht, "3"));
            pht.Modules.Add(MT1);
            _db.CurriculumModules.Add(MT1);

            var WT2 = new CurriculumModule()
            {
                ShortName = "WT2",
                Name = "Werkstofftechnik 2",
                ModuleId = "PHB370",
                Description = "Die Studierenden vertiefen das Grundlagenwissen in speziell für die physikalische Technik ausgewählten Kapiteln der Werkstofftechnik",
                ECTS = 4,
                MV = GetHost(fk06, "KOC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wertstofftechnik 2"
                    },
                }

            };
            WT2.Groups.Add(GetGroup(pht, "3"));
            pht.Modules.Add(WT2);
            _db.CurriculumModules.Add(WT2);

            var PHY4 = new CurriculumModule()
            {
                ShortName = "PHY4",
                Name = "Physik 4",
                ModuleId = "PHB410",
                Description = "Fundamentale physikalische Sachverhalte zu verstehen, zu beschreiben und anzuwenden Verständnis grundlegender akustischer Größen und deren Anwendungen.  ",
                ECTS = 4,
                MV = GetHost(fk06, "HER"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik 4"
                    },
                }

            };
            PHY4.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(PHY4);
            _db.CurriculumModules.Add(PHY4);

            var AP = new CurriculumModule()
            {
                ShortName = "AP",
                Name = "Atomphysik",
                ModuleId = "PHB420",
                Description = "Fundamentale Sachverhalte der modernen Physik zu verstehen, zu beschreiben und anzuwenden Grundlagen für die Module Festkörperphysik und Kernphysik / Strahlenschutz",
                ECTS = 5,
                MV = GetHost(fk06, "GER"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Atomphysik"
                    },
                }

            };
            AP.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(AP);
            _db.CurriculumModules.Add(AP);

            var ET_d = new CurriculumModule()
            {
                ShortName = "ET_d",
                Name = "Elektronik digital",
                ModuleId = "PHB440",
                Description = "Fundamentale digital-elektronische Sachverhalte zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 4,
                MV = GetHost(fk06, "PAR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Elektronik digital"
                    },
                }

            };
            ET_d.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(ET_d);
            _db.CurriculumModules.Add(ET_d);

            var MT2 = new CurriculumModule()
            {
                ShortName = "MT2",
                Name = "Messtechnik 2",
                ModuleId = "PHB450",
                Description = "Grundlegende Prinzipien, Verfahren und Geräte zur Gewinnung, Übertragung, Speicherung, Verarbeitung und Darstellung von Messdaten und Signalen zu verstehen, zu beschreiben und anzuwenden",
                ECTS = 4,
                MV = GetHost(fk06, "HEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Messtechnik 2"
                    },
                }

            };
            MT2.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(MT2);
            _db.CurriculumModules.Add(MT2);

            var TO = new CurriculumModule()
            {
                ShortName = "TO",
                Name = "Technische Optik",
                ModuleId = "PHB460",
                Description = "Fundamentale optische Sachverhalte und deren Anwendung in physikalisch-technischen Geräten zu verstehen, zu beschreiben und anzuwenden ",
                ECTS = 4,
                MV = GetHost(fk06, "WON"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Technische Optik"
                    },
                }

            };
            TO.Groups.Add(GetGroup(pht, "4"));
            pht.Modules.Add(TO);
            _db.CurriculumModules.Add(TO);

            var RT = new CurriculumModule()
            {
                ShortName = "RT",
                Name = "Regelungstechnik",
                ModuleId = "PHB470",
                Description = "Fundamentale ingenieurtechnische Sachverhalte zu verstehen, zu beschreiben und anzuwenden",
                ECTS = 4,
                MV = GetHost(fk06, "STE"),
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

            var IP = new CurriculumModule()
            {
                ShortName = "IP",
                Name = "Industriepraktikum",
                ModuleId = "PHB510",
                Description = "die Arbeitsmethodik des Physik-Ingenieurs im betrieblichen Umfeld anhand konkreter Aufgabenstellungen",
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
            IP.Groups.Add(GetGroup(pht, "5"));
            pht.Modules.Add(IP);
            _db.CurriculumModules.Add(IP);

            var PS = new CurriculumModule()
            {
                ShortName = "PS",
                Name = "Praxisseminar",
                ModuleId = "PHB520",
                Description = "Ausbildung, in der soziale Kompetenzen im Kontext mit einer Ingenieurtätigkeit vermittelt und erworben werden.",
                ECTS = 4,
                MV = GetHost(fk06, "MAN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Praxisseminar"
                    },
                }

            };
            PS.Groups.Add(GetGroup(pht, "5"));
            pht.Modules.Add(PS);
            _db.CurriculumModules.Add(PS);

            var BWL = new CurriculumModule()
            {
                ShortName = "BWL",
                Name = "Betriebswirtschaftliche Grundlagen",
                ModuleId = "PHB530",
                Description = "Dieses Modul vermittelt grundlegende fachübergreifende, insbesondere betriebswirtschaftliche Kenntnisse",
                ECTS = 4,
                MV = GetHost(fk06, "ZAN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Betriebswirtschaftliche Grundlagen"
                    },
                }

            };
            BWL.Groups.Add(GetGroup(pht, "5"));
            pht.Modules.Add(BWL);
            _db.CurriculumModules.Add(BWL);

            var FP = new CurriculumModule()
            {
                ShortName = "FP",
                Name = "Festkörperphysik",
                ModuleId = "PHB610",
                Description = "Fundamentale physikalische Sachverhalte zu verstehen, zu beschreiben und anzuwenden Verständnis grundlegender Eigenschaften von Festkörpern",
                ECTS = 5,
                MV = GetHost(fk06, "ALT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Festkörperphysik"
                    },
                }

            };
            FP.Groups.Add(GetGroup(pht, "6"));
            pht.Modules.Add(FP);
            _db.CurriculumModules.Add(FP);

            var NM = new CurriculumModule()
            {
                ShortName = "NM",
                Name = "Nukleare Messtechnik/Strahlenschutz",
                ModuleId = "PHB620",
                Description = "Verständnis der Grundlagen der angewandten Kernphysik sowie der Wechselwirkung, Radiometrie und Spektrometrie ionisierender Wellen-, Teilchen- und Neutronenstrahlung. ",
                ECTS = 5,
                MV = GetHost(fk06, "SWA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Nukleare Messtechnik/Strahlenschutz"
                    },
                }

            };
            NM.Groups.Add(GetGroup(pht, "6"));
            pht.Modules.Add(NM);
            _db.CurriculumModules.Add(NM);

            var LT = new CurriculumModule()
            {
                ShortName = "LT",
                Name = "Lasertechnik",
                ModuleId = "PHB630",
                Description = "Einarbeitung in ein Spezialgebiet der physikalischen Technik",
                ECTS = 5,
                MV = GetHost(fk06, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Lasertechnik"
                    },
                }

            };
            LT.Groups.Add(GetGroup(pht, "6"));
            pht.Modules.Add(LT);
            _db.CurriculumModules.Add(LT);

            var VK = new CurriculumModule()
            {
                ShortName = "VK",
                Name = "Vakuum-/Kryotechnik",
                ModuleId = "PHB640",
                Description = "Einarbeitung in ein Spezialgebiet der physikalischen Technik",
                ECTS = 5,
                MV = GetHost(fk06, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Vakuum-/Kryotechnik"
                    },
                }

            };
            VK.Groups.Add(GetGroup(pht, "6"));
            pht.Modules.Add(VK);
            _db.CurriculumModules.Add(VK);

            var AE = new CurriculumModule()
            {
                ShortName = "AE",
                Name = "Angewandte Elektronik",
                ModuleId = "PHB650",
                Description = "Einarbeitung in ein Spezialgebiet der physikalischen Technik",
                ECTS = 5,
                MV = GetHost(fk06, "MEN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Angewandte Elektronik"
                    },
                }

            };
            AE.Groups.Add(GetGroup(pht, "6"));
            pht.Modules.Add(AE);
            _db.CurriculumModules.Add(AE);

            var RE = new CurriculumModule()
            {
                ShortName = "RE",
                Name = "Regenerative Energietechnik",
                ModuleId = "PHB660",
                Description = "Einarbeitung in ein Spezialgebiet der physikalischen Technik",
                ECTS = 5,
                MV = GetHost(fk06, "WON"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Regenerative Energietechnik"
                    },
                }

            };
            RE.Groups.Add(GetGroup(pht, "6"));
            pht.Modules.Add(RE);
            _db.CurriculumModules.Add(RE);

            var BA = new CurriculumModule()
            {
                ShortName = "BA",
                Name = "Bachelorarbeit",
                ModuleId = "PHB700",
                Description = "Durchführen einer größeren Projektarbeit, Selbständigkeit, fachübergreifendes Anwenden des im Studium erlernten Stoffes nachweisen.",
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

            var OE = new CurriculumModule()
            {
                ShortName = "OE",
                Name = "Optelektronik",
                ModuleId = "PHB730",
                Description = "Einarbeitung in ein Spezialgebiet der physikalischen Technik (Wahlfach - nur in Verbindung mit Lasertechnik) ",
                ECTS = 5,
                MV = GetHost(fk06, "ROT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Optelektronik"
                    },
                }

            };
            OE.Groups.Add(GetGroup(pht, "7"));
            pht.Modules.Add(OE);
            _db.CurriculumModules.Add(OE);

            var HD = new CurriculumModule()
            {
                ShortName = "HD",
                Name = " 	Halbleiter-/ Dünnschichttechnik",
                ModuleId = "PHB740",
                Description = "	Bezogen auf die übergeordneten Lernziele des Studiengangs vermittelt dieses Modul vertiefende ingenieurswissenschaftliche Kenntnisse der physikalischen Technik",
                ECTS = 5,
                MV = GetHost(fk06, "ROT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Halbleiter-/ Dünnschichttechnik"
                    },
                }

            };
            HD.Groups.Add(GetGroup(pht, "7"));
            pht.Modules.Add(HD);
            _db.CurriculumModules.Add(HD);

            var SA = new CurriculumModule()
            {
                ShortName = "SA",
                Name = "Sensorik/Aktorik",
                ModuleId = "PHB750",
                Description = "Bezogen auf die übergeordneten Lernziele des Studiengangs vermittelt dieses Modul vertiefende ingenieurswissenschaftliche Kenntnisse der physikalischen Technik mit dem Schwerpunkt auf Mikrosensoren und -aktuatoren.",
                ECTS = 5,
                MV = GetHost(fk06, "SIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Sensorik/Aktorik"
                    },
                }

            };
            SA.Groups.Add(GetGroup(pht, "6"));
            pht.Modules.Add(SA);
            _db.CurriculumModules.Add(SA);

            var SN = new CurriculumModule()
            {
                ShortName = "SN",
                Name = "Simulation/Numerische Physik",
                ModuleId = "PHB750",
                Description = "Einarbeitung in ein Spezialgebiet der physikalischen Technik (Wahlfach)Die Fähgikeit,realitätsnahe Probleme,deren Physik durch gewöhnliche oder partielle Differentialgleichungen beschrieben wird",
                ECTS = 5,
                MV = GetHost(fk06, "GER"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Simulation/Numerische Physik"
                    },
                }

            };
            SN.Groups.Add(GetGroup(pht, "7"));
            pht.Modules.Add(SN);
            _db.CurriculumModules.Add(SN);

            var TWP = new CurriculumModule()
            {
                ShortName = "TWP",
                Name = " 	Wahlpflichtmodul Technik",
                ModuleId = "PHB800",
                Description = "Einarbeitung in ein Spezialgebiet der physikalischen Technik",
                ECTS = 5,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = " 	Wahlpflichtmodul Technik"
                    },
                }

            };
            TWP.Groups.Add(GetGroup(pht, "7"));
            pht.Modules.Add(TWP);
            _db.CurriculumModules.Add(TWP);

            var UWP = new CurriculumModule()
            {
                ShortName = "UWP",
                Name = "Fachübergreifendes Wahlpflichtmodul",
                ModuleId = "PHB900",
                Description = "Einarbeitung in ein Spezialgebiet",
                ECTS = 5,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fachübergreifendes Wahlpflichtmodul"
                    },
                }

            };
            UWP.Groups.Add(GetGroup(pht, "7"));
            pht.Modules.Add(UWP);
            _db.CurriculumModules.Add(UWP);




            _db.SaveChanges();
        }

    }
}
