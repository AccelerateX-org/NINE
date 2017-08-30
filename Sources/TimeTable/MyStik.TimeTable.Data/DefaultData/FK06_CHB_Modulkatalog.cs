namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogCHB()
        {
            var fk06 = GetOrganiser("FK 06");
            var CHB = GetCurriculum(fk06, "CHB");

            //InitCatalogCHB_GS(fk06, CHB);

            //InitCatalogCHB_Studienrichtungen(fk06, CHB);

            //InitCatalogCHB_Integration(fk06, CHB);

            //InitCatalogCHB_Wahl(fk06, CHB);
        }
    }
}
