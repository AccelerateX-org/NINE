using System;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices
{
    public partial class InfrastructureDataService
    {
        private TimeTableDbContext _db = new TimeTableDbContext();


        public ActivityOrganiser AddOrganiser(string shortName, string name, bool isFaculty, bool isStudent)
        {
            var org = GetOrganiser(shortName);

            if (org == null)
            {
                org = new ActivityOrganiser
                {
                    ShortName = shortName,
                    Name = name,
                    IsFaculty = isFaculty,
                    IsStudent = isStudent
                };

                _db.Organisers.Add(org);
                _db.SaveChanges();
            }

            return org;
        }

        public Data.Curriculum AddCurriculum(string shortName, string name, string orgName)
        {
            var cur = GetCurriculum(shortName);
            var org = GetOrganiser(orgName);

            if (cur == null && org != null)
            {
                cur = new Data.Curriculum
                {
                    ShortName = shortName,
                    Name = name,
                    Organiser = org,
                };

                _db.Curricula.Add(cur);
                _db.SaveChanges();
            }

            return cur;
        }

        public CurriculumGroup AddCurriculumGroup(string shortName, string groupName, string aliasName, string semesterGroup)
        {
            var cur = GetCurriculum(shortName);
            var group = cur.CurriculumGroups.SingleOrDefault(g => g.Name.Equals(groupName));

            if (group == null)
            {
                group = new CurriculumGroup
                {
                    Name = groupName,
                    Curriculum = cur
                };

                _db.CurriculumGroups.Add(group);
                _db.SaveChanges();
            }

            /*
            var alias = cur.GroupAliases.SingleOrDefault(a => a.Name.Equals(aliasName));

            if (alias == null)
            {
                alias = new GroupAlias
                {
                    Curriculum = cur,
                    Name = aliasName
                };

                _db.GroupAliases.Add(alias);
                _db.SaveChanges();
            }


            var template = alias.GroupTemplates.SingleOrDefault(t => t.CurriculumGroupName.Equals(groupName));
            if (template == null)
            {
                template = new GroupTemplate
                {
                    Alias = alias,
                    CurriculumGroupName = groupName,
                    SemesterGroupName = semesterGroup
                };

                _db.GroupTemplates.Add(template);
                _db.SaveChanges();
            }
            */
            return group;
        }


        public Data.Curriculum GetCurriculum(string shortName)
        {
            return _db.Curricula.SingleOrDefault(d => d.ShortName.Equals(shortName, StringComparison.OrdinalIgnoreCase));
        }

        public ActivityOrganiser GetOrganiser(string shortName)
        {
            return _db.Organisers.SingleOrDefault(d => d.ShortName.Equals(shortName, StringComparison.OrdinalIgnoreCase));
        }

    }
}
