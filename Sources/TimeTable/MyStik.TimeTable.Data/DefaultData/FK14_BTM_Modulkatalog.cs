namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogBTM()
        {
            var fk14 = GetOrganiser("FK 14");
            var BTM = GetCurriculum(fk14, "BTM");

            //InitCatalogBTM_GS(fk14, BTM);

            //InitCatalogBTM_Studienrichtungen(fk14, BTM);

            //InitCatalogBTM_Integration(fk14, BTM);

            //InitCatalogBTM_Wahl(fk14, BTM);
        }
    }
}
