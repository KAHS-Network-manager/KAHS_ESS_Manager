using System.Data;

namespace Common.Data
{
    public class Message
    {
        public Message(DataRow data)
        {
            Date = data["Date"].ToString();
            Title = data["Title"].ToString();
            Content = data["Content"].ToString();
        }

        public string Date { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}