using System;
using System.Collections.Generic;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Areas.InfoScreen.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class InfoscreenModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ICollection<RoomInfoModel> CurrentFreeRooms { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<RoomInfoModel> NextFreeRooms { get; set; } 
        /// <summary>
        /// 
        /// </summary>
        public NextEventContract NextEvent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<MvvViewModel> MVVallDepartures { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<ActivityDate> NowPlayingDates { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<ActivityDate> UpcomingDates { get; set; }

        public ICollection<RoomScheduleViewModel> RoomSchedules { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MensaView_Tag SpeiseplanHeute { get; set; }

        public InfoscreenPage Page { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MvvViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Liniennummer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Richtung { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AbfahrtszeitInMin { get; set; }
    }



    /// <summary>
    /// 
    /// </summary>
    public class RoomScheduleViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<ActivityDate> CurrentDates { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<ActivityDate> NextDates { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NachmittagRechtsModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ICollection<InfoscreenEventViewModel> Events { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InfoscreenEventViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime Datum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Titel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Beschreibung { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Bild { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Ort { get; set; }
//        public string Titel { get; set; }
//        public string Raumnummer { get; set; }


    }

    /// <summary>
    /// 
    /// </summary>
    public class CanceledLecturesVíewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Vorlesung {get;set;}
        /// <summary>
        /// 
        /// </summary>
        public DateTime Beginn {get; set;}
        /// <summary>
        /// 
        /// </summary>
        public DateTime Ende { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Dozent { get; set; }
       /// <summary>
       /// 
       /// </summary>
        public ICollection<OrganiserMember> Dozenten { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<SemesterGroup> Studiengruppe { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class LinksVormittagsModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ICollection<CanceledLecturesVíewModel> CanceledLectures { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MensaViewModel MensaSpeiseplan { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InfoscreenEventsRightViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Wochentag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ActivityDate Datum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Beginn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Ende { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Beschreibung { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Titel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Kurzbeschreibung { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Raumnummer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Fakultät { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AmInfoscreenAnzeigen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Infotext { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RechtsEventsModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ICollection<InfoscreenEventsRightViewModel> InfoscreenEventsRight { get; set; }


    }

}
