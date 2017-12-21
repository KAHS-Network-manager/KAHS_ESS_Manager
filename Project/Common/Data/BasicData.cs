using System.Data;

namespace Common.Data
{
    public class BasicData
    {
        public BasicData(DataRow data)
        {
            RoomNumber = data["RoomNumber"].ToString();
            Name = data["Name"].ToString();
        }

        public string RoomNumber { get; set; }
        public string Name { get; set; }
    }
}