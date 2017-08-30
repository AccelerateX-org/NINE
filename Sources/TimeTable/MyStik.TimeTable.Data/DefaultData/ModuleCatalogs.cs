using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void
            InitCatalogWI_GS(ActivityOrganiser fk09, Curriculum wi)
        {
            var mathe = new CurriculumModule()
            {
                ShortName = "Mathe 1",
                Name = "Mathematik 1",
                ModuleId = "G1",
                Description = "Rechnen",
                ECTS = 6,
                MV = GetHost(fk09, "STU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Mathe1 Vorl"
                    },
                    new ModuleCourse
                    {
                        CourseType = CourseType.Practice,
                        ExternalId = "MatheÜB"
                    },
                }
                    
            };

            mathe.Groups.Add(GetGroup(wi, "1"));
            wi.Modules.Add(mathe);
            _db.CurriculumModules.Add(mathe);

            var mathe2 = new CurriculumModule()
            {
                ShortName = "Mathe 2",
                Name = "Mathematik 2",
                ModuleId = "G2",
                Description = "mehr Rechnen",
                ECTS = 5,
                MV = GetHost(fk09, "STU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathe2"
                    },
                }
            };

            mathe2.Groups.Add(GetGroup(wi, "2"));
            wi.Modules.Add(mathe2);
            _db.CurriculumModules.Add(mathe2);

            var tm = new CurriculumModule()
            {
                ShortName = "TM",
                Name = "Technische Mechanik",
                ModuleId = "G3",
                Description = "Maschinen berechnen",
                ECTS = 5,
                MV = GetHost(fk09, "ANZ"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TM"
                    },
                }
            };

            tm.Groups.Add(GetGroup(wi, "1"));
            wi.Modules.Add(tm);
            _db.CurriculumModules.Add(tm);


            var physik = new CurriculumModule()
            {
                ShortName = "Physik",
                Name = "Physik",
                ModuleId = "G4",
                Description = "Mechanik, Thermo",
                ECTS = 5,
                MV = GetHost(fk09, "REB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik"
                    },
                }

            };

            physik.Groups.Add(GetGroup(wi, "2"));
            wi.Modules.Add(physik);
            _db.CurriculumModules.Add(physik);

            var cuw = new CurriculumModule()
            {
                ShortName = "Chemie/Wkst",
                Name = "Chemie und Werkstoffe",
                ModuleId = "G5",
                Description = "organ./anorgan. Chemie, Werkstoffgrundlagen",
                ECTS = 4,
                MV = GetHost(fk09, "RAB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Chemie"
                    },
                }

            };

            cuw.Groups.Add(GetGroup(wi, "1"));
            wi.Modules.Add(cuw);
            _db.CurriculumModules.Add(cuw);

            var wt = new CurriculumModule()
            {
                ShortName = "WT",
                Name = "Werkstofftechnik",
                ModuleId = "G6",
                Description = "Werkstoffprüfung und Eigenschaften",
                ECTS = 4,
                MV = GetHost(fk09, "RAB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Werkstoffe"
                    },
                }

            };

            wt.Groups.Add(GetGroup(wi, "2"));
            wi.Modules.Add(wt);
            _db.CurriculumModules.Add(wt);

            var et = new CurriculumModule()
            {
                ShortName = "E-Tech",
                Name = "Elektrotechnik",
                ModuleId = "G7",
                Description = "Gleich- und Wechselstromlehre",
                ECTS = 5,
                MV = GetHost(fk09, "PIR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ETech"
                    },
                }
            };

            et.Groups.Add(GetGroup(wi, "2"));
            wi.Modules.Add(et);
            _db.CurriculumModules.Add(et);

            var tz = new CurriculumModule()
            {
                ShortName = "TZ",
                Name = "Technisches Zeichnen",
                ModuleId = "G8",
                Description = "Grundlagen des technischen Zeichnens",
                ECTS = 4,
                MV = GetHost(fk09, "SCU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TZ"
                    },
                }

            };

            tz.Groups.Add(GetGroup(wi, "1"));
            wi.Modules.Add(tz);
            _db.CurriculumModules.Add(tz);

            var me = new CurriculumModule()
            {
                ShortName = "ME1",
                Name = "Maschinenelemente 1",
                ModuleId = "G9",
                Description = "Schraubenberechnung, Verfahrenstechnik",
                ECTS = 5,
                MV = GetHost(fk09, "DAE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ME1"
                    },
                }
            };

            me.Groups.Add(GetGroup(wi, "2"));
            wi.Modules.Add(me);
            _db.CurriculumModules.Add(me);

            var bwl = new CurriculumModule()
            {
                ShortName = "BWL",
                Name = "Betriebswirtschaftslehre",
                ModuleId = "G10",
                Description = "Grundlagen der BWL",
                ECTS = 4,
                MV = GetHost(fk09, "SAC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BWL"
                    },
                }

            };

            bwl.Groups.Add(GetGroup(wi, "1"));
            wi.Modules.Add(bwl);
            _db.CurriculumModules.Add(bwl);

            var buBi = new CurriculumModule()
            {
                ShortName = "BuBi",
                Name = "Buchhaltung und Bilanzierung",
                ModuleId = "G11",
                Description = "Bilanzierungsgrundlagen",
                ECTS = 4,
                MV = GetHost(fk09, "ENB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BuBi"
                    },
                }
            };

            buBi.Groups.Add(GetGroup(wi, "2"));
            wi.Modules.Add(buBi);
            _db.CurriculumModules.Add(buBi);

            var vwl = new CurriculumModule()
            {
                ShortName = "VWL",
                Name = "Volkswirtschaftslehre",
                ModuleId = "G13",
                Description = "Mikro- und Makroökonomische Zusammenhänge",
                ECTS = 4,
                MV = GetHost(fk09, "WOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "VWL"
                    },
                }
            };

            vwl.Groups.Add(GetGroup(wi, "2"));
            wi.Modules.Add(vwl);
            _db.CurriculumModules.Add(vwl);


            var gdi = new CurriculumModule()
            {
                ShortName = "GdI",
                Name = "Grundlagen der Informatik",
                ModuleId = "G12",
                Description = "Programmieren lernen",
                ECTS = 5,
                MV = GetHost(fk09, "HIN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Inform"
                    },
                }
            };

            gdi.Groups.Add(GetGroup(wi, "1"));
            wi.Modules.Add(gdi);
            _db.CurriculumModules.Add(gdi);
        }

        public void InitCatalogWI_Wahl(ActivityOrganiser fk09, Curriculum wi)
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
