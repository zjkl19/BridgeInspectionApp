using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeInspectionApp.Messages;

public class BridgeDeletedMessage
{
    public Guid BridgeId { get; }

    public BridgeDeletedMessage(Guid bridgeId)
    {
        BridgeId = bridgeId;
    }
}
