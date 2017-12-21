 using System;
using System.Collections.Generic;
using System.Data;
 using System.Linq;

namespace Common.Data
{
    public class StudentData : IEquatable<StudentData>
    {
        private static readonly List<string> Times =
            new List<string>
            {
                "17:00",
                "17:30",
                "18:00",
                "18:30",
                "19:00",
                "19:30",
                "20:00",
                "20:30",
                "21:00",
                "21:30",
                "22:00",
                "22:30",
                "23:00",
                "23:30"
            };

        public StudentData(DataRow data)
        {
            _data = data;

            Name = _data["Name"].ToString();
            Number = _data["Number"].ToString();

            Prep = _data["Prep"].Equals("Y");
            Outing = _data["Outing"].Equals("Y");
            OutingAuthorization = _data["OutingAuthorization"].Equals("Y");
            OutingStartTime = GetIndexFromTime(_data["OutingStartTime"].ToString());
            OutingReturnTime = GetIndexFromTime(_data["OutingReturnTime"].ToString());

            Academy = _data["Academy"].Equals("Y");
            Monday = _data["Monday"].ToString();
            Tuesday = _data["Tuesday"].ToString();
            Wednesday = _data["Wednesday"].ToString();
            Thursday = _data["Thursday"].ToString();
            Friday = _data["Friday"].ToString();
            AcademyStartTime = GetIndexFromTime(_data["AcademyStartTime"].ToString());
            AcademyEndTime = GetIndexFromTime(_data["AcademyEndTime"].ToString());
            Specificant = _data["Specificant"].ToString();
        }
        // 데이터
        private readonly DataRow _data;

        // 개인정보
        public string Name { get; set; }
        public string Number { get; set; }

        // 작화
        public bool Prep { get; set; }

        // 외출
        public bool Outing { get; set; }
        public bool OutingAuthorization { get; set; }
        public int OutingStartTime { get; set; }
        public int OutingReturnTime { get; set; }

        // 학원
        public bool Academy { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public int AcademyStartTime { get; set; }
        public int AcademyEndTime { get; set; }
        public string Specificant { get; set; }

        public string Sql
        {
            get
            {
                var sql = $"UPDATE AfterSchoolActivityStatus SET Prep='{(Prep ? "Y" : "N")}' WHERE Number={Number};";

                if (Outing)
                {
                    if (!OutingAuthorization)
                        throw new Exception($"외출허가 버튼이 체크되어있지 않습니다.;{Name}");
                    if (OutingStartTime < 0 || OutingReturnTime < 0)
                        throw new Exception($"외출시간을 제대로 설정해주세요;{Name}");
                    if (OutingStartTime >= OutingReturnTime)
                        throw new Exception($"외출시간을 제대로 설정해주세요;{Name}");
                    sql +=
                        $"UPDATE AfterSchoolActivityStatus SET Outing='Y', OutingAuthorization='Y', OutingStartTime='{GetTimeFromIndex(OutingStartTime)}', OutingReturnTime='{GetTimeFromIndex(OutingReturnTime)}' WHERE Number={Number};";
                }
                else
                {
                    sql +=
                        $"UPDATE AfterSchoolActivityStatus SET Outing='N', OutingAuthorization='N', OutingStartTime=NULL, OutingReturnTime=NULL WHERE Number={Number};";
                }

                if (Academy)
                {
                    if (AcademyStartTime < 0 || AcademyEndTime < 0)
                        throw new Exception($"학원시간을 제대로 설정해주세요;{Name}");
                    if (AcademyStartTime >= AcademyEndTime)
                        throw new Exception($"학원시간을 제대로 설정해주세요;{Name}");
                    sql +=
                        $"UPDATE AfterSchoolActivityStatus SET Academy='Y' WHERE Number={Number}; " +
                        $"UPDATE AcademyInfo SET Monday='{Monday}', Tuesday='{Tuesday}', Wednesday='{Wednesday}', Thursday='{Thursday}', Friday='{Friday}', AcademyStartTime='{GetTimeFromIndex(AcademyStartTime)}', AcademyEndTime='{GetTimeFromIndex(AcademyEndTime)}', Specificant='{Specificant}' WHERE Number={Number};";
                }
                else
                {
                    sql +=
                        $"UPDATE AfterSchoolActivityStatus SET Academy='N' WHERE Number={Number}; " +
                        $"UPDATE AcademyInfo SET Monday='N', Tuesday='N', Wednesday='N', Thursday='N', Friday='N', AcademyStartTime=NULL, AcademyEndTime=NULL, Specificant=NULL WHERE Number={Number};";
                }

                return sql;
            }
        }

        private static int GetIndexFromTime(string time)
        {
            return Times.FindIndex(current => current.Equals(time));
        }

        private static string GetTimeFromIndex(int index)
        {
            return index >= 0 && index < Times.Count ? Times[index] : "NULL";
        }

        public bool Equals(StudentData other)
        {
            return other != null && other._data.ItemArray.Cast<KeyValuePair<string, string>>().All(col => _data[col.Key].Equals(col.Value));
        }
    }
}