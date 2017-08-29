using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class MensaViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ICollection<MensaView_Tag> Tage { get; set;  }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<VorschauView_Tag> Vorschautage { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MensaView_Tag
    {
        /// <summary>
        /// 
        /// </summary>
        public string date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string closed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<MensaView_Meal> meals { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MensaView_Meal
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MensaView_Prices prices { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> notes { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MensaView_Prices
    {
        /// <summary>
        /// 
        /// </summary>
        public double price_student { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double price_employees { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double price_pupils { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double price_others { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool allPricesAreZero()
        {
            return (price_employees + price_others + price_student + price_pupils == 0);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class VorschauView_Tag
    {
        /// <summary>
        /// 
        /// </summary>
        public string Datum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool closed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<VorschauView_Gerichte> Gerichte { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VorschauView_Gerichte
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Kategorie { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Notizen { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PreisStudent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PreisMitarbeiter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PreisGaeste { get; set; }
    } 



    //===================================== StuCafe
    /// <summary>
    /// 
    /// </summary>
    public class StuCafeViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<StuCafeGerichteViewModel> Gerichte { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<StuCafeSnacksViewModel> Snacks { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<StuCafeGetraenkViewModel> Getraenke { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StuCafeGerichteViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Preis { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StuCafeSnacksViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Preis { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StuCafeGetraenkViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Preis { get; set; }
    }
}