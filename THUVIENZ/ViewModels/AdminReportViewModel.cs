using System;
using LiveCharts;
using LiveCharts.Wpf;
using THUVIENZ.Core;

namespace THUVIENZ.ViewModels
{
    public class AdminReportViewModel : ObservableObject
    {
        // --- DATA CHO KPI CARDS ---
        public string TotalBooks { get; set; }
        public string TotalReaders { get; set; }
        public string BorrowedBooks { get; set; }
        public string OverdueBooks { get; set; }

        // --- DATA CHO BIỂU ĐỒ ĐƯỜNG/CỘT (Lượt mượn / Trễ hạn) ---
        public SeriesCollection BorrowingTrendSeries { get; set; }
        public string[] TrendLabels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        // --- DATA CHO BIỂU ĐỒ TRÒN (Tỉ lệ thể loại) ---
        public SeriesCollection CategoryRatioSeries { get; set; }

        public AdminReportViewModel()
        {
            LoadMockData();
        }

        private void LoadMockData()
        {
            // 1. Gán số liệu KPI
            TotalBooks = "1,250";
            TotalReaders = "845";
            BorrowedBooks = "128";
            OverdueBooks = "14";

            // 2. Cấu hình Biểu đồ Đường (Lượt mượn 7 ngày qua)
            BorrowingTrendSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Sách được mượn",
                    Values = new ChartValues<int> { 25, 30, 45, 20, 60, 80, 55 },
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                },
                new LineSeries
                {
                    Title = "Sách trễ hạn",
                    Values = new ChartValues<int> { 2, 0, 5, 1, 3, 2, 4 },
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 10
                }
            };
            TrendLabels = new[] { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };
            YFormatter = value => value.ToString("N0");

            // 3. Cấu hình Biểu đồ Tròn (Tỉ lệ thể loại)
            CategoryRatioSeries = new SeriesCollection
            {
                new PieSeries { Title = "Công nghệ", Values = new ChartValues<double> { 40 }, DataLabels = true },
                new PieSeries { Title = "Tiểu thuyết", Values = new ChartValues<double> { 30 }, DataLabels = true },
                new PieSeries { Title = "Kỹ năng mềm", Values = new ChartValues<double> { 20 }, DataLabels = true },
                new PieSeries { Title = "Khác", Values = new ChartValues<double> { 10 }, DataLabels = true }
            };
        }
    }
}