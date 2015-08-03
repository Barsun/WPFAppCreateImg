using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFAppCreateImg.Control
{
    public partial class Colorpicker : UserControl
    {

        public Colorpicker()
        {
            InitializeComponent();
        }

        public Brush SelectedColor
        {
            get { return (Brush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Brush), typeof(Colorpicker), new UIPropertyMetadata(null));


    }
}
