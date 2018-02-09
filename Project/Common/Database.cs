using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Common
{
    public static class Database
    {
        private static MySqlConnection _dbConnection;

        public static bool IsConnectionOpen { get; private set; }

        public static void Disconnect()
        {
            _dbConnection.Close();
            IsConnectionOpen = false;
        }

        public static bool Connect(string connectionInfo)
        {
            try
            {
                _dbConnection = new MySqlConnection(connectionInfo);
                _dbConnection.Open();
                IsConnectionOpen = true;
            }
            catch (Exception)
            {
                MessageBox.Show("서버에 연결할 수 없습니다. 인터넷 상태를 확인해 주세요.", "Error", MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        public static string GetSelectSql(ComboBox grade, ComboBox @class)
        {
            var gd = ((ComboBoxItem) grade.SelectedItem).Content.ToString();
            var cs = ((ComboBoxItem) @class.SelectedItem).Content.ToString();

            if (string.IsNullOrEmpty(gd))
            {
                throw new Exception("학년을 제대로 선택 해 주십시오.");
            }
            if (string.IsNullOrEmpty(cs))
            {
                throw new Exception("반을 제대로 선택 해 주십시오.");
            }

            return "SELECT * " +
                   "FROM Student " +
                   $"WHERE Grade='{gd}'{(cs.Equals("전체") ? ";" : $" AND Class='{cs}';")}";
        }

        public static MySqlDataAdapter GetAdapter(string sql)
        {
            return new MySqlDataAdapter(sql, _dbConnection);
        }

        public static MySqlCommand GetCommand(string sql)
        {
            return new MySqlCommand(sql, _dbConnection);
        }
    }
}