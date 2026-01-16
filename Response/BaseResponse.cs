using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaxiLogging.Response
{
    public class BaseResponse<T>
    {
        public int PageSize { get; set; } = 20;
        public int PageIndex { get; set; } = 1;
        public long TotalCount { get; set; }
        public int TotalPage => (int)Math.Ceiling((double)this.TotalCount / this.PageSize);
        public bool NextPage => this.PageIndex < this.TotalPage;
        public bool HasPrevious => this.PageIndex > 1 && this.PageIndex <= this.TotalPage;
        public List<T> Items { get; set; } = new List<T>();
    }
}
