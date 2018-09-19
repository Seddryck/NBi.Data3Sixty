using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Data3Sixty.Engine.Result
{
    public class RuleResult
    {
        public int ID { get; set; }
        public int? RuleImplementationID { get; set; }
        public string RuleImplementation { get; set; }
        
        public int RowsPassed { get; set; }
        public int RowsFailed { get; set; }
        public decimal PassFraction { get; set; }
        public decimal FailFraction { get; set; }
        public Boolean Passed { get; set; }

        public DateTime EffectiveDate { get; set; }
        public DateTime RunDate { get; set; }

        public Dictionary<string, string> Qualifiers { get; } = new Dictionary<string, string>();

    }
}
