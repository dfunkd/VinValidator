using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Reflection;

public static class ExcelExporter
{
    public static SheetData CreateSheetDataFromList<T>(this List<T> data)
    {
        SheetData sheetData = new();

        // Add header row
        Row headerRow = new();
        uint rowIndex = 1;
        headerRow.RowIndex = rowIndex++;

        PropertyInfo[] properties = typeof(T).GetProperties();
        uint colIndex = 1;
        foreach (PropertyInfo prop in properties)
        {
            Cell headerCell = CreateCell(colIndex++, headerRow.RowIndex.Value, prop.Name);
            headerRow.AppendChild(headerCell);
            //#FFBD2036
        }
        sheetData.AppendChild(headerRow);

        // Add data rows
        foreach (T item in data)
        {
            Row dataRow = new() { RowIndex = rowIndex++ };
            colIndex = 1; // Reset column index for each new row

            foreach (PropertyInfo prop in properties)
            {
                object value = prop.GetValue(item);
                Cell dataCell = CreateCell(colIndex++, dataRow.RowIndex.Value, value?.ToString() ?? string.Empty);
                dataRow.AppendChild(dataCell);
            }
            sheetData.AppendChild(dataRow);
        }

        return sheetData;
    }

    private static Cell CreateCell(uint columnIndex, uint rowIndex, string text, bool isHeader = false)
    {
        Cell cell = new()
        {
            CellReference = GetColumnName(columnIndex) + rowIndex,
            DataType = CellValues.String, // You can adjust data type based on content
            StyleIndex = 1U
        };
        CellValue cellValue = new(text);
        cell.AppendChild(cellValue);
        return cell;
    }

    public static void ExportToExcel<T>(this List<T> data, string path)
    {
        using SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
        WorkbookPart workbookPart = document.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        SheetData sheetData = new();

        // Add header row
        Row headerRow = new();
        uint rowIndex = 1;
        headerRow.RowIndex = rowIndex++;

        PropertyInfo[] properties = typeof(T).GetProperties();
        uint colIndex = 1;
        foreach (PropertyInfo prop in properties)
        {
            Cell headerCell = CreateCell(colIndex++, headerRow.RowIndex.Value, prop.Name);
            headerRow.AppendChild(headerCell);
        }
        sheetData.AppendChild(headerRow);

        // Add data rows
        foreach (T item in data)
        {
            Row dataRow = new() { RowIndex = rowIndex++ };
            colIndex = 1; // Reset column index for each new row

            foreach (PropertyInfo prop in properties)
            {
                object value = prop.GetValue(item);
                Cell dataCell = CreateCell(colIndex++, dataRow.RowIndex.Value, value?.ToString() ?? string.Empty);
                dataRow.AppendChild(dataCell);
            }
            sheetData.AppendChild(dataRow);
        }

        // Add a WorksheetPart to the WorkbookPart.
        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet(sheetData);

        // Add Sheets to the Workbook.
        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

        // Append a new worksheet and associate it with the workbook.
        Sheet sheet = new() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Vin Validation" };
        sheets.Append(sheet);
    }

    private static string GetColumnName(uint columnIndex)
    {
        string columnName = "";
        while (columnIndex > 0)
        {
            uint modulo = (columnIndex - 1) % 26;
            columnName = (char)('A' + modulo) + columnName;
            columnIndex = (columnIndex - modulo) / 26;
        }
        return columnName;
    }
}