﻿using System.Collections.Generic;

namespace TestProject.Query
{
    public class FilterInfo
    {
        public string Condition { get; set; }

        public List<RuleInfo> Rules = new List<RuleInfo>();
    }
}
