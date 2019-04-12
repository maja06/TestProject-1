using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TestProject.Query
{
    public class RuleInfo : Entity
    {
        public string Property { get; set; }
        
        public string Operator { get; set; }
        
        public string Value { get; set; }

        public string Condition { get; set; }
        
        public List<RuleInfo> Rules = new List<RuleInfo>();
    }
}
