using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Horst.Rules
{
    public interface ICurriculumRule
    {
        RuleEvaluation Evaluate(CurriculumRequirement req);


    }
}
