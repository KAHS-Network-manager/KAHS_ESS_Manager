using System;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Common;
using Common.Data;

namespace ManagementTool.Windows
{
    public partial class NoticeBoard
    {
        private WriteBoard _writeBoard;

        private bool _shouldRun;

        public NoticeBoard(out bool complete)
        {
            InitializeComponent();

            _writeBoard = new WriteBoard();


            // 게시글을 가져오는 쿼리
            #region Excute Query

            const string sql = "SELECT * FROM Message ORDER BY WroteDate DESC";
            try
            {
                using (var adapter = Database.GetAdapter(sql))
                using (var datas = new DataSet())
                {
                    adapter.Fill(datas);
                    foreach (DataRow data in datas.Tables[0].Rows)
                    {
                        LvMesssages.Items.Add(new Message(data));
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK);
                complete = false;
                Close();
            }

            complete = true;
            _shouldRun = true;

            #endregion

            // 초당 한번씩 DB에서 메시지를 가져오는 스레드
            #region AutoRefreshing

            new Thread(() =>
            {
                while (_shouldRun)
                {
                    // 추가한 스레드 에서는 WPF 컴포넌트의 스레드를 거쳐서 WPF 변수의 값을 바꿀 수 있음
                    LvMesssages.Dispatcher.Invoke(() =>
                    {
                        using (var adapter = Database.GetAdapter(sql))
                        using (var datas = new DataSet())
                        {
                            adapter.Fill(datas);
                            if (datas.Tables[0].Rows.Count == LvMesssages.Items.Count) return;
                            LvMesssages.Items.Clear();
                            foreach (DataRow data in datas.Tables[0].Rows)
                            {
                                LvMesssages.Items.Add(new Message(data));
                            }

                        }
                    });
                    Thread.Sleep(1000);
                }
            }).Start();

            #endregion
        }

        private void LvMesssages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvMesssages.SelectedIndex < 0)
            {
                return;
            }

            var msg = LvMesssages.SelectedItem as Message;
            if (msg is null)
            {
                return;
            }

            NoticeTitle.Content = msg.Title;
            NoticeContent.Text = msg.Content;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _shouldRun = false;

            if (_writeBoard.IsLoaded)
            {
                _writeBoard.Close();
            }

            _writeBoard = null;
        }

        private void OpenWriteBoard(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                if (_writeBoard.IsLoaded && _writeBoard.IsActive)
                {
                    _writeBoard.Activate();
                }
                else
                {
                    _writeBoard = new WriteBoard();
                    _writeBoard.Show();
                    continue;
                }
                break;
            }
        }
    }
}