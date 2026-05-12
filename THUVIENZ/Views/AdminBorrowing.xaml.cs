using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using THUVIENZ.ViewModels;

namespace THUVIENZ.Views
{
    public partial class AdminBorrowing : UserControl
    {
        private readonly AdminCirculationViewModel _viewModel;
        private Border? _btnCurrentTab;
        private Border? _btnHistoryTab;

        public AdminBorrowing()
        {
            InitializeComponent();
            _viewModel = new AdminCirculationViewModel();
            this.DataContext = _viewModel;
            this.Loaded += AdminBorrowing_Loaded;
        }

        private void AdminBorrowing_Loaded(object sender, RoutedEventArgs e)
        {
            FindTabBorders(this);
        }

        private void FindTabBorders(DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is Border b && b.Child is TextBlock tb)
                {
                    if (tb.Text.Contains("Đang mượn"))
                    {
                        _btnCurrentTab = b;
                        b.MouseLeftButtonDown -= CurrentTab_Click;
                        b.MouseLeftButtonDown += CurrentTab_Click;
                    }
                    else if (tb.Text.Contains("Lịch sử"))
                    {
                        _btnHistoryTab = b;
                        b.MouseLeftButtonDown -= HistoryTab_Click;
                        b.MouseLeftButtonDown += HistoryTab_Click;
                    }
                }
                else
                {
                    FindTabBorders(child);
                }
            }
        }

        private void CurrentTab_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _viewModel.IsHistoryTab = false;
            UpdateTabStyles();
        }

        private void HistoryTab_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _viewModel.IsHistoryTab = true;
            UpdateTabStyles();
        }

        private void UpdateTabStyles()
        {
            if (_btnCurrentTab?.Child is TextBlock tbCurrent)
            {
                _btnCurrentTab.Background = !_viewModel.IsHistoryTab ? new SolidColorBrush(Color.FromRgb(239, 246, 255)) : new SolidColorBrush(Colors.Transparent);
                _btnCurrentTab.BorderBrush = !_viewModel.IsHistoryTab ? new SolidColorBrush(Color.FromRgb(59, 130, 246)) : new SolidColorBrush(Colors.Transparent);
                tbCurrent.Foreground = !_viewModel.IsHistoryTab ? new SolidColorBrush(Color.FromRgb(29, 78, 216)) : new SolidColorBrush(Color.FromRgb(107, 114, 128));
                tbCurrent.FontWeight = !_viewModel.IsHistoryTab ? FontWeights.Bold : FontWeights.SemiBold;
            }

            if (_btnHistoryTab?.Child is TextBlock tbHistory)
            {
                _btnHistoryTab.Background = _viewModel.IsHistoryTab ? new SolidColorBrush(Color.FromRgb(239, 246, 255)) : new SolidColorBrush(Colors.Transparent);
                _btnHistoryTab.BorderBrush = _viewModel.IsHistoryTab ? new SolidColorBrush(Color.FromRgb(59, 130, 246)) : new SolidColorBrush(Colors.Transparent);
                tbHistory.Foreground = _viewModel.IsHistoryTab ? new SolidColorBrush(Color.FromRgb(29, 78, 216)) : new SolidColorBrush(Color.FromRgb(107, 114, 128));
                tbHistory.FontWeight = _viewModel.IsHistoryTab ? FontWeights.Bold : FontWeights.SemiBold;
            }
        }
    }
}