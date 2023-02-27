using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PhoneAssistant.WPF.Features.ServiceRequest
{
    /// <summary>
    /// Interaction logic for ServiceRequestMainView.xaml
    /// </summary>
    public partial class ServiceRequestMainView : UserControl
    {
        //public List<Employee> Employees { get; set; }
        //public List<string> Genders { get; set; }

        public ServiceRequestMainView()
        {
            InitializeComponent();

            //Employees = new List<Employee>()
            //{
            //    new Employee() { Name = "ABC", Gender = "Female" },
            //    new Employee() { Name = "DEF", Gender = "Female" },
            //    new Employee() { Name = "HIJ", Gender = "Male" },
            //    new Employee() { Name = "XYZ" }
            //};

            //Genders = new List<string>
            //{
            //    "Male",
            //    "Female"
            //};

            //InitializeComponent();

            //myGrid.ItemsSource = Employees;
            //Gender.ItemsSource = Genders;
            //Gender1.ItemsSource = Genders;
        }
        //private void ShowPersonDetails_Click(object sender, RoutedEventArgs e)
        //{
        //    foreach (Employee employee in Employees)
        //    {
        //        string text = string.Empty;
        //        text = "Name : " + employee.Name + Environment.NewLine;
        //        text += "Gender : " + employee.Gender + Environment.NewLine;
        //        MessageBox.Show(text);
        //    }
        //}
    }
}

public class Employee
{
    public string Name { get; set; }
    public string Gender { get; set; }
}
