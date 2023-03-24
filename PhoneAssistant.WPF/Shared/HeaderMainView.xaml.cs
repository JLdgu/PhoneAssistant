using System.Windows;
using System.Windows.Controls;

namespace PhoneAssistant.WPF.Shared
{
    /// <summary>
    /// Interaction logic for HeaderMainView.xaml
    /// </summary>
    public partial class HeaderMainView : UserControl
    {
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
            nameof(HeaderText), typeof(string), typeof(HeaderMainView), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty HeaderIconProperty = DependencyProperty.Register(
            nameof(HeaderIcon), typeof(string), typeof(HeaderMainView), new PropertyMetadata(default(string)));

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public string HeaderIcon
        {
            get { return (string)GetValue(HeaderIconProperty); }
            set { SetValue(HeaderIconProperty, value); }
        }

        public HeaderMainView()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
