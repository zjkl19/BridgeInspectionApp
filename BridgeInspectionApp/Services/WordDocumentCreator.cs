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

            foreach (var defect in defects)
            {
                // 添加桥梁名称
                Paragraph para = body.AppendChild(new Paragraph());
                Run run = para.AppendChild(new Run());
                run.AppendChild(new Text($"桥梁名称: {defect.BridgeName}"));
                run.AppendChild(new Break());

                // 创建表格
                Table table = new Table();

                // 创建表头
                TableRow headerRow = new TableRow();
                headerRow.Append(
                    new TableCell(new Paragraph(new Run(new Text("构件部位")))),
                    new TableCell(new Paragraph(new Run(new Text("病害类型")))),
                    new TableCell(new Paragraph(new Run(new Text("照片名字")))));
                table.Append(headerRow);

                // 添加病害信息
                TableRow row = new TableRow();
                row.Append(
                    new TableCell(new Paragraph(new Run(new Text(defect.ComponentPart)))),
                    new TableCell(new Paragraph(new Run(new Text(defect.DefectType)))),
                    new TableCell(new Paragraph(new Run(new Text(string.Join(", ", defect.Photos.Select(p => Path.GetFileName(p.FilePath))))))));

                table.Append(row);

                // 将表格添加到文档中
                body.AppendChild(table);
            }
        }
    }



}
