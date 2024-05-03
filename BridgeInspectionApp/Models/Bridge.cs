using System;
using System.Collections.Generic;

namespace BridgeInspectionApp.Models;

public class Bridge
{
    public Guid Id { get; set; } = Guid.NewGuid(); // 使用GUID作为ID
    public string Name { get; set; }
    public string? Location { get; set; }
    public string? MapId { get; set; } // 地图API的标识符或地理坐标
    public List<Defect> Defects { get; set; }
}

public class Defect
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BridgeId { get; set; } // 关联桥梁ID
    public string? ComponentPart { get; set; } // 构件部位
    public string? DefectType { get; set; } // 缺陷类型
    public string? DefectLocation { get; set; } // 缺损位置
    public string? DefectSeverity { get; set; } // 缺损程度
    public string? Note { get; set; } // 缺陷备注
    public List<Photo>? Photos { get; set; } // 关联的照片
}

public class Photo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DefectId { get; set; }
    public string? FilePath { get; set; }
    public string? Note { get; set; }
}
