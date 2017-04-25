using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices
{
    public partial class InfrastructureDataService
    {
        private string FK09 = "FK 09";
        private string FS09 = "FS 09";
        private string HM = "HM";

        private string WI = "WI";
        private string AU = "AU";
        private string LM = "LM";
        private string WIM = "WIM";
        private string MBA = "MBA";

        public void InitData()
        {
            AddOrganiser(HM, "Hochschule München", false, false);
            AddOrganiser(FK09, "Fakultät 09", true, false);
            AddOrganiser(FS09, "Fachschaft 09", false, true);

            AddCurriculum(WI, "Bachelor Wirtschaftsingenieurwesen", FK09);
            AddCurriculum(AU, "Bachelor Wirtschaftsingenieurwesen Automobilindustrie", FK09);
            AddCurriculum(LM, "Bachelor Wirtschaftsingenieurwesen Logistik", FK09);
            AddCurriculum(WIM, "Master Wirtschaftsingenieurwesen", FK09);
            AddCurriculum(MBA, "Master of Business Administration and Engineering", FK09);
            AddCurriculum("CIE", "Courses in English", FK09);
            AddCurriculum("Export", "Export", HM);

            // Sudiengang
            // Curriculumsgruppe ("SPO")
            // Alias gpUntis
            // korrespondierende Semestergruppe
            #region WI
            AddCurriculumGroup(WI, "1", "1A", "A");
            AddCurriculumGroup(WI, "1", "1B", "B");
            AddCurriculumGroup(WI, "1", "1C", "C");
            
            AddCurriculumGroup(WI, "2", "2A", "A");
            AddCurriculumGroup(WI, "2", "2B", "B");
            AddCurriculumGroup(WI, "2", "2C", "C");

            AddCurriculumGroup(WI, "3 BIO", "3Bio", "");
            AddCurriculumGroup(WI, "3 INF", "3Inf", "");
            AddCurriculumGroup(WI, "3 TEC", "3Tec", "G1");
            AddCurriculumGroup(WI, "3 TEC", "3Tec-G2", "G2");
            AddCurriculumGroup(WI, "3 BIO", "3Bio/Inf", "");
            AddCurriculumGroup(WI, "3 INF", "3Bio/Inf", "");
            AddCurriculumGroup(WI, "3 BIO", "3Inf/Bio", "");
            AddCurriculumGroup(WI, "3 INF", "3Inf/Bio", "");
            AddCurriculumGroup(WI, "3 INF", "3Inf/Tec", "");
            AddCurriculumGroup(WI, "3 TEC", "3Inf/Tec", "G1");
            AddCurriculumGroup(WI, "3 BIO", "3Bio/Tec", "");
            AddCurriculumGroup(WI, "3 TEC", "3Bio/Tec", "G1");
            AddCurriculumGroup(WI, "3 TEC", "3Tec-G2/Inf", "G2");
            AddCurriculumGroup(WI, "3 INF", "3Tec-G2/Inf", "");
            AddCurriculumGroup(WI, "3 TEC", "3Tec-G2/Bio", "G2");
            AddCurriculumGroup(WI, "3 BIO", "3Tec-G2/Bio", "");

            AddCurriculumGroup(WI, "4 BIO", "4Bio", "");
            AddCurriculumGroup(WI, "4 INF", "4Inf", "");
            AddCurriculumGroup(WI, "4 TEC", "4Tec", "G1");
            AddCurriculumGroup(WI, "4 TEC", "4Tec-G2", "G2");
            AddCurriculumGroup(WI, "4 BIO", "4Bio/Inf", "");
            AddCurriculumGroup(WI, "4 INF", "4Bio/Inf", "");
            AddCurriculumGroup(WI, "4 BIO", "4Inf/Bio", "");
            AddCurriculumGroup(WI, "4 INF", "4Inf/Bio", "");
            AddCurriculumGroup(WI, "4 INF", "4Inf/Tec", "");
            AddCurriculumGroup(WI, "4 TEC", "4Inf/Tec", "G1");
            AddCurriculumGroup(WI, "4 BIO", "4Bio/Tec", "");
            AddCurriculumGroup(WI, "4 TEC", "4Bio/Tec", "G1");
            AddCurriculumGroup(WI, "4 TEC", "4Tec-G2/Inf", "G2");
            AddCurriculumGroup(WI, "4 INF", "4Tec-G2/Inf", "");
            AddCurriculumGroup(WI, "4 TEC", "4Tec-G2/Bio", "G2");
            AddCurriculumGroup(WI, "4 BIO", "4Tec-G2/Bio", "");


            AddCurriculumGroup(WI, "5 BIO", "5Bio", "");
            AddCurriculumGroup(WI, "5 INF", "5Inf", "");
            AddCurriculumGroup(WI, "5 TEC", "5Tec", "G1");
            AddCurriculumGroup(WI, "5 TEC", "5Tec-G2", "G2");
            AddCurriculumGroup(WI, "5 BIO", "5Bio/Inf", "");
            AddCurriculumGroup(WI, "5 INF", "5Bio/Inf", "");
            AddCurriculumGroup(WI, "5 BIO", "5Inf/Bio", "");
            AddCurriculumGroup(WI, "5 INF", "5Inf/Bio", "");
            AddCurriculumGroup(WI, "5 INF", "5Inf/Tec", "");
            AddCurriculumGroup(WI, "5 TEC", "5Inf/Tec", "G1");
            AddCurriculumGroup(WI, "5 BIO", "5Bio/Tec", "");
            AddCurriculumGroup(WI, "5 TEC", "5Bio/Tec", "G1");
            AddCurriculumGroup(WI, "5 TEC", "5Tec-G2/Inf", "G2");
            AddCurriculumGroup(WI, "5 INF", "5Tec-G2/Inf", "");
            AddCurriculumGroup(WI, "5 TEC", "5Tec-G2/Bio", "G2");
            AddCurriculumGroup(WI, "5 BIO", "5Tec-G2/Bio", "");

            AddCurriculumGroup(WI, "6 BIO", "6Bio", "");
            AddCurriculumGroup(WI, "6 INF", "6Inf", "");
            AddCurriculumGroup(WI, "6 TEC", "6Tec", "G1");
            AddCurriculumGroup(WI, "6 TEC", "6Tec-G2", "G2");
            AddCurriculumGroup(WI, "6 BIO", "6Bio/Inf", "");
            AddCurriculumGroup(WI, "6 INF", "6Bio/Inf", "");
            AddCurriculumGroup(WI, "6 BIO", "6Inf/Bio", "");
            AddCurriculumGroup(WI, "6 INF", "6Inf/Bio", "");
            AddCurriculumGroup(WI, "6 INF", "6Inf/Tec", "");
            AddCurriculumGroup(WI, "6 TEC", "6Inf/Tec", "G1");
            AddCurriculumGroup(WI, "6 BIO", "6Bio/Tec", "");
            AddCurriculumGroup(WI, "6 TEC", "6Bio/Tec", "G1");
            AddCurriculumGroup(WI, "6 TEC", "6Tec-G2/Inf", "G2");
            AddCurriculumGroup(WI, "6 INF", "6Tec-G2/Inf", "");
            AddCurriculumGroup(WI, "6 TEC", "6Tec-G2/Bio", "G2");
            AddCurriculumGroup(WI, "6 BIO", "6Tec-G2/Bio", "");

            AddCurriculumGroup(WI, "7 BIO", "7Bio", "");
            AddCurriculumGroup(WI, "7 INF", "7Inf", "");
            AddCurriculumGroup(WI, "7 TEC", "7Tec", "G1");
            AddCurriculumGroup(WI, "7 TEC", "7Tec-G2", "G2");
            AddCurriculumGroup(WI, "7 BIO", "7Bio/Inf", "");
            AddCurriculumGroup(WI, "7 INF", "7Bio/Inf", "");
            AddCurriculumGroup(WI, "7 BIO", "7Inf/Bio", "");
            AddCurriculumGroup(WI, "7 INF", "7Inf/Bio", "");
            AddCurriculumGroup(WI, "7 INF", "7Inf/Tec", "");
            AddCurriculumGroup(WI, "7 TEC", "7Inf/Tec", "G1");
            AddCurriculumGroup(WI, "7 BIO", "7Bio/Tec", "");
            AddCurriculumGroup(WI, "7 TEC", "7Bio/Tec", "G1");
            AddCurriculumGroup(WI, "7 TEC", "7Tec-G2/Inf", "G2");
            AddCurriculumGroup(WI, "7 INF", "7Tec-G2/Inf", "");
            AddCurriculumGroup(WI, "7 TEC", "7Tec-G2/Bio", "G2");
            AddCurriculumGroup(WI, "7 BIO", "7Tec-G2/Bio", "");
            AddCurriculumGroup(WI, "WPM", "WPM WI", "");
            #endregion

            #region AU
            AddCurriculumGroup(AU, "1", "1AU", "");
            AddCurriculumGroup(AU, "2", "2AU", "");
            AddCurriculumGroup(AU, "3", "3AU", "");
            AddCurriculumGroup(AU, "4", "4AU", "");
            AddCurriculumGroup(AU, "5", "5AU", "");
            AddCurriculumGroup(AU, "6", "6AU", "");
            AddCurriculumGroup(AU, "7", "7AU", "");
            AddCurriculumGroup(AU, "WPM", "WPM AU", "");
            #endregion

            #region LM
            AddCurriculumGroup(LM, "1", "1LM", "");
            AddCurriculumGroup(LM, "2", "2LM", "");
            AddCurriculumGroup(LM, "3", "3LM", "");
            AddCurriculumGroup(LM, "4", "4LM", "");
            AddCurriculumGroup(LM, "5", "5LM", "");
            AddCurriculumGroup(LM, "6", "6LM", "");
            AddCurriculumGroup(LM, "7", "7LM", "");
            AddCurriculumGroup(LM, "WPM", "WPM LM", "");
            #endregion

            #region WIM
            AddCurriculumGroup(WIM, "1", "WI M1", "G1");
            AddCurriculumGroup(WIM, "1", "WI M1 G2", "G2");
            AddCurriculumGroup(WIM, "2", "WI M2", "G1");
            AddCurriculumGroup(WIM, "2", "WI M2 G2", "G2");
            AddCurriculumGroup(WIM, "3", "WI M3", "G1");
            AddCurriculumGroup(WIM, "3", "WI M3 G2", "G2");
            AddCurriculumGroup(WIM, "WPM", "WPM WI-M", "");
            #endregion


            #region MBA
            AddCurriculumGroup(MBA, "1 NW", "WW M1 NW", "G1");
            AddCurriculumGroup(MBA, "1 NW", "WW M1 NW-G2", "G2");
            AddCurriculumGroup(MBA, "1 WI", "WW M1 WI", "");
            AddCurriculumGroup(MBA, "1 BAU", "WW M1 Bau", "");
            AddCurriculumGroup(MBA, "2 NW", "WW M2 NW", "G1");
            AddCurriculumGroup(MBA, "2 NW", "WW M2 NW-G2", "G2");
            AddCurriculumGroup(MBA, "2 WI", "WW M2 WI", "");
            AddCurriculumGroup(MBA, "2 BAU", "WW M2 Bau", "");
            AddCurriculumGroup(MBA, "4 NW", "WW M4 NW", "G1");
            AddCurriculumGroup(MBA, "4 NW", "WW M4 NW-G2", "G2");
            AddCurriculumGroup(MBA, "4 WI", "WW M4 WI", "");
            AddCurriculumGroup(MBA, "4 BAU", "WW M4 Bau", "");
            AddCurriculumGroup(MBA, "3", "WW M3", "");
            AddCurriculumGroup(MBA, "5", "WW M5", "");
            AddCurriculumGroup(WIM, "WPM", "WPM WW-M", "");
            #endregion

            AddCurriculumGroup("CIE", "CIE", "CIE", "");
            AddCurriculumGroup("Export", "Export", "Export", "");


        }
    }
}
