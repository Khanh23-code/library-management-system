using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace THUVIENZ.Views.Components
{
    public partial class CustomDatePicker : UserControl
    {
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(CustomDatePicker), new PropertyMetadata(null, OnSelectedDateChanged));

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public event Action<DateTime?>? OnDateSelected;

        public CustomDatePicker()
        {
            InitializeComponent();
            IsVisibleChanged += CustomDatePicker_IsVisibleChanged;
        }

        private void CustomDatePicker_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                // Bảo vệ chọn ngày từ mốc hiện tại tới tương lai
                calendarControl.DisplayDateStart = DateTime.Now.Date;
            }
        }

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomDatePicker control)
            {
                control.UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (SelectedDate.HasValue)
            {
                txtDateDisplay.Text = SelectedDate.Value.ToString("dd/MM/yyyy");
                calendarControl.SelectedDate = SelectedDate.Value;
                calendarControl.DisplayDate = SelectedDate.Value;
            }
            else
            {
                txtDateDisplay.Text = "--/--/----";
                calendarControl.SelectedDate = null;
            }
        }

        private void BorderWrapper_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Cập nhật lại mốc DisplayDateStart an toàn trước khi bung mở
            calendarControl.DisplayDateStart = DateTime.Now.Date;
            popupCalendar.IsOpen = true;
        }

        private void CalendarControl_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calendarControl.SelectedDate.HasValue)
            {
                SelectedDate = calendarControl.SelectedDate.Value;
                popupCalendar.IsOpen = false;
                OnDateSelected?.Invoke(SelectedDate);
            }
        }
    }
}
