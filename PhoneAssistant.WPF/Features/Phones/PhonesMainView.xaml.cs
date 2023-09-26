﻿using System.Windows.Controls;

using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Phones;
/// <summary>
/// Interaction logic for PhonesMainView.xaml
/// </summary>
public partial class PhonesMainView : UserControl
{
    public PhonesMainView()
    {
        InitializeComponent();
    }

    private void PhonesGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => UI_Interactions.SelectRowFromWhiteSpaceClick(e);    
}
