using System;
using System.Collections.Generic;
using System.Text;
using TestProject.Query;

namespace TestProject.QueryInfo
{
    public class QueryInfo
    {
        public int Skip { get; set; }
        
        public int Take { get; set; }
        
        public string SearchText { get; set; }

        public List<string> SearchProperties { get; set; }
        
        public List<SortInfo> Sorters = new List<SortInfo>();
        
        public FilterInfo Filter { get; set; }

























    }
}
