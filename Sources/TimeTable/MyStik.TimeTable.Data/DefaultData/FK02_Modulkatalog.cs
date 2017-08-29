namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogBAU(ActivityOrganiser fk02)
        {
            var bau = GetCurriculum(fk02, "BAU");

            InitCatalogBAU(fk02, bau);
        }


        public void InitModulkatalogBAUDUAL(ActivityOrganiser fk02)
        {
            var bau = GetCurriculum(fk02, "BAUDUAL");

            InitCatalogBAUDUAL(fk02, bau);
        }
    }
}

