﻿using System.Windows;
using System.Windows.Controls;

namespace PhoneAssistant.WPF.Features.Phones;
/// <summary>
/// Interaction logic for emal.xaml
/// </summary>
public partial class EmailView : UserControl
{    
    public EmailView()
    {
        InitializeComponent();
    }

    private void DeliveryAddress_TextChanged(object sender, TextChangedEventArgs e)
    {
        string newValue = EmailViewModel.ReformatDeliveryAddress(DeliveryAddress.Text);
        if (newValue != DeliveryAddress.Text)
            DeliveryAddress.Text = newValue;
    }
}