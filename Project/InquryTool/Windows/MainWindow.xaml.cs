using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Office.Interop.Excel;
using Common;
using Common.Data;
using Application = System.Windows.Application;

namespace InquryTool.Windows
{
    public partial class MainWindow
    {
        private NoticeBoard _noticeBoard;

        public MainWindow()
        {
            InitializeComponent();

            _noticeBoard = new NoticeBoard(out var complete);
            if (!complete)
            {
                Close();
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (Database.Connect().Equals(false))
            {
                MessageBox.Show("MysqlConnection variable has an error", "ERROR");
                Close();
            }

            const string sql =
                "SELECT * FROM Students Std INNER JOIN AfterSchoolActivityStatus Act, AcademyInfo Acd WHERE Std.Number=Act.Number AND Std.Number=Acd.Number ORDER BY RoomNumber ASC, Std.Name ASC;";

            #region 쿼리 실행

            try
            {
                using (var adapter = Database.GetAdapter(sql))
                using (var set = new DataSet())
                {
                    adapter.Fill(set);
                    foreach (DataRow data in set.Tables[0].Rows)
                    {
                        if (data["Prep"].Equals("Y"))
                        {
                            LvPrep.Items.Add(new BasicData(data));
                        }
                        else if (data["prep"].Equals("N"))
                        {
                            LvAnihome.Items.Add(new BasicData(data));
                        }
                        else if (data["Outing"].Equals("Y"))
                        {
                            LvOuting.Items.Add(new OutingData(data));
                        }
                        else if (data["Academy"].Equals("Y"))
                        {
                            if (data[DateTime.Today.ToString("dddd", new CultureInfo("en-US"))].Equals("Y"))
                            {
                                LvAcademy.Items.Add(new AcademyData(data));
                            }
                        }
                        else
                        {
                            LvUnchecked.Items.Add(new BasicData(data));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK);
                Close();
            }

            #endregion
        }

        // 파일로 저장 버튼
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(LvAnihome.HasItems || LvPrep.HasItems || LvOuting.HasItems || LvAcademy.HasItems))
            {
                return;
            }

            var folderDir =
                $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop, Environment.SpecialFolderOption.None)}\\작화조회파일";
            if (!Directory.Exists(folderDir))
            {
                Directory.CreateDirectory(folderDir);
            }

            var fileName = $"{DateTime.Today.ToShortDateString()}.xlsx";

            var excel = new Excel(folderDir, fileName);

            #region UI설정

            excel.Worksheet.Cells[2, 1] = "기숙사";
            excel.Worksheet.Cells[3, 1] = "호실";
            excel.Worksheet.Cells[3, 2] = "이름";
            var r = excel.Worksheet.Range[excel.Worksheet.Cells[2, 1], excel.Worksheet.Cells[2, 2]];
            r.Merge(true);

            excel.Worksheet.Cells[2, 4] = "작화실";
            excel.Worksheet.Cells[3, 4] = "호실";
            excel.Worksheet.Cells[3, 5] = "이름";
            r = excel.Worksheet.Range[excel.Worksheet.Cells[2, 4], excel.Worksheet.Cells[2, 5]];
            r.Merge(true);

            excel.Worksheet.Cells[2, 7] = "외출";
            excel.Worksheet.Cells[3, 7] = "호실";
            excel.Worksheet.Cells[3, 8] = "이름";
            excel.Worksheet.Cells[3, 9] = "외출시간";
            excel.Worksheet.Cells[3, 10] = "복귀시간";
            r = excel.Worksheet.Range[excel.Worksheet.Cells[2, 7], excel.Worksheet.Cells[2, 10]];
            r.Merge(true);

            excel.Worksheet.Cells[2, 12] = "학원";
            excel.Worksheet.Cells[3, 12] = "호실";
            excel.Worksheet.Cells[3, 13] = "이름";
            excel.Worksheet.Cells[3, 14] = "학원시간";
            excel.Worksheet.Cells[3, 15] = "복귀시간";
            r = excel.Worksheet.Range[excel.Worksheet.Cells[2, 12], excel.Worksheet.Cells[2, 15]];
            r.Merge(true);

            excel.Worksheet.Cells[2, 17] = "불출석";
            excel.Worksheet.Cells[3, 17] = "호실";
            excel.Worksheet.Cells[3, 18] = "이름";
            r = excel.Worksheet.Range[excel.Worksheet.Cells[2, 17], excel.Worksheet.Cells[2, 18]];
            r.Merge(true);

            r = excel.Worksheet.Range[excel.Worksheet.Cells[2, 1], excel.Worksheet.Cells[3, 18]];
            r.HorizontalAlignment = XlHAlign.xlHAlignCenter;

            #endregion

