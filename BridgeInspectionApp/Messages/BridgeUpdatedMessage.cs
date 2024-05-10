using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeInspectionApp.Messages;


public class BridgeUpdatedMessage(Guid Id)
{
    public Guid Id { get; private set; } = Id;
}


