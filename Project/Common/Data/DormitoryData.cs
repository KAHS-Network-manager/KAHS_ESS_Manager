using System.Data;

namespace Common.Data
{
    public class DormitoryData
    {
        public DormitoryData(DataRow data)
        {
            RoomNumber = data["RoomNumber"].ToString();
            Name = data["Name"].ToString();
        }

        public string RoomNumber { get; set; }
        public string Name { get; set; }
    }
}