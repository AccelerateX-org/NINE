using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogMGEO()
        {
            var fk08 = GetOrganiser("FK 08");
            var MGEO = GetCurriculum(fk08, "MGEO");

            InitCatalogMGEO_Studiengänge(fk08, MGEO);

        }
    }
}