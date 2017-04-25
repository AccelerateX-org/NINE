using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class Question
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Titel der Frage
        /// </summary>
        [DisplayName("Titel")]
        public string Title { get; set; }


        /// <summary>
        /// Uberschrift
        /// </summary>
      [DisplayName("Überschrift")]
        public string Uberschrift { get; set; }


        /// <summary>
        /// Fragetext
        /// </summary>
       [DisplayName("Text")]
        public string Text { get; set; }

        /// <summary>
        /// Reihenfolge innerhalb der Kategorie
        /// </summary>
       public int Reihenfolge { get; set; }


        /// <summary>
        /// Ein Bild (Optional)
        /// </summary>
       [DisplayName("Image")]
        public virtual BinaryStorage Image { get; set; }


        /// <summary>
        /// Zugehörige Kategorie der Frage
        /// </summary>
        public virtual QuestionCategory Category { get; set; }
        

        /// <summary>
        /// Liste der zugehörigen Antworten
        /// </summary>
        public virtual ICollection<QuestionAnswer> Answers { get; set; }

    }
}