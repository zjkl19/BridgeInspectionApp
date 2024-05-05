

namespace BridgeInspectionApp.Messages;
public class DefectUpdatedMessage(Guid defectId)
{
    public Guid DefectId { get; private set; } = defectId;
}

