using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Aspose.Cells;

namespace _9M.Work.Utility
{
    public class ExcelExport
    {
        private ExcelExport()
        { }
        public static bool ExportExcelWithAspose(System.Data.DataTable dt, Dictionary<string, string> dicColumn,
            string path)
        {
            bool succeed = false;
            if (dt != null)
            {
                try
                {
                    /*       Aspose.Cells.License li = new Aspose.Cells.License();
                    string lic = Resources.License;
                     Stream s = new MemoryStream(ASCIIEncoding.Default.GetBytes(lic));
                     li.SetLicense(s);
 */
                    Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();
                    Aspose.Cells.Worksheet cellSheet = workbook.Worksheets[0];

                    cellSheet.Name = dt.TableName;

                    int rowIndex = 0;
                    int colIndex = 0;
                    int colCount = dt.Columns.Count;
                    int rowCount = dt.Rows.Count;

                    //列名的处理

                    for (int i = 0; i < colCount; i++)
                    {
                        if (dicColumn != null && dicColumn.Keys.Contains(dt.Columns[i].ColumnName))
                        {
                            string columnName = dicColumn[dt.Columns[i].ColumnName];
                            cellSheet.Cells[rowIndex, colIndex].PutValue(columnName);
                            cellSheet.Cells[rowIndex, colIndex].Style.Font.IsBold = true;
                            cellSheet.Cells[rowIndex, colIndex].Style.Font.Name = "宋体";
                        }
                        else
                        {
                            cellSheet.Cells[rowIndex, colIndex].PutValue(dt.Columns[i].ColumnName);
                        }
                        colIndex++;
                    }
                    Aspose.Cells.Style style = workbook.Styles[workbook.Styles.Add()];
                    style.Font.Name = "Arial";
                    style.Font.Size = 10;
                    Aspose.Cells.StyleFlag styleFlag = new Aspose.Cells.StyleFlag();
                    cellSheet.Cells.ApplyStyle(style, styleFlag);

                    rowIndex++;

                    for (int i = 0; i < rowCount; i++)
                    {
                        colIndex = 0;
                        for (int j = 0; j < colCount; j++)
                        {
                            cellSheet.Cells[rowIndex, colIndex].PutValue(dt.Rows[i][j].ToString());
                            colIndex++;
                        }
                        rowIndex++;
                    }
                    cellSheet.AutoFitColumns();

                    path = Path.GetFullPath(path);
                    workbook.Save(path);
                    succeed = true;
                }
                catch (Exception ex)
                {
                    succeed = false;
                    Console.WriteLine(ex.Message);
                }
            }

            return succeed;
        }

        public static bool ExportExcelWithAspose2(System.Data.DataSet ds, List<object> listColumn, string path)
        {
            bool succeed = false;
            if (ds != null && ds.Tables.Count > 0)
            {
                try
                {
                    /*       Aspose.Cells.License li = new Aspose.Cells.License();
                    string lic = Resources.License;
                     Stream s = new MemoryStream(ASCIIEncoding.Default.GetBytes(lic));
                     li.SetLicense(s);
                     */
                    Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();

                    for (int i = workbook.Worksheets.Count - 1; i >= 0; i--)
                    {
                        workbook.Worksheets.RemoveAt(i);
                    }

                    int dtIndex = 0;
                    foreach (DataTable dt in ds.Tables)
                    {
                        Aspose.Cells.Worksheet cellSheet = workbook.Worksheets.Add(dt.TableName);
                        int rowIndex = 0;
                        int colIndex = 0;
                        int colCount = dt.Columns.Count;
                        int rowCount = dt.Rows.Count;

                        //列名的处理
                        if (listColumn.Count > 0)
                        {
                            Dictionary<string, string> dicColumn = listColumn[dtIndex] as Dictionary<string, string>;
                            for (int i = 0; i < colCount; i++)
                            {
                                if (dicColumn.Keys.Contains(dt.Columns[i].ColumnName))
                                {
                                    string columnName = dicColumn[dt.Columns[i].ColumnName];
                                    cellSheet.Cells[rowIndex, colIndex].PutValue(columnName);
                                    cellSheet.Cells[rowIndex, colIndex].Style.Font.IsBold = true;
                                    cellSheet.Cells[rowIndex, colIndex].Style.Font.Name = "宋体";
                                }
                                colIndex++;
                            }
                        }


                        Aspose.Cells.Style style = workbook.Styles[workbook.Styles.Add()];
                        style.Font.Name = "Arial";
                        style.Font.Size = 10;
                        Aspose.Cells.StyleFlag styleFlag = new Aspose.Cells.StyleFlag();
                        cellSheet.Cells.ApplyStyle(style, styleFlag);

                        rowIndex++;

                        for (int i = 0; i < rowCount; i++)
                        {
                            colIndex = 0;
                            for (int j = 0; j < colCount; j++)
                            {
                                cellSheet.Cells[rowIndex, colIndex].PutValue(dt.Rows[i][j].ToString());
                                colIndex++;
                            }
                            rowIndex++;
                        }
                        cellSheet.AutoFitColumns();
                        dtIndex++;
                    }


                    path = Path.GetFullPath(path);
                    workbook.Save(path);
                    succeed = true;
                }
                catch (Exception ex)
                {
                    succeed = false;
                    Console.WriteLine(ex.Message);
                }
            }

            return succeed;
        }



        /// <summary>
        /// Excel导入
        /// </summary>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public static System.Data.DataTable ReadExcel(String strFileName)
        {
            try
            {
                Workbook book = new Workbook();
                book.Open(strFileName);
                Worksheet sheet = book.Worksheets[0];
                Cells cells = sheet.Cells;
                return cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
            }
            catch
            {
                return null;
            }



        }

    }
}
