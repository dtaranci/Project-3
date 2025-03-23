namespace WebAPI.Models
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public override string ToString()
        => $"{Id} - {Timestamp} - {LogLevel} - {Message}";
    }
}
