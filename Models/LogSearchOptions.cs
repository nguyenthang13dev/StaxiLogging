using System;
using System.Collections.Generic;
using System.Text;

namespace StaxiLogging.Models
{
    public class LogSearchOptions
    {
        /// <summary>
        /// 1. Information
        /// 2. Error
        /// 3. Warning
        /// </summary>
        public string Loglevel { get; set; } 
        public string SearchTerm { get; set; } = string.Empty;
        public string Application { get; set; } = string.Empty;
        public string RequestApi { get; set; }
        public string Environment { get; set; } = string.Empty;
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int PageSize { get; set; } = 20;
        public int PageIndex { get; set; } = 1;

    }
}
