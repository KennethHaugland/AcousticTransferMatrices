using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ExcelOpenXMLInterface
{
    public static class ExcelFunctions
    {
        public static void WriteExcelFile(DataSet ds, string fileName)
        {
            using (var workbook = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new Workbook { Sheets = new Sheets() };

                foreach (DataTable table in ds.Tables)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    uint sheetId = 1;
                    if (sheets.Elements<Sheet>().Count() > 0)
                        sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                    Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                    sheets.Append(sheet);

                    Row headerRow = new Row();
                    List<string> columns = new List<string>();
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);
                        Cell cell = new Cell
                        {
                            //TODO: Add value specific code, i.e. for spesific datatypes in Excel
                            DataType = CellValues.String,
                            CellValue = new CellValue(column.ColumnName)
                        };
                        _ = headerRow.AppendChild(cell);
                    }
                    _ = sheetData.AppendChild(headerRow);
                    foreach (DataRow dsrow in table.Rows)
                    {
                        Row newRow = new Row();
                        foreach (String col in columns)
                        {
                            Cell cell = new Cell
                            {
                                //TODO: Add value specific code, i.e. for spesific datatypes in Excel
                                DataType = CellValues.String,
                                CellValue = new CellValue(dsrow[col].ToString())
                            };
                            newRow.AppendChild(cell);
                        }
                        sheetData.AppendChild(newRow);
                    }
                }
            }
        }

        public static void GenerateExcelRapport(DataTable Table, string filename = "DefaultName.xlsx")
        {
            if (!filename.ToLower().Contains(".xlsx"))
                filename += ".xlsx";

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filename, SpreadsheetDocumentType.Workbook))
            {
                // Add a WorkbookPart to the document.
                WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

                // Add Sheets to the Workbook.
                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                    AppendChild(new Sheets());

                SheetData sheetData = GenerateSheet(Table);
                worksheetPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                // Add the sheet and make relation to workbook
                var sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = (uint)(spreadsheetDocument.WorkbookPart.Workbook.Sheets.Count() + 1),
                    Name = Table.TableName
                };

                sheets.Append(sheet);


                workbookpart.Workbook.Save();

                // Close the document
                spreadsheetDocument.Close();
            }
        }


        public static void GenerateExcelRapport(DataSet ds, string filename = "DefaultName.xlsx")
        {

            if (!string.IsNullOrEmpty(ds.DataSetName) && filename == "DefaultName.xlsx")
            {
                filename = ds.DataSetName;
            }

            if (!filename.ToLower().Contains(".xlsx"))
                filename += ".xlsx";

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filename, SpreadsheetDocumentType.Workbook))
            {
                // Add a WorkbookPart to the document.
                WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

                // Add Sheets to the Workbook.
                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                    AppendChild(new Sheets());

                foreach (DataTable Table in ds.Tables)
                {
                    SheetData sheetData = GenerateSheet(Table);
                    worksheetPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    // Add the sheet and make relation to workbook
                    var sheet = new Sheet()
                    {
                        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = (uint)(spreadsheetDocument.WorkbookPart.Workbook.Sheets.Count() + 1),
                  
                    };

                    if (!string.IsNullOrEmpty(Table.TableName))
                        sheet.Name = Table.TableName;
                    else
                        sheet.Name = "Sheet" + sheet.SheetId.ToString();
                    

                    sheets.Append(sheet);

                }

                workbookpart.Workbook.Save();

                // Close the document
                spreadsheetDocument.Close();
            }
        }

        private static SheetData GenerateSheet(DataTable Measurements)
        {
            SheetData sheetData = new SheetData();
            Dictionary<int, Type> keyValues = new Dictionary<int, Type>();
   
            Row row = new Row() { RowIndex = 1 };
            for (int i = 0; i < Measurements.Columns.Count; i++)
            {
                row.Append(CreateCell(i + 1, 1, Measurements.Columns[i].ColumnName, CellValues.String));
                keyValues[i] = Measurements.Columns[i].DataType;
            }
            sheetData.Append(row);

            for (int rad = 0; rad < Measurements.Rows.Count; rad++)
            {
                Row fileRow = new Row() { RowIndex = (UInt32)(rad + 2) };
                for (int i = 0; i < Measurements.Columns.Count; i++)
                {
                    fileRow.Append(CreateCell(i + 1, rad + 2, Measurements.Rows[rad].ItemArray[i].ToString(), GetCellType(keyValues[i])));
                }
                sheetData.Append(fileRow);
            }
            return sheetData;
        }

        private static CellValues GetCellType(Type type)
        {
            if (type.Name.ToLower() == "double" || type.Name.ToLower() == "int" || type.Name.ToLower() == "float" || type.Name.ToLower() == "int32")
                return CellValues.Number;
            //else if(type.Name.ToLower() == "datetime")
            //       return CellValues.Date;
            else
                return CellValues.String;
        }

        private static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }
            return columnName;
        }

        private static Cell CreateCell(int col, int row, string value, CellValues cellValues)
        {
            UInt32Value format = 0;
            string UsedValue = value;
            if (cellValues == CellValues.Number)
                UsedValue = value.Replace(',', '.');
            else if (cellValues == CellValues.Date)
            {
                double oaValue = DateTime.Parse(value).ToOADate();
                UsedValue = oaValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                cellValues = CellValues.Number;
                format = 22;
            }
            new NumberingFormat() { NumberFormatId = 164, FormatCode = StringValue.FromString($"[$-409]dd.mm.yyyy HH:mm:ss;@") };
            Cell cell = new Cell() { CellReference = GetExcelColumnName(col) + (row).ToString() };
            cell.CellValue = new CellValue(UsedValue.ToString());
            cell.DataType = new EnumValue<CellValues>(cellValues);

            if (format != 0)
                cell.StyleIndex = format;

            return cell;
        }

        //public static void WriteExcelFile(DataTable table, string fileName)
        //{
        //    using (var workbook = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook))
        //    {
        //        WorkbookPart workbookPart = workbook.AddWorkbookPart();
        //        workbook.WorkbookPart.Workbook = new Workbook { Sheets = new Sheets() };

        //        var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
        //        var sheetData = new SheetData();
        //        sheetPart.Worksheet = new Worksheet(sheetData);

        //        Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
        //        string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);
        //        uint sheetId = 1;
        //        if (sheets.Elements<Sheet>().Count() > 0)
        //            sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;

        //        Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
        //        sheets.Append(sheet);

        //        Row headerRow = new Row();
        //        List<string> columns = new List<string>();
        //        foreach (DataColumn column in table.Columns)
        //        {
        //            columns.Add(column.ColumnName);
        //            Cell cell = new Cell
        //            {
        //                //TODO: Add value specific code, i.e. for spesific datatypes in Excel
        //                DataType = CellValues.String,
        //                CellValue = new CellValue(column.ColumnName)
        //            };
        //            _ = headerRow.AppendChild(cell);
        //        }
        //        _ = sheetData.AppendChild(headerRow);
        //        foreach (DataRow dsrow in table.Rows)
        //        {
        //            Row newRow = new Row();
        //            foreach (string col in columns)
        //            {
        //                Cell cell = new Cell
        //                {
        //                    //TODO: Add value specific code, i.e. for spesific datatypes in Excel
        //                    DataType = CellValues.String,
        //                    CellValue = new CellValue(dsrow[col].ToString())
        //                };
        //                newRow.AppendChild(cell);
        //            }
        //            sheetData.AppendChild(newRow);
        //        }
        //    }
        //}

        public static List<(string ExcelName, string[,] ExcelData)> GetAllExcelSheetsAsStringArray(string fileName)
        {
            List<(string ExcelName, string[,] ExcelData)> result = new List<(string ExcelName, string[,] ExcelData)>();

            //open the excel using openxml sdk
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
            {
                //create the object for workbook part
                WorkbookPart wbPart = doc.WorkbookPart;
                Workbook workbook = wbPart.Workbook;

                // Get all the sheets in workbook
                IEnumerable<Sheet> sheets = workbook.Descendants<Sheet>();
                foreach (Sheet sheet in sheets)
                {
                    WorksheetPart worksheetPart = (WorksheetPart)wbPart.GetPartById(sheet.Id);
                    var sharedStringPart = wbPart.SharedStringTablePart;
                    IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>();

                    // Get used range, ex: A1:Z43
                    string UsedRange = worksheetPart.Worksheet.SheetDimension.Reference;
                    int[] ends;
                    // Get last row and column, assume used range starts on 1,1
                    if (UsedRange.Contains(":"))
                        ends = ExcelCell(UsedRange.Split(':')[1]);
                    else
                        ends = ExcelCell(UsedRange);

                    // Make a string array of Excel cell elements matching current sheet
                    string[,] ExcelStringMatrix = new string[ends[0] + 1, ends[1] + 1];

                    // Loop trough all rows
                    foreach (Row rad in rows)
                    {
                        // Then all cells in that row
                        foreach (Cell cell in rad.Elements<Cell>())
                        {
                            int[] CurrentCellIndex = ExcelCell(cell.CellReference);
                            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                            {
                                // String
                                if (cell.InnerText.Length > 0)
                                {
                                    var value = cell.InnerText;

                                    // If the cell represents an integer number, you are done.
                                    // For dates, this code returns the serialized value that
                                    // represents the date. The code handles strings and
                                    // Booleans individually. For shared strings, the code
                                    // looks up the corresponding value in the shared string
                                    // table. For Booleans, the code converts the value into
                                    // the words TRUE or FALSE.
                                    if (cell.DataType != null)
                                    {
                                        switch (cell.DataType.Value)
                                        {
                                            case CellValues.SharedString:
                                                // For shared strings, look up the value in the
                                                // shared strings table.
                                                var stringTable =
                                                    wbPart.GetPartsOfType<SharedStringTablePart>()
                                                    .FirstOrDefault();

                                                // If the shared string table is missing, something
                                                // is wrong. Return the index that is in
                                                // the cell. Otherwise, look up the correct text in
                                                // the table.
                                                if (stringTable != null)
                                                {
                                                    value = stringTable.SharedStringTable
                                                        .ElementAt(int.Parse(value)).InnerText;
                                                }
                                                break;

                                            case CellValues.Boolean:
                                                switch (value)
                                                {
                                                    case "0":
                                                        value = "FALSE";
                                                        break;
                                                    default:
                                                        value = "TRUE";
                                                        break;
                                                }
                                                break;
                                        }
                                        int index = int.Parse(cell.CellValue.Text);
                                        ExcelStringMatrix[CurrentCellIndex[0], CurrentCellIndex[1]] = value;
                                    }
                                }
                            }
                            else
                            {
                                // Number
                                ExcelStringMatrix[CurrentCellIndex[0], CurrentCellIndex[1]] = cell.CellValue.Text.ToString();
                            }
                        }
                    }
                    result.Add((ExcelName: sheet.Name, ExcelData: ExcelStringMatrix));
                }
            }
            return result;
        }

        /// <summary>
        /// Convert Excel row and column to number
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int[] ExcelCell(string values)
        {
            int[] result = new int[2];
            result[1] = ExcelColumnNameToNumber(new string(values.Where(char.IsLetter).ToArray())) - 1;
            result[0] = int.Parse(new string(values.Where(char.IsDigit).ToArray())) - 1;
            return result;
        }

        public static int ExcelColumnNameToNumber(string name)
        {
            int number = 0;
            int pow = 1;
            for (int i = name.Length - 1; i >= 0; i--)
            {
                number += (name[i] - 'A' + 1) * pow;
                pow *= 26;
            }
            return number;
        }

        public static string GetExcelColumnName2(int columNumber)
        {
            int dividend = columNumber;
            string columname = string.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columname = Convert.ToChar(65 + modulo).ToString() + columname;
                dividend = (int)((dividend - modulo) / 26);
            }
            return columname;
        }
    }
}