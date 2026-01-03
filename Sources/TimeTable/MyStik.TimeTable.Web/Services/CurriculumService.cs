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
        /// <param name="id"></param>
        public void DeleteCurriculum(Guid id)
        {
            var c = Db.Curricula.SingleOrDefault(x => x.Id == id);
            if (c == null)
                return;

            foreach (var @group in c.CurriculumGroups.ToList())
            {
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
    }

}