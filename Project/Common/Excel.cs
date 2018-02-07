using System;
using System.Runtime.InteropServices;
using System.Windows;
using MsExcel = Microsoft.Office.Interop.Excel;

namespace Common
{
    public class Excel
    {
        private readonly MsExcel.Application _excelApp;
        private readonly MsExcel.Workbook _workbook;
        public MsExcel.Worksheet Worksheet { get; set; }

        public Excel(string dirPath, string fileName)
        {
            _excelApp = new MsExcel.Application { Visible = false };
            _workbook = _excelApp.Workbooks.Add("");
            Worksheet = _workbook.ActiveSheet;

            FileName = fileName;
            DirPath = dirPath;
        }

        public Excel(string dirPath, string fileName, string sheetName)
        {
            _excelApp = new MsExcel.Application { Visible = false};
            _workbook = _excelApp.Workbooks.Open($"{dirPath}\\{fileName}");

            if (sheetName == null)
                Worksheet = _workbook.ActiveSheet;
            else
                foreach (MsExcel.Worksheet sheet in _workbook.Worksheets)
                {
                    if (!sheet.Name.Equals(sheetName)) continue;
                    Worksheet = sheet;
                    break;
                }

            FileName = fileName;
            DirPath = dirPath;
        }

        public string FileName { get; }
        public string DirPath { get; }

        public void Save()
        {
            _workbook.SaveAs($"{DirPath}\\{FileName}");
            MessageBox.Show("저장 완료!");
        }

        public void Release()
        {
            _workbook.Close(false);
            ReleaseExcelObject(_excelApp);
        }

        private static void ReleaseExcelObject(object obj)
        {
            try
            {
                if (obj == null) return;
                Marshal.ReleaseComObject(obj);
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}