using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using THUVIENZ.Models;

namespace THUVIENZ.Views.Popups
{
    public partial class ReaderActionPopup : UserControl
    {
        private bool _isSyncing;

        public static readonly DependencyProperty PopupTitleProperty =
            DependencyProperty.Register("PopupTitle", typeof(string), typeof(ReaderActionPopup), new PropertyMetadata("Xử lý Độc giả"));
        public string PopupTitle { get => (string)GetValue(PopupTitleProperty); set => SetValue(PopupTitleProperty, value); }

        public static readonly DependencyProperty ActionModeProperty =
            DependencyProperty.Register("ActionMode", typeof(string), typeof(ReaderActionPopup), new PropertyMetadata("Suspend", OnActionModeChanged));
        public string ActionMode { get => (string)GetValue(ActionModeProperty); set => SetValue(ActionModeProperty, value); }

        public static readonly DependencyProperty TargetReaderProperty =
            DependencyProperty.Register("TargetReader", typeof(DocGia), typeof(ReaderActionPopup), new PropertyMetadata(null));
        public DocGia? TargetReader { get => (DocGia?)GetValue(TargetReaderProperty); set => SetValue(TargetReaderProperty, value); }

        public static readonly DependencyProperty ReasonTextProperty =
            DependencyProperty.Register("ReasonText", typeof(string), typeof(ReaderActionPopup), new PropertyMetadata(string.Empty));
        public string ReasonText { get => (string)GetValue(ReasonTextProperty); set => SetValue(ReasonTextProperty, value); }

        public static readonly DependencyProperty ExpiryPreviewTextProperty =
            DependencyProperty.Register("ExpiryPreviewText", typeof(string), typeof(ReaderActionPopup), new PropertyMetadata("Tài khoản sẽ tự động mở khóa vào: --/--/----"));
        public string ExpiryPreviewText { get => (string)GetValue(ExpiryPreviewTextProperty); set => SetValue(ExpiryPreviewTextProperty, value); }

        // Nguồn chân lý duy nhất (Single Source of Truth)
        private DateTime _selectedExpiryTime;
        public DateTime SelectedExpiryTime
        {
            get => _selectedExpiryTime;
            set
            {
                _selectedExpiryTime = value;
                ExpiryPreviewText = $"Tài khoản sẽ tự động mở khóa vào: {_selectedExpiryTime:HH:mm - dd/MM/yyyy}";
            }
        }

        // Các thuộc tính hỗ trợ giao diện động
        public static readonly DependencyProperty HeaderIconProperty =
            DependencyProperty.Register("HeaderIcon", typeof(string), typeof(ReaderActionPopup), new PropertyMetadata("⏳"));
        public string HeaderIcon { get => (string)GetValue(HeaderIconProperty); set => SetValue(HeaderIconProperty, value); }

        public static readonly DependencyProperty HeaderBgBrushProperty =
            DependencyProperty.Register("HeaderBgBrush", typeof(Brush), typeof(ReaderActionPopup), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF3C7"))));
        public Brush HeaderBgBrush { get => (Brush)GetValue(HeaderBgBrushProperty); set => SetValue(HeaderBgBrushProperty, value); }

        public static readonly DependencyProperty HeaderTextBrushProperty =
            DependencyProperty.Register("HeaderTextBrush", typeof(Brush), typeof(ReaderActionPopup), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B45309"))));
        public Brush HeaderTextBrush { get => (Brush)GetValue(HeaderTextBrushProperty); set => SetValue(HeaderTextBrushProperty, value); }

        public static readonly DependencyProperty ActionButtonTextProperty =
            DependencyProperty.Register("ActionButtonText", typeof(string), typeof(ReaderActionPopup), new PropertyMetadata("Xác nhận Đình chỉ"));
        public string ActionButtonText { get => (string)GetValue(ActionButtonTextProperty); set => SetValue(ActionButtonTextProperty, value); }

        public static readonly DependencyProperty ActionButtonBgProperty =
            DependencyProperty.Register("ActionButtonBg", typeof(Brush), typeof(ReaderActionPopup), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"))));
        public Brush ActionButtonBg { get => (Brush)GetValue(ActionButtonBgProperty); set => SetValue(ActionButtonBgProperty, value); }

        public static readonly DependencyProperty DurationVisibilityProperty =
            DependencyProperty.Register("DurationVisibility", typeof(Visibility), typeof(ReaderActionPopup), new PropertyMetadata(Visibility.Visible));
        public Visibility DurationVisibility { get => (Visibility)GetValue(DurationVisibilityProperty); set => SetValue(DurationVisibilityProperty, value); }

        public event Action<string, string>? OnActionSubmitted; // tham số: Reason, ExpirySummary

        public ReaderActionPopup()
        {
            InitializeComponent();
            IsVisibleChanged += ReaderActionPopup_IsVisibleChanged;
        }

        private void ReaderActionPopup_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                // Mặc định trỏ về 30 ngày tới
                _isSyncing = true;
                SelectedExpiryTime = DateTime.Now.AddDays(30);
                txtDuration.Text = "30";
                dpExpiryDate.SelectedDate = SelectedExpiryTime.Date;
                _isSyncing = false;
            }
        }

        private static void OnActionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ReaderActionPopup popup && e.NewValue is string mode)
            {
                popup.UpdateDynamicStyling(mode);
            }
        }

