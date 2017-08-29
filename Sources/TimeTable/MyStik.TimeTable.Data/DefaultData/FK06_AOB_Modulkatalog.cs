namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogAOB()
        {
            var fk06 = GetOrganiser("FK 06");
            var AOB = GetCurriculum(fk06, "AOB");

            InitCatalogAOB_GS(fk06, AOB);

        }
    }
}
   