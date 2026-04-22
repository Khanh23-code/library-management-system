using System.Windows;
using System.Windows.Controls;

namespace THUVIENZ.Views.Components
{
    public partial class BookCard : UserControl
    {
        // 1. Tên sách
        public static readonly DependencyProperty BookTitleProperty =
            DependencyProperty.Register("BookTitle", typeof(string), typeof(BookCard), new PropertyMetadata("Tên cuốn sách"));

        public string BookTitle
        {
            get { return (string)GetValue(BookTitleProperty); }
            set { SetValue(BookTitleProperty, value); }
        }

        // 2. Tác giả
        public static readonly DependencyProperty AuthorProperty =
            DependencyProperty.Register("Author", typeof(string), typeof(BookCard), new PropertyMetadata("Chưa rõ tác giả"));

        public string Author
        {
            get { return (string)GetValue(AuthorProperty); }
            set { SetValue(AuthorProperty, value); }
        }

        // 3. Mã sách (Book ID)
        public static readonly DependencyProperty BookIdProperty =
            DependencyProperty.Register("BookId", typeof(string), typeof(BookCard), new PropertyMetadata("ID-0000"));

        public string BookId
        {
            get { return (string)GetValue(BookIdProperty); }
            set { SetValue(BookIdProperty, value); }
        }

        // 4. Giá tiền
        public static readonly DependencyProperty PriceProperty =
            DependencyProperty.Register("Price", typeof(string), typeof(BookCard), new PropertyMetadata("0"));

        public string Price
        {
            get { return (string)GetValue(PriceProperty); }
            set { SetValue(PriceProperty, value); }
        }

        // 5. Ảnh bìa
        public static readonly DependencyProperty CoverImageProperty =
            DependencyProperty.Register("CoverImage", typeof(string), typeof(BookCard), new PropertyMetadata(""));

        public string CoverImage
        {
            get { return (string)GetValue(CoverImageProperty); }
            set { SetValue(CoverImageProperty, value); }
        }

        public BookCard()
        {
            InitializeComponent();
        }
    }
}