namespace MyStik.TimeTable.DataServices
{


    public partial class InfrastructureDataService
    {
        
        private string FK01 = "FK 01";
        private string FS01 = "FK 01";
        private string FK03 = "FK 03";
        private string FS03 = "FS 03";
        private string FK04 = "FK 04";
        private string FS04 = "FK 04";
        private string FK06 = "FK 06";
        private string FS06 = "FS 06";
        private string FK09 = "FK 09";
        private string FS09 = "FS 09";
        private string HM = "HM";

        /*
        private string AR = "AR";
        private string FAB = "FAB";
        private string LRB = "LRB";
        private string MBB = "MBB";
        private string FEM_TBM_FAM_LRM_MBM = "FEM_TBM_FAM_LRM_MBM";
        private string BA = "BA";
        private string EIB = "EIB";
        private string REB = "REB";
        private string EMB = "EMB";
        private string ELM = "ELM";
        private string EEM = "EEM";
        private string SSM = "SSM";
        */
        private string WI = "WI";
        private string AU = "AU";
        private string LM = "LM";
        private string WIM = "WIM";
        private string MBA = "MBA";
        //Fakultät 06
        private string AOB = "AOB";
        private string BBM = "BBM";
        private string BOB = "BOB";
        private string CHB = "CHB";
        private string MFB = "MFB";
        private string MFM = "MFM";
        private string MNM = "MNM";
        private string PAB = "PAB";
        private string PAA = "PAA";
        //private string PHB = "PHB";
        private string PNB = "PNB";
        private string POM = "POM";

        /// <summary>
        /// 
        /// </summary>
        public void InitDataFK09()
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


        public void InitDataFK01()
        {
            AddOrganiser(HM, "Hochschule München", false, false);
            AddOrganiser(FK01, "Fakultät 01", true, false);
            AddOrganiser(FS01, "Fachschaft 01", false, true);

            AddCurriculum(WI, "Bachelor Architektur", FK01);
        }

        public void InitDataFK03()
        {
            AddOrganiser(HM, "Hochschule München", false, false);
            AddOrganiser(FK03, "Fakultät 03", true, false);
            AddOrganiser(FS03, "Fachschaft 03", false, true);

            AddCurriculum(WI, "Bachelor Fahrzeugtechnik", FK03);
            AddCurriculum(AU, "Bachelor Luft- und Raumfahrtechnik", FK03);
            AddCurriculum(LM, "Bachelor Maschinenbau", FK03);
            AddCurriculum(WIM, "Master FEM_TBM_FAM_LRM_MBM", FK03);
            AddCurriculum("CIE", "Courses in English", FK03);
            /*AddCurriculum(MBA, "Master of Business Administration and Engineering", FK03);         
            AddCurriculum("Export", "Export", HM);
            */
        }
        public void InitDataFK04()
        {
            AddOrganiser(HM, "Hochschule München", false, false);
            AddOrganiser(FK04, "Fakultät 04", true, false);
            AddOrganiser(FS04, "Fachschaft 04", false, true);

            AddCurriculum(WI, "Bachelor Erstsemestergruppen", FK04);
            AddCurriculum(AU, "Bachelor Luft- und Raumfahrtechnik", FK04);
            AddCurriculum(LM, "Bachelor Elektro- & Informationtechnik", FK04);
            AddCurriculum(WIM, "Bachelor Regenerative Energien", FK04);
            AddCurriculum(WIM, "Bachelor Elektromobilität", FK04);
            AddCurriculum(WIM, "Master Elektrotechnik", FK04);
            AddCurriculum(WIM, "Master Electrical Engineering", FK04);
            AddCurriculum(WIM, "Master Systems Engineering", FK04);
            AddCurriculum("CIE", "Courses in English", FK04);
            /*AddCurriculum(MBA, "Master of Business Administration and Engineering", FK03);         
            AddCurriculum("Export", "Export", HM);
            */
        }
        public void InitDataFK06()
        {
            AddOrganiser(HM, "Hochschule München", false, false);
            AddOrganiser(FK06, "Fakultät 06", true, false);
            AddOrganiser(FS06, "Fachschaft 06", false, true);

            AddCurriculum(AOB, "Bachelor Augenoptik und Optometrie", FK06);
            AddCurriculum(BBM, "Master  Biotechnologie und Bioingenieurwesen", FK06);
            AddCurriculum(BOB, "Bachelor Bioingenieurwesen", FK06);
            AddCurriculum(CHB, "Bachelor  Chemische Technik", FK06);
            AddCurriculum(MFB, "Bachelor  Mechatronik und Feinwerktechnik", FK06);
            AddCurriculum(MFM, "Master  Mechatronik und Feinwerktechnik", FK06);
            AddCurriculum(MNM, "Master  Mikotechnik und Nanotechnik", FK06);
            AddCurriculum(PAB, "Bachelor Produktion und Automatisierung", FK06);
            AddCurriculum(PAA, "Bachelor Produktion und Automatisierung (D/F)", FK06);
            AddCurriculum(PNB, "Bachelor Produktion und Automatisierung (national)", FK06);
            AddCurriculum(POM, "Master Photonik", FK06);



        }


    }
}
