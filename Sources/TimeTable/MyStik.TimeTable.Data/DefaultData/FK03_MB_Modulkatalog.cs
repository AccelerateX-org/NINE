namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogMB(ActivityOrganiser fk03)
        {
            var mb = GetCurriculum(fk03, "MB");
        }
    }
}
