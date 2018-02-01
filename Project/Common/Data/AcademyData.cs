using System.Data;

namespace Common.Data
{
    public class AcademyData : DormitoryData
    {
        public AcademyData(DataRow data) : base(data)
        {
            StartTime = data["A.StartTime"].ToString();
            EndTime = data["A.EndTime"].ToString();
        }

        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}