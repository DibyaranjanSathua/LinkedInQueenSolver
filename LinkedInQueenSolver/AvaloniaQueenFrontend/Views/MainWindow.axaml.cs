using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaQueenFrontend.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void ButtonClicked(object source, RoutedEventArgs args)
    {
        if (double.TryParse(Celsius.Text, out double celsius))
        {
            double fahrenheit = celsius * (9d / 5d) + 32;
            Fahrenheit.Text = fahrenheit.ToString("0.0");
        }
        else
        {
            Celsius.Text = "0";
            Fahrenheit.Text = "32";
        }
    }

    private void Celsius_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (double.TryParse(Celsius.Text, out double celsius))
        {
            double fahrenheit = celsius * (9d / 5d) + 32;
            Fahrenheit.Text = fahrenheit.ToString("0.0");
        }
        else
        {
            Celsius.Text = "0";
            Fahrenheit.Text = "32";
        }
    }
}
