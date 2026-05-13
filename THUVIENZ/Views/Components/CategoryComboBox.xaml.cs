using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using THUVIENZ.DAL;
using THUVIENZ.Models;

namespace THUVIENZ.Views.Components
{
    public partial class CategoryComboBox : UserControl
    {
        public static readonly DependencyProperty SelectedCategoryIdProperty =
            DependencyProperty.Register("SelectedCategoryId", typeof(int), typeof(CategoryComboBox), 
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedCategoryIdChanged));

        public int SelectedCategoryId
        {
            get => (int)GetValue(SelectedCategoryIdProperty);
            set => SetValue(SelectedCategoryIdProperty, value);
        }

        private static void OnSelectedCategoryIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CategoryComboBox control && e.NewValue is int newId)
            {
                control.SyncSelection(newId);
            }
        }

        public CategoryComboBox()
        {
            InitializeComponent();
            Loaded += CategoryComboBox_Loaded;
        }

        private void CategoryComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
        }

        public void LoadCategories()
        {
            try
            {
                using var context = new LmsDbContext();
                var list = context.TheLoaiSachs.OrderBy(t => t.TenTheLoai).ToList();
                cboCategories.ItemsSource = list;

                SyncSelection(SelectedCategoryId);
            }
            catch (Exception)
            {
                // Bỏ qua lỗi kết nối DB khi Designer preview
            }
        }

        private void SyncSelection(int categoryId)
        {
            if (cboCategories.ItemsSource is List<TheLoaiSach> list)
            {
                var target = list.FirstOrDefault(t => t.MaTheLoai == categoryId);
                if (target != null)
                {
                    cboCategories.SelectedItem = target;
                }
                else if (list.Any())
                {
                    cboCategories.SelectedItem = list.First();
                    SelectedCategoryId = list.First().MaTheLoai;
                }
            }
        }

        private void CboCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboCategories.SelectedItem is TheLoaiSach selected)
            {
                SelectedCategoryId = selected.MaTheLoai;
            }
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CategoryInputDialog
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (dialog.ShowDialog() == true)
            {
                string newName = dialog.CategoryName.Trim();
                if (!string.IsNullOrEmpty(newName))
                {
                    await CreateCategoryAsync(newName);
                }
            }
        }

        private async Task CreateCategoryAsync(string name)
        {
            try
            {
                using var context = new LmsDbContext();
                // Chuẩn hóa: Viết hoa chữ cái đầu
                string normalized = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
                
                // Kiểm tra trùng lặp
                var existing = await context.TheLoaiSachs
                    .FirstOrDefaultAsync(t => t.TenTheLoai.ToLower() == name.ToLower());

                if (existing != null)
                {
                    SelectedCategoryId = existing.MaTheLoai;
                    SyncSelection(existing.MaTheLoai);
                    MessageBox.Show($"Thể loại '{existing.TenTheLoai}' đã tồn tại và tự động được chọn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var newCat = new TheLoaiSach { TenTheLoai = normalized };
                await context.TheLoaiSachs.AddAsync(newCat);
                await context.SaveChangesAsync();

                LoadCategories();
                SelectedCategoryId = newCat.MaTheLoai;
                SyncSelection(newCat.MaTheLoai);
                
                MessageBox.Show($"Thêm thể loại '{normalized}' thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tạo thể loại: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (cboCategories.SelectedItem is not TheLoaiSach selected)
            {
                MessageBox.Show("Vui lòng chọn một thể loại để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var context = new LmsDbContext();
                // Kiểm tra ràng buộc khóa ngoại (Referential Integrity)
                int booksCount = await context.Sachs.CountAsync(s => s.MaTheLoai == selected.MaTheLoai);
                if (booksCount > 0)
                {
                    MessageBox.Show($"Thể loại '{selected.TenTheLoai}' đang được liên kết với {booksCount} đầu sách.\n\nVui lòng chuyển các đầu sách này sang thể loại khác trước khi xóa để bảo đảm toàn vẹn dữ liệu.", 
                        "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa vĩnh viễn thể loại '{selected.TenTheLoai}' không?", 
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    var target = await context.TheLoaiSachs.FindAsync(selected.MaTheLoai);
                    if (target != null)
                    {
                        context.TheLoaiSachs.Remove(target);
                        await context.SaveChangesAsync();

                        MessageBox.Show("Xóa thể loại thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadCategories();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xóa thể loại: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class CategoryInputDialog : Window
    {
        private TextBox _txtInput;
        public string CategoryName => _txtInput.Text;

        public CategoryInputDialog()
        {
            Title = "Thêm Thể Loại Mới";
            Width = 400;
            Height = 200;
            WindowStyle = WindowStyle.ToolWindow;
            ResizeMode = ResizeMode.NoResize;
            Background = System.Windows.Media.Brushes.White;

            var grid = new Grid { Margin = new Thickness(20) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var lbl = new TextBlock 
            { 
                Text = "Nhập tên thể loại sách mới:", 
                FontSize = 14, 
                FontWeight = FontWeights.SemiBold,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#374151")),
                Margin = new Thickness(0, 0, 0, 10) 
            };
            Grid.SetRow(lbl, 0);
            grid.Children.Add(lbl);

            var border = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F9FAFB")),
                CornerRadius = new CornerRadius(8),
                BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E5E7EB")),
                BorderThickness = new Thickness(1),
                Height = 38
            };
            Grid.SetRow(border, 1);

            _txtInput = new TextBox
            {
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(10, 0, 10, 0),
                FontSize = 14
            };
            border.Child = _txtInput;
            grid.Children.Add(border);

            var panel = new StackPanel 
            { 
                Orientation = Orientation.Horizontal, 
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 15, 0, 0)
            };
            Grid.SetRow(panel, 3);

            var btnCancel = new Button
            {
                Content = "Hủy",
                Width = 80,
                Height = 35,
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F3F4F6")),
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4B5563")),
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0, 0, 10, 0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            btnCancel.Resources.Add(typeof(Border), new Style(typeof(Border)) { Setters = { new Setter(Border.CornerRadiusProperty, new CornerRadius(6)) } });
            btnCancel.Click += (s, e) => DialogResult = false;

            var btnOk = new Button
            {
                Content = "Xác nhận",
                Width = 100,
                Height = 35,
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#10B981")),
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand,
                IsDefault = true
            };
            btnOk.Resources.Add(typeof(Border), new Style(typeof(Border)) { Setters = { new Setter(Border.CornerRadiusProperty, new CornerRadius(6)) } });
            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(_txtInput.Text))
                {
                    MessageBox.Show("Tên thể loại không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                DialogResult = true;
            };

            panel.Children.Add(btnCancel);
            panel.Children.Add(btnOk);
            grid.Children.Add(panel);

            Content = grid;
            Loaded += (s, e) => _txtInput.Focus();
        }
    }
}
