using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ImportReportViewModel
    {
        /// <summary>
        /// Name der Datei
        /// </summary>
        public string Name { get; set; }
    }


    public class CurriculumImportModel
    {
        public Curriculum Curriculum { get; set; }

        public HttpPostedFileBase AttachmentStructure { get; set; }

    }

    public class OrganiserImportModel
    {
        public ActivityOrganiser Organiser { get; set; }

        public HttpPostedFileBase AttachmentStructure { get; set; }

    }

}