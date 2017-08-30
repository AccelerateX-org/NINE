namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitModulkatalogMFB()
        {
            var fk06 = GetOrganiser("FK 06");
            var mfb = GetCurriculum(fk06, "MFB");

        }
    }
}
