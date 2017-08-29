namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogFAB(ActivityOrganiser fk03)
        {
            var fab = GetCurriculum(fk03, "FAB");
        }
    }
}