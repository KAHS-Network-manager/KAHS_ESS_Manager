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

            OpenNoticeBoard(null, null);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (!Database.Connect(""))
            {
                MessageBox.Show("Failed to connect to database.", "ERROR");
                Close();
            }

            // 전체 데이터 조회 - 호실, 이름순
            const string sql =
                "SELECT * " +
                "FROM Student " +
                "ORDER BY RoomNumber ASC, Name ASC;";

            #region 쿼리 실행

            try
            {
                using (var adapter = Database.GetAdapter(sql))
                using (var set = new DataSet())
                {
                    adapter.Fill(set);
                    foreach (DataRow data in set.Tables[0].Rows)
                    {
                        // 작화실
                        if (data["Ess"].Equals("Y"))
                        {
                            LvEss.Items.Add(new DormitoryData(data));
                        }
                        // 기숙사
                        else if (data["Dormitory"].Equals("Y"))
                        {
                            LvDormitory.Items.Add(new DormitoryData(data));
                        }
                        // 외출
                        else if (data["Outing"].Equals("Y"))
                        {
                            LvOuting.Items.Add(new OutingData(data));
                        }
                        // 학원
                        else if (data[DateTime.Today.ToString("dddd", new CultureInfo("en-US"))].Equals("Y"))
                        {
                            LvAcademy.Items.Add(new AcademyData(data));
                        }
                        // 미출석
                        else
                        {
                            LvUnchecked.Items.Add(new DormitoryData(data));
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
        private void ExportToFile(object sender, RoutedEventArgs e)
        {
            if (!(LvDormitory.HasItems || LvEss.HasItems || LvOuting.HasItems || LvAcademy.HasItems))
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

            for (var i = 0; i < LvDormitory.Items.Count; ++i)
            {
                if (!(LvDormitory.Items[i] is DormitoryData tmp))
                {
                    continue;
                }

                excel.Worksheet.Cells[4 + i, 1] = tmp.RoomNumber;
                excel.Worksheet.Cells[4 + i, 2] = tmp.Name;
            }
            r = excel.Worksheet.Range[excel.Worksheet.Cells[4, 1],
                excel.Worksheet.Cells[4 + LvDormitory.Items.Count - 1, 2]];
            r.HorizontalAlignment = XlHAlign.xlHAlignCenter;

            #endregion

            #region 작화실 데이터 입력

            for (var i = 0; i < LvEss.Items.Count; ++i)
            {
                if (!(LvEss.Items[i] is DormitoryData tmp))
                {
                    continue;
                }

                excel.Worksheet.Cells[4 + i, 4] = tmp.RoomNumber;
                excel.Worksheet.Cells[4 + i, 5] = tmp.Name;
            }
            r = excel.Worksheet.Range[excel.Worksheet.Cells[4, 4],
                excel.Worksheet.Cells[4 + LvEss.Items.Count - 1, 5]];
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
                excel.Worksheet.Cells[4 + i, 9] = tmp.StartTime;
                excel.Worksheet.Cells[4 + i, 10] = tmp.EndTime;
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
                excel.Worksheet.Cells[4 + i, 14] = tmp.StartTime;
                excel.Worksheet.Cells[4 + i, 15] = tmp.EndTime;
            }
            r = excel.Worksheet.Range[excel.Worksheet.Cells[4, 12],
                excel.Worksheet.Cells[4 + LvAcademy.Items.Count - 1, 15]];
            r.HorizontalAlignment = XlHAlign.xlHAlignCenter;

            #endregion

            #region 비출석인원 입력

            for (var i = 0; i < LvUnchecked.Items.Count; ++i)
            {
                if (!(LvUnchecked.Items[i] is DormitoryData tmp))
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
            LvDormitory.SelectedIndex = -1;
        }

        private void LvEss_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LvEss.SelectedIndex = -1;
        }

        private void LvOuting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LvOuting.SelectedIndex = -1;
        }

        private void LvAcademy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LvAcademy.SelectedIndex = -1;
        }

        private void OpenNoticeBoard(object sender, RoutedEventArgs e)
        {
            if (_noticeBoard is null || !_noticeBoard.IsLoaded)
            {
                // 게시판 창이 열려 있지 않을 경우
                //
                // 창을 새로 만듬

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
            else if(_noticeBoard.IsLoaded && !_noticeBoard.IsActive)
            {
                // 게시판 창이 이미 실행 되어 있을 경우
                //
                // 창을 새로 만들지 않고 맨 위로 올림
                _noticeBoard.Activate();
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