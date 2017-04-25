using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices
{
    public class MigrationService
    {
        public void RepairCurricula()
        {
            var db = new TimeTableDbContext();
            
            // Ergänze alle bisherigen Studiengänge um den Veranstalter FK 09
            var org = db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));

            foreach (var curriculum in db.Curricula.ToList())
            {
                if (curriculum.Organiser == null)
                {
                    curriculum.Organiser = org;
                }
            }

            var hm = db.Organisers.SingleOrDefault(o => o.ShortName.Equals("HM"));
            // Ergänze einen Veranstalter HM
            if (hm == null)
            {
                hm = new ActivityOrganiser
                {
                    Name = "Hochschule München",
                    ShortName = "HM",
                    IsFaculty = false,
                    IsStudent = false
                };

                db.Organisers.Add(hm);
            }

            var export = db.Curricula.SingleOrDefault(c => c.ShortName.Equals("Export"));

            // Ergänze ein Studienprogramm Export => so lange wir nicht wissen wohin damit
            if (export == null)
            {
                export = new Curriculum
                {
                    Name = "Export",
                    ShortName = "Export",
                    Organiser = hm
                };
                db.Curricula.Add(export);
            }

            // Im Studiengang "Export" die Gruppenvorlage einbauen
            /*
            if (!export.GroupAliases.Any())
            {
                export.GroupAliases.Add(new GroupAlias
                {
                    Name = "Export",
                    GroupTemplates =
                    {
                        new GroupTemplate {CurriculumGroupName = "Export"}
                    }
                });
            }
             */

            // Im Studengang "Export" die Curriculumsgruppe ergänzen!
            if (!export.CurriculumGroups.Any())
            {
                export.CurriculumGroups.Add(new CurriculumGroup { Name = "Export" });
            }


            db.SaveChanges();
        }


        public void RenameGroups()
        {
            var db = new TimeTableDbContext();
            /*
            RenameGroup(db, "WI", "1 A", "1", "A");
            RenameGroup(db, "WI", "1 B", "1", "B");
            RenameGroup(db, "WI", "1 C", "1", "C");

            RenameGroup(db, "WI", "2 A", "2", "A");
            RenameGroup(db, "WI", "2 B", "2", "B");
            RenameGroup(db, "WI", "2 C", "2", "C");

            RenameGroup(db, "WI", "3 TEC G1", "3 TEC", "G1");
            RenameGroup(db, "WI", "3 TEC G2", "3 TEC", "G2");

            RenameGroup(db, "WI", "4 TEC G1", "4 TEC", "G1");
            RenameGroup(db, "WI", "4 TEC G2", "4 TEC", "G2");

            RenameGroup(db, "WI", "5 TEC G1", "5 TEC", "G1");
            RenameGroup(db, "WI", "5 TEC G2", "5 TEC", "G2");

            RenameGroup(db, "WI", "6 TEC", "6 TEC", "G1");
            RenameGroup(db, "WI", "6 TEC G2", "6 TEC", "G2");

            RenameGroup(db, "WI", "7 TEC", "7 TEC", "G1");
            RenameGroup(db, "WI", "7 TEC G2", "7 TEC", "G2");

            RenameGroup(db, "WIM", "1 A", "1", "A");
            RenameGroup(db, "WIM", "1 B G1", "1", "B G1");
            RenameGroup(db, "WIM", "1 B G2", "1", "B G2");

            RenameGroup(db, "WIM", "2 A", "2", "A");
            RenameGroup(db, "WIM", "3 A", "3", "A");
             */


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

        private void RenameGroup(TimeTableDbContext db, string curr, string oldName, string currGroupName, string semGroupName)
        {
            var groupList = db.CurriculumGroups.Where(g => g.Curriculum.ShortName.Equals(curr) && g.Name.Equals(oldName)).ToList();
            foreach (var group in groupList)
            {
                //var group = db.CurriculumGroups.SingleOrDefault(g => g.Curriculum.ShortName.Equals(curr) && g.Name.Equals(oldName));


                if (group != null)
                {
                    // Gruppe umbennen

                    group.Name = currGroupName;

                    // Alle zugehörigen Semestergruppen umbenennen
                    foreach (var semGroup in group.SemesterGroups.ToList())
                    {
                        semGroup.Name = semGroupName;
                    }

                    db.SaveChanges();
                }
            }
        }

        private void InsertGroupTemplate(TimeTableDbContext db,
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


        public void RepairGroups()
        {
            var db = new TimeTableDbContext();

            /*
            RepairGroup(db, "WI", "1");
            RepairGroup(db, "WI", "2");
            RepairGroup(db, "WI", "3 INF");
            RepairGroup(db, "WI", "3 BIO");
            RepairGroup(db, "WI", "3 TEC");
            RepairGroup(db, "WI", "4 INF");
            RepairGroup(db, "WI", "4 BIO");
            RepairGroup(db, "WI", "4 TEC");
            RepairGroup(db, "WI", "5 INF");
            RepairGroup(db, "WI", "5 BIO");
            RepairGroup(db, "WI", "5 TEC");
            RepairGroup(db, "WI", "6 INF");
            RepairGroup(db, "WI", "6 BIO");
            RepairGroup(db, "WI", "6 TEC");
            RepairGroup(db, "WI", "7 INF");
            RepairGroup(db, "WI", "7 BIO");
            RepairGroup(db, "WI", "7 TEC");

            RepairGroup(db, "WIM", "1");
            */


            RemoveTemplate(db, "WI", "3Bio/Inf", "3 INF", null);
            RemoveTemplate(db, "WI", "3Inf/Bio", "3 BIO", null);
            RemoveTemplate(db, "WI", "3Inf/Tec", "3 TEC", "G1");
            RemoveTemplate(db, "WI", "3Tec-G2/Inf", "3 INF", null);
            RemoveTemplate(db, "WI", "4Bio/Inf", "4 INF", null);
            RemoveTemplate(db, "WI", "4Bio/Tec", "4 TEC", "G1");
            RemoveTemplate(db, "WI", "4Inf/Bio", "4 BIO", null);
            RemoveTemplate(db, "WI", "4Inf/Tec", "4 TEC", "G1");
            RemoveTemplate(db, "WI", "4Tec-G2/Bio", "4 BIO", null);
            RemoveTemplate(db, "WI", "4Tec-G2/Inf", "4 INF", null);
            RemoveTemplate(db, "WI", "5Bio/Inf", "5 INF", null);
            RemoveTemplate(db, "WI", "5Bio/Tec", "5 TEC", "G1");
            RemoveTemplate(db, "WI", "5Inf/Bio", "5 BIO", null);
            RemoveTemplate(db, "WI", "5Tec-G2/Bio", "5 BIO", null);
            RemoveTemplate(db, "WI", "6Bio/Inf", "6 INF", null);
            RemoveTemplate(db, "WI", "6Inf/Bio", "6 BIO", null);
            RemoveTemplate(db, "WI", "7Bio/Inf", "7 INF", null);
            RemoveTemplate(db, "WI", "7Inf/Bio", "7 BIO", null);
            RemoveTemplate(db, "WI", "7Inf/Tec", "7 TEC", "G1");
            RemoveTemplate(db, "WI", "7Tec-G2/Inf", "7 INF", null);

        }

        private void RemoveTemplate(TimeTableDbContext db, string currName, string aliasName, string currGroup, string semGroup)
        {
            /*
            var curriculum = db.Curricula.SingleOrDefault(c => c.ShortName.Equals(currName));

            if (curriculum == null)
                return;

            var alias = curriculum.GroupAliases.SingleOrDefault(a => a.Name.Equals(aliasName));
            if (alias == null)
                return;

            GroupTemplate tpl;
            if (string.IsNullOrEmpty(semGroup))
            {
                tpl =
                    alias.GroupTemplates.SingleOrDefault(
                        t => t.CurriculumGroupName.Equals(currGroup) && string.IsNullOrEmpty(t.SemesterGroupName));
            }
            else
            {
                tpl =
                    alias.GroupTemplates.SingleOrDefault(
                        t => t.CurriculumGroupName.Equals(currGroup) && t.SemesterGroupName.Equals(semGroup));
                
            }

            if (tpl == null)
                return;

            alias.GroupTemplates.Remove(tpl);
            db.GroupTemplates.Remove(tpl);
            db.SaveChanges();
             */
        }



        private void RepairGroup(TimeTableDbContext db, string currName, string currGroupName)
        {
            var curriculum = db.Curricula.SingleOrDefault(c => c.ShortName.Equals(currName));

            var groupList = curriculum.CurriculumGroups.Where(g => g.Name.Equals(currGroupName)).ToList();

            if (groupList.Count() > 1)
            {
                var firstGroup = groupList.First();

                foreach (var curriculumGroup in groupList)
                {
                    if (curriculumGroup != firstGroup)
                    {
                        foreach (var semesterGroup in curriculumGroup.SemesterGroups)
                        {
                            firstGroup.SemesterGroups.Add(semesterGroup);
                        }
                        curriculumGroup.SemesterGroups.Clear();
                    }
                }


                var groups2delete = groupList.Where(g => !g.SemesterGroups.Any()).ToList();

                foreach (var curriculumGroup in groups2delete)
                {
                    curriculum.CurriculumGroups.Remove(curriculumGroup);
                    db.CurriculumGroups.Remove(curriculumGroup);
                }

                db.SaveChanges();
            }




        }



    }
}
