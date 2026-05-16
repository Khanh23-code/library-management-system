using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using LiveCharts;
using LiveCharts.Wpf;
using THUVIENZ.BLL;
using THUVIENZ.Core;
using THUVIENZ.Models;

namespace THUVIENZ.ViewModels
{
    public class AdminReportViewModel : ObservableObject
    {
        private readonly ReportService _reportService;
        private int _currentDays = 7;

        // --- COMMANDS ---
        public RelayCommand ChangeFilterCommand { get; }
        public RelayCommand ExportCommand { get; }

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

        private string _lastUpdated = "Đang cập nhật...";
        public string LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
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

        // --- INTELLIGENCE DATA (APRIORI) ---
        private ObservableCollection<BookPairDTO> _frequentPairs = new();
        public ObservableCollection<BookPairDTO> FrequentPairs
        {
            get => _frequentPairs;
            set => SetProperty(ref _frequentPairs, value);
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

            ExportCommand = new RelayCommand(_ => _ = ExportReportAsync());

            _ = LoadDataAsync(7);
        }

        private async Task ExportReportAsync()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = $"BaoCaoThongKe_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                Title = "Lưu báo cáo thống kê"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var sb = new StringBuilder();
                    // Thêm BOM cho UTF-8 để Excel đọc được tiếng Việt
                    sb.Append('\uFEFF');

                    // 1. Header & KPI Summary
                    sb.AppendLine("BÁO CÁO THỐNG KÊ THƯ VIỆN");
                    sb.AppendLine($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    sb.AppendLine($"Khoảng thời gian: {_currentDays} ngày qua");
                    sb.AppendLine();
                    sb.AppendLine("TÓM TẮT CHỈ SỐ");
                    sb.AppendLine($"Tổng đầu sách: {TotalBooks}");
                    sb.AppendLine($"Tổng độc giả: {TotalReaders}");
                    sb.AppendLine($"Sách đang mượn: {BorrowedBooks}");
                    sb.AppendLine($"Sách trễ hạn: {OverdueBooks}");
                    sb.AppendLine();

                    // 2. Chi tiết xu hướng (Trend Data)
                    sb.AppendLine("CHI TIẾT XU HƯỚNG MƯỢN TRẢ");
                    sb.AppendLine("Thời gian,Số lượt mượn,Số lượt trễ");
                    
                    if (TrendLabels != null && BorrowingTrendSeries != null)
                    {
                        var borrowValues = BorrowingTrendSeries[0].Values;
                        var overdueValues = BorrowingTrendSeries[1].Values;
                        for (int i = 0; i < TrendLabels.Length; i++)
                        {
                            sb.AppendLine($"{TrendLabels[i]},{borrowValues[i]},{overdueValues[i]}");
                        }
                    }
                    sb.AppendLine();

                    // 3. Thống kê thể loại
                    sb.AppendLine("TỈ LỆ THỂ LOẠI (Lượt mượn)");
                    if (CategoryRatioSeries != null)
                    {
                        foreach (var series in CategoryRatioSeries)
                        {
                            sb.AppendLine($"{series.Title},{series.Values[0]}");
                        }
                    }

                    await File.WriteAllTextAsync(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);
                    
                    // Thông báo thành công (có thể dùng MessageBox)
                    System.Windows.MessageBox.Show("Xuất báo cáo thành công!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private async Task LoadDataAsync(int days)
        {
            _currentDays = days;
            TrendTitle = $"Lượt mượn & Trễ hạn ({days} ngày qua)";

            try
            {
                // 1. Tải KPI Summary
                var summary = await _reportService.GetDashboardSummaryAsync();
                TotalBooks = summary.TotalBooks.ToString("N0");
                TotalReaders = summary.TotalReaders.ToString("N0");
                BorrowedBooks = summary.BorrowedBooks.ToString("N0");
                OverdueBooks = summary.OverdueBooks.ToString("N0");

                // 2. Tải Xu hướng mượn sách
                var toDate = DateTime.Now;
                var fromDate = toDate.AddDays(-(days - 1));
                var trendData = await _reportService.GetBorrowingTrendAsync(fromDate, toDate);

                var borrowValues = new ChartValues<int>();
                var overdueValues = new ChartValues<int>();
                var labels = new List<string>();

                if (days > 60)
                {
                    LabelStep = 1;
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
                        Values = new ChartValues<int> { (int)stat.BorrowCount },
                        DataLabels = true
                    });
                }
                CategoryRatioSeries = pieSeries;

                // 4. Intelligence: Tải các cặp sách hay được mượn cùng nhau (Apriori)
                var pairs = await _reportService.GetFrequentBookPairsWithNamesAsync(minSupport: 2);
                FrequentPairs = new ObservableCollection<BookPairDTO>(pairs.Take(5));

                LastUpdated = $"Cập nhật lúc {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading report data: {ex.Message}");
                LastUpdated = "Lỗi cập nhật";
            }
        }
    }
}
