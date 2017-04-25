using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Areas.InfoScreen.Models
{
    public class InfoscreenModel
    {
        public ICollection<RoomInfoModel> CurrentFreeRooms { get; set; }
        public ICollection<RoomInfoModel> NextFreeRooms { get; set; } 
        
        public NextEventContract NextEvent { get; set; }
        public ICollection<MvvViewModel> MVVallDepartures { get; set; }

        public ICollection<ActivityDate> NowPlayingDates { get; set; }
        public ICollection<ActivityDate> UpcomingDates { get; set; }

        public MensaView_Tag SpeiseplanHeute { get; set; }
    }

    public class MvvViewModel
    {
        public string Liniennummer { get; set; }
        public string Richtung { get; set; }
        public int AbfahrtszeitInMin { get; set; }
    }



    
    public class RoomViewModel
    {
        public string Raumnummer { get; set; }
        public string Beschreibung { get; set; }
        public string Status { get; set; }
        public int AnzahlSitze { get; set; }
             
        }

    public class NachmittagRechtsModel
    {
        public ICollection<InfoscreenEventViewModel> Events { get; set; }
    }

    public class InfoscreenEventViewModel
    {
        public DateTime Datum { get; set; }
        public string Titel { get; set; }

        public string Beschreibung { get; set; }

        public string Bild { get; set; }
        public string Ort { get; set; }

    }

    public class CanceledLecturesVíewModel
    {
        public string Vorlesung {get;set;}
        
        public DateTime Beginn {get; set;}

        public DateTime Ende { get; set; }

        public string Dozent { get; set; }
       
        public ICollection<OrganiserMember> Dozenten { get; set; }
        public ICollection<SemesterGroup> Studiengruppe { get; set; }
    }
    public class LinksVormittagsModel
    {
        public ICollection<CanceledLecturesVíewModel> CanceledLectures { get; set; }

        public MensaViewModel MensaSpeiseplan { get; set; }
    }

    public class InfoscreenEventsRightViewModel
    {
        public string Wochentag { get; set; }

        public ActivityDate Datum { get; set; }

        public DateTime Beginn { get; set; }

        public DateTime Ende { get; set; }

        public string Titel { get; set; }

        public string Raumnummer { get; set; }
    }

    public class RechtsEventsModel
    {
        public ICollection<InfoscreenEventsRightViewModel> InfoscreenEventsRight { get; set; }


    }

}
