using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeInspectionApp.Models;
public class Bridge
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public List<Defect> Defects { get; set; }
}

public class Defect
{
    public int Id { get; set; }
    public string ComponentPart { get; set; }
    public string DefectType { get; set; }
    public string DefectLocation { get; set; }
    public string DefectSeverity { get; set; }
    public string Description { get; set; }
}
