using System;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Common;
using Common.Data;

namespace InquryTool.Windows
{
    /// <summary>
    ///     NoticeBoard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NoticeBoard
    {
        private WriteBoard _wb;

        public NoticeBoard(out bool complete)
        {
            InitializeComponent();

            _wb = new WriteBoard();

            #region Excute Query

            const string sql = "SELECT * FROM Messages ORDER BY WroteDate DESC";

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
            ShouldRun = true;

            #endregion

            #region AutoRefreshing

            new Thread(() =>
            {
                while (ShouldRun)
                {
                    using (var adapter = Database.GetAdapter(sql))
                    using (var datas = new DataSet())
                    {
                        adapter.Fill(datas);

                        LvMesssages.Dispatcher.Invoke(() =>
                        {
                            if (datas.Tables[0].Rows.Count == LvMesssages.Items.Count)
                            {
                                return;
                            }

                            LvMesssages.Items.Clear();
                            foreach (DataRow data in datas.Tables[0].Rows)
                            {
                                LvMesssages.Items.Add(new Message(data));
                            }
                        });
                    }
                    Thread.Sleep(1000);
                }
            }).Start();

            #endregion
        }

        private bool ShouldRun { get; set; }

        private void LvMesssages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvMesssages.SelectedIndex < 0)
            {
                return;
            }

            var msg = LvMesssages.SelectedItem as Message;

            if (msg == null)
            {
                return;
            }

            NoticeTitle.Content = msg.Title;
            NoticeContent.Text = msg.Content;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                if (_wb.IsLoaded && _wb.IsActive)
                {
                    _wb.Activate();
                }
                else
                {
                    _wb = new WriteBoard();
                    _wb.Show();
                    continue;
                }
                break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ShouldRun = false;

            if (_wb.IsLoaded)
            {
                _wb.Close();
            }

            _wb = null;
        }
    }
}