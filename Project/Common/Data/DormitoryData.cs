using System.Data;

namespace Common.Data
{
    public class DormitoryData
    {
        public DormitoryData(DataRow data)
        {
            RoomNumber = data["S.RoomNumber"].ToString();
            Name = data["S.Name"].ToString();
        }

        public string RoomNumber { get; set; }
        public string Name { get; set; }
    }
}