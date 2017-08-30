namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogGUG()
        {
            var fk08 = GetOrganiser("FK 08");
            var GUG = GetCurriculum(fk08, "GUG");

            InitCatalogGUG_GS(fk08, GUG);

        }
    }
}
