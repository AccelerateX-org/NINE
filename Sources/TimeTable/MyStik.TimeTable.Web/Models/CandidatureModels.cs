using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CandidatureUploadMaterialModel
    {
        public CandidatureStage Stage { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public HttpPostedFileBase File { get; set; }

    }
}