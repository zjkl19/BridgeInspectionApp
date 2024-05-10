using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using BridgeInspectionApp.ViewModels;

namespace BridgeInspectionApp.Services;

public class WordDocumentCreator
{
    public void CreateWordDocument(string filePath, List<DefectViewModel> defects)
    {
        using (WordprocessingDocument doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
        {
            MainDocumentPart mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document();
            Body body = mainPart.Document.AppendChild(new Body());

            // 定义表格的列
            var columns = new List<(string Header, string Width)>
            {
                  ("构件部位", CmToTwips(2)),
                ("病害类型", CmToTwips(2)),
                ("缺损位置", CmToTwips(2)),
                ("缺损程度", CmToTwips(1)),
                ("缺陷备注", CmToTwips(1)),
                ("照片名字", CmToTwips(3))
            };

            // 按桥梁名称分组
            var groupedDefects = defects.GroupBy(d => d.BridgeName);

            foreach (var bridgeDefects in groupedDefects)
            {
                // 添加桥梁名称
                Paragraph para = body.AppendChild(new Paragraph());
                Run run = para.AppendChild(new Run());
                run.AppendChild(new Text($"桥梁名称: {bridgeDefects.Key}"));
                run.AppendChild(new Break());

                // 创建表格
                Table table = new Table();

                // 创建表头
                TableRow headerRow = new TableRow();
                foreach (var column in columns)
                {
                    headerRow.Append(CreateCell(column.Header, column.Width));
                }
                table.Append(headerRow);

                // 添加病害信息
                foreach (var defect in bridgeDefects)
                {
                    TableRow row = new TableRow();
                    row.Append(
                        CreateCell(defect.ComponentPart, columns[0].Width),
                        CreateCell(defect.DefectType, columns[1].Width),
                        CreateCell(defect.DefectLocation, columns[2].Width),
                        CreateCell(defect.DefectSeverity, columns[3].Width),
                        CreateCell(defect.Note, columns[4].Width),
                        CreateCell(string.Join(", ", defect.Photos.Select(p => Path.GetFileName(p.FilePath))), columns[5].Width));

                    table.Append(row);
                }

                // 将表格添加到文档中
                body.AppendChild(table);

            }

        }
    }
    private string CmToTwips(double cm)
    {
        // 1 英寸 = 2.54 厘米
        // 1 英寸 = 1440 Twips
        double twips = (cm / 2.54) * 1440;
        return ((int)twips).ToString();
    }
    private TableCell CreateCell(string text, string width)
    {
        return new TableCell(new Paragraph(new Run(new Text(text))))
        {
            TableCellProperties = new TableCellProperties { TableCellWidth = new TableCellWidth { Width = width } }
        };
    }


}
