using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class QuestLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual User User { get; set; }

        /// <summary>
        /// Um diese Frage geht es
        /// </summary>
        public virtual Question Question { get; set; }

        /// <summary>
        /// Zeitpunkt der erstmaligen Anzeige
        /// </summary>
        public virtual DateTime FirstView { get; set; }

        /// <summary>
        /// Zeitpunkt der letzen Aktion "weiter" oder "Ende"
        /// oder nichts, wenn Browser zugemacht
        /// </summary>
        public virtual DateTime? LastAction { get; set; }

        /// <summary>
        /// Zählt die Anzahl wie oft die Frage angezeigt wurde
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// Kürzeste Anzeigedauer
        /// </summary>
        public int MinViewDuration { get; set; }

        /// <summary>
        /// Längste Anzeigedauer
        /// </summary>
        public int MaxViewDuration { get; set; }

        /// <summary>
        /// Eine Antwort oder keine
        /// </summary>
        public virtual QuestionAnswer Answer { get; set; }

    }
}