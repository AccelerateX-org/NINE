using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.SqlServer.Server;

namespace MyStik.TimeTable.Data
{
    public class ActivityOwner
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual OrganiserMember Member { get; set; }

        public string PlanningPreferences { get; set; }

        /// <summary>
        /// Ein Owner, der keine Änderungen vornehmen darf
        /// </summary>
        public bool IsLocked { get; set; }

    }
}
