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
        private int _currentDays = 7;

        // --- COMMANDS ---
        public RelayCommand ChangeFilterCommand { get; }

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
        private string _trendTitle = "Lượt mượn & Trễ hạn (7 ngày qua)";
        public string TrendTitle
        {
            get => _trendTitle;
            set => SetProperty(ref _trendTitle, value);
        }

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

        private double _labelStep = 1;
        public double LabelStep
        {
            get => _labelStep;
            set => SetProperty(ref _labelStep, value);
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
            ChangeFilterCommand = new RelayCommand(p =>
            {
                if (int.TryParse(p?.ToString(), out int days))
                {
                    _ = LoadDataAsync(days);
                }
            });

            _ = LoadDataAsync(7);
        }

        private async Task LoadDataAsync(int days)
        {
            _currentDays = days;
            TrendTitle = $"Lượt mượn & Trễ hạn ({days} ngày qua)";

            try
            {
                // 1. Tải KPI Summary (Luôn lấy tổng thể)
                var summary = await _reportService.GetDashboardSummaryAsync();
                TotalBooks = summary.TotalBooks.ToString("N0");
                TotalReaders = summary.TotalReaders.ToString("N0");
                BorrowedBooks = summary.BorrowedBooks.ToString("N0");
                OverdueBooks = summary.OverdueBooks.ToString("N0");

                // 2. Tải Xu hướng mượn sách
                var toDate = DateTime.Now;
                var fromDate = toDate.AddDays(-(days - 1));
                var trendData = await _reportService.GetBorrowingTrendAsync(fromDate, toDate);

                // Chuẩn bị dữ liệu cho LiveCharts
                var borrowValues = new ChartValues<int>();
                var overdueValues = new ChartValues<int>();
                var labels = new List<string>();

                // Nếu là 1 năm, chúng ta nên nhóm theo tháng thay vì theo ngày
                if (days > 60)
                {
                    LabelStep = 1;
                    // Nhóm theo tháng
                    var monthlyGroups = trendData
                        .GroupBy(t => new { t.Date.Year, t.Date.Month })
                        .Select(g => new 
                        { 
                            Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                            BorrowCount = g.Sum(x => x.BorrowCount),
                            OverdueCount = g.Sum(x => x.OverdueCount)
                        })
                        .OrderBy(x => x.Date);

                    foreach (var m in monthlyGroups)
                    {
                        borrowValues.Add(m.BorrowCount);
                        overdueValues.Add(m.OverdueCount);
                        labels.Add(m.Date.ToString("MM/yy"));
                    }
                }
                else
                {
                    LabelStep = days == 30 ? 5 : 1;
                    // Nhóm theo ngày (đảm bảo đủ các ngày)
                    for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
                    {
                        var dayStat = trendData.FirstOrDefault(t => t.Date.Date == date);
                        borrowValues.Add(dayStat?.BorrowCount ?? 0);
                        overdueValues.Add(dayStat?.OverdueCount ?? 0);
                        labels.Add(date.ToString("dd/MM"));
                    }
                }

                BorrowingTrendSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Sách được mượn",
                        Values = borrowValues,
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = days > 30 ? 4 : 8
                    },
                    new LineSeries
                    {
                        Title = "Sách trễ hạn",
                        Values = overdueValues,
                        PointGeometry = DefaultGeometries.Square,
                        PointGeometrySize = days > 30 ? 4 : 8,
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
                System.Diagnostics.Debug.WriteLine($"Error loading report data: {ex.Message}");
            }
        }
    }
}