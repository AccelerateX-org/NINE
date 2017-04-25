using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data.DefaultData
{
    public class InfrastructureData
    {
        public static void InitOrganisation(TimeTableDbContext db)
        {
            var orgas = new List<ActivityOrganiser>()
            {
                new ActivityOrganiser
                {
                    Name = "Fakultät 09",
                    ShortName = "FK 09",
                    IsFaculty = true,
                    IsStudent = false,
                },

                new ActivityOrganiser
                {
                    Name = "Fachschaft 09",
                    ShortName = "FS 09",
                    IsFaculty = false,
                    IsStudent = true,
                },

                new ActivityOrganiser
                {
                    Name = "Hochschule München",
                    ShortName = "HM",
                    IsFaculty = false,
                    IsStudent = false
                },
            };

            orgas.ForEach(c => db.Organisers.Add(c));

            db.SaveChanges();

        }


        public static void InitCurriculum(TimeTableDbContext db)
        {

            var fk09 = db.Organisers.SingleOrDefault(d => d.ShortName.Equals("FK 09"));
            var hm = db.Organisers.SingleOrDefault(d => d.ShortName.Equals("HM"));

            // Studienprogramme
            var curs = new List<Curriculum>()
            {
                new Curriculum
                {
                    ShortName = "WI", 
                    Name = "Bachelor Wirtschaftsingenieurwesen",
                    Organiser = fk09,
                    CurriculumGroups = new HashSet<CurriculumGroup>
                    {
                        new CurriculumGroup { Name = "1"},
                        new CurriculumGroup { Name = "2"},
                        new CurriculumGroup { Name = "3 TEC"},
                        new CurriculumGroup { Name = "3 BIO"},
                        new CurriculumGroup { Name = "3 INF"},
                        new CurriculumGroup { Name = "4 TEC"},
                        new CurriculumGroup { Name = "4 BIO"},
                        new CurriculumGroup { Name = "4 INF"},
                        new CurriculumGroup { Name = "5 TEC"},
                        new CurriculumGroup { Name = "5 BIO"},
                        new CurriculumGroup { Name = "5 INF"},
                        new CurriculumGroup { Name = "6 TEC"},
                        new CurriculumGroup { Name = "6 BIO"},
                        new CurriculumGroup { Name = "6 INF"},
                        new CurriculumGroup { Name = "7 TEC"},
                        new CurriculumGroup { Name = "7 BIO"},
                        new CurriculumGroup { Name = "7 INF"},
                        new CurriculumGroup { Name = "WPM"},
                    },
                },
                new Curriculum
                {
                    ShortName = "LM", 
                    Name = "Bachelor Wirtschaftsingenieurwesen Logistik",
                    Organiser = fk09,
                    CurriculumGroups = new HashSet<CurriculumGroup>
                    {
                        new CurriculumGroup { Name = "1"},
                        new CurriculumGroup { Name = "2"},
                        new CurriculumGroup { Name = "3"},
                        new CurriculumGroup { Name = "4"},
                        new CurriculumGroup { Name = "5"},
                        new CurriculumGroup { Name = "6"},
                        new CurriculumGroup { Name = "7"},
                        new CurriculumGroup { Name = "WPM"},
                    }
                },
                new Curriculum
                {
                    ShortName = "AU", 
                    Name = "Bachelor Wirtschaftsingenieurwesen Automobilindustrie",
                    Organiser = fk09,
                    CurriculumGroups = new HashSet<CurriculumGroup>
                    {
                        new CurriculumGroup { Name = "1"},
                        new CurriculumGroup { Name = "2"},
                        new CurriculumGroup { Name = "3"},
                        new CurriculumGroup { Name = "4"},
                        new CurriculumGroup { Name = "5"},
                        new CurriculumGroup { Name = "6"},
                        new CurriculumGroup { Name = "7"},
                        new CurriculumGroup { Name = "WPM"},
                    }
                },
                new Curriculum
                {
                    ShortName = "WIM", 
                    Name = "Master Wirtschaftsingenieurwesen",
                    Organiser = fk09,
                    CurriculumGroups = new HashSet<CurriculumGroup>
                    {
                        new CurriculumGroup { Name = "1"},
                        new CurriculumGroup { Name = "2"},
                        new CurriculumGroup { Name = "3"},
                        new CurriculumGroup { Name = "WPM"},
                    }

                },
                new Curriculum
                {
                    ShortName = "MBA", 
                    Name = "Master of Business Administration and Engineering",
                    Organiser = fk09,
                    CurriculumGroups = new HashSet<CurriculumGroup>
                    {
                        new CurriculumGroup { Name = "1 BAU"},
                        new CurriculumGroup { Name = "1 NW"},
                        new CurriculumGroup { Name = "1 WI"},
                        new CurriculumGroup { Name = "2 BAU"},
                        new CurriculumGroup { Name = "2 NW"},
                        new CurriculumGroup { Name = "2 WI"},
                        new CurriculumGroup { Name = "3"},
                        new CurriculumGroup { Name = "4 BAU"},
                        new CurriculumGroup { Name = "4 NW"},
                        new CurriculumGroup { Name = "4 WI"},
                        new CurriculumGroup { Name = "5"},
                        new CurriculumGroup { Name = "WPM"},
                    }
                },
                new Curriculum
                {
                    ShortName = "CIE", 
                    Name = "Courses in English",
                    Organiser = fk09,
                    CurriculumGroups = new HashSet<CurriculumGroup>
                    {
                        new CurriculumGroup { Name = "CIE"},
                    }
                },
                new Curriculum
                {
                    ShortName = "Export", 
                    Name = "Export",
                    Organiser = hm,
                    CurriculumGroups = new HashSet<CurriculumGroup>
                    {
                        new CurriculumGroup { Name = "Export"},
                    }
                },
            };

            curs.ForEach(c => db.Curricula.Add(c));

            db.SaveChanges();
        }

        public static void InitGroupTemplates(TimeTableDbContext db)
        {
            InsertGroupTemplate(db, "1AU", "AU", "1", "");
            InsertGroupTemplate(db, "2AU", "AU", "2", "");
            InsertGroupTemplate(db, "3AU", "AU", "3", "");
            InsertGroupTemplate(db, "4AU", "AU", "4", "");
            InsertGroupTemplate(db, "5AU", "AU", "5", "");
            InsertGroupTemplate(db, "6AU", "AU", "6", "");
            InsertGroupTemplate(db, "7AU", "AU", "7", "");
            InsertGroupTemplate(db, "WPM AU", "AU", "WPM", "");
            InsertGroupTemplate(db, "1LM", "LM", "1", "");
            InsertGroupTemplate(db, "2LM", "LM", "2", "");
            InsertGroupTemplate(db, "3LM", "LM", "3", "");
            InsertGroupTemplate(db, "4LM", "LM", "4", "");
            InsertGroupTemplate(db, "5LM", "LM", "5", "");
            InsertGroupTemplate(db, "6LM", "LM", "6", "");
            InsertGroupTemplate(db, "7LM", "LM", "7", "");
            InsertGroupTemplate(db, "WPM LM", "LM", "WPM", "");
            
            InsertGroupTemplate(db, "WPM WW-M", "MBA", "WPM", "");
            InsertGroupTemplate(db, "WW M1 NW", "MBA", "1 NW", "G1");
            InsertGroupTemplate(db, "WW M1 NW-G2", "MBA", "1 NW", "G2");
            InsertGroupTemplate(db, "WW M1 WI", "MBA", "1 WI", "");
            InsertGroupTemplate(db, "WW M2 Bau", "MBA", "2 BAU", "");
            InsertGroupTemplate(db, "WW M2 NW", "MBA", "2 NW", "");
            InsertGroupTemplate(db, "WW M2 WI", "MBA", "2 WI", "");
            InsertGroupTemplate(db, "WW M3", "MBA", "3", "");
            InsertGroupTemplate(db, "WW M4 Bau", "MBA", "4 BAU", "");
            InsertGroupTemplate(db, "WW M4 NW", "MBA", "4 NW", "");
            InsertGroupTemplate(db, "WW M4 WI", "MBA", "4 WI", "");
            InsertGroupTemplate(db, "WW M5", "MBA", "5", "");




            InsertGroupTemplate(db, "WI M1", "WIM", "1", "G1");
            InsertGroupTemplate(db, "WI M1 G2", "WIM", "2", "G2");
            InsertGroupTemplate(db, "WI M2", "WIM", "2", "");
            InsertGroupTemplate(db, "WI M3", "WIM", "3", "");
            InsertGroupTemplate(db, "WPM WI-M", "WIM", "WPM", "");
            InsertGroupTemplate(db, "CIE", "CIE", "CIE", "");
            InsertGroupTemplate(db, "Export", "Export", "Export", "");

            InsertGroupTemplate(db, "1A", "WI", "1", "A");
            InsertGroupTemplate(db, "1B", "WI", "1", "B");
            InsertGroupTemplate(db, "1C", "WI", "1", "C");
            InsertGroupTemplate(db, "2A", "WI", "2", "A");
            InsertGroupTemplate(db, "2B", "WI", "2", "B");
            InsertGroupTemplate(db, "2C", "WI", "2", "C");
            InsertGroupTemplate(db, "3Bio", "WI", "3 BIO", "");
            InsertGroupTemplate(db, "3Bio/Inf", "WI", "3 BIO", "");
            InsertGroupTemplate(db, "3Bio/Inf", "WI", "3 INF", "");
            InsertGroupTemplate(db, "3Inf", "WI", "3 INF", "");
            InsertGroupTemplate(db, "3Inf/Bio", "WI", "3 INF", "");
            InsertGroupTemplate(db, "3Inf/Bio", "WI", "3 BIO", "");
            InsertGroupTemplate(db, "3Inf/Tec", "WI", "3 INF", "");
            InsertGroupTemplate(db, "3Inf/Tec", "WI", "3 TEC", "G1");
            InsertGroupTemplate(db, "3Tec", "WI", "3 TEC", "G1");
            InsertGroupTemplate(db, "3Tec-G2", "WI", "3 TEC", "G2");
            InsertGroupTemplate(db, "3Tec-G2/Inf", "WI", "3 TEC", "G2");
            InsertGroupTemplate(db, "3Tec-G2/Inf", "WI", "3 INF", "");
            
            InsertGroupTemplate(db, "4Bio", "WI", "4 BIO", "");
            InsertGroupTemplate(db, "4Bio/Inf", "WI", "4 BIO", "");
            InsertGroupTemplate(db, "4Bio/Inf", "WI", "4 INF", "");
            InsertGroupTemplate(db, "4Bio/Tec", "WI", "4 BIO", "");
            InsertGroupTemplate(db, "4Bio/Tec", "WI", "4 TEC", "G1");
            InsertGroupTemplate(db, "4Inf", "WI", "4 INF", "");
            InsertGroupTemplate(db, "4Inf/Bio", "WI", "4 INF", "");
            InsertGroupTemplate(db, "4Inf/Bio", "WI", "4 BIO", "");
            InsertGroupTemplate(db, "4Inf/Tec", "WI", "4 INF", "");
            InsertGroupTemplate(db, "4Inf/Tec", "WI", "4 TEC", "G1");
            InsertGroupTemplate(db, "4Tec", "WI", "4 TEC", "G1");
            InsertGroupTemplate(db, "4Tec-G2", "WI", "4 TEC", "G2");
            InsertGroupTemplate(db, "4Tec-G2/Bio", "WI", "4 TEC", "G2");
            InsertGroupTemplate(db, "4Tec-G2/Bio", "WI", "4 BIO", "");
            InsertGroupTemplate(db, "4Tec-G2/Inf", "WI", "4 TEC", "G2");
            InsertGroupTemplate(db, "4Tec-G2/Inf", "WI", "4 INF", "");

            
            
            InsertGroupTemplate(db, "5Bio/Inf", "WI", "5 BIO", "");
            InsertGroupTemplate(db, "5Bio/Inf", "WI", "5 INF", "");
            InsertGroupTemplate(db, "5Bio/Tec", "WI", "5 BIO", "");
            InsertGroupTemplate(db, "5Bio/Tec", "WI", "5 TEC", "G1");
            InsertGroupTemplate(db, "5Inf", "WI", "5 INF", "");
            InsertGroupTemplate(db, "5Inf/Bio", "WI", "5 INF", "");
            InsertGroupTemplate(db, "5Inf/Bio", "WI", "5 BIO", "");
            InsertGroupTemplate(db, "5Tec", "WI", "5 TEC", "G1");
            InsertGroupTemplate(db, "5Tec-G2", "WI", "5 TEC", "G2");
            InsertGroupTemplate(db, "5Tec-G2/Bio", "WI", "5 TEC", "G2");
            InsertGroupTemplate(db, "5Tec-G2/Bio", "WI", "5 BIO", "");
            
            InsertGroupTemplate(db, "6Bio/Inf", "WI", "6 BIO", "");
            InsertGroupTemplate(db, "6Bio/Inf", "WI", "6 INF", "");
            InsertGroupTemplate(db, "6Inf/Bio", "WI", "6 INF", "");
            InsertGroupTemplate(db, "6Inf/Bio", "WI", "6 BIO", "");
            InsertGroupTemplate(db, "6Tec", "WI", "6 TEC", "G1");
            InsertGroupTemplate(db, "6Tec-G2", "WI", "6 TEC", "G2");
            
            InsertGroupTemplate(db, "7Bio", "WI", "7 BIO", "");
            InsertGroupTemplate(db, "7Bio/Inf", "WI", "7 BIO", "");
            InsertGroupTemplate(db, "7Bio/Inf", "WI", "7 INF", "");
            InsertGroupTemplate(db, "7Inf", "WI", "7 INF", "");
            InsertGroupTemplate(db, "7Inf/Bio", "WI", "7 INF", "");
            InsertGroupTemplate(db, "7Inf/Bio", "WI", "7 BIO", "");
            InsertGroupTemplate(db, "7Inf/Tec", "WI", "7 INF", "");
            InsertGroupTemplate(db, "7Inf/Tec", "WI", "7 TEC", "G1");
            InsertGroupTemplate(db, "7Tec", "WI", "7 TEC", "G1");
            InsertGroupTemplate(db, "7Tec-G2", "WI", "7 TEC", "G2");
            InsertGroupTemplate(db, "7Tec-G2/Inf", "WI", "7 TEC", "G2");
            InsertGroupTemplate(db, "7Tec-G2/Inf", "WI", "7 INF", "");
            InsertGroupTemplate(db, "WPM WI", "WI", "WPM", "");
        }

        private static void InsertGroupTemplate(TimeTableDbContext db,
            string aliasName, string currName, string currGroupName, string semGroupName)
        {
            /*
            var curriculum = db.Curricula.FirstOrDefault(g => g.ShortName.Equals(currName));
            if (curriculum != null)
            {
                var alias = curriculum.GroupAliases.FirstOrDefault(a => a.Name.Equals(aliasName));
                if (alias == null)
                {
                    alias = new GroupAlias
                    {
                        Curriculum = curriculum,
                        Name = aliasName
                    };
                    db.GroupAliases.Add(alias);
                }

                var template = alias.GroupTemplates.FirstOrDefault(t => t.CurriculumGroupName.Equals(currGroupName) &&
                                                          ((string.IsNullOrEmpty(semGroupName) && string.IsNullOrEmpty(t.SemesterGroupName)) ||
                                                          (!string.IsNullOrEmpty(semGroupName) && semGroupName.Equals(t.SemesterGroupName))
                                                           ));
                if (template == null)
                {
                    template = new GroupTemplate
                    {
                        Alias = alias,
                        CurriculumGroupName = currGroupName,
                        SemesterGroupName = semGroupName
                    };
                    db.GroupTemplates.Add(template);
                }

                // Curriculumsgruppe prüfen
                var currGroup = curriculum.CurriculumGroups.FirstOrDefault(g => g.Name.Equals(currGroupName));
                if (currGroup == null)
                {
                    currGroup = new CurriculumGroup
                    {
                        Curriculum = curriculum,
                        Name = currGroupName
                    };

                    db.CurriculumGroups.Add(currGroup);
                }

                db.SaveChanges();
            }
             */
        }

        public static void InitSemester(TimeTableDbContext db)
        {
            /*
            var semester = new Semester
            {
                Name = "WS15",
                BookingEnabled = false,
                StartCourses = new DateTime(2015, 10, 1),
                EndCourses = new DateTime(2016, 1, 22),     // hier das offizielle Ende, es wird als post process die Termine verschobe
                Dates = new HashSet<SemesterDate>
                {
                    new SemesterDate
                    {
                        Description = "Weihnachten",
                        From = new DateTime(2015, 12, 24),
                        To = new DateTime(2016, 1, 7),
                        HasCourses = false,
                    },
                }
            };
            db.Semesters.Add(semester);

            // Semestergruppen
            foreach (var curriculumGroup in db.CurriculumGroups)
            {
                var semGroup = new SemesterGroup
                {
                    CurriculumGroup = curriculumGroup,
                    Semester = semester,
                    Name = string.Empty
                };

                db.SemesterGroups.Add(semGroup);
            }

            db.SaveChanges();
             */
        }
    }
}
