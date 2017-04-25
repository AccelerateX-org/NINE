using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Quest.Models
{
    public class QuizQuestionViewModel
    {
        /// <summary>
        /// Die aktuell angezeigte Frage
        /// </summary>
        public Question Question { get; set; }

        /// <summary>
        /// der aktuelle Benutzer
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Die bisher gegebene Antwort, falls vorhanden
        /// </summary>
        public QuestionAnswer Answer { get; set; }

        public int NumQuestions { get; set; }

        public bool IsFirstQuestion { get; set; }
    }
}