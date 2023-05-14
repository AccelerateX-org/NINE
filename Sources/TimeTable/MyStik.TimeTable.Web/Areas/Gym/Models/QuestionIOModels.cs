using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Areas.Gym.Models
{
    public class QuestionCatalogIOModel
    {
        public string tag { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public List<QuestionIOModel> questions { get; set; }

    }

    public class QuestionIOModel
    {
        public string tag { get; set; }

        public string problem { get; set; }

        public List<AnswerIOModel> answers { get; set; }
    }

    public class AnswerIOModel
    {
        public string solution { get; set; }

        public bool iscorrect { get; set; }
    }
}