namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogMBB(ActivityOrganiser fk03)
        {
            var MBB = GetCurriculum(fk03, "MBB");
        }
    }
}
