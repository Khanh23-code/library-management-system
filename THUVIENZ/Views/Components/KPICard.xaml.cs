using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FontAwesome.Sharp;

namespace THUVIENZ.Views.Components
{
    public partial class KPICard : UserControl
    {
        public static readonly DependencyProperty CardTitleProperty =
            DependencyProperty.Register("CardTitle", typeof(string), typeof(KPICard), new PropertyMetadata("Title"));

        public static readonly DependencyProperty CardValueProperty =
            DependencyProperty.Register("CardValue", typeof(string), typeof(KPICard), new PropertyMetadata("0"));

        public static readonly DependencyProperty CardIconProperty =
            DependencyProperty.Register("CardIcon", typeof(IconChar), typeof(KPICard), new PropertyMetadata(IconChar.ChartBar));

        public static readonly DependencyProperty IconBgColorProperty =
            DependencyProperty.Register("IconBgColor", typeof(Brush), typeof(KPICard), new PropertyMetadata(Brushes.LightGray));

        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.Register("IconColor", typeof(Brush), typeof(KPICard), new PropertyMetadata(Brushes.DarkGray));

        public string CardTitle { get => (string)GetValue(CardTitleProperty); set => SetValue(CardTitleProperty, value); }
        public string CardValue { get => (string)GetValue(CardValueProperty); set => SetValue(CardValueProperty, value); }
        public IconChar CardIcon { get => (IconChar)GetValue(CardIconProperty); set => SetValue(CardIconProperty, value); }
        public Brush IconBgColor { get => (Brush)GetValue(IconBgColorProperty); set => SetValue(IconBgColorProperty, value); }
        public Brush IconColor { get => (Brush)GetValue(IconColorProperty); set => SetValue(IconColorProperty, value); }

        public KPICard()
        {
            InitializeComponent();
        }
    }
}