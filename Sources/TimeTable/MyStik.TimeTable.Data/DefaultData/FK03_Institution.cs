namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK03()
        {
            var fk03 = GetOrganiser("FK 03");

            if (fk03 != null) return;

            fk03 = new ActivityOrganiser
            {
                Name = "Fakultät 03",
                ShortName = "FK 03",
                IsFaculty = true,
                IsStudent = false,
            };
           
            _db.Organisers.Add(fk03);
            _db.SaveChanges();
        }

        public void InitMemberFK03()
        {
            var fk03 = GetOrganiser("FK 03");
            AddMember(fk03, "AMF", "Amft");
            AddMember(fk03, "BUC", "Buch");
            AddMember(fk03, "DAH", "Dahn");
            AddMember(fk03, "DOL", "Doll");
            AddMember(fk03, "EIC", "Eiche");
            AddMember(fk03, "ENG", "Engelberger");
            AddMember(fk03, "EUR", "Eursch");
            AddMember(fk03, "FRT", "Fritsch");
            AddMember(fk03, "GIT", "Gitterle");
            AddMember(fk03, "GRA", "Grabner");
            AddMember(fk03, "GUB", "Gubner");
            AddMember(fk03, "HAK", "Hakenesch");
            AddMember(fk03, "HEN", "Henze");
            AddMember(fk03, "HOR", "Hornfecke");
            AddMember(fk03, "HOS", "Horoschenkoff");
            AddMember(fk03, "HUB", "Huber");
            AddMember(fk03, "KLI", "Klippel");
            AddMember(fk03, "KNA", "Knauer");
            AddMember(fk03, "KNI", "Kniesner");
            AddMember(fk03, "KNO", "Knoll");
            AddMember(fk03, "KRA", "Krafft");
            AddMember(fk03, "KRU", "Krug");
            AddMember(fk03, "LOE", "Löw");
            AddMember(fk03, "LOR", "Lorenz");
            AddMember(fk03, "LAN", "Langhorst");
            AddMember(fk03, "MAU", "Maurer");
            AddMember(fk03, "MID", "Middendorf");
            AddMember(fk03, "MIN", "Mintzlaff");
            AddMember(fk03, "MOE", "Möller");
            AddMember(fk03, "MUE", "Müller-Syhre");
            AddMember(fk03, "NIT", "Nitzsche");
            AddMember(fk03, "N.N.", "N.N.");
            AddMember(fk03, "PAL", "Palme");
            AddMember(fk03, "PFE", "Pfeffer");
            AddMember(fk03, "POE", "Pöschl");
            AddMember(fk03, "POK", "Pokluda");
            AddMember(fk03, "RAS", "Rascher");
            AddMember(fk03, "RAU", "Rau");
            AddMember(fk03, "REI", "Reichl");
            AddMember(fk03, "ROH", "Rohnen");
            AddMember(fk03, "ROT", "Rother");
            AddMember(fk03, "SAI", "Sailer");
            AddMember(fk03, "SCH", "Schiebener");
            AddMember(fk03, "SCL", "Schlüchtermann");
            AddMember(fk03, "SCR", "Schröpfer");
            AddMember(fk03, "SCW", "Schwerin");
            AddMember(fk03, "SEE", "Seefried");
            AddMember(fk03, "SEL", "Selting");
            AddMember(fk03, "SEN", "Sentpali");
            AddMember(fk03, "SIE", "Siebold");
            AddMember(fk03, "SPE", "Sperl");
            AddMember(fk03, "STO", "Stoll");
            AddMember(fk03, "THI", "Thiessen");
            AddMember(fk03, "TIL", "Tille");
            AddMember(fk03, "URB", "Urban");
            AddMember(fk03, "VIE", "Vielemeyer");
            AddMember(fk03, "WAN", "Wandinger");
            AddMember(fk03, "WAR", "Warendorf");
            AddMember(fk03, "WES", "Westenthanner");
            AddMember(fk03, "WIL", "Wilhelm");
            AddMember(fk03, "WOL", "Wolfsteiner");
            AddMember(fk03, "YUA", "Yuan");
            AddMember(fk03, "ZAN", "Zanker");
            AddMember(fk03, "ZAU", "Zauner", true);
        }

        public void InitCurriculaFK03()
        {
            var fk03 = GetOrganiser("FK 03");

            FK03_InitCurriculumFAB(fk03);
            FK03_InitCurriculumLRB(fk03);
            FK03_InitCurriculumMBB(fk03);
            FK03_InitCurriculumFEM_TBM_FAM_LRM_MBM(fk03);
        }

        public void InitModulesFK03()
        {
            var fk03 = GetOrganiser("FK 03");

            InitModulkatalogFAB(fk03);
            InitModulkatalogLRB(fk03);
            InitModulkatalogMB(fk03);
            InitModulkatalogMBB(fk03);
        }

    }

}