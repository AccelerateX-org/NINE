using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Areas.Gym.Models
{
    public class QuizIOModel
    {
        public string title { get; set; }
        public string description { get; set; }
        
        public List<QuizSectionIOModel> sections { get; set; }
    }

    public class QuizSectionIOModel
    {
        public string title { get; set; }
        public int position { get; set; }

        public List<QuizQuestionIOModel> questions { get; set; }
    }

    public class QuizQuestionIOModel
    {
        public string id { get; set; }
    }
}