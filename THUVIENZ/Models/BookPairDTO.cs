namespace THUVIENZ.Models
{
    public class BookPairDTO
    {
        public string Book1Name { get; set; } = string.Empty;
        public string Book2Name { get; set; } = string.Empty;
        public int SupportCount { get; set; }
        public string Description => $"Được mượn cùng nhau {SupportCount} lần";
    }
}
