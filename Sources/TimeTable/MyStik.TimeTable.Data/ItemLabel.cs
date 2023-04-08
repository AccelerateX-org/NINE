using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class ItemLabel
    {
        public ItemLabel()
        {
            LabelSets = new HashSet<ItemLabelSet>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string HtmlColor { get; set; }


        /// <summary>
        /// gesetzt: Lokal
        /// null: global
        /// </summary>
        //public virtual ActivityOrganiser Organiser { get; set; }


        public virtual ICollection<ItemLabelSet> LabelSets { get; set; }

    }


    public class ItemLabelSet
    {
        public ItemLabelSet()
        {
            ItemLabels = new HashSet<ItemLabel>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual ICollection<ItemLabel> ItemLabels { get; set; }

    }
}
