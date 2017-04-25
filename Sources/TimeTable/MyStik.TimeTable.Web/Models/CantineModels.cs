using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Models
{
    public class MensaViewModel
    {
        public ICollection<MensaView_Tag> Tage { get; set;  }
        public ICollection<VorschauView_Tag> Vorschautage { get; set; }
    }

    public class MensaView_Tag
    {
        public string date { get; set; }
        public string closed { get; set; }
        public ICollection<MensaView_Meal> meals { get; set; }
    }
    public class MensaView_Meal
    {
        public int id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public MensaView_Prices prices { get; set; }
        public List<string> notes { get; set; }
    }
    public class MensaView_Prices
    {
        public double price_student { get; set; }
        public double price_employees { get; set; }
        public double price_pupils { get; set; }
        public double price_others { get; set; }
        public bool allPricesAreZero()
        {
            return (price_employees + price_others + price_student + price_pupils == 0);
        }
    }
    /*Vorschau*/

      public class VorschauView_Tag
    {
        public string Datum { get; set; }
        public string Name { get; set; }
        public bool closed { get; set; }
        public ICollection<VorschauView_Gerichte> Gerichte { get; set; }
    }
    public class VorschauView_Gerichte
    {
        public string Name { get; set; }
        public string Kategorie { get; set; }
        public List<string> Notizen { get; set; }
        public string PreisStudent { get; set; }
        public string PreisMitarbeiter { get; set; }
        public string PreisGaeste { get; set; }
    } 



    //===================================== StuCafe

    public class StuCafeViewModel
    {
        public string Name { get; set; }

        public ICollection<StuCafeGerichteViewModel> Gerichte { get; set; }
        public ICollection<StuCafeSnacksViewModel> Snacks { get; set; }
        public ICollection<StuCafeGetraenkViewModel> Getraenke { get; set; }


    }
    public class StuCafeGerichteViewModel
    {
        public string Name { get; set; }
        public string Preis { get; set; }
    }
    public class StuCafeSnacksViewModel
    {
        public string Name { get; set; }
        public string Preis { get; set; }
    }
    public class StuCafeGetraenkViewModel
    {
        public string Name { get; set; }
        public string Preis { get; set; }
    }
}