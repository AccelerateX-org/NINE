using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.Horst.Rules
{
    public class RuleEvaluation
    {
        /// <summary>
        /// Hat Einfluss
        /// Nein heisst ist nicht relevant
        /// </summary>
        public bool HasImpact { get; set; }

        /// <summary>
        /// Ist möglich, belegbar
        /// Nein heisst, es fehlt eine Voraussetzung
        /// </summary>
        public bool IsPossible { get; set; }

        /// <summary>
        /// Ist ein MUSS
        /// </summary>
        public bool IsMandatory { get; set; }

    }
}
