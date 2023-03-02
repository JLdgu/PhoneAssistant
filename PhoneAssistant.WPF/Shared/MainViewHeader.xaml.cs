using System.Windows;
using System.Windows.Controls;

namespace PhoneAssistant.WPF.Shared;
/// <summary>
/// Interaction logic for MainViewHeader.xaml
/// </summary>
public partial class MainViewHeader : UserControl
{
    public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
    "HeaderText", typeof(string), typeof(MainViewHeader), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty HeaderIconProperty = DependencyProperty.Register(
    "HeaderIcon", typeof(string), typeof(MainViewHeader), new PropertyMetadata(default(string)));

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

    public MainViewHeader()
    {
        InitializeComponent();
        this.DataContext = this;
    }
}
