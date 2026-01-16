using System;
using System.Collections.Generic;
using System.Text;

namespace StaxiLogging.Models
{
    public class LogSearchOptions
    {


        public string SearchTerm { get; set; } = string.Empty;
        public string Application { get; set; } = string.Empty;
        public string RequestApi { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int PageSize { get; set; } = 20;
        public int PageIndex { get; set; } = 1;

    }
}
