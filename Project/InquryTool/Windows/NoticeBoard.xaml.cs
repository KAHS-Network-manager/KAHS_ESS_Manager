using System;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Common;
using Common.Data;

namespace InquryTool.Windows
{
    public partial class NoticeBoard
    {
        private WriteBoard _writeBoard;

        private volatile bool _shouldRun;

        public NoticeBoard(out bool complete)
        {
            InitializeComponent();

            #region Excute Query

            // 게시판 전체 조회 - 최근순
            const string sql = 
                "SELECT * " +
                "FROM Message " +
                "ORDER BY WroteDate DESC";

            try
            {
                using (var adapter = Database.GetAdapter(sql))
                using (var datas = new DataSet())
                {
                    adapter.Fill(datas);
                    foreach (DataRow data in datas.Tables[0].Rows)
                    {
                        LvMessage.Items.Add(new Message(data));
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

            #region AutoRefreshing

            new Thread(() =>
            {
                while (_shouldRun)
                {
                    using (var adapter = Database.GetAdapter(sql))
                    using (var datas = new DataSet())
                    {
                        adapter.Fill(datas);

                        LvMessage.Dispatcher.Invoke(() =>
                        {
                            if (datas.Tables[0].Rows.Count == LvMessage.Items.Count) return;
                            if (!IsActive) Activate();

                            LvMessage.Items.Clear();
                            foreach (DataRow data in datas.Tables[0].Rows)
                            {
                                LvMessage.Items.Add(new Message(data));
                            }
                        });
                    }
                    Thread.Sleep(1000);
                }
            }).Start();

            #endregion
        }

        private void LvMesssages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvMessage.SelectedIndex < 0)
            {
                return;
            }

            if (!(LvMessage.SelectedItem is Message msg))
            {
                return;
            }

            NoticeTitle.Content = msg.Title;
            NoticeContent.Text = msg.Content;
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

        private void Window_Closed(object sender, EventArgs e)
        {
            _shouldRun = false;

            if (_writeBoard.IsLoaded)
            {
                _writeBoard.Close();
            }

            _writeBoard = null;
        }
    }
}