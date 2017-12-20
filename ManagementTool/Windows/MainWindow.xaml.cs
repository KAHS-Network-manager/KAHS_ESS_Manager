using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Common;
using Common.Data;
using MySql.Data.MySqlClient;

namespace ManagementTool.Windows
{
    public partial class MainWindow
    {
        private NoticeBoard _noticeBoard;

        private readonly List<StudentData> _prevStdudentDatas;

        public MainWindow()
        {
            InitializeComponent();

            if (!Database.Connect())
            {
                Close();
            }

            _noticeBoard = new NoticeBoard(out var complete);
            if (!complete)
            {
                Close();
            }
            _prevStdudentDatas = new List<StudentData>();
        }

        private void BtInqury_Click(object sender, RoutedEventArgs e)
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

                    // 리스트뷰에 데이터가 있을 경우 초기화
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // If NoticeBoard is Shown and is not activated
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

        private void BtDone_Click(object sender, RoutedEventArgs e)
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

            MessageBox.Show("수정완료!");
            Bt_Inqury_Click(null, null);

            #endregion
        }

        private static string[] ParseExceptionMessage(string message)
        {
            int idx;
            var sb1 = new StringBuilder();
            for (idx = 0; message[idx] != ';' || idx < message.Length; ++idx)
            {
                sb1.Append(message[idx]);
            }
            var sb2 = new StringBuilder();
            for (idx += 1; idx < message.Length; ++idx)
            {
                sb2.Append(message[idx]);
            }

            return new[] {sb1.ToString(), sb2.ToString()};
        }

        private void LvStudentData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // User Cannot Select Item  
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