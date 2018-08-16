using System;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CurriculumService
    {
        private TimeTableDbContext Db = new TimeTableDbContext();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currName"></param>
        /// <param name="groupName"></param>
        /// <param name="isSubscrible"></param>
        public void SetSubscriptionState(string currName, string groupName, bool isSubscrible)
        {
            var group = Db.CurriculumGroups.SingleOrDefault(g => g.Curriculum.ShortName.Equals(currName) &&
                                                     g.Name.Equals(groupName));

            if (group != null)
            {
                group.IsSubscribable = isSubscrible;
                Db.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currName">Name des Studiengangs: WI, MBA</param>
        /// <param name="semName">Name des Semesters: WS15</param>
        /// <param name="srcCurrGroupName">Name der Studiengruppe (Ausgangslage)</param>
        /// <param name="srcSemGroupName">Name in der Semestergruppe (veraltet)</param>
        /// <param name="trgCurrGroupName">Name der Studiengruppe (Ziel) - i.d.R. identisch zu Ausgangslage</param>
        /// <param name="trgCapGroupName">Name der Kapazitätsgruppe</param>
        public void MoveGroup(string currName, string semName, string srcCurrGroupName, string srcSemGroupName,
            string trgCurrGroupName, string trgCapGroupName)
        {
            // Die Semestergruppe, um die es geht
            SemesterGroup semGroup = null;
            // Die Semestergruppe hat keinen Eigennamen, d.h. es muss nach "leer" gesucht werden
            if (string.IsNullOrEmpty(srcSemGroupName))
            {
                semGroup = Db.SemesterGroups.SingleOrDefault(s => s.Semester.Name.Equals(semName) &&
                    s.CurriculumGroup.Curriculum.ShortName.Equals(currName) && s.CurriculumGroup.Name.Equals(srcCurrGroupName) &&
                    string.IsNullOrEmpty(s.Name));
            }
            else
            {
                semGroup = Db.SemesterGroups.SingleOrDefault(s => s.Semester.Name.Equals(semName) &&
                    s.CurriculumGroup.Curriculum.ShortName.Equals(currName) && s.CurriculumGroup.Name.Equals(srcCurrGroupName) &&
                    !string.IsNullOrEmpty(s.Name) && s.Name.Equals(srcSemGroupName));
            }

            // Die Kapazitätsgruppe, die verbunden werden soll
            CapacityGroup capGroup = null;
            if (string.IsNullOrEmpty(trgCapGroupName))
            {
                capGroup = Db.CapacityGroups.SingleOrDefault(g => 
                    g.CurriculumGroup.Curriculum.ShortName.Equals(currName) &&
                    g.CurriculumGroup.Name.Equals(trgCurrGroupName) &&
                    string.IsNullOrEmpty(g.Name));
            }
            else
            {
                capGroup = Db.CapacityGroups.SingleOrDefault(g =>
                    g.CurriculumGroup.Curriculum.ShortName.Equals(currName) &&
                    g.CurriculumGroup.Name.Equals(trgCurrGroupName) &&
                    !string.IsNullOrEmpty(g.Name) && g.Name.Equals(trgCapGroupName));
                
            }

            MoveGroup(semGroup, capGroup);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semGroupId"></param>
        /// <param name="capGroupId"></param>
        public void MoveGroup(string semGroupId, string capGroupId)
        {
            var semGroup = Db.SemesterGroups.SingleOrDefault(g => string.Equals(g.Id.ToString(), semGroupId));
            var capGroup = Db.CapacityGroups.SingleOrDefault(g => string.Equals(g.Id.ToString(), capGroupId));

            MoveGroup(semGroup, capGroup);
        }


        private void MoveGroup(SemesterGroup semGroup, CapacityGroup capGroup)
        {
            // beide Gruppen müssen existieren
            if (semGroup != null && capGroup != null && semGroup.CapacityGroup != capGroup)
            {
                if (semGroup.CapacityGroup != null)
                {
                    // aus der bisherigen CapGroup die Semestergruppe entfernen
                    semGroup.CapacityGroup.SemesterGroups.Remove(semGroup);
                }
                semGroup.CapacityGroup = capGroup;
                capGroup.SemesterGroups.Add(semGroup);
                Db.SaveChanges();
            }
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currName"></param>
        /// <param name="groupName"></param>
        /// <param name="capName"></param>
        /// <param name="aliasName"></param>
        public void DeleteAlias(string currName, string groupName, string capName, string aliasName)
        {
            var alias = Db.GroupAliases.SingleOrDefault(a =>
                a.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals(currName) &&
                a.CapacityGroup.CurriculumGroup.Name.Equals(groupName) &&
                a.CapacityGroup.Name.Equals(capName) &&
                a.Name.Equals(aliasName)
                );

            if (alias != null)
            {
                Db.GroupAliases.Remove(alias);
                Db.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currName"></param>
        /// <param name="semName"></param>
        /// <param name="srcCurrGroupName"></param>
        /// <param name="srcSemGroupName"></param>
        /// <param name="trgCurrGroupName"></param>
        /// <param name="trgSemGroupName"></param>
        public void MergeGroup(string currName, string semName, string srcCurrGroupName, string srcSemGroupName,
            string trgCurrGroupName, string trgSemGroupName)
        {
            // Die Semestergruppe, um die es geht
            SemesterGroup srcSemGroup = null;
            // Die Semestergruppe hat keinen Eigennamen, d.h. es muss nach "leer" gesucht werden
            if (string.IsNullOrEmpty(srcSemGroupName))
            {
                srcSemGroup = Db.SemesterGroups.SingleOrDefault(s => s.Semester.Name.Equals(semName) &&
                                                                     s.CurriculumGroup.Curriculum.ShortName.Equals(
                                                                         currName) &&
                                                                     s.CurriculumGroup.Name.Equals(srcCurrGroupName) &&
                                                                     string.IsNullOrEmpty(s.Name));
            }
            else
            {
                srcSemGroup = Db.SemesterGroups.SingleOrDefault(s => s.Semester.Name.Equals(semName) &&
                                                                     s.CurriculumGroup.Curriculum.ShortName.Equals(
                                                                         currName) &&
                                                                     s.CurriculumGroup.Name.Equals(srcCurrGroupName) &&
                                                                     !string.IsNullOrEmpty(s.Name) &&
                                                                     s.Name.Equals(srcSemGroupName));
            }

            SemesterGroup trgSemGroup = null;
            // Die Semestergruppe hat keinen Eigennamen, d.h. es muss nach "leer" gesucht werden
            if (string.IsNullOrEmpty(trgSemGroupName))
            {
                trgSemGroup = Db.SemesterGroups.SingleOrDefault(s => s.Semester.Name.Equals(semName) &&
                                                                     s.CurriculumGroup.Curriculum.ShortName.Equals(
                                                                         currName) &&
                                                                     s.CurriculumGroup.Name.Equals(trgCurrGroupName) &&
                                                                     string.IsNullOrEmpty(s.Name));
            }
            else
            {
                trgSemGroup = Db.SemesterGroups.SingleOrDefault(s => s.Semester.Name.Equals(semName) &&
                                                                     s.CurriculumGroup.Curriculum.ShortName.Equals(
                                                                         currName) &&
                                                                     s.CurriculumGroup.Name.Equals(trgCurrGroupName) &&
                                                                     !string.IsNullOrEmpty(s.Name) &&
                                                                     s.Name.Equals(trgSemGroupName));

                if (trgSemGroup == null)
                {
                    trgSemGroup = Db.SemesterGroups.SingleOrDefault(s => s.Semester.Name.Equals(semName) &&
                                                                         s.CapacityGroup != null &&
                                                                         s.CapacityGroup.CurriculumGroup.Curriculum
                                                                             .ShortName.Equals(currName) &&
                                                                         s.CapacityGroup.CurriculumGroup.Name.Equals(
                                                                             trgCurrGroupName) &&
                                                                         s.CapacityGroup.Name.Equals(trgSemGroupName));
                }
            }

            MergeGroup(srcSemGroup, trgSemGroup);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcGroupId"></param>
        /// <param name="trgGroupId"></param>
        public void MergeGroup(string srcGroupId, string trgGroupId)
        {
            var srcGroup = Db.SemesterGroups.SingleOrDefault(g => string.Equals(g.Id.ToString(), srcGroupId));
            var trgGroup = Db.SemesterGroups.SingleOrDefault(g => string.Equals(g.Id.ToString(), trgGroupId));

            MergeGroup(srcGroup, trgGroup);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delGroupId"></param>
        public void DeleteGroup(string delGroupId)
        {
            var delGroup = Db.SemesterGroups.SingleOrDefault(g => string.Equals(g.Id.ToString(), delGroupId));
            DeleteGroup(delGroup);
        }

        private void MergeGroup(SemesterGroup srcSemGroup, SemesterGroup trgSemGroup)
        {
            if (srcSemGroup != null && trgSemGroup != null)
            {
                foreach (var activity in srcSemGroup.Activities.ToList())
                {
                    srcSemGroup.Activities.Remove(activity);
                    trgSemGroup.Activities.Add(activity);
                }

                foreach (var subscription in srcSemGroup.Subscriptions.ToList())
                {
                    srcSemGroup.Subscriptions.Remove(subscription);
                    trgSemGroup.Subscriptions.Add(subscription);
                }

                foreach (var occurrenceGroup in srcSemGroup.OccurrenceGroups.ToList())
                {
                    srcSemGroup.OccurrenceGroups.Remove(occurrenceGroup);
                    trgSemGroup.OccurrenceGroups.Add(occurrenceGroup);
                }

                Db.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currName"></param>
        /// <param name="semName"></param>
        /// <param name="srcCurrGroupName"></param>
        /// <param name="srcSemGroupName"></param>
        public void DeleteGroup(string currName, string semName, string srcCurrGroupName, string srcSemGroupName)
        {
            // Die Semestergruppe, um die es geht
            SemesterGroup srcSemGroup = null;
            // Die Semestergruppe hat keinen Eigennamen, d.h. es muss nach "leer" gesucht werden
            if (string.IsNullOrEmpty(srcSemGroupName))
            {
                srcSemGroup = Db.SemesterGroups.SingleOrDefault(s => s.Semester.Name.Equals(semName) &&
                                                                     s.CurriculumGroup.Curriculum.ShortName.Equals(
                                                                         currName) &&
                                                                     s.CurriculumGroup.Name.Equals(srcCurrGroupName) &&
                                                                     string.IsNullOrEmpty(s.Name));
            }
            else
            {
                srcSemGroup = Db.SemesterGroups.SingleOrDefault(s => s.Semester.Name.Equals(semName) &&
                                                                     s.CurriculumGroup.Curriculum.ShortName.Equals(
                                                                         currName) &&
                                                                     s.CurriculumGroup.Name.Equals(srcCurrGroupName) &&
                                                                     !string.IsNullOrEmpty(s.Name) &&
                                                                     s.Name.Equals(srcSemGroupName));
            }


            DeleteGroup(srcSemGroup);
        }

        private void DeleteGroup(SemesterGroup semGroup)
        {
            if (semGroup != null && !semGroup.Subscriptions.Any() && !semGroup.Activities.Any())
            {
                Db.SemesterGroups.Remove(semGroup);
                Db.SaveChanges();
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void DeleteCurriculum(Guid id)
        {
            var c = Db.Curricula.SingleOrDefault(x => x.Id == id);
            if (c == null)
                return;

            foreach (var @group in c.CurriculumGroups.ToList())
            {

                // ALTE STRUKTUR
                // alle zugehörigen Semestergruppen löschen 
                // => Die Kurse sind davon nicht betroffen
                //    sie werden u.U. nicht mehr angezeigt / gefunden
                /*
                foreach (var semesterGroup in group.SemesterGroups.ToList())
                {
                    group.SemesterGroups.Remove(semesterGroup);

                    // Alle ggf. vorhandenen Eintragungen (Subscriptions) der
                    // Semestergruppe löschen
                    foreach (var semSub in semesterGroup.Subscriptions.ToList())
                    {
                        semesterGroup.Subscriptions.Remove(semSub);
                        Db.Subscriptions.Remove(semSub);
                    }

                    Db.SemesterGroups.Remove(semesterGroup);
                }
                */

                // NEUE STRUKTUR
                // alle zugehörigen CapactityGroups löschen
                foreach (var capGroup in group.CapacityGroups.ToList())
                {
                    foreach (var semesterGroup in capGroup.SemesterGroups.ToList())
                    {
                        //group.SemesterGroups.Remove(semesterGroup);

                        // Alle ggf. vorhandenen Eintragungen (Subscriptions) der
                        // Semestergruppe löschen
                        foreach (var semSub in semesterGroup.Subscriptions.ToList())
                        {
                            semesterGroup.Subscriptions.Remove(semSub);
                            Db.Subscriptions.Remove(semSub);
                        }

                        Db.SemesterGroups.Remove(semesterGroup);
                    }

                    foreach (var groupAlias in capGroup.Aliases.ToList())
                    {
                        Db.GroupAliases.Remove(groupAlias);
                    }

                    Db.CapacityGroups.Remove(capGroup);
                }

                Db.CurriculumGroups.Remove(group);
            }

            Db.Curricula.Remove(c);
            Db.SaveChanges();
        }


        /// <summary>
        /// 
        /// </summary>
        public void MoveWI()
        {
            MoveGroup("WI", "WS13", "1", "A", "1", "A");
            MoveGroup("WI", "SS14", "1", "A", "1", "A");
            MoveGroup("WI", "WS14", "1", "A", "1", "A");
            MoveGroup("WI", "SS15", "1", "A", "1", "A");
            MoveGroup("WI", "WS15", "1", "A", "1", "A");

            MoveGroup("WI", "WS13", "1", "B", "1", "B");
            MoveGroup("WI", "SS14", "1", "B", "1", "B");
            MoveGroup("WI", "WS14", "1", "B", "1", "B");
            MoveGroup("WI", "SS15", "1", "B", "1", "B");
            MoveGroup("WI", "WS15", "1", "B", "1", "B");

            MoveGroup("WI", "WS13", "1", "C", "1", "C");
            MoveGroup("WI", "WS14", "1", "C", "1", "C");
            MoveGroup("WI", "WS15", "1", "C", "1", "C");

            MoveGroup("WI", "WS13", "2", "A", "2", "A");
            MoveGroup("WI", "SS14", "2", "A", "2", "A");
            MoveGroup("WI", "WS14", "2", "A", "2", "A");
            MoveGroup("WI", "SS15", "2", "A", "2", "A");
            MoveGroup("WI", "WS15", "2", "A", "2", "A");

            MoveGroup("WI", "WS13", "2", "B", "2", "B");
            MoveGroup("WI", "SS14", "2", "B", "2", "B");
            MoveGroup("WI", "WS14", "2", "B", "2", "B");
            MoveGroup("WI", "SS15", "2", "B", "2", "B");
            MoveGroup("WI", "WS15", "2", "B", "2", "B");

            MoveGroup("WI", "SS14", "2", "C", "2", "C");
            MoveGroup("WI", "SS15", "2", "C", "2", "C");

            MoveGroup("WI", "WS13", "3 BIO", "", "3 BIO", "");
            MoveGroup("WI", "SS14", "3 BIO", "", "3 BIO", "");
            MoveGroup("WI", "WS14", "3 BIO", "", "3 BIO", "");
            MoveGroup("WI", "SS15", "3 BIO", "", "3 BIO", "");
            MoveGroup("WI", "WS15", "3 BIO", "", "3 BIO", "");

            MoveGroup("WI", "WS13", "3 INF", "", "3 INF", "");
            MoveGroup("WI", "SS14", "3 INF", "", "3 INF", "");
            MoveGroup("WI", "WS14", "3 INF", "", "3 INF", "");
            MoveGroup("WI", "SS15", "3 INF", "", "3 INF", "");
            MoveGroup("WI", "WS15", "3 INF", "", "3 INF", "");

            MoveGroup("WI", "WS13", "3 TEC", "G1", "3 TEC", "G1");
            MoveGroup("WI", "SS14", "3 TEC", "G1", "3 TEC", "G1");
            MoveGroup("WI", "WS14", "3 TEC", "G1", "3 TEC", "G1");
            MoveGroup("WI", "SS15", "3 TEC", "G1", "3 TEC", "G1");
            MoveGroup("WI", "WS15", "3 TEC", "G1", "3 TEC", "G1");

            MoveGroup("WI", "WS13", "3 TEC", "G2", "3 TEC", "G2");
            MoveGroup("WI", "SS14", "3 TEC", "G2", "3 TEC", "G2");
            MoveGroup("WI", "WS14", "3 TEC", "G2", "3 TEC", "G2");
            MoveGroup("WI", "SS15", "3 TEC", "G2", "3 TEC", "G2");
            MoveGroup("WI", "WS15", "3 TEC", "G2", "3 TEC", "G2");

            MoveGroup("WI", "WS13", "4 BIO", "", "4 BIO", "");
            MoveGroup("WI", "SS14", "4 BIO", "", "4 BIO", "");
            MoveGroup("WI", "WS14", "4 BIO", "", "4 BIO", "");
            MoveGroup("WI", "SS15", "4 BIO", "", "4 BIO", "");
            MoveGroup("WI", "WS15", "4 BIO", "", "4 BIO", "");

            MoveGroup("WI", "WS13", "4 INF", "", "4 INF", "");
            MoveGroup("WI", "SS14", "4 INF", "", "4 INF", "");
            MoveGroup("WI", "WS14", "4 INF", "", "4 INF", "");
            MoveGroup("WI", "SS15", "4 INF", "", "4 INF", "");
            MoveGroup("WI", "WS15", "4 INF", "", "4 INF", "");

            MoveGroup("WI", "WS13", "4 TEC", "G1", "4 TEC", "G1");
            MoveGroup("WI", "SS14", "4 TEC", "G1", "4 TEC", "G1");
            MoveGroup("WI", "WS14", "4 TEC", "G1", "4 TEC", "G1");
            MoveGroup("WI", "SS15", "4 TEC", "G1", "4 TEC", "G1");
            MoveGroup("WI", "WS15", "4 TEC", "G1", "4 TEC", "G1");

            MoveGroup("WI", "WS13", "4 TEC", "G2", "4 TEC", "G2");
            MoveGroup("WI", "SS14", "4 TEC", "G2", "4 TEC", "G2");
            MoveGroup("WI", "WS14", "4 TEC", "G2", "4 TEC", "G2");
            MoveGroup("WI", "SS15", "4 TEC", "G2", "4 TEC", "G2");
            MoveGroup("WI", "WS15", "4 TEC", "G2", "4 TEC", "G2");

            MoveGroup("WI", "WS13", "5 BIO", "", "5 BIO", "");
            MoveGroup("WI", "SS14", "5 BIO", "", "5 BIO", "");
            MoveGroup("WI", "WS14", "5 BIO", "", "5 BIO", "");
            MoveGroup("WI", "SS15", "5 BIO", "", "5 BIO", "");
            MoveGroup("WI", "WS15", "5 BIO", "", "5 BIO", "");

            MoveGroup("WI", "WS13", "5 INF", "", "5 INF", "");
            MoveGroup("WI", "SS14", "5 INF", "", "5 INF", "");
            MoveGroup("WI", "WS14", "5 INF", "", "5 INF", "");
            MoveGroup("WI", "SS15", "5 INF", "", "5 INF", "");
            MoveGroup("WI", "WS15", "5 INF", "", "5 INF", "");

            MoveGroup("WI", "WS13", "5 TEC", "G1", "5 TEC", "G1");
            MoveGroup("WI", "SS14", "5 TEC", "G1", "5 TEC", "G1");
            MoveGroup("WI", "WS14", "5 TEC", "G1", "5 TEC", "G1");
            MoveGroup("WI", "SS15", "5 TEC", "G1", "5 TEC", "G1");
            MoveGroup("WI", "WS15", "5 TEC", "G1", "5 TEC", "G1");

            MoveGroup("WI", "WS13", "5 TEC", "G2", "5 TEC", "G2");
            MoveGroup("WI", "SS14", "5 TEC", "G2", "5 TEC", "G2");
            MoveGroup("WI", "WS14", "5 TEC", "G2", "5 TEC", "G2");
            MoveGroup("WI", "SS15", "5 TEC", "G2", "5 TEC", "G2");
            MoveGroup("WI", "WS15", "5 TEC", "G2", "5 TEC", "G2");

            MoveGroup("WI", "WS13", "6 BIO", "", "6 BIO", "");
            MoveGroup("WI", "SS14", "6 BIO", "", "6 BIO", "");
            MoveGroup("WI", "WS14", "6 BIO", "", "6 BIO", "");
            MoveGroup("WI", "SS15", "6 BIO", "", "6 BIO", "");
            MoveGroup("WI", "WS15", "6 BIO", "", "6 BIO", "");

            MoveGroup("WI", "WS13", "6 INF", "", "6 INF", "");
            MoveGroup("WI", "SS14", "6 INF", "", "6 INF", "");
            MoveGroup("WI", "WS14", "6 INF", "", "6 INF", "");
            MoveGroup("WI", "SS15", "6 INF", "", "6 INF", "");
            MoveGroup("WI", "WS15", "6 INF", "", "6 INF", "");

            MoveGroup("WI", "WS13", "6 TEC", "G1", "6 TEC", "G1");
            MoveGroup("WI", "SS14", "6 TEC", "G1", "6 TEC", "G1");
            MoveGroup("WI", "WS14", "6 TEC", "G1", "6 TEC", "G1");
            MoveGroup("WI", "SS15", "6 TEC", "G1", "6 TEC", "G1");
            MoveGroup("WI", "WS15", "6 TEC", "G1", "6 TEC", "G1");

            MoveGroup("WI", "WS13", "6 TEC", "G2", "6 TEC", "G2");
            MoveGroup("WI", "SS14", "6 TEC", "G2", "6 TEC", "G2");
            MoveGroup("WI", "WS14", "6 TEC", "G2", "6 TEC", "G2");
            MoveGroup("WI", "SS15", "6 TEC", "G2", "6 TEC", "G2");
            MoveGroup("WI", "WS15", "6 TEC", "G2", "6 TEC", "G2");

            MoveGroup("WI", "WS13", "7 BIO", "", "7 BIO", "");
            MoveGroup("WI", "SS14", "7 BIO", "", "7 BIO", "");
            MoveGroup("WI", "WS14", "7 BIO", "", "7 BIO", "");
            MoveGroup("WI", "SS15", "7 BIO", "", "7 BIO", "");
            MoveGroup("WI", "WS15", "7 BIO", "", "7 BIO", "");

            MoveGroup("WI", "WS13", "7 INF", "", "7 INF", "");
            MoveGroup("WI", "SS14", "7 INF", "", "7 INF", "");
            MoveGroup("WI", "WS14", "7 INF", "", "7 INF", "");
            MoveGroup("WI", "SS15", "7 INF", "", "7 INF", "");
            MoveGroup("WI", "WS15", "7 INF", "", "7 INF", "");

            MoveGroup("WI", "WS13", "7 TEC", "G1", "7 TEC", "G1");
            MoveGroup("WI", "SS14", "7 TEC", "G1", "7 TEC", "G1");
            MoveGroup("WI", "WS14", "7 TEC", "G1", "7 TEC", "G1");
            MoveGroup("WI", "SS15", "7 TEC", "G1", "7 TEC", "G1");
            MoveGroup("WI", "WS15", "7 TEC", "G1", "7 TEC", "G1");

            MoveGroup("WI", "WS13", "7 TEC", "G2", "7 TEC", "G2");
            MoveGroup("WI", "SS14", "7 TEC", "G2", "7 TEC", "G2");
            MoveGroup("WI", "WS14", "7 TEC", "G2", "7 TEC", "G2");
            MoveGroup("WI", "SS15", "7 TEC", "G2", "7 TEC", "G2");
            MoveGroup("WI", "WS15", "7 TEC", "G2", "7 TEC", "G2");

            MoveGroup("WI", "WS13", "WPM", "", "WPM", "");
            MoveGroup("WI", "SS14", "WPM", "", "WPM", "");
            MoveGroup("WI", "WS14", "WPM", "", "WPM", "");
            MoveGroup("WI", "SS15", "WPM", "", "WPM", "");
            MoveGroup("WI", "WS15", "WPM", "", "WPM", "");
        }

        /// <summary>
        /// 
        /// </summary>
        public void MoveLM()
        {
            MoveGroup("LM", "WS13", "1", "", "1", "");
            MoveGroup("LM", "WS14", "1", "", "1", "");
            MoveGroup("LM", "WS15", "1", "", "1", "");

            MoveGroup("LM", "SS14", "2", "", "2", "");
            MoveGroup("LM", "SS15", "2", "", "2", "");

            MoveGroup("LM", "WS13", "3", "", "3", "");
            MoveGroup("LM", "WS14", "3", "", "3", "");
            MoveGroup("LM", "WS15", "3", "", "3", "");

            MoveGroup("LM", "SS14", "4", "", "4", "");
            MoveGroup("LM", "SS15", "4", "", "4", "");

            MoveGroup("LM", "WS13", "5", "", "5", "");
            MoveGroup("LM", "WS14", "5", "", "5", "");
            MoveGroup("LM", "WS15", "5", "", "5", "");

            MoveGroup("LM", "SS14", "6", "", "6", "");
            MoveGroup("LM", "SS15", "6", "", "6", "");

            MoveGroup("LM", "WS13", "7", "", "7", "");
            MoveGroup("LM", "WS14", "7", "", "7", "");
            MoveGroup("LM", "WS15", "7", "", "7", "");

            MoveGroup("LM", "WS13", "WPM", "", "WPM", "");
            MoveGroup("LM", "SS14", "WPM", "", "WPM", "");
            MoveGroup("LM", "WS14", "WPM", "", "WPM", "");
            MoveGroup("LM", "SS15", "WPM", "", "WPM", "");
            MoveGroup("LM", "WS15", "WPM", "", "WPM", "");
        }

        /// <summary>
        /// 
        /// </summary>
        public void MoveAU()
        {
            MoveGroup("AU", "SS14", "1", "", "1", "");
            MoveGroup("AU", "SS15", "1", "", "1", "");

            MoveGroup("AU", "WS13", "2", "", "2", "");
            MoveGroup("AU", "WS14", "2", "", "2", "");
            MoveGroup("AU", "WS15", "2", "", "2", "");

            MoveGroup("AU", "SS14", "3", "", "3", "");
            MoveGroup("AU", "SS15", "3", "", "3", "");

            MoveGroup("AU", "WS13", "4", "", "4", "");
            MoveGroup("AU", "WS14", "4", "", "4", "");
            MoveGroup("AU", "WS15", "4", "", "4", "");

            MoveGroup("AU", "SS14", "5", "", "5", "");
            MoveGroup("AU", "SS15", "5", "", "5", "");

            MoveGroup("AU", "WS13", "6", "", "6", "");
            MoveGroup("AU", "WS14", "6", "", "6", "");
            MoveGroup("AU", "WS15", "6", "", "6", "");

            MoveGroup("AU", "SS14", "7", "", "7", "");
            MoveGroup("AU", "SS15", "7", "", "7", "");

            MoveGroup("AU", "WS13", "WPM", "", "WPM", "");
            MoveGroup("AU", "SS14", "WPM", "", "WPM", "");
            MoveGroup("AU", "WS14", "WPM", "", "WPM", "");
            MoveGroup("AU", "SS15", "WPM", "", "WPM", "");
            MoveGroup("AU", "WS15", "WPM", "", "WPM", "");
        }

        /// <summary>
        /// 
        /// </summary>
        public void MoveWIM()
        {
            MoveGroup("WIM", "WS13", "1", "A", "1", "G1");
            MoveGroup("WIM", "SS14", "1", "A", "1", "G1");
            MoveGroup("WIM", "WS14", "1", "A", "1", "G1");
            MoveGroup("WIM", "WS14", "1", "B G1", "1", "G2");
            MoveGroup("WIM", "SS15", "1", "", "1", "G1");
            MoveGroup("WIM", "WS15", "1", "G1", "1", "G1");

            MoveGroup("WIM", "WS13", "2", "A", "2", "G1");
            MoveGroup("WIM", "SS14", "2", "A", "2", "G1");
            MoveGroup("WIM", "WS14", "2", "A", "2", "G1");
            MoveGroup("WIM", "SS15", "2", "", "2", "G1");
            MoveGroup("WIM", "WS15", "2", "A", "2", "G1");

            MoveGroup("WIM", "WS13", "3", "A", "3", "G1");
            MoveGroup("WIM", "SS14", "3", "A", "3", "G1");
            MoveGroup("WIM", "WS14", "3", "A", "3", "G1");
            MoveGroup("WIM", "SS15", "3", "", "3", "G1");
            MoveGroup("WIM", "WS15", "3", "A", "3", "G1");

            MoveGroup("WIM", "WS13", "WPM", "", "WPM", "");
            MoveGroup("WIM", "SS14", "WPM", "", "WPM", "");
            MoveGroup("WIM", "WS14", "WPM", "", "WPM", "");
            MoveGroup("WIM", "SS15", "WPM", "", "WPM", "");
            MoveGroup("WIM", "WS15", "WPM", "", "WPM", "");
        }

        /// <summary>
        /// 
        /// </summary>
        public void MoveMBA()
        {
            MoveGroup("MBA", "WS13", "1 ING / NW", "", "1 ING / NW", "G1");
            MoveGroup("MBA", "SS14", "1 ING / NW", "", "1 ING / NW", "G1");
            MoveGroup("MBA", "WS14", "1 ING / NW", "", "1 ING / NW", "G1");
            MoveGroup("MBA", "SS15", "1 ING / NW", "", "1 ING / NW", "G1");
            MoveGroup("MBA", "WS15", "1 ING / NW", "", "1 ING / NW", "G1");

            MoveGroup("MBA", "WS13", "1 BAU", "", "1 BAU", "");
            MoveGroup("MBA", "SS14", "1 BAU", "", "1 BAU", "");
            MoveGroup("MBA", "WS14", "1 BAU", "", "1 BAU", "");
            MoveGroup("MBA", "SS15", "1 BAU", "", "1 BAU", "");
            MoveGroup("MBA", "WS15", "1 BAU", "", "1 BAU", "");

            MoveGroup("MBA", "WS13", "1 WI", "", "1 WI", "");
            MoveGroup("MBA", "SS14", "1 WI", "", "1 WI", "");
            MoveGroup("MBA", "WS14", "1 WI", "", "1 WI", "");
            MoveGroup("MBA", "SS15", "1 WI", "", "1 WI", "");
            MoveGroup("MBA", "WS15", "1 WI", "", "1 WI", "");

            MoveGroup("MBA", "WS13", "2 ING / NW", "", "2 ING / NW", "G1");
            MoveGroup("MBA", "SS14", "2 ING / NW", "", "2 ING / NW", "G1");
            MoveGroup("MBA", "WS14", "2 ING / NW", "", "2 ING / NW", "G1");
            MoveGroup("MBA", "SS15", "2 ING / NW", "", "2 ING / NW", "G1");
            MoveGroup("MBA", "WS15", "2 ING / NW", "", "2 ING / NW", "G1");

            MoveGroup("MBA", "WS13", "2 BAU", "", "2 BAU", "");
            MoveGroup("MBA", "SS14", "2 BAU", "", "2 BAU", "");
            MoveGroup("MBA", "WS14", "2 BAU", "", "2 BAU", "");
            MoveGroup("MBA", "SS15", "2 BAU", "", "2 BAU", "");
            MoveGroup("MBA", "WS15", "2 BAU", "", "2 BAU", "");

            MoveGroup("MBA", "WS13", "2 WI", "", "2 WI", "");
            MoveGroup("MBA", "SS14", "2 WI", "", "2 WI", "");
            MoveGroup("MBA", "WS14", "2 WI", "", "2 WI", "");
            MoveGroup("MBA", "SS15", "2 WI", "", "2 WI", "");
            MoveGroup("MBA", "WS15", "2 WI", "", "2 WI", "");


            MoveGroup("MBA", "WS13", "3 ING / NW", "", "3 ING / NW", "G1");
            MoveGroup("MBA", "SS14", "3 ING / NW", "", "3 ING / NW", "G1");
            MoveGroup("MBA", "WS14", "3 ING / NW", "", "3 ING / NW", "G1");
            MoveGroup("MBA", "SS15", "3 ING / NW", "", "3 ING / NW", "G1");
            MoveGroup("MBA", "WS15", "3 ING / NW", "", "3 ING / NW", "G1");

            MoveGroup("MBA", "WS13", "3 BAU", "", "3 BAU", "");
            MoveGroup("MBA", "SS14", "3 BAU", "", "3 BAU", "");
            MoveGroup("MBA", "WS14", "3 BAU", "", "3 BAU", "");
            MoveGroup("MBA", "SS15", "3 BAU", "", "3 BAU", "");
            MoveGroup("MBA", "WS15", "3 BAU", "", "3 BAU", "");

            MoveGroup("MBA", "WS13", "3 WI", "", "3 WI", "");
            MoveGroup("MBA", "SS14", "3 WI", "", "3 WI", "");
            MoveGroup("MBA", "WS14", "3 WI", "", "3 WI", "");
            MoveGroup("MBA", "SS15", "3 WI", "", "3 WI", "");
            MoveGroup("MBA", "WS15", "3 WI", "", "3 WI", "");


            MoveGroup("MBA", "WS13", "4 ING / NW", "", "4 ING / NW", "G1");
            MoveGroup("MBA", "SS14", "4 ING / NW", "", "4 ING / NW", "G1");
            MoveGroup("MBA", "WS14", "4 ING / NW", "", "4 ING / NW", "G1");
            MoveGroup("MBA", "SS15", "4 ING / NW", "", "4 ING / NW", "G1");
            MoveGroup("MBA", "WS15", "4 ING / NW", "", "4 ING / NW", "G1");

            MoveGroup("MBA", "WS13", "4 BAU", "", "4 BAU", "");
            MoveGroup("MBA", "SS14", "4 BAU", "", "4 BAU", "");
            MoveGroup("MBA", "WS14", "4 BAU", "", "4 BAU", "");
            MoveGroup("MBA", "SS15", "4 BAU", "", "4 BAU", "");
            MoveGroup("MBA", "WS15", "4 BAU", "", "4 BAU", "");

            MoveGroup("MBA", "WS13", "4 WI", "", "4 WI", "");
            MoveGroup("MBA", "SS14", "4 WI", "", "4 WI", "");
            MoveGroup("MBA", "WS14", "4 WI", "", "4 WI", "");
            MoveGroup("MBA", "SS15", "4 WI", "", "4 WI", "");
            MoveGroup("MBA", "WS15", "4 WI", "", "4 WI", "");

            MoveGroup("MBA", "WS13", "5 ING / NW", "", "5 ING / NW", "G1");
            MoveGroup("MBA", "SS14", "5 ING / NW", "", "5 ING / NW", "G1");
            MoveGroup("MBA", "WS14", "5 ING / NW", "", "5 ING / NW", "G1");
            MoveGroup("MBA", "SS15", "5 ING / NW", "", "5 ING / NW", "G1");
            MoveGroup("MBA", "WS15", "5 ING / NW", "", "5 ING / NW", "G1");

            MoveGroup("MBA", "WS13", "5 BAU", "", "5 BAU", "");
            MoveGroup("MBA", "SS14", "5 BAU", "", "5 BAU", "");
            MoveGroup("MBA", "WS14", "5 BAU", "", "5 BAU", "");
            MoveGroup("MBA", "SS15", "5 BAU", "", "5 BAU", "");
            MoveGroup("MBA", "WS15", "5 BAU", "", "5 BAU", "");

            MoveGroup("MBA", "WS13", "5 WI", "", "5 WI", "");
            MoveGroup("MBA", "SS14", "5 WI", "", "5 WI", "");
            MoveGroup("MBA", "WS14", "5 WI", "", "5 WI", "");
            MoveGroup("MBA", "SS15", "5 WI", "", "5 WI", "");
            MoveGroup("MBA", "WS15", "5 WI", "", "5 WI", "");

            
            MoveGroup("MBA", "WS13", "WPM", "", "WPM", "");
            MoveGroup("MBA", "SS14", "WPM", "", "WPM", "");
            MoveGroup("MBA", "WS14", "WPM", "", "WPM", "");
            MoveGroup("MBA", "SS15", "WPM", "", "WPM", "");
            MoveGroup("MBA", "WS15", "WPM", "", "WPM", "");

        }

        /// <summary>
        /// 
        /// </summary>
        public void MoveMisc()
        {
            MoveGroup("CIE", "WS13", "CIE", "", "CIE", "");
            MoveGroup("CIE", "SS14", "CIE", "", "CIE", "");
            MoveGroup("CIE", "WS14", "CIE", "", "CIE", "");
            MoveGroup("CIE", "SS15", "CIE", "", "CIE", "");
            MoveGroup("CIE", "WS15", "CIE", "", "CIE", "");

            MoveGroup("Export", "WS13", "Export", "", "Export", "");
            MoveGroup("Export", "SS14", "Export", "", "Export", "");
            MoveGroup("Export", "WS14", "Export", "", "Export", "");
            MoveGroup("Export", "SS15", "Export", "", "Export", "");
            MoveGroup("Export", "WS15", "Export", "", "Export", "");
        }

    }
}