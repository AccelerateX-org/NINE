using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogBOB_GS(ActivityOrganiser fk06, Curriculum BOB)
        {
            var Chemie = new CurriculumModule()
            {
                ShortName = "Chemie 1",
                Name = "Chemie 1",
                ModuleId = "BOB110",
                Description = "Chemie",
                ECTS = 4,
                MV = GetHost(fk06, "DIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Chemie 1 "
                    }
                }

            };

            Chemie.Groups.Add(GetGroup(BOB, "1"));
            BOB.Modules.Add(Chemie);
            _db.CurriculumModules.Add(Chemie);
        



            var mathe1 = new CurriculumModule()
            {
                ShortName = "Mathe 1",
                Name = "Mathematik 1",
                ModuleId = "BOB130",
                Description = "Rechnen",
                ECTS = 4,
                MV = GetHost(fk06, "HIR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathe2"
                    },
                }
            };

            mathe1.Groups.Add(GetGroup(BOB, "1"));
            BOB.Modules.Add(mathe1);
            _db.CurriculumModules.Add(mathe1);



            var physik1 = new CurriculumModule()
            {
                ShortName = "Physik 1",
                Name = "Physik 1",
                ModuleId = "BOB120",
                Description = "Mechanik, Thermo",
                ECTS = 5,
                MV = GetHost(fk06, "CLS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik"
                    },
                }

            };

            physik1.Groups.Add(GetGroup(BOB, "1"));
            BOB.Modules.Add(physik1);
            _db.CurriculumModules.Add(physik1);




            var CAD = new CurriculumModule()
            {
                ShortName = "Konstruktion/CAD ",
                Name = "Konstruktion/CAD",
                ModuleId = "BOB140",
                Description = "Konstruiren am PC",
                ECTS = 5,
                MV = GetHost(fk06, "CLS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "CAD"
                    },
                }

            };

            CAD.Groups.Add(GetGroup(BOB, "1"));
            BOB.Modules.Add(CAD);
            _db.CurriculumModules.Add(CAD);

            var Informatik = new CurriculumModule()
            {
                ShortName = "Informatik",
                Name = "Informatik ",
                ModuleId = "BOB150",
                Description = "Programmieren am PC",
                ECTS = 6,
                MV = GetHost(fk06, "Brau"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Informatik"
                    },
                }

            };

            Informatik.Groups.Add(GetGroup(BOB, "1"));
            BOB.Modules.Add(Informatik);
            _db.CurriculumModules.Add(Informatik);

            var Biologie = new CurriculumModule()
            {
                ShortName = "Biologie",
                Name = "Biologie",
                ModuleId = "BOB160",
                Description = "Lehre der Stoffe",
                ECTS = 4,
                MV = GetHost(fk06, "Hill"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Biologie"
                    },
                }

            };

            Biologie.Groups.Add(GetGroup(BOB, "1"));
            BOB.Modules.Add(Biologie);
            _db.CurriculumModules.Add(Biologie);


            var physik2 = new CurriculumModule()
            {
                ShortName = "Physik 2",
                Name = "Physik 2",
                ModuleId = "BOB220",
                Description = "mehr Physik",
                ECTS = 5,
                MV = GetHost(fk06, "CLS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik2"
                    },
                }

            };

            physik1.Groups.Add(GetGroup(BOB, "2"));
            BOB.Modules.Add(physik2);
            _db.CurriculumModules.Add(physik2);

            var mathe2 = new CurriculumModule()
            {
                ShortName = "Mathe 2",
                Name = "Mathematik 2",
                ModuleId = "BOB230",
                Description = "mehr Rechnen",
                ECTS = 5,
                MV = GetHost(fk06, "HIR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathe2"
                    },
                }
            };

            mathe2.Groups.Add(GetGroup(BOB, "2"));
            BOB.Modules.Add(mathe2);
            _db.CurriculumModules.Add(mathe2);


            var Chemie2 = new CurriculumModule()
            {
                ShortName = "Chemie 2",
                Name = "Chemie 2",
                ModuleId = "BOB210",
                Description = "Chemie2",
                ECTS = 4,
                MV = GetHost(fk06, "DIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Chemie 2 "
                    }
                }

            };
            Chemie2.Groups.Add(GetGroup(BOB, "2"));
            BOB.Modules.Add(Chemie2);
            _db.CurriculumModules.Add(Chemie2);


            var TM = new CurriculumModule()
            {
                ShortName = "TM",
                Name = "Technische Mechanik",
                ModuleId = "BOB240",
                Description = "Statik und Dynamik",
                ECTS = 6,
                MV = GetHost(fk06, "Nie"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TM"
                    },
                }

            };

            TM.Groups.Add(GetGroup(BOB, "2"));
            BOB.Modules.Add(TM);
            _db.CurriculumModules.Add(TM);

            var ZellMikrobiologie = new CurriculumModule()
            {
                ShortName = "Zell- und Mikrobiologie",
                Name = "Zell- und Mikrobiologie",
                ModuleId = "BOB250",
                Description = "Zell- und Mikrobiologie",
                ECTS = 6,
                MV = GetHost(fk06, "Hill"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Zell- und Mikrobiologie"
                    },
                }

            };

            ZellMikrobiologie.Groups.Add(GetGroup(BOB, "1"));
            BOB.Modules.Add(ZellMikrobiologie);
            _db.CurriculumModules.Add(ZellMikrobiologie);

            var WerkstoffeBiomaterialien = new CurriculumModule()
            {
                ShortName = "Werkstoffe/Biomaterialien",
                Name = "Werkstoffe/Biomaterialien ",
                ModuleId = "BOB260",
                Description = "Werkstoffe/Biomaterialien ",
                ECTS = 4,
                MV = GetHost(fk06, "Stei"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Zell- und Mikrobiologie"
                    },
                }

            };

            ZellMikrobiologie.Groups.Add(GetGroup(BOB, "2"));
            BOB.Modules.Add(ZellMikrobiologie);
            _db.CurriculumModules.Add(ZellMikrobiologie);
        }


        public void InitCatalogBOB_Wahl(ActivityOrganiser fk09, Curriculum BOB)
        {

            // WPMs, AW und Abschlussarbeit
            var aw = new CurriculumModule()
            {
                ShortName = "AW",
                Name = "Allgemeinwissenschaften",
                ModuleId = "BOB100.1",
                Description = "",
                ECTS = 2,
            };

            // gehört zu allen Studiengruppen im 5. Semester
            aw.Groups.Add(GetGroup(BOB, "1"));

            BOB.Modules.Add(aw);
            _db.CurriculumModules.Add(aw);

            var aw2 = new CurriculumModule()
            {
                ShortName = "AW2",
                Name = "Allgemeinwissenschaften",
                ModuleId = "BOB100.2",
                Description = "",
                ECTS = 2,
            };
            aw2.Groups.Add(GetGroup(BOB, "4"));
            BOB.Modules.Add(aw2);
            _db.CurriculumModules.Add(aw2);

            var wpm1 = new CurriculumModule()
            {
                ShortName = "WPM1",
                Name = "Fachwissenschaftliches Wahlpflichtmodul I",
                ModuleId = "BOB640",
                Description = "",
                ECTS = 5,
            };

            wpm1.Groups.Add(GetGroup(BOB, "6"));
            BOB.Modules.Add(wpm1);
            _db.CurriculumModules.Add(wpm1);

            var wpm2 = new CurriculumModule()
            {
                ShortName = "WPM2",
                Name = "Fachwissenschaftliches Wahlpflichtmodul II",
                ModuleId = "BOB650",
                Description = "",
                ECTS = 5,
            };

            wpm2.Groups.Add(GetGroup(BOB, "6"));
            BOB.Modules.Add(wpm2);
            _db.CurriculumModules.Add(wpm2);

            var wpm3 = new CurriculumModule()
            {
                ShortName = "wpm3",
                Name = "Fachwissenschaftliches Wahlpflichtmodul III",
                ModuleId = "BOB730",
                Description = "",
                ECTS = 5,
            };

            wpm3.Groups.Add(GetGroup(BOB, "7"));
            BOB.Modules.Add(wpm3);
            _db.CurriculumModules.Add(wpm3);

            var wpm4 = new CurriculumModule()
            {
                ShortName = "wpm4",
                Name = "Fachwissenschaftliches Wahlpflichtmodul IV",
                ModuleId = "BOB740",
                Description = "",
                ECTS = 5,
            };

            wpm4.Groups.Add(GetGroup(BOB, "7"));
            BOB.Modules.Add(wpm4);
            _db.CurriculumModules.Add(wpm4);



            var UWP = new CurriculumModule()
            {
                ShortName = "UWP",
                Name = "Fachübergreifendes Wahlpflichtmodul (UWP) ",
                ModuleId = "W4",
                Description = "",
                ECTS = 5,
            };

            UWP.Groups.Add(GetGroup(BOB, "6"));
            BOB.Modules.Add(UWP);
            _db.CurriculumModules.Add(UWP);

            var prak = new CurriculumModule()
            {
                ShortName = "PRAK",
                Name = "Industriepraktikum",
                ModuleId = "BOB510",
                Description = "",
                ECTS = 22,
            };

            prak.Groups.Add(GetGroup(BOB, "5"));
            BOB.Modules.Add(prak);
            _db.CurriculumModules.Add(prak);



            var bac = new CurriculumModule()
            {
                ShortName = "BAC",
                Name = "Abschlussarbeit",
                ModuleId = "BOB700",
                Description = "",
                ECTS = 12,
            };

            bac.Groups.Add(GetGroup(BOB, "7"));
            BOB.Modules.Add(bac);
            _db.CurriculumModules.Add(bac);


            _db.SaveChanges();
        }

        public void InitCatalogBOB_ab3(ActivityOrganiser fk06, Curriculum BOB)
        {
            //3.SEMESTER

            var Humanbiologie = new CurriculumModule()
            {
                ShortName = "Humanbiologie",
                Name = "Humanbiologie",
                ModuleId = "BOB320",
                Description = "Erwerb von Grundkenntnissen der Terminologie, Anatomie und Pathophysiologie",
                ECTS = 5,
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

            Humanbiologie.Groups.Add(GetGroup(BOB, "3"));
            BOB.Modules.Add(Humanbiologie);
            _db.CurriculumModules.Add(Humanbiologie);
            _db.SaveChanges();


            var PhysChemie = new CurriculumModule()
            {
                ShortName = "PhysChemie",
                Name = "Physikalische Chemie",
                ModuleId = "BOB330",
                Description = "Kenntnis der molekularen und makroskopischen Eigenschaften der Materie",
                ECTS = 4,
                MV = GetHost(fk06, "DIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PhysChemie"
                    },
                }
            };

            PhysChemie.Groups.Add(GetGroup(BOB, "3"));
            BOB.Modules.Add(PhysChemie);
            _db.CurriculumModules.Add(PhysChemie);
            _db.SaveChanges();


            var ThermoFluid = new CurriculumModule()
            {
                ShortName = "ThermoFluid",
                Name = "Thermodynamik/Fluidmechanik",
                ModuleId = "BOB340",
                Description = "Überblick über verwendete Infosysteme",
                ECTS = 6,
                MV = GetHost(fk06, "ALT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ThermoFluid"
                    },
                }
            };
            ThermoFluid.Groups.Add(GetGroup(BOB, "3"));
            BOB.Modules.Add(ThermoFluid);
            _db.CurriculumModules.Add(ThermoFluid);
            _db.SaveChanges();

            var Statistik = new CurriculumModule()
            {
                ShortName = "Statistik",
                Name = "Statistik",
                ModuleId = "BOB350",
                Description = "Kenntnis der einschlägigen Begriffe und Methoden der Wahrscheinlichkeitsrechnung",
                ECTS = 4,
                MV = GetHost(fk06, "Sac"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Statistik"
                    },
                }
            };

            Statistik.Groups.Add(GetGroup(BOB, "3"));
            BOB.Modules.Add(Statistik);
            _db.CurriculumModules.Add(Statistik);
            _db.SaveChanges();

            var Elektronik = new CurriculumModule()
            {
                ShortName = "Elektronik",
                Name = "Elektronik",
                ModuleId = "BOB360",
                Description = "Vermittlung von Grundkenntnissen der Elektronik",
                ECTS = 4,
                MV = GetHost(fk06, "MAH"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Elektronik"
                    },
                }
            };

            Elektronik.Groups.Add(GetGroup(BOB, "3"));
            BOB.Modules.Add(Elektronik);
            _db.CurriculumModules.Add(Elektronik);
            _db.SaveChanges();

            var Biochemie = new CurriculumModule()
            {
                ShortName = "Biochemie",
                Name = "Biochemie",
                ModuleId = "BOB370",
                Description = "enntnis und Verständnis der biochemischen Prozesse in lebenden Organismen",
                ECTS = 7,
                MV = GetHost(fk06, "DIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Biochemie"
                    },
                }
            };
            Biochemie.Groups.Add(GetGroup(BOB, "3"));
            BOB.Modules.Add(Biochemie);
            _db.CurriculumModules.Add(Biochemie);
            _db.SaveChanges();
            //4.SEMESTER
            var InstruAnalytik = new CurriculumModule()
            {
                ShortName = "InstruAnalytik",
                Name = "Instrumentelle Analytik ",
                ModuleId = "BOB410",
                Description = "Grundlegende Kenntnisse der modernen Verfahren in der instrumentellen Analytik",
                ECTS = 5,
                MV = GetHost(fk06, "MAI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "InstruAnalytik"
                    },
                }
            };

            InstruAnalytik.Groups.Add(GetGroup(BOB, "4"));
            BOB.Modules.Add(InstruAnalytik);
            _db.CurriculumModules.Add(InstruAnalytik);
            _db.SaveChanges();


            var Regel = new CurriculumModule()
            {
                ShortName = "Regel",
                Name = "Regelungstechnik/Simulation ",
                ModuleId = "BOB420",
                Description = "Erkennen, wie mit Steuerungen und Regelungen Vorgänge in realen Systemen gezielt beeinflusst werden können",
                ECTS = 7,
                MV = GetHost(fk06, "STE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Regel"
                    },
                }
            };

            Regel.Groups.Add(GetGroup(BOB, "4"));
            BOB.Modules.Add(Regel);
            _db.CurriculumModules.Add(Regel);
            _db.SaveChanges();


            var Messtechnik = new CurriculumModule()
            {
                ShortName = "Messtechnik",
                Name = "Messtechnik",
                ModuleId = "BOB430",
                Description = "Überblick über verwendete Infosysteme",
                ECTS = 4,
                MV = GetHost(fk06, "QU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Messtechnik"
                    },
                }
            };
            Messtechnik.Groups.Add(GetGroup(BOB, "4"));
            BOB.Modules.Add(Messtechnik);
            _db.CurriculumModules.Add(Messtechnik);
            _db.SaveChanges();

            var Biophysik = new CurriculumModule()
            {
                ShortName = "Biophysik",
                Name = "Biophysik",
                ModuleId = "BOB440",
                Description = "Verständnis grundlegender biophysikalischer Prinzipien",
                ECTS = 4,
                MV = GetHost(fk06, "CLS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Biophysik"
                    },
                }
            };

            Biophysik.Groups.Add(GetGroup(BOB, "4"));
            BOB.Modules.Add(Biophysik);
            _db.CurriculumModules.Add(Biophysik);
            _db.SaveChanges();

            var Apparatetechnik = new CurriculumModule()
            {
                ShortName = "Apparatetechnik",
                Name = "Apparatetechnik",
                ModuleId = "BOB450",
                Description = "Kenntnis von Apparatebauteilen in der angewandten Biotechnologie und deren Verwendung",
                ECTS = 4,
                MV = GetHost(fk06, "HER"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Apparatetechnik"
                    },
                }
            };

            Apparatetechnik.Groups.Add(GetGroup(BOB, "4"));
            BOB.Modules.Add(Apparatetechnik);
            _db.CurriculumModules.Add(Apparatetechnik);
            _db.SaveChanges();

            var Gerätetechnik = new CurriculumModule()
            {
                ShortName = "Gerätetechnik",
                Name = "Gerätetechnik",
                ModuleId = "BOB460",
                Description = "Verständnis der Funktionsweise moderner Laborgeräte",
                ECTS = 4,
                MV = GetHost(fk06, "ALT"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Gerätetechnik"
                    },
                }
            };
            Gerätetechnik.Groups.Add(GetGroup(BOB, "4"));
            BOB.Modules.Add(Gerätetechnik);
            _db.CurriculumModules.Add(Gerätetechnik);
            _db.SaveChanges();
            //5.SEMESTER
            var Praxisseminar = new CurriculumModule()
            {
                ShortName = "Praxisseminar",
                Name = "Praxisseminar",
                ModuleId = "BOB520",
                Description = "Kenntnisse der Präsentationstechniken",
                ECTS = 2,
                MV = GetHost(fk06, "Men"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Praxisseminar"
                    },
                }
            };
            Praxisseminar.Groups.Add(GetGroup(BOB, "5"));
            BOB.Modules.Add(Praxisseminar);
            _db.CurriculumModules.Add(Praxisseminar);
            _db.SaveChanges();

            var BWL = new CurriculumModule()
            {
                ShortName = "BWL",
                Name = "Betriebswirtschaftliche Grundlagen ",
                ModuleId = "BOB530",
                Description = "betriebswirtschaftliche Kenntnisse",
                ECTS = 4,
                MV = GetHost(fk06, "ZAN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BWL"
                    },
                }
            };
            BWL.Groups.Add(GetGroup(BOB, "5"));
            BOB.Modules.Add(BWL);
            _db.CurriculumModules.Add(BWL);
            _db.SaveChanges();
            //6.SEMESTER
            var Proteinchemie = new CurriculumModule()
            {
                ShortName = "Proteinchemie ",
                Name = "Proteinchemie",
                ModuleId = "BOB610",
                Description = "Eigenschaften von Proteinen",
                ECTS = 5,
                MV = GetHost(fk06, "DIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Proteinchemie "
                    },
                }
            };
            Proteinchemie.Groups.Add(GetGroup(BOB, "6"));
            BOB.Modules.Add(Proteinchemie);
            _db.CurriculumModules.Add(Proteinchemie);
            _db.SaveChanges();

            var Gentechnik = new CurriculumModule()
            {
                ShortName = "Gentechnik  ",
                Name = "Gentechnik ",
                ModuleId = "BOB620",
                Description = "Prinzipien und Technologien zur gezielten genetischen Veränderung von Mikroorganismen",
                ECTS = 5,
                MV = GetHost(fk06, "TRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Gentechnik "
                    },
                }
            };
            Gentechnik.Groups.Add(GetGroup(BOB, "6"));
            BOB.Modules.Add(Gentechnik);
            _db.CurriculumModules.Add(Gentechnik);
            _db.SaveChanges();

            var PRBioverfahrenstechnik = new CurriculumModule()
            {
                ShortName = "PR. Bioverfahrenstechnik",
                Name = "Praktische Bioverfahrenstechnik ",
                ModuleId = "BOB630",
                Description = "Kenntnis der verfahrenstechnischen Parameter bei Bioreaktionen ",
                ECTS = 4,
                MV = GetHost(fk06, "GRÜ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bioverfahrenstechnik"
                    },
                }
            };
            PRBioverfahrenstechnik.Groups.Add(GetGroup(BOB, "6"));
            BOB.Modules.Add(PRBioverfahrenstechnik);
            _db.CurriculumModules.Add(PRBioverfahrenstechnik);
            _db.SaveChanges();
            //7.SEMESTER

            var Bioverfahrenstechnik = new CurriculumModule()
            {
                ShortName = "Bioverfahrenstechnik ",
                Name = "Bioverfahrenstechnik",
                ModuleId = "BOB710",
                Description = "die Funktion und Beeinflussung verfahrenstechnischer Apparate und Bioreaktionen kennen und verstehen",
                ECTS = 4,
                MV = GetHost(fk06, "DIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bioverfahrenstechnik "
                    },
                }
            };
            Bioverfahrenstechnik.Groups.Add(GetGroup(BOB, "7"));
            BOB.Modules.Add(Bioverfahrenstechnik);
            _db.CurriculumModules.Add(Bioverfahrenstechnik);
            _db.SaveChanges();

            var Qualitätsmanagement = new CurriculumModule()
            {
                ShortName = "Qualitätsmanagement  ",
                Name = "Qualitätsmanagement ",
                ModuleId = "BOB720",
                Description = "Kenntnisse über strategisches Qualitätsmanagement",
                ECTS = 4,
                MV = GetHost(fk06, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Qualitätsmanagement  "
                    },
                }
            };
            Qualitätsmanagement.Groups.Add(GetGroup(BOB, "7"));
            BOB.Modules.Add(Qualitätsmanagement);
            _db.CurriculumModules.Add(Qualitätsmanagement);
            _db.SaveChanges();
            //SP1
            var MicroNanotech = new CurriculumModule()
            {
                ShortName = "MicroNanotech ",
                Name = "Mikro- und Nanotechnologie ",
                ModuleId = "BOB800",
                Description = "grundlegender Phänomene und ausgewählter Prozesse der Halbleiterphysik ",
                ECTS = 5,
                MV = GetHost(fk06, "CLS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MicroNanotech "
                    },
                }
            };
            MicroNanotech.Groups.Add(GetGroup(BOB, "SP1"));
            BOB.Modules.Add(MicroNanotech);
            _db.CurriculumModules.Add(MicroNanotech);
            _db.SaveChanges();

            var Strahlenschutz = new CurriculumModule()
            {
                ShortName = "Strahlenschutz ",
                Name = "Strahlenschutz/Ionisierende Strahlung  ",
                ModuleId = "BOB805",
                Description = "Verständnis der Grundlagen der angewandten Kernphysik sowie der Wechselwirkung",
                ECTS = 5,
                MV = GetHost(fk06, "SCHW"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Strahlenschutz "
                    },
                }
            };
            Strahlenschutz.Groups.Add(GetGroup(BOB, "SP1"));
            BOB.Modules.Add(Strahlenschutz);
            _db.CurriculumModules.Add(Strahlenschutz);
            _db.SaveChanges();

            var Biomechanik = new CurriculumModule()
            {
                ShortName = "Biomechanik  ",
                Name = "Biomechanik",
                ModuleId = "BOB810",
                Description = "Fähigkeit, die Mechanik der Bewegungen von Mensch und Tier zu verstehen",
                ECTS = 5,
                MV = GetHost(fk06, "STEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Biomechanik "
                    },
                }
            };
            Biomechanik.Groups.Add(GetGroup(BOB, "SP1"));
            BOB.Modules.Add(Biomechanik);
            _db.CurriculumModules.Add(Biomechanik);
            _db.SaveChanges();

            var MedtechSysteme = new CurriculumModule()
            {
                ShortName = "MedtechSysteme",
                Name = "Medizinisch-technische Systeme",
                ModuleId = "BOB830",
                Description = "Verständnis medizintechnischer Systeme in ihren Möglichkeiten und Grenzen der Anwendung",
                ECTS = 5,
                MV = GetHost(fk06, "WON"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MedtechSysteme"
                    },
                }
            };
            MedtechSysteme.Groups.Add(GetGroup(BOB, "SP1"));
            BOB.Modules.Add(MedtechSysteme);
            _db.CurriculumModules.Add(MedtechSysteme);
            _db.SaveChanges();

            var Bioinformatik = new CurriculumModule()
            {
                ShortName = "Bioinformatik  ",
                Name = "Bioinformatik",
                ModuleId = "BOB840",
                Description = "Kenntnis ausgewählter aktueller Gegenstände der Bioinformatik mit Schwerpunkt auf Sequenzen",
                ECTS = 5,
                MV = GetHost(fk06, "GER"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bioinformatik"
                    },
                }
            };
            Bioinformatik.Groups.Add(GetGroup(BOB, "SP1"));
            BOB.Modules.Add(Bioinformatik);
            _db.CurriculumModules.Add(Bioinformatik);
            _db.SaveChanges();

            var Immunologie = new CurriculumModule()
            {
                ShortName = "Immunologie  ",
                Name = "Immunologie",
                ModuleId = "BOB850",
                Description = "Kenntnis und Verständnis der Entwicklung und der Funktion des menschlichen Immunsystems",
                ECTS = 5,
                MV = GetHost(fk06, "HILL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Immunologie "
                    },
                }
            };
            Immunologie.Groups.Add(GetGroup(BOB, "SP1"));
            BOB.Modules.Add(Immunologie);
            _db.CurriculumModules.Add(Immunologie);
            _db.SaveChanges();

            var Pharmakologie = new CurriculumModule()
            {
                ShortName = "Pharmakologie",
                Name = "Pharmakologie",
                ModuleId = "BOB860",
                Description = "Kenntnis und Verständnis der für das Gebiet Bioingenieurwesen wichtigen Grundlagen der Pharmakologie",
                ECTS = 5,
                MV = GetHost(fk06, "SCHW"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Pharmakologie"
                    },
                }
            };
            Pharmakologie.Groups.Add(GetGroup(BOB, "SP1"));
            BOB.Modules.Add(Pharmakologie);
            _db.CurriculumModules.Add(Pharmakologie);
            _db.SaveChanges();

            var DrugDiscovery = new CurriculumModule()
            {
                ShortName = "DrugDiscovery",
                Name = "Drug Discovery ",
                ModuleId = "BOB870",
                Description = "Grundlegendes Verständnis der Entwicklung pharmazeutischer Wirkstoffe auf molekularer Ebene",
                ECTS = 5,
                MV = GetHost(fk06, "DIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DrugDiscovery "
                    },
                }
            };
            DrugDiscovery.Groups.Add(GetGroup(BOB, "SP1"));
            BOB.Modules.Add(DrugDiscovery);
            _db.CurriculumModules.Add(DrugDiscovery);
            _db.SaveChanges();

            var ChemoBiosensorik = new CurriculumModule()
            {
                ShortName = "ChemoBiosensorik",
                Name = "Chemo- und Biosensorik",
                ModuleId = "BOB880",
                Description = "Kenntnis der für Anwendungen in Medizintechnik und Umweltanalytik einsetzbaren chemischen und biochemischen Sensorsysteme",
                ECTS = 5,
                MV = GetHost(fk06, "SCHW"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ChemoBiosensorik "
                    },
                }
            };
            ChemoBiosensorik.Groups.Add(GetGroup(BOB, "SP1"));
            BOB.Modules.Add(ChemoBiosensorik);
            _db.CurriculumModules.Add(ChemoBiosensorik);
            _db.SaveChanges();


            var SIUE = new CurriculumModule()
            {
                ShortName = "SIUE",
                Name = "Skelettale Implantate und Exoprothetik",
                ModuleId = "BOB890",
                Description = "Einarbeitung in das Gebiet der Life Sciences und ingenieurwissenschaftliche Anwendung der hierbei gewonnenen Erkenntnisse. ",
                ECTS = 5,
                MV = GetHost(fk06, "STEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "SIUE "
                    },
                }
            };
            SIUE.Groups.Add(GetGroup(BOB, "SP1"));
            BOB.Modules.Add(SIUE);
            _db.CurriculumModules.Add(SIUE);
            _db.SaveChanges();



            //SP2

            var Umweltchemie = new CurriculumModule()
            {
                ShortName = "Umweltchemie  ",
                Name = "Umweltchemie",
                ModuleId = "BOB910",
                Description = "Verständnis der Grundlagen der angewandten Kernphysik sowie der Wechselwirkung",
                ECTS = 5,
                MV = GetHost(fk06, "DIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Strahlenschutz "
                    },
                }
            };
            Umweltchemie.Groups.Add(GetGroup(BOB, "SP2"));
            BOB.Modules.Add(Umweltchemie);
            _db.CurriculumModules.Add(Umweltchemie);
            _db.SaveChanges();

            var AnalChemie = new CurriculumModule()
            {
                ShortName = "AnalChemie",
                Name = "Analytische Chemie",
                ModuleId = "BOB920",
                Description = "Kenntnisse chemischer Reaktionen zur qualitativen und quantitativen Bestimmung chemischer Stoffe",
                ECTS = 5,
                MV = GetHost(fk06, "VAS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "AnalChemie"
                    },
                }
            };
            AnalChemie.Groups.Add(GetGroup(BOB, "SP2"));
            BOB.Modules.Add(AnalChemie);
            _db.CurriculumModules.Add(AnalChemie);
            _db.SaveChanges();

            var TechnischerUmweltschutz = new CurriculumModule()
            {
                ShortName = "Technischer Umweltschutz ",
                Name = "Technischer Umweltschutz",
                ModuleId = "BOB930",
                Description = "Überblick und vertiefte Kenntnisse über die Auswirkungen menschlichen Handelns auf Wasser, Boden, Luft, Klima, Flora und Fauna",
                ECTS = 5,
                MV = GetHost(fk06, "DIM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischerUmweltschutz "
                    },
                }
            };
            TechnischerUmweltschutz.Groups.Add(GetGroup(BOB, "SP2"));
            BOB.Modules.Add(TechnischerUmweltschutz);
            _db.CurriculumModules.Add(TechnischerUmweltschutz);
            _db.SaveChanges();

            var TechÖkologie = new CurriculumModule()
            {
                ShortName = "TechÖkologie",
                Name = "Technische Ökologie",
                ModuleId = "BOB940",
                Description = "Die Studierenden kennen biologische und ökologische Grundlagen für das Lehrgebiet Technische Ökologie",
                ECTS = 5,
                MV = GetHost(fk06, "TRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Strahlenschutz "
                    },
                }
            };
            TechÖkologie.Groups.Add(GetGroup(BOB, "SP2"));
            BOB.Modules.Add(TechÖkologie);
            _db.CurriculumModules.Add(TechÖkologie);
            _db.SaveChanges();

            var RegEnergietechnik = new CurriculumModule()
            {
                ShortName = "Strahlenschutz ",
                Name = "Regenerative Energietechnik",
                ModuleId = "BOB950",
                Description = "Vertiefte Kenntnisse der Konzepte zum Einsatz regenerativer Energien im Bereich der Energie- und Wärmetechnik",
                ECTS = 5,
                MV = GetHost(fk06, "WON"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "RegEnergietechnik"
                    },
                }
            };
            RegEnergietechnik.Groups.Add(GetGroup(BOB, "SP2"));
            BOB.Modules.Add(RegEnergietechnik);
            _db.CurriculumModules.Add(RegEnergietechnik);
            _db.SaveChanges();

            var CheRaÖkotoxizität = new CurriculumModule()
            {
                ShortName = "CheRaÖkotoxizität",
                Name = "Chemo-, Radio-, Ökotoxizität ",
                ModuleId = "BOB960",
                Description = "Vermittlung von Grundlagen der ökologischen, pharmakologischen, Radio- und Gewerbetoxikologie im Hinblick auf Gesundheitsrisiken durch Chemikalien und Strahlung ",
                ECTS = 5,
                MV = GetHost(fk06, "SCHW"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "CheRaÖkotoxizität "
                    },
                }
            };
            CheRaÖkotoxizität.Groups.Add(GetGroup(BOB, "SP2"));
            BOB.Modules.Add(CheRaÖkotoxizität);
            _db.CurriculumModules.Add(CheRaÖkotoxizität);
            _db.SaveChanges();





            //UWP

            var Bioethik = new CurriculumModule()
            {
                ShortName = "Bioethik  ",
                Name = "Bioethik ",
                ModuleId = "BOB915",
                Description = "Fähigkeit, sich auf der Basis von Fakten eine ethisch reflektierte Meinung zu den moralisch strittigen Themen des eigenen Berufsfeldes zu bilden",
                ECTS = 5,
                MV = GetHost(fk06, "BRI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bioethik"
                    },
                }
            };
            Bioethik.Groups.Add(GetGroup(BOB, "UWP"));
            BOB.Modules.Add(Bioethik);
            _db.CurriculumModules.Add(Bioethik);
            _db.SaveChanges();

            var Bionik = new CurriculumModule()
            {
                ShortName = "Bionik",
                Name = "Bionik",
                ModuleId = "BO925",
                Description = "Fähigkeit, die Bedeutung der Ingenieurwissenschaften für das Verständnis von Bau und Funktion von Lebewesen zu erkennen ",
                ECTS = 5,
                MV = GetHost(fk06, "GEI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bionik"
                    },
                }
            };
            Bionik.Groups.Add(GetGroup(BOB, "UWP"));
            BOB.Modules.Add(Bionik);
            _db.CurriculumModules.Add(Bionik);
            _db.SaveChanges();

            var Ergonomie = new CurriculumModule()
            {
                ShortName = "Ergonomie  ",
                Name = "Ergonomie ",
                ModuleId = "BOB935",
                Description = "Verständnis der Grundlagen der angewandten Kernphysik sowie der Wechselwirkung",
                ECTS = 5,
                MV = GetHost(fk06, "LES"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ergonomie  "
                    },
                }
            };
            Ergonomie.Groups.Add(GetGroup(BOB, "UWP"));
            BOB.Modules.Add(Ergonomie);
            _db.CurriculumModules.Add(Ergonomie);
            _db.SaveChanges();

            var TechnEnglish = new CurriculumModule()
            {
                ShortName = "TechnEnglish ",
                Name = "Technical English ",
                ModuleId = "BOB945",
                Description = "Improvement of the knowledge of technical or scientific English",
                ECTS = 3,
                MV = GetHost(fk06, "SAR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechEnglish"
                    },
                }
            };
            TechnEnglish.Groups.Add(GetGroup(BOB, "UWP"));
            BOB.Modules.Add(TechnEnglish);
            _db.CurriculumModules.Add(TechnEnglish);
            _db.SaveChanges();
        }
    }
}
