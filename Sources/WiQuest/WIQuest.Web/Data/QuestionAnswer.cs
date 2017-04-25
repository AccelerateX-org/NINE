using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace WIQuest.Web.Data
{
    public class QuestionAnswer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Antworttext
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Ein Bild (optional)
        /// </summary>
        public virtual BinaryStorage Image { get; set; }

        /// <summary>
        /// Antwort korrekt
        /// </summary>
        public bool IsCorrect { get; set; }

        public int Reihenfolge { get; set; }


        /// <summary>
        /// Zugehörige Frage
        /// </summary>
        public virtual Question Question { get; set; }

    }
}