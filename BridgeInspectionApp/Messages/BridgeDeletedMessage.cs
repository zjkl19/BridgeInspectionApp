﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeInspectionApp.Messages;

public class BridgeDeletedMessage(Guid bridgeId)
{
    public Guid BridgeId { get; } = bridgeId;
}
