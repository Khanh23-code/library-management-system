using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;
using THUVIENZ.BLL;
using THUVIENZ.Core;

namespace THUVIENZ.ViewModels
{
    public class AdminReportViewModel : ObservableObject
    {
        private readonly ReportService _reportService;

        // --- DATA CHO KPI CARDS ---
        private string _totalBooks = "0";
        public string TotalBooks
        {
            get => _totalBooks;
            set => SetProperty(ref _totalBooks, value);
        }

        private string _totalReaders = "0";
        public string TotalReaders
        {
            get => _totalReaders;
            set => SetProperty(ref _totalReaders, value);
        }

        private string _borrowedBooks = "0";
        public string BorrowedBooks
        {
            get => _borrowedBooks;
            set => SetProperty(ref _borrowedBooks, value);
        }

        private string _overdueBooks = "0";
        public string OverdueBooks
        {
            get => _overdueBooks;
            set => SetProperty(ref _overdueBooks, value);
        }

        // --- DATA CHO BIỂU ĐỒ ĐƯỜNG/CỘT (Lượt mượn / Trễ hạn) ---
        private SeriesCollection? _borrowingTrendSeries;
        public SeriesCollection? BorrowingTrendSeries
        {
            get => _borrowingTrendSeries;
            set => SetProperty(ref _borrowingTrendSeries, value);
        }

        private string[]? _trendLabels;
        public string[]? TrendLabels
        {
            get => _trendLabels;
            set => SetProperty(ref _trendLabels, value);
        }

        public Func<double, string> YFormatter { get; set; } = value => value.ToString("N0");

        // --- DATA CHO BIỂU ĐỒ TRÒN (Tỉ lệ thể loại) ---
        private SeriesCollection? _categoryRatioSeries;
        public SeriesCollection? CategoryRatioSeries
        {
            get => _categoryRatioSeries;
            set => SetProperty(ref _categoryRatioSeries, value);
        }

        public AdminReportViewModel()
        {
            _reportService = new ReportService();
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // 1. Tải KPI Summary
                var summary = await _reportService.GetDashboardSummaryAsync();
                TotalBooks = summary.TotalBooks.ToString("N0");
                TotalReaders = summary.TotalReaders.ToString("N0");
                BorrowedBooks = summary.BorrowedBooks.ToString("N0");
                OverdueBooks = summary.OverdueBooks.ToString("N0");

                // 2. Tải Xu hướng mượn sách (7 ngày qua)
                var toDate = DateTime.Now;
                var fromDate = toDate.AddDays(-6);
                var trendData = await _reportService.GetBorrowingTrendAsync(fromDate, toDate);

                // Chuẩn bị dữ liệu cho LiveCharts
                var borrowValues = new ChartValues<int>();
                var overdueValues = new ChartValues<int>();
                var labels = new List<string>();

                // Đảm bảo đủ 7 ngày kể cả ngày không có dữ liệu
                for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
                {
                    var dayStat = trendData.FirstOrDefault(t => t.Date.Date == date);
                    borrowValues.Add(dayStat?.BorrowCount ?? 0);
                    overdueValues.Add(dayStat?.OverdueCount ?? 0);
                    labels.Add(date.ToString("dd/MM"));
                }

                BorrowingTrendSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Sách được mượn",
                        Values = borrowValues,
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 8
                    },
                    new LineSeries
                    {
                        Title = "Sách trễ hạn",
                        Values = overdueValues,
                        PointGeometry = DefaultGeometries.Square,
                        PointGeometrySize = 8,
                        Stroke = System.Windows.Media.Brushes.Red,
                        Fill = System.Windows.Media.Brushes.Transparent
                    }
                };
                TrendLabels = labels.ToArray();

                // 3. Tải Tỉ lệ thể loại
                var categoryStats = await _reportService.GetBorrowingStatsByCategoryAsync(fromDate, toDate);
                var pieSeries = new SeriesCollection();

                foreach (dynamic stat in categoryStats)
                {
                    pieSeries.Add(new PieSeries
                    {
                        Title = stat.CategoryName,
                        Values = new ChartValues<int> { stat.BorrowCount },
                        DataLabels = true
                    });
                }
                CategoryRatioSeries = pieSeries;
            }
            catch (Exception ex)
            {
                // TODO: Handle error properly (e.g. show message box)
                System.Diagnostics.Debug.WriteLine($"Error loading report data: {ex.Message}");
            }
        }
    }
}