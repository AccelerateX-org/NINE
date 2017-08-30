namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogKG()
        {
            var fk08 = GetOrganiser("FK 08");
            var kg = GetCurriculum(fk08, "KG");
        }
    }
}