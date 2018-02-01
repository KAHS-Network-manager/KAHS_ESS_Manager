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

            Name = _data["S.Name"].ToString();
            Number = _data["S.Number"].ToString();


            EssStatus = _data["E.Ess"].Equals("Y")
                ? 0
                : _data["E.Dormitory"].Equals("Y")
                    ? 1
                    : _data["O.Whether"].Equals("Y")
                        ? 2
                        : _data["A.Whether"].Equals("Y")
                            ? 3
                            : 4;
            OutingStartTime = GetIndexFromTime(_data["E.StartTime"].ToString());
            OutingReturnTime = GetIndexFromTime(_data["E.EndTime"].ToString());

            Monday = _data["A.Monday"].ToString();
            Tuesday = _data["A.Tuesday"].ToString();
            Wednesday = _data["A.Wednesday"].ToString();
            Thursday = _data["A.Thursday"].ToString();
            Friday = _data["A.Friday"].ToString();
            AcademyStartTime = GetIndexFromTime(_data["A.StartTime"].ToString());
            AcademyEndTime = GetIndexFromTime(_data["A.EndTime"].ToString());
            Remarks = _data["A.Remarks"].ToString();
        }
        // 데이터
        private readonly DataRow _data;

        // 개인 정보
        public string Name { get; set; }
        public string Number { get; set; }

        // 작화
        public int EssStatus { get; set; }

        // 외출
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
        public string Remarks { get; set; }

        public string Sql
        {
            get
            {
                var sql = "UPDATE ESS SET " +
                          $"Ess={(EssStatus == 0 ? "Y" : "N")}, Dormitory={(EssStatus == 1 ? "Y" : "N")} " +
                          $"WHERE Number = {Number}";

                if (EssStatus == 2)
                {
                    if (!OutingAuthorization)
                        throw new Exception($"외출 허가 버튼이 체크 되지 않았습니다;{Name}");
                    if (OutingStartTime < 0 || OutingReturnTime < 0)
                        throw new Exception($"외출 시간을 제대로 설정해 주세요;{Name}");
                    if (OutingStartTime >= OutingReturnTime)
                        throw new Exception($"외출 시간을 제대로 설정해 주세요;{Name}");
                    sql +=
                        "UPDATE Outing SET " +
                        $"Whether='Y', StartTime='{GetTimeFromIndex(OutingStartTime)}', EndTime='{GetTimeFromIndex(OutingReturnTime)}' " +
                        $"WHERE Number={Number};";
                }
                else
                {
                    sql +=
                        "UPDATE ESS SET " +
                        "Outing='N', OutingAuthorization='N', OutingStartTime=NULL, OutingReturnTime=NULL " +
                        $"WHERE Number={Number};";
                }

                if (EssStatus == 3)
                {
                    if (AcademyStartTime < 0 || AcademyEndTime < 0)
                        throw new Exception($"학원 시간을 제대로 설정해 주세요;{Name}");
                    if (AcademyStartTime >= AcademyEndTime)
                        throw new Exception($"학원 시간을 제대로 설정해 주세요;{Name}");
                    sql +=
                        "UPDATE Academy SET " +
                        $"Whether='Y' Monday='{Monday}', Tuesday='{Tuesday}', Wednesday='{Wednesday}', Thursday='{Thursday}', Friday='{Friday}', " +
                        $"StartTime='{GetTimeFromIndex(AcademyStartTime)}', EndTime='{GetTimeFromIndex(AcademyEndTime)}', Remarks='{Remarks}' " +
                        $"WHERE Number={Number};";
                }
                else
                {
                    sql +=
                        "UPDATE Academy SET " +
                        "Whether='N', Monday='N', Tuesday='N', Wednesday='N', Thursday='N', Friday='N', AcademyStartTime=NULL, AcademyEndTime=NULL, Remarks=NULL " +
                        $"WHERE Number={Number};";
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