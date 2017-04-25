using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Curriculum
    {
        public Curriculum()
        {
            CurriculumGroups = new HashSet<CurriculumGroup>();
            GroupAliases = new HashSet<GroupAlias>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public virtual ActivityOrganiser Organiser { get; set; }

        public virtual ICollection<CurriculumGroup> CurriculumGroups { get; set; }

        public virtual ICollection<GroupAlias> GroupAliases { get; set; }
    }
}
