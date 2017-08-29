namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogEG(ActivityOrganiser fk05)
        {
            var eg = GetCurriculum(fk05, "EG");

        }
    }
}

