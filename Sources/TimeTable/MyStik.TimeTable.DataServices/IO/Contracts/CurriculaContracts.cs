using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.IO.Contracts
{
    public class CurriculumApiContract
    {
        public string InstitutionId { get; set; }

        public string OrganiserId { get; set; }

        /// <summary>
        /// Das ist der Tag, d.h. der primuss code
        /// </summary>
        public string CurriculumId { get; set; }
        // Es sollte ein Versionsdatum geben, wenn Id angegeben ist
        public DateTime? Version { get; set; }

        // Altrnative zu Id und Version
        public string Alias { get; set; }

        public string Title { get; set; }

        public string Level { get; set; }

        public string Degree { get; set; }

        public List<CurriculumSegmentApiContract> Segments { get; set; }
    }

    public class CurriculumSegmentApiContract
    {
        // Das Fachsemester, in dem der Slot angeboten wird
        public int Position { get; set; }

        public string Title { get; set; }

        public List<CurriculumSlotApiContract> Slots { get; set; }
    }

    public class CurriculumSlotApiContract
    {
        public string TopicId { get; set; }

        public string OptionId { get; set; }

        public string SlotId { get; set; }


        
    }

    public class CurriculumAccreditionApiContract
    {
        public string InstitutionId { get; set; }

        public string OrganiserId { get; set; }

        public string CatalogId { get; set; }

        public string ModuleId { get; set; }

        public string SubjectId { get; set; }

    }
}
