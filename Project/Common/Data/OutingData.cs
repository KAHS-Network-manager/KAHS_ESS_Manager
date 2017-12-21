using System.Data;

namespace Common.Data
{
    public class OutingData : BasicData
    {
        public OutingData(DataRow data) : base(data)
        {
            OutingStartTime = data["OutingStartTime"].ToString();
            OutingReturnTime = data["OutingReturnTime"].ToString();
        }

        public string OutingStartTime { get; }
        public string OutingReturnTime { get; }
    }
}