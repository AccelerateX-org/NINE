using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class PersonalContact
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Name etc. steckt in der UserDb
        /// </summary>
        public string UserId { get; set; }

        public string Department { get; set; }

        public virtual CorporateContact Corporate { get; set; }

        /// <summary>
        /// deprecated
        /// </summary>
        public virtual OrganiserMember Owner { get; set; }

        /// <summary>
        /// Der einladende User
        /// </summary>
        public string HostUserId { get; set; }

        public DateTime Created { get; set; }


    }
}
