using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Common;
using Common.Window;

namespace DataInsertTool
{
    internal struct StudentData
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

            string fileName;
            string sheetName;

            while (true)
            { 
                if (Interaction.InputBox("파일 이름 입력", "파일 이름을 입력 해 주세요", out fileName)
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

                if (Interaction.InputBox("시트 이름 입력", "엑셀 시트의 이름을 입력 해 주세요", out sheetName)
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

            // g is grade
            // c is class
            // n is number
            // m is number of record currently being explored
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
                Database.Connect("");

                const string preperformSql =
                    // Table `Message`
                    "DROP TABLE IF EXISTS `Message`" +
                    "CREATE TABLE Message(" +
                    "`Index` int NOT NULL AUTO_INCREMENT," +
                    "Title varchar(20) NOT NULL," +
                    "Content varchar(100) NOT NULL," +
                    "Date timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP," +
                    "CONSTRAINT Message_pk PRIMARY KEY(`Index`));" +
                    // Table `Student`
                    "DROP TABLE IF EXISTS `Student`" +
                    "CREATE TABLE Student(" +
                    "Number char(4) NOT NULL," +
                    "Name varchar(5) NOT NULL," +
                    "RoomNumber char(3) NOT NULL," +
                    "Grade char(1) NOT NULL," +
                    "Class char(1) NOT NULL," +
                    "Ess char(1) NOT NULL DEFAULT 'N'," +
                    "Dormitory char(1) NOT NULL DEFAULT 'N'," +
                    "Outing char(1) NOT NULL DEFAULT 'N'," +
                    "OutingStart char(5) NULL," +
                    "OutingEnd char(5) NULL," +
                    "AcademyStart char(5) NULL," +
                    "AcademyEnd char(5) NULL," +
                    "Monday char(1) NULL," +
                    "Tuesday char(1) NULL," +
                    "Wednesday char(1) NULL," +
                    "Thursday char(1) NULL," +
                    "Friday char(1) NULL," +
                    "Remarks varchar(50) NULL," +
                    "CONSTRAINT Student_pk PRIMARY KEY(Number));";

                using (var cmd = Database.GetCommand(preperformSql))
                {
                    cmd.ExecuteNonQuery();
                }
                    // 1프로에 한번씩 ProgressBar 를 진행 시키는 코드
                    rate += (double) 1 / studentsData.Count;
                    if (rate >= 0.01d)
                    {
                        progressbar.PerformStep();
                        rate -= 0.01d;
                    }

                    var sql = string.Empty;

                    // SQL 을 하나의 Variable 에 모아서 한번에 DB에 쿼리함.
                    // 하나씩 보내는 것 보다 훨씬 속도가 빠름
                    for (var i = 0; i < studentsData.Count; ++i)
                    {
                        rate += (double)1 / studentsData.Count;
                        if (rate >= 0.01d)
                        {
                            progressbar.PerformStep();
                            rate -= 0.01d;
                        }
                        sql +=
                            "INSERT INTO Student(`Number`,`Name`,`Grade`,`Class`,`RoomNumber`) " +
                            $"VALUES({studentsData[i].Number},'{studentsData[i].Name}',{studentsData[i].Grade},{studentsData[i].Class},{studentsData[i].RoomNumber});";
                    }
                using (var cmd = Database.GetCommand(sql))
                {
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("데이터 입력을 완료하였습니다.", "작업 성공", MessageBoxButtons.OK);
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