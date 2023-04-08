using System;
using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseDto()
        {
            Actions = new List<ActionDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<ActionDto> Actions { get; }

        internal void AddAction(string name, string url)
        {
            Actions.Add(new ActionDto
            {
                Title = name,
                Href = url
            });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NamedDto : BaseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShortName { get; set; }

    }
}