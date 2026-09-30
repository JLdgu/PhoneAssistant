using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhoneAssistant.WPF.Behaviors
{
    // Lightweight attached behavior that exposes an ICommand for TextChanged.
    // Does not require external packages; can be used with XAML as an attached property.
    public static class TextChangedToCommandBehavior
    {
        public static readonly DependencyProperty TextChangedCommandProperty =
            DependencyProperty.RegisterAttached(
                "TextChangedCommand",
                typeof(ICommand),
                typeof(TextChangedToCommandBehavior),
                new PropertyMetadata(null, OnTextChangedCommandChanged));

        public static void SetTextChangedCommand(DependencyObject obj, ICommand? value)
            => obj.SetValue(TextChangedCommandProperty, value);

        public static ICommand? GetTextChangedCommand(DependencyObject obj)
            => (ICommand?)obj.GetValue(TextChangedCommandProperty);

        private static void OnTextChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uiElement)
            {
                if (e.OldValue is ICommand)
                    uiElement.RemoveHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(InnerTextChanged));

                if (e.NewValue is ICommand)
                    uiElement.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(InnerTextChanged));
            }
        }

        private static void InnerTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is DependencyObject d)
            {
                var command = GetTextChangedCommand(d);
                if (command is null)
                    return;

                var parameter = e;
                if (command.CanExecute(parameter))
                    command.Execute(parameter);
            }
        }
    }
}
