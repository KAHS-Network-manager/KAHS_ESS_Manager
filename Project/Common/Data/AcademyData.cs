using System.Data;

namespace Common.Data
{
    public class AcademyData : BasicData
    {
        public AcademyData(DataRow data) : base(data)
        {
            AcademyStartTime = data["AcademyStartTime"].ToString();
            AcademyEndTime = data["AcademyEndTime"].ToString();
        }

        public string AcademyStartTime { get; set; }
        public string AcademyEndTime { get; set; }
    }
}