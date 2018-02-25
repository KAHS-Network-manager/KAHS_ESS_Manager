 using System;
using System.Collections.Generic;
using System.Data;
 using System.Globalization;
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
            Name = data["Name"].ToString();
            Number = data["Number"].ToString();
            
            /*
             * 0 : 작화실
             * 1 : 기숙사
             * 2 : 외출
             * 3 : 학원
             * 4 : 출석안함
             */
            EssStatus = data[DateTime.Today.ToString("dddd", new CultureInfo("en-US"))].Equals("Y")
                ? 3
                : data["Outing"].Equals("Y")
                    ? 2
                    : data["Dormitory"].Equals("Y")
                        ? 1
                        : data["Ess"].Equals("Y")
                            ? 0
                            : 4;
            OutingStartTime = GetIndexFromTime(data["OutingStart"].ToString());
            OutingEndTime = GetIndexFromTime(data["OutingEnd"].ToString());

            Monday = data["Monday"].Equals("Y");
            Tuesday = data["Tuesday"].Equals("Y");
            Wednesday = data["Wednesday"].Equals("Y");
            Thursday = data["Thursday"].Equals("Y");
            Friday = data["Friday"].Equals("Y");
            AcademyStartTime = GetIndexFromTime(data["AcademyStart"].ToString());
            AcademyEndTime = GetIndexFromTime(data["AcademyEnd"].ToString());
            Remarks = data["Remarks"].ToString();
        }
      
        // XAML 연동 변수
        
        // 개인 정보
        public string Name { get; set; }
        public string Number { get; set; }

        // 작화
        public int EssStatus { get; set; }

        // 외출
        public int OutingStartTime { get; set; }
        public int OutingEndTime { get; set; }

        // 학원
        public bool Academy { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public int AcademyStartTime { get; set; }
        public int AcademyEndTime { get; set; }
        public string Remarks { get; set; }

        public string Sql
        {
            get
            {
                var sql = "UPDATE Student SET ";

                if (Monday || Tuesday || Wednesday || Thursday || Friday)
                {
                    if (AcademyStartTime >= AcademyEndTime)
                        throw new Exception($"학원 시간을 제대로 설정해 주세요;{Name}");
                    if (AcademyStartTime < 0 && AcademyEndTime < 0)
                        throw new Exception($"학원 시간을 제대로 설정해 주세요;{Name}");

                    sql +=
                            $"AcademyStart='{GetTimeFromIndex(AcademyStartTime)}',AcademyEnd='{GetTimeFromIndex(AcademyEndTime)}'," +
                            $"Monday='{(Monday ? "Y" : "N")}',Tuesday='{(Tuesday ? "Y" : "N")}',Wednesday='{(Wednesday ? "Y" : "N")}',Thursday='{(Thursday ? "Y" : "N")}',Friday='{(Friday ? "Y" : "N")}',Remarks='{Remarks}',";


                    /*
                     * 위에서 SQL을 만들었을 때 이 SQL이
                     * Monday='Y', Tues.... 이렇게 만들어져 있다.
                     * 이 때 오늘 날짜에 학원을 가는지에 대한 여부를 확인하는 코드이다.
                     *
                     * Example (오늘이 월요일일 경우)
                     * today = "Monday";
                     * sql 내에서 Monday='Y' 일 경우
                     * idx = sql.IndexOf(today) (today string이 sql에서 위치하고있는 pos) + today.Length + 2 ( "='" 이 두 characters)
                     * sql[idx] 는 Y를 가지고 있게 된다.
                     */
                    var today = DateTime.Today.ToString("dddd", new CultureInfo("en-US"));
                    if (sql[sql.IndexOf(today, StringComparison.Ordinal) + today.Length + 2] == 'Y')
                        EssStatus = 3;
                }
                else
                {
                    sql += "AcademyStart=NULL, AcademyEnd=NULL," +
                    "Monday=\'N\',Tuesday=\'N\',Wednesday=\'N\',Thursday=\'N\',Friday=\'N\',Remarks=NULL,";
                }

                sql += $"Ess='{(EssStatus == 0 ? "Y" : "N")}'," +
                       $"Dormitory='{(EssStatus == 1 ? "Y" : "N")}'," +
                       $"Outing='{(EssStatus == 2 ? "Y" : "N")}',";

                switch (EssStatus)
                {
                    case 0:
                    case 1:
                    case 3:
                    case 4:
                        sql += "OutingStart=NULL, OutingEnd=NULL ";
                        break;
                    case 2:
                        if (OutingStartTime < 0 || OutingEndTime < 0)
                            throw new Exception($"외출 시간을 제대로 설정해 주세요;{Name}");
                        if (OutingStartTime >= OutingEndTime)
                            throw new Exception($"외출 시간을 제대로 설정해 주세요;{Name}");
                        sql +=
                            $"OutingStart='{GetTimeFromIndex(OutingStartTime)}',OutingEnd='{GetTimeFromIndex(OutingEndTime)}' ";
                        break;
                }
                sql += $"WHERE Number='{Number}';";

                return sql;
            }
        }

        private static int GetIndexFromTime(string time)
        {
            return Times.IndexOf(time);
        }

        private static string GetTimeFromIndex(int index)
        {
            return index >= 0 && index < Times.Count ? Times[index] : "NULL";
        }

        public bool Equals(StudentData other)
        {
            return other != null && (Name == other.Name && Number == other.Number) && EssStatus == other.EssStatus && OutingStartTime == other.OutingStartTime && OutingEndTime == other.OutingEndTime && Academy == other.Academy && Monday == other.Monday && Tuesday == other.Tuesday && Wednesday == other.Wednesday && Thursday == other.Thursday && Friday == other.Friday && AcademyStartTime == other.AcademyEndTime && AcademyEndTime == other.AcademyEndTime && Remarks == other.Remarks;
        }
    }
}