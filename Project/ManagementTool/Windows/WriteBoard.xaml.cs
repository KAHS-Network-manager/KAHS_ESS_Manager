using System;
using System.Windows;
using Common;

namespace ManagementTool.Windows
{     
    public partial class WriteBoard
    {
        public WriteBoard()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TbWriteTitle.Text) || string.IsNullOrEmpty(TbWriteContent.Text))
            {
                return;
            }

            var sql =
                $"INSERT INTO Messages(`Title`, `Content`) values('{TbWriteTitle.Text}', '{TbWriteContent.Text}');";
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
                Close();
            }
            MessageBox.Show("전송 완료", "Complete");
        }
    }
}