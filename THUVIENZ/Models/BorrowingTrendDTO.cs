using System;

namespace THUVIENZ.Models
{
    public class BorrowingTrendDTO
    {
        public DateTime Date { get; set; }
        public int BorrowCount { get; set; }
        public int OverdueCount { get; set; }
    }
}
