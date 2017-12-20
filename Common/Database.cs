using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Common
{
    public static class Database
    {
        private const string ConnectionInfo = "Server = 127.0.0.1; Database=MainDB; Uid=xklest; Pwd=zxc123;";
        private static MySqlConnection _dbConnection;

        public static bool IsConnectionOpen { get; private set; }
        private static string _prevGrade;
        private static string _prevClass;

        public static void Disconnect()
        {
            _dbConnection.Close();
            IsConnectionOpen = false;
        }

        public static bool Connect()
        {
            try
            {
                _dbConnection = new MySqlConnection(ConnectionInfo);
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
            if (_prevGrade.Equals(gd) && _prevClass.Equals(cs))
            {
                return string.Empty;
            }

            _prevGrade = gd;
            _prevClass = cs;

            return cs.Equals("전체")
                ? $"SELECT * FROM Students Std INNER JOIN AfterSchoolActivityStatus Act, AcademyInfo Acd WHERE Std.Number=Act.Number AND Std.Number=Acd.Number AND Std.Grade='{gd}';"
                : $"SELECT * FROM Students Std INNER JOIN AfterSchoolActivityStatus Act, AcademyInfo Acd WHERE Std.Number=Act.Number AND Std.Number=Acd.Number AND Std.Grade='{gd}' AND Std.Class='{cs}';";
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