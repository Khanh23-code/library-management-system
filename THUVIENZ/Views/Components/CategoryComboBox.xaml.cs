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
        private List<TheLoaiSach> _allCategories = new();
        private bool _isSyncingText = false;

        public static readonly DependencyProperty SelectedCategoryIdProperty =
            DependencyProperty.Register("SelectedCategoryId", typeof(int), typeof(CategoryComboBox), 
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedCategoryIdChanged));

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
                _allCategories = context.TheLoaiSachs.OrderBy(t => t.TenTheLoai).ToList();
                
                SyncSelection(SelectedCategoryId);
                FilterSuggestions();
            }
            catch (Exception)
            {
                // Bỏ qua lỗi khi hiển thị trên Designer preview
            }
        }

        public void SyncSelection(int categoryId)
        {
            var target = _allCategories.FirstOrDefault(t => t.MaTheLoai == categoryId);
            if (target != null)
            {
                _isSyncingText = true;
                txtSearch.Text = target.TenTheLoai;
                _isSyncingText = false;
            }
            else
            {
                _isSyncingText = true;
                txtSearch.Text = "";
                _isSyncingText = false;
            }
        }

        private void FilterSuggestions()
        {
            string query = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(query))
            {
                icCategories.ItemsSource = _allCategories;
            }
            else
            {
                icCategories.ItemsSource = _allCategories
                    .Where(t => t.TenTheLoai.ToLower().Contains(query))
                    .ToList();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSyncingText) return;

            FilterSuggestions();
            if (!PopupSuggestions.IsOpen)
            {
                PopupSuggestions.IsOpen = true;
            }
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            FilterSuggestions();
            PopupSuggestions.IsOpen = true;
        }

        private void TxtSearch_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FilterSuggestions();
            if (!PopupSuggestions.IsOpen)
            {
                PopupSuggestions.IsOpen = true;
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            // Popup tự đóng nhờ thuộc tính StaysOpen="False"
        }

        private void BtnSelectCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is TheLoaiSach target)
            {
                SelectedCategoryId = target.MaTheLoai;
                _isSyncingText = true;
                txtSearch.Text = target.TenTheLoai;
                _isSyncingText = false;
                
                PopupSuggestions.IsOpen = false;
            }
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string input = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Vui lòng gõ tên thể loại cần thêm vào ô tìm kiếm trước.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtSearch.Focus();
                return;
            }

            try
            {
                using var context = new LmsDbContext();
                string normalized = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
                
                var existing = await context.TheLoaiSachs
                    .FirstOrDefaultAsync(t => t.TenTheLoai.ToLower() == input.ToLower());

                if (existing != null)
                {
                    SelectedCategoryId = existing.MaTheLoai;
                    SyncSelection(existing.MaTheLoai);
                    PopupSuggestions.IsOpen = false;
                    MessageBox.Show($"Thể loại '{existing.TenTheLoai}' đã tồn tại và tự động được chọn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var newCat = new TheLoaiSach { TenTheLoai = normalized };
                await context.TheLoaiSachs.AddAsync(newCat);
                await context.SaveChangesAsync();

                // Nạp lại danh sách từ DB
                _allCategories = context.TheLoaiSachs.OrderBy(t => t.TenTheLoai).ToList();
                SelectedCategoryId = newCat.MaTheLoai;
                SyncSelection(newCat.MaTheLoai);
                
                PopupSuggestions.IsOpen = false;
                MessageBox.Show($"Thêm thể loại '{normalized}' thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm thể loại: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is TheLoaiSach target)
            {
                e.Handled = true;

                try
                {
                    using var context = new LmsDbContext();
                    int booksCount = await context.Sachs.CountAsync(s => s.MaTheLoai == target.MaTheLoai);
                    if (booksCount > 0)
                    {
                        MessageBox.Show($"Thể loại '{target.TenTheLoai}' đang được liên kết với {booksCount} đầu sách.\n\nVui lòng chuyển các đầu sách này sang thể loại khác trước khi xóa.", 
                            "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa thể loại '{target.TenTheLoai}' không?", 
                        "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (confirm == MessageBoxResult.Yes)
                    {
                        var itemDb = await context.TheLoaiSachs.FindAsync(target.MaTheLoai);
                        if (itemDb != null)
                        {
                            context.TheLoaiSachs.Remove(itemDb);
                            await context.SaveChangesAsync();

                            _allCategories = context.TheLoaiSachs.OrderBy(t => t.TenTheLoai).ToList();
                            FilterSuggestions();

                            if (SelectedCategoryId == target.MaTheLoai && _allCategories.Any())
                            {
                                SelectedCategoryId = _allCategories.First().MaTheLoai;
                                SyncSelection(SelectedCategoryId);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xóa thể loại: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
