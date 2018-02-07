using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Common;
using Common.Data;

namespace ManagementTool.Windows
{
    public partial class MainWindow
    {
        private NoticeBoard _noticeBoard;

        private readonly List<StudentData> _prevStdudentDatas;

        public MainWindow()
        {
            InitializeComponent();

            if (!Database.Connect(""))
            {
                Close();
            }

            OpenNoticeBoard(null, null);
            _prevStdudentDatas = new List<StudentData>();
        }

        private void GetData(object sender, RoutedEventArgs e)
        {
            if (CbClass.SelectedIndex < 0 || CbGrade.SelectedIndex < 0)
            {
                return;
            }

            try
            {
                var sql = Database.GetSelectSql(CbGrade, CbClass);
                if (string.IsNullOrEmpty(sql))
                {
                    return;
                }

                using (var dataAdapter = Database.GetAdapter(sql))
                using (var datas = new DataSet())
                {
                    dataAdapter.Fill(datas);

                    ClearListView();

                    foreach (DataRow data in datas.Tables[0].Rows)
                    {
                        LvStudentData.Items.Add(new StudentData(data));
                        _prevStdudentDatas.Add(new StudentData(data));
                    } 
                }
            }
            catch (Exception exception)
            {
                ClearListView();
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void ClearListView()
        {
            if (!LvStudentData.HasItems)
            {
                return;
            }

            LvStudentData.Items.Clear();
            _prevStdudentDatas.Clear();
        }

        private void OpenNoticeBoard(object sender, RoutedEventArgs e)
        {
            if (_noticeBoard.IsLoaded && !_noticeBoard.IsActive)
            {
                // 게시판 창이 이미 실행 되어 있을 경우
                //
                // 창을 새로 만들지 않고 맨 위로 올림

                _noticeBoard.Activate();
            }
            else
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
        }

        private void Inqury(object sender, RoutedEventArgs e)
        {
            if (LvStudentData.HasItems.Equals(false))
            {
                return;
            }

            #region MakeQuery

            var sql = string.Empty;
            try
            {
                sql = LvStudentData.Items.Cast<StudentData>().Where((t, i) => !_prevStdudentDatas[i].Equals(t))
                    .Aggregate(sql, (current, t) => current + t.Sql);
            }
            catch (Exception exception)
            {
                var data = ParseExceptionMessage(exception.Message);
                MessageBox.Show(data[0], data[1]);
                return;
            }

            #endregion

            #region ExcuteQuery

            if (string.IsNullOrEmpty(sql))
            {
                return;
            }

            try
            {
                using (var cmd = Database.GetCommand(sql))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK);
            }

            MessageBox.Show("수정 완료!");
            GetData(null, null);

            #endregion
        }

        private static string[] ParseExceptionMessage(string message)
        {
            int idx;
            StringBuilder[] sb = { new StringBuilder(), new StringBuilder() };
            for (idx = 0; message[idx] != ';' || idx < message.Length; ++idx)
            {
                sb[0].Append(message[idx]);
            }
            for (idx += 1; idx < message.Length; ++idx)
            {
                sb[1].Append(message[idx]);
            }
            return new[] { sb[0].ToString(), sb[1].ToString() };
        }

        private void LvStudentData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LvStudentData.SelectedIndex = -1;
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

            System.Windows.Application.Current.Shutdown();
        }
    }
}