using System.Windows.Documents;
using System.Windows.Media.Media3D;
using System.Xml;

using PhoneAssistant.WPF.Application.Entities;

using static System.Net.Mime.MediaTypeNames;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed class EmailViewModel
{
    public EmailViewModel() { }

    public EmailViewModel(v1Phone phone) 
    {
    }

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