        public void UpdateDynamicStyling(string mode)
        {
            if (mode == "Suspend")
            {
                PopupTitle = "Đình chỉ tài khoản Độc giả";
                HeaderIcon = "⏳";
                HeaderBgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF3C7"));
                HeaderTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B45309"));
                ActionButtonText = "Xác nhận Đình chỉ";
                ActionButtonBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"));
                DurationVisibility = Visibility.Visible;
            }
            else // Delete / Ban
            {
                PopupTitle = "Vô hiệu hóa / Khóa Độc giả";
                HeaderIcon = "🚫";
                HeaderBgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEE2E2"));
                HeaderTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#991B1B"));
                ActionButtonText = "Vô hiệu hóa tài khoản";
                ActionButtonBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626"));
                DurationVisibility = Visibility.Collapsed;
            }
        }

        private void PresetChip_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tagStr)
            {
                _isSyncing = true;
                DateTime target = DateTime.Now;
                if (tagStr.EndsWith("h"))
                {
                    if (int.TryParse(tagStr.TrimEnd('h'), out int hours))
                        target = target.AddHours(hours);
                }
                else if (tagStr.EndsWith("d"))
                {
                    if (int.TryParse(tagStr.TrimEnd('d'), out int days))
                        target = target.AddDays(days);
                }
                
                SelectedExpiryTime = target;
                
                // Đồng bộ cập nhật UI
                dpExpiryDate.SelectedDate = target.Date;
                var span = target - DateTime.Now;
                txtDuration.Text = Math.Max(1, (int)Math.Round(span.TotalDays)).ToString();
                
                _isSyncing = false;
            }
        }

        private void TxtDuration_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSyncing) return;
            if (int.TryParse(txtDuration.Text, out int days) && days > 0)
            {
                _isSyncing = true;
                SelectedExpiryTime = DateTime.Now.AddDays(days);
                dpExpiryDate.SelectedDate = SelectedExpiryTime.Date;
                _isSyncing = false;
            }
        }

        private void DpExpiryDate_SelectedDateChanged(DateTime? newDate)
        {
            if (_isSyncing) return;
            if (newDate.HasValue)
            {
                _isSyncing = true;
                // Lấy mốc cuối ngày của ngày được chọn
                DateTime chosenDate = newDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                if (chosenDate < DateTime.Now) chosenDate = DateTime.Now.AddDays(1); // Tránh ngày quá khứ
                
                SelectedExpiryTime = chosenDate;
                
                // Đồng bộ điền lại số ngày làm tròn vào ô Textbox
                var span = chosenDate - DateTime.Now;
                txtDuration.Text = Math.Max(1, (int)Math.Round(span.TotalDays)).ToString();
                
                _isSyncing = false;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReasonText))
            {
                MessageBox.Show("Vui lòng nhập lý do xử lý chi tiết để lưu vết hệ thống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string durationSummary = ActionMode == "Suspend" 
                ? $"Đến {SelectedExpiryTime:HH:mm - dd/MM/yyyy} ({txtDuration.Text} ngày)" 
                : "Vô hiệu hóa vô thời hạn";

            OnActionSubmitted?.Invoke(ReasonText, durationSummary);
            Visibility = Visibility.Collapsed;
        }
    }
}
