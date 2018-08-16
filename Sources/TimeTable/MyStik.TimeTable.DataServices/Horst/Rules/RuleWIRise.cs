using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Horst.Rules
{
    public class RuleWIRise
    {
        /// <summary>
        /// Vorrückensregel WI
        /// ins Dritte darf nur, wer
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public RuleEvaluation Evaluate(CurriculumRequirement req)
        {
            var eval = new RuleEvaluation();

            var fullFillmentSemester = GetFullfillmentSemester(req);
            // Gilt nur für 3 Semester und aufwärts
            if (fullFillmentSemester <= 2)
            {
                eval.HasImpact = false;
                return eval;
            }

            eval.HasImpact = true;

            // Mathe + 8 weitere Module






            return eval;
        }

        /// <summary>
        /// Es ist der Anfang, nicht das Ende
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private int GetFullfillmentSemester(CurriculumRequirement req)
        {
            return req.Criterias.Min(x => x.Term);
        }

        public List<CurriculumRequirement> GetAsRiseRule()
        {
            return null;
        }


    }
}
