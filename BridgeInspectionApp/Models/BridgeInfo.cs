using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeInspectionApp.Models
{
    public class BridgeInfo
    {
        public string Name { get; set; }
        public string ComponentPart { get; set; }
        public string DefectType { get; set; }
        public string DefectLocation { get; set; }
        public string DefectSeverity { get; set; }
    }
}
