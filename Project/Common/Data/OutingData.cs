using System.Data;

namespace Common.Data
{
    public class OutingData : DormitoryData
    {
        public OutingData(DataRow data) : base(data)
        {
            StartTime = data["OutingStart"].ToString();
            EndTime = data["OutingEnd"].ToString();
        }

        public string StartTime { get; }
        public string EndTime { get; }
    }
}