namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogAW(ActivityOrganiser fk13)
        {
            var aw = GetCurriculum(fk13, "AW");
        }
    }
}
