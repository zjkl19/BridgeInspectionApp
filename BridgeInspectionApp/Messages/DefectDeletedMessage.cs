using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeInspectionApp.Messages;

public class DefectDeletedMessage(Guid defectId)
{
    public Guid DefectId { get; } = defectId;
}