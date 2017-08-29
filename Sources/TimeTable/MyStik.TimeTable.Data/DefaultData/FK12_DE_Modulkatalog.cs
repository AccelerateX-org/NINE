namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogDE(ActivityOrganiser fk12)
        {
            var de = GetCurriculum(fk12, "DE");
        }

        public void InitModulkatalogMDE(ActivityOrganiser fk12)
        {
            var de = GetCurriculum(fk12, "MDE");
        }
    }
}

