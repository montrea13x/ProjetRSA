using System.Text.RegularExpressions;
using System.Windows;

namespace RSA.Guest;

public partial class EmailPromptWindow : Window
{
    private static readonly Regex EmailRegex = new(
        @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    public string Email { get; private set; } = string.Empty;

    public EmailPromptWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => EmailTextBox.Focus();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        var value = EmailTextBox.Text.Trim();

        if (!EmailRegex.IsMatch(value))
        {
            ErrorText.Text = "Please enter a valid email address.";
            ErrorText.Visibility = Visibility.Visible;
            return;
        }

        Email = value;
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
