namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalog_FK06_BOB()
        {
            var fk06 = GetOrganiser("FK 06");
            var BOB = GetCurriculum(fk06, "BOB");

            InitCatalogBOB_GS(fk06, BOB);

            InitCatalogBOB_ab3(fk06, BOB);

            InitCatalogBOB_Wahl(fk06, BOB);
        }
    }
}
