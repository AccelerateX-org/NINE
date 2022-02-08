using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class ModuleCatalog
    {
        public ModuleCatalog()
        {
            Publishings = new HashSet<ModulePublishing>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual OrganiserMember Owner { get; set; }

        public bool IsPublic { get; set; }

        public ICollection<ModulePublishing> Publishings { get; set; }
    }

    public class ModulePublishing
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual ModuleCatalog Catalog { get; set; }


        public virtual TeachingBuildingBlock Module { get; set; }
    }
}
