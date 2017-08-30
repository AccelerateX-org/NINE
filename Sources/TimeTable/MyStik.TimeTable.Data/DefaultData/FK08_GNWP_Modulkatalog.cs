namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogGNWP()
        {
            var fk08 = GetOrganiser("FK 08");
            var gnwp = GetCurriculum(fk08, "GNWP");

        }
    }
}