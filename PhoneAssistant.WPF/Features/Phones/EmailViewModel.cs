using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

using CommunityToolkit.Mvvm.ComponentModel;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public partial class EmailViewModel : ObservableObject
{
    public EmailViewModel()
    {
        var fd = GenerateFlowDocument();

        
    }

    public EmailViewModel(v1Phone phone) 
    {
    }

    [ObservableProperty]
    private string _imei;

    [ObservableProperty]
    private string _phoneNumber;

    public string EmailDocument { get; set; }

    public FlowDocument GenerateFlowDocument()
    {
        return new FlowDocument(
            new Paragraph(new Bold(new Run( "Rich Text Box.")))
            //<Paragraph>
            //    <Run FontWeight="Bold">Rich Text Box.</Run>
            //    <LineBreak />
            //    <Run FontStyle="Italic">With formatting support</Run>
            //    <LineBreak />
            //    <Hyperlink Cursor="Hand" NavigateUri="https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit">
            //        Material Design in XAML
            //    </Hyperlink>
            //</Paragraph>

        );
    }
}
