namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogWI(ActivityOrganiser fk09)
        {
            var wi = GetCurriculum(fk09, "WI");

            InitCatalogWI_GS(fk09, wi);

            InitCatalogWI_Studienrichtungen(fk09, wi);
            
            InitCatalogWI_Integration(fk09, wi);
            
            InitCatalogWI_Wahl(fk09, wi);
        }
    }
}
