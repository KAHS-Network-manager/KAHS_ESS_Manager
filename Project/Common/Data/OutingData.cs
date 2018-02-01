using System.Data;

namespace Common.Data
{
    public class OutingData : DormitoryData
    {
        public OutingData(DataRow data) : base(data)
        {
            StartTime = data["O.StartTime"].ToString();
            EndTime = data["O.EndTime"].ToString();
        }

        public string StartTime { get; }
        public string EndTime { get; }
    }
}