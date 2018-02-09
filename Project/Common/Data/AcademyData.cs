using System.Data;

namespace Common.Data
{
    public class AcademyData : DormitoryData
    {
        public AcademyData(DataRow data) : base(data)
        {
            StartTime = data["AcademyStart"].ToString();
            EndTime = data["AcademyEnd"].ToString();
        }

        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}