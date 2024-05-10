using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using BridgeInspectionApp.ViewModels;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

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

                // 创建新的图片表格
                Table photoTable = new Table();
                TableRow photoRow = null;

                // 添加照片
                int photoIndex = 0;
                foreach (var defect in bridgeDefects)
                {
                    foreach (var photo in defect.Photos)
                    {
                        if (photoIndex % 2 == 0)
                        {
                            photoRow = new TableRow();
                            photoTable.Append(photoRow);
                        }

                        string photoInfo = $"{defect.ComponentPart} - {defect.DefectType} - {defect.DefectLocation} - {defect.DefectSeverity} - {defect.Note} - {Path.GetFileName(photo.FilePath)}";
                        TableCell photoCell = CreateCell(photoInfo, CmToTwips(7));
                        photoRow.Append(photoCell);

                        AddImageToCell(doc, mainPart, photoCell, photo.FilePath);

                        photoIndex++;
                    }
                }

                // 将图片表格添加到文档中
                body.AppendChild(photoTable);

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
    private void AddImageToCell(WordprocessingDocument doc, MainDocumentPart mainPart, TableCell cell, string imagePath)
    {
        ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

        using (FileStream stream = new FileStream(imagePath, FileMode.Open))
        {
            imagePart.FeedData(stream);
        }

        var element = CreateImageElement(doc, mainPart, imagePart);

        // Append the reference to cell, the element should be in a Run.
        cell.Append(new Paragraph(new Run(element)));
    }

    private Drawing CreateImageElement(WordprocessingDocument doc, MainDocumentPart mainPart, ImagePart imagePart)
    {

        return new Drawing(
            new DW.Inline(
                new DW.Extent() { Cx = CmToEmu(7), Cy = CmToEmu(5) },
                new DW.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
                new DW.DocProperties() { Id = (UInt32Value)1U, Name = "Picture 1" },
                new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
                new A.Graphic(
                    new A.GraphicData(
                        new PIC.Picture(
                            new PIC.NonVisualPictureProperties(
                                new PIC.NonVisualDrawingProperties() { Id = (UInt32Value)0U, Name = "New Bitmap Image.jpg" },
                                new PIC.NonVisualPictureDrawingProperties()),
                            new PIC.BlipFill(
                                new A.Blip(
                                    new A.BlipExtensionList(
                                        new A.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" })
                                )
                                {
                                    Embed = doc.MainDocumentPart.GetIdOfPart(imagePart),
                                    CompressionState = A.BlipCompressionValues.Print
                                },
                                new A.Stretch(
                                    new A.FillRectangle())),
                            new PIC.ShapeProperties(
                                new A.Transform2D(
                                    new A.Offset() { X = 0L, Y = 0L },
                                    new A.Extents() { Cx = CmToEmu(7), Cy = CmToEmu(5) }),
                                new A.PresetGeometry(
                                    new A.AdjustValueList()
                                )
                                { Preset = A.ShapeTypeValues.Rectangle }))
                    )
                    { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
            )
            {
                DistanceFromTop = (UInt32Value)0U,
                DistanceFromBottom = (UInt32Value)0U,
                DistanceFromLeft = (UInt32Value)0U,
                DistanceFromRight = (UInt32Value)0U,
                EditId = "50D07946"
            });
    }


    private long CmToEmu(double cm)
    {
        // 1 cm = 360000 EMUs
        return (long)(cm * 360000);
    }

}
