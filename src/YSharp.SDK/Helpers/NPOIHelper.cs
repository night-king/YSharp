﻿using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace YSharp.SDK.Helpers
{
    public class UnsupportedExcelFileFormatException : Exception
    {
        public string Reason { set; get; }
        public UnsupportedExcelFileFormatException(string reason)
        {
            this.Reason = reason;
        }
    }
    public class NPOIHelper
    {
        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置</param>
        public static void Export(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = Export(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        public static void ExportSet(DataSet dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = Export(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        public static MemoryStream Export(DataTable dtSource, string strHeaderText)
        {
            var workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet1");

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "ChianForex"; //填加xls文件作者信息
                si.ApplicationName = "ChianForex"; //填加xls文件创建程序信息
                si.LastAuthor = ""; //填加xls文件最后保存者信息
                si.Comments = "ChianForex"; //填加xls文件作者信息
                si.Title = "ChianForex Export"; //填加xls文件标题信息
                si.Subject = strHeaderText;//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet();
                    }

                    #region 表头及样式
                    {
                        var headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        var headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        var font = workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.IsBold = true;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                    }
                    #endregion


                    #region 列头及样式
                    {
                        var headerRow = sheet.CreateRow(1);
                        var headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        var font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.IsBold = true;
                        headStyle.SetFont(font);
                        var maxCellWidth = 255 * 100;
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            var colWidth = (arrColWidth[column.Ordinal] + 1) * 256;
                            if (colWidth > maxCellWidth)
                            {
                                colWidth = maxCellWidth;
                            }
                            sheet.SetColumnWidth(column.Ordinal, colWidth);
                        }
                    }
                    #endregion

                    rowIndex = 2;
                }
                #endregion


                #region 填充内容
                var dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    var newCell = dataRow.CreateCell(column.Ordinal);

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle;//格式化显示
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }
                #endregion

                rowIndex++;
            }
            MemoryStream ms = new MemoryStream();
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }

        public static MemoryStream Export(DataSet dtSet, string strHeaderText)
        {
            var workbook = new HSSFWorkbook();
            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "ChianForex"; //填加xls文件作者信息
                si.ApplicationName = "ChianForex"; //填加xls文件创建程序信息
                si.LastAuthor = ""; //填加xls文件最后保存者信息
                si.Comments = "ChianForex"; //填加xls文件作者信息
                si.Title = "ChianForex Export"; //填加xls文件标题信息
                si.Subject = strHeaderText;//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            foreach (DataTable dtSource in dtSet.Tables)
            {
                var sheetName = dtSource.TableName;
                ISheet sheet = workbook.CreateSheet(sheetName);
                //取得列宽
                int[] arrColWidth = new int[dtSource.Columns.Count];
                foreach (DataColumn item in dtSource.Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                }
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                        if (intTemp > arrColWidth[j])
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }
                int rowIndex = 0;
                foreach (DataRow row in dtSource.Rows)
                {
                    #region 新建表，填充表头，填充列头，样式
                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheet = workbook.CreateSheet();
                        }

                        #region 表头及样式
                        {
                            var headerRow = sheet.CreateRow(0);
                            headerRow.HeightInPoints = 25;
                            headerRow.CreateCell(0).SetCellValue(string.IsNullOrEmpty(strHeaderText) ? dtSource.TableName : strHeaderText);

                            var headStyle = workbook.CreateCellStyle();
                            headStyle.Alignment = HorizontalAlignment.Center;
                            var font = workbook.CreateFont();
                            font.FontHeightInPoints = 20;
                            font.IsBold = true;
                            headStyle.SetFont(font);
                            headerRow.GetCell(0).CellStyle = headStyle;
                            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                        }
                        #endregion


                        #region 列头及样式
                        {
                            var headerRow = sheet.CreateRow(1);
                            var headStyle = workbook.CreateCellStyle();
                            headStyle.Alignment = HorizontalAlignment.Center;
                            var font = workbook.CreateFont();
                            font.FontHeightInPoints = 10;
                            font.IsBold = true;
                            headStyle.SetFont(font);
                            var maxCellWidth = 255 * 100;
                            foreach (DataColumn column in dtSource.Columns)
                            {
                                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                                headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                                //设置列宽
                                var colWidth = (arrColWidth[column.Ordinal] + 1) * 256;
                                if (colWidth > maxCellWidth)
                                {
                                    colWidth = maxCellWidth;
                                }
                                sheet.SetColumnWidth(column.Ordinal, colWidth);
                            }
                        }
                        #endregion

                        rowIndex = 2;
                    }
                    #endregion


                    #region 填充内容
                    var dataRow = sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in dtSource.Columns)
                    {
                        var newCell = dataRow.CreateCell(column.Ordinal);

                        string drValue = row[column].ToString();

                        switch (column.DataType.ToString())
                        {
                            case "System.String"://字符串类型
                                newCell.SetCellValue(drValue);
                                break;
                            case "System.DateTime"://日期类型
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);

                                newCell.CellStyle = dateStyle;//格式化显示
                                break;
                            case "System.Boolean"://布尔型
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                newCell.SetCellValue(boolV);
                                break;
                            case "System.Int16"://整型
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Decimal"://浮点型
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(doubV);
                                break;
                            case "System.DBNull"://空值处理
                                newCell.SetCellValue("");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }

                    }
                    #endregion

                    rowIndex++;
                }
            }
            MemoryStream ms = new MemoryStream();
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }
        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <param name="skipRow"></param>
        /// <param name="isCheckCloumn"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static DataTable Import(string strFileName, int skipRow, bool isCheckCloumn = false, string[] columns = null)
        {
            DataTable dt = new DataTable();

            IWorkbook hssfworkbook = null;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                var ext = Path.GetExtension(strFileName).ToLower();
                if (ext == ".xlsx")
                {
                    hssfworkbook = new XSSFWorkbook(file);
                }
                else if (ext == ".xls")
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
            }
            var sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            var headerRow = sheet.GetRow(skipRow);
            int cellCount = headerRow.LastCellNum;
            if (isCheckCloumn)
            {
                if (cellCount != columns.Length)
                {
                    throw new UnsupportedExcelFileFormatException(string.Format("Wrong format，total column is {0}，but it is {1} actually", columns.Count(), cellCount));
                }
                for (int j = 0; j < cellCount; j++)
                {
                    var cell = headerRow.GetCell(j);
                    var cname = cell.ToString().Trim();
                    if (cname != columns[j].Trim())
                    {
                        throw new UnsupportedExcelFileFormatException(string.Format("Wrong format，the {0} column is {1}，but it is {2} actually", j + 1, columns[j].Trim(), cname));
                    }
                    dt.Columns.Add(cname);
                }
            }
            else
            {
                for (int j = 0; j < cellCount; j++)
                {
                    var cell = headerRow.GetCell(j);
                    dt.Columns.Add(cell.ToString().Trim());
                }
            }


            for (int i = (sheet.FirstRowNum + skipRow + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        public static DataTable Import(Stream sm, string ext, int skipRow, bool isCheckCloumn = false, string[] columns = null)
        {
            DataTable dt = new DataTable();

            IWorkbook hssfworkbook = null;
            if (ext == ".xlsx")
            {
                hssfworkbook = new XSSFWorkbook(sm);
            }
            else if (ext == ".xls")
            {
                hssfworkbook = new HSSFWorkbook(sm);
            }
            var sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            var headerRow = sheet.GetRow(skipRow);
            int cellCount = headerRow.LastCellNum;
            if (isCheckCloumn)
            {
                if (cellCount != columns.Length)
                {
                    throw new UnsupportedExcelFileFormatException(string.Format("Wrong format，total column is {0}，but it is {1} actually", columns.Count(), cellCount));
                }
                for (int j = 0; j < cellCount; j++)
                {
                    var cell = headerRow.GetCell(j);
                    var cname = cell.ToString().Trim();
                    if (cname != columns[j].Trim())
                    {
                        throw new UnsupportedExcelFileFormatException(string.Format("Wrong format，the {0} column is {1}，but it is {2} actually", j + 1, columns[j].Trim(), cname));
                    }
                    dt.Columns.Add(cname);
                }
            }
            else
            {
                for (int j = 0; j < cellCount; j++)
                {
                    var cell = headerRow.GetCell(j);
                    dt.Columns.Add(cell.ToString().Trim());
                }
            }


            for (int i = (sheet.FirstRowNum + skipRow + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }

    }
}
