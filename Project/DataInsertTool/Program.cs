using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Common;

namespace DataInsertTool
{
    struct StudentData
    {
        public string Name;
        public string Number;
        public string RoomNumber;
        public string Grade;
        public string Class;
    }

    public class Program
    {
       

        private const int Hide = 0;
        private const int Show = 5;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static void Main()
        {
            var consoleWindow = GetConsoleWindow();
            ShowWindow(consoleWindow, Hide);

            var fileName = string.Empty;
            var sheetName = string.Empty;

            while (true)
            { 
                if (Interaction.InputBox("파일 이름 입력", "파일 이름을 입력 해 주세요", ref fileName)
                    .Equals(DialogResult.Cancel))
                {
                    return;
                }

                if (File.Exists($"{Environment.CurrentDirectory}\\{fileName}.xls"))
                {
                    fileName += ".xls";
                }
                else if (File.Exists($"{Environment.CurrentDirectory}\\{fileName}.xlsx"))
                {
                    fileName += ".xlsx";
                }
                else
                {
                    MessageBox.Show($"파일 \"{fileName}\" 이(가) 존재하지 않습니다. 파일 이름을 확인해주세요", "오류", MessageBoxButtons.OK);
                    continue;
                }

                if (Interaction.InputBox("시트 이름 입력", "엑셀 시트의 이름을 입력 해 주세요", ref sheetName)
                    .Equals(DialogResult.Cancel))
                    return;
                break;
            }

            var studentsData = new List<StudentData>();

            var excel = new Excel(Environment.CurrentDirectory, fileName, sheetName);

            if (excel.Worksheet is null)
            {
                return;
            }
            for (int g = 0, m = 2; g < 3; ++g, ++m)
            for (var c = 1; (excel.Worksheet.Cells[3, m] as Range)?.Value != null; m += 2, ++c)
            for (var n = 3; (excel.Worksheet.Cells[n, m] as Range)?.Value != null; ++n)
            {
                if (((Range) excel.Worksheet.Cells[n, m + 1]).Value is null)
                    continue;

                studentsData.Add(new StudentData
                {
                    Number = ((g + 1) * 1000 + c * 100 + (n - 2)).ToString(),
                    Name = ((Range) excel.Worksheet.Cells[n, m]).Value.ToString(),
                    Grade = (g + 1).ToString(),
                    Class = c.ToString(),
                    RoomNumber = ((Range) excel.Worksheet.Cells[n, m + 1])?.Value.ToString()
                });
            }

            excel.Release();

            var form = Interaction.LoadingBox("데이터 입력 작업중", out var progressbar);
            var rate = 0d;
            try
            {
                Database.Connect();
                for (var i = 0; i < studentsData.Count; ++i)
                {
                    rate += (double) 1 / studentsData.Count;
                    if (rate >= 1d)
                    {
                        progressbar.PerformStep();
                        rate -= 1d;
                    }

                    var sql =
                        $"INSERT INTO Student(`Number`,`Name`,`Grade`,`Class`,`RoomNumber`) values({studentsData[i].Number},'{studentsData[i].Name}',{studentsData[i].Grade},{studentsData[i].Class},{studentsData[i].RoomNumber});" +
                        $"INSERT INTO ESS(`Number`) values({studentsData[i].Number});" +
                        $"INSERT INTO Outing(`Number`) values({studentsData[i].Number});" +
                        $"INSERT INTO Academy(`Number`) values({studentsData[i].Number});";
                    using (var cmd = Database.GetCommand(sql))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("데이터 입력이 완료되었습니다", "작업 성공", MessageBoxButtons.OK);
            }
            catch (Exception e)
            {
                ShowWindow(consoleWindow, Show);
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (Database.IsConnectionOpen)
                {
                    Database.Disconnect();
                }

                form.Close();
            }
        }
    }
}