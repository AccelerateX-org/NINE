using System;
using System.Linq;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        TimeTableDbContext _db = new TimeTableDbContext();

        private ActivityOrganiser GetOrganiser(string shortName)
        {
            return _db.Organisers.SingleOrDefault(o => o.ShortName.Equals(shortName, StringComparison.OrdinalIgnoreCase));
        }

        private Curriculum GetCurriculum(ActivityOrganiser org, string shortName)
        {
            return org.Curricula.SingleOrDefault(c => c.ShortName.Equals(shortName, StringComparison.OrdinalIgnoreCase));
        }

        private void AddMember(ActivityOrganiser org, string shortName, string name, bool isAdmin = false)
        {
            var member = org.Members.SingleOrDefault(m => m.ShortName.Equals(shortName, StringComparison.OrdinalIgnoreCase));
            if (member == null)
            {

                member = new OrganiserMember()
                {
                    ShortName = shortName,
                    IsAdmin = isAdmin
                };

                org.Members.Add(member);
            }

            member.Name = name;

            _db.SaveChanges();
        }


        public OrganiserMember GetHost(ActivityOrganiser org, string doz)
        {
            return org.Members.SingleOrDefault(m => m.ShortName.Equals(doz, StringComparison.OrdinalIgnoreCase));
        }

        public CurriculumGroup GetGroup(Curriculum curr, string name)
        {
            return curr.CurriculumGroups.SingleOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private void AddCapacityGroup(Curriculum curr, string name, string subGroupName, bool inWS, bool inSS, string[] aliases)
        {
            var group = GetGroup(curr, name);
            if (group == null)
                return;

            var capacityGroup = group.CapacityGroups.SingleOrDefault(g => g.Name.Equals(subGroupName));
            if (capacityGroup == null)
            {
                capacityGroup = new CapacityGroup()
                {
                    Name = subGroupName
                };
                group.CapacityGroups.Add(capacityGroup);
            }

            capacityGroup.InWS = inWS;
            capacityGroup.InSS = inSS;


            foreach (var alias in aliases)
            {
                var groupAlias = capacityGroup.Aliases.SingleOrDefault(a => a.Name.Equals(alias));
                if (groupAlias == null)
                {
                    groupAlias = new GroupAlias()
                    {
                        Name = alias
                    };
                    capacityGroup.Aliases.Add(groupAlias);
                }
            }
        }
    }
}
