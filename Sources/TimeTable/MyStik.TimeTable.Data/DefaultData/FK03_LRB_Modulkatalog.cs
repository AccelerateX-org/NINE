namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogLRB(ActivityOrganiser fk03)
        {
            var LRB = GetCurriculum(fk03, "LRB");
        }
    }
}