            #region 기숙사 데이터 입력

            for (var i = 0; i < LvAnihome.Items.Count; ++i)
            {
                if (!(LvAnihome.Items[i] is BasicData tmp))
                {
                    continue;
                }

                excel.Worksheet.Cells[4 + i, 1] = tmp.RoomNumber;
                excel.Worksheet.Cells[4 + i, 2] = tmp.Name;
            }
            r = excel.Worksheet.Range[excel.Worksheet.Cells[4, 1],
                excel.Worksheet.Cells[4 + LvAnihome.Items.Count - 1, 2]];
            r.HorizontalAlignment = XlHAlign.xlHAlignCenter;

            #endregion

            #region 작화실 데이터 입력

            for (var i = 0; i < LvPrep.Items.Count; ++i)
            {
                if (!(LvPrep.Items[i] is BasicData tmp))
                {
                    continue;
                }

                excel.Worksheet.Cells[4 + i, 4] = tmp.RoomNumber;
                excel.Worksheet.Cells[4 + i, 5] = tmp.Name;
            }
            r = excel.Worksheet.Range[excel.Worksheet.Cells[4, 4],
                excel.Worksheet.Cells[4 + LvPrep.Items.Count - 1, 5]];
            r.HorizontalAlignment = XlHAlign.xlHAlignCenter;

            #endregion

            #region 외출 데이터 입력

            for (var i = 0; i < LvOuting.Items.Count; ++i)
            {
                if (!(LvOuting.Items[i] is OutingData tmp))
                {
                    continue;
                }

                excel.Worksheet.Cells[4 + i, 7] = tmp.RoomNumber;
                excel.Worksheet.Cells[4 + i, 8] = tmp.Name;
                excel.Worksheet.Cells[4 + i, 9] = tmp.OutingStartTime;
                excel.Worksheet.Cells[4 + i, 10] = tmp.OutingReturnTime;
            }
            r = excel.Worksheet.Range[excel.Worksheet.Cells[4, 7],
                excel.Worksheet.Cells[4 + LvOuting.Items.Count - 1, 10]];
            r.HorizontalAlignment = XlHAlign.xlHAlignCenter;

            #endregion

            #region 학원 데이터 입력

            for (var i = 0; i < LvAcademy.Items.Count; ++i)
            {
                if (!(LvAcademy.Items[i] is AcademyData tmp))
                {
                    continue;
                }

                excel.Worksheet.Cells[4 + i, 12] = tmp.RoomNumber;
                excel.Worksheet.Cells[4 + i, 13] = tmp.Name;
                excel.Worksheet.Cells[4 + i, 14] = tmp.AcademyStartTime;
                excel.Worksheet.Cells[4 + i, 15] = tmp.AcademyEndTime;
            }
            r = excel.Worksheet.Range[excel.Worksheet.Cells[4, 12],
                excel.Worksheet.Cells[4 + LvAcademy.Items.Count - 1, 15]];
            r.HorizontalAlignment = XlHAlign.xlHAlignCenter;

            #endregion

            #region 비출석인원 입력

            for (var i = 0; i < LvUnchecked.Items.Count; ++i)
            {
                if (!(LvUnchecked.Items[i] is BasicData tmp))
                {
                    continue;
                }

                excel.Worksheet.Cells[4 + i, 17] = tmp.RoomNumber;
                excel.Worksheet.Cells[4 + i, 18] = tmp.Name;
            }
            r = excel.Worksheet.Range[excel.Worksheet.Cells[4, 17],
                excel.Worksheet.Cells[4 + LvUnchecked.Items.Count - 1, 18]];
            r.HorizontalAlignment = XlHAlign.xlHAlignCenter;

            #endregion

            excel.Save();

            excel.Release();

            if (CbOpenFolder.IsChecked.Equals(true))
                Process.Start(folderDir);
        }

        private void LvAnihome_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LvAnihome.SelectedIndex = -1;
        }

        private void LvPrep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LvPrep.SelectedIndex = -1;
        }

        private void LvOuting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LvOuting.SelectedIndex = -1;
        }

        private void LvAcademy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LvAcademy.SelectedIndex = -1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_noticeBoard.IsLoaded && !_noticeBoard.IsActive)
            {
                _noticeBoard.Activate();
            }
            else
            {
                // If NoticeBoard Closed
                _noticeBoard = new NoticeBoard(out var complete);

                if (complete)
                {
                    _noticeBoard.Show();
                }
                else
                {
                    Close();
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_noticeBoard.IsLoaded)
            {
                _noticeBoard.Close();
                _noticeBoard = null;
            }
            if (Database.IsConnectionOpen)
            {
                Database.Disconnect();
            }

            Application.Current.Shutdown();
        }
    }
}