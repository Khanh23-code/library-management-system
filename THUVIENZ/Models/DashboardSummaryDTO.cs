namespace THUVIENZ.Models
{
    public class DashboardSummaryDTO
    {
        public int TotalBooks { get; set; }
        public int TotalReaders { get; set; }
        public int BorrowedBooks { get; set; }
        public int OverdueBooks { get; set; }
    }
}
