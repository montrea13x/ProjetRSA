using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Net.Http;
using System.Net.Http.Json;

using ProjetRSA.CertificateOperations;
using System.Drawing.Text;

namespace RSA.Guest;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly HttpClient _httpClient = new HttpClient();
    private static readonly Regex EmailRegex = new(
        @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    public MainWindow()
    {
        InitializeComponent();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void CreateProfileEmail_Click(object sender, RoutedEventArgs e)
    {
        CreateProfilePanel.Visibility = Visibility.Visible;
        EmailStepPanel.Visibility = Visibility.Visible;
        CodeStepPanel.Visibility = Visibility.Collapsed;
        EmailErrorText.Visibility = Visibility.Collapsed;
        CodeErrorText.Visibility = Visibility.Collapsed;
        VerificationCodeTextBox.Clear();
        EmailTextBox.Focus();
    }

    private async void GenerateCodeProfile_Click(object sender, RoutedEventArgs e)
    {
        string email = EmailTextBox.Text.Trim();
        if (!EmailRegex.IsMatch(email))
        {
            EmailErrorText.Text = "Please enter a valid email address.";
            EmailErrorText.Visibility = Visibility.Visible;
            return;
        }

        EmailErrorText.Visibility = Visibility.Collapsed;
        EmailStepPanel.Visibility = Visibility.Collapsed;
        CodeStepPanel.Visibility = Visibility.Visible;
        CodeErrorText.Visibility = Visibility.Collapsed;
        VerificationCodeTextBox.Clear();
        VerificationCodeTextBox.Focus();
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "https://pc-2.ca/request_verification.php",
                new { Email = email }
            );

            string content = await response.Content.ReadAsStringAsync();

            MessageBox.Show("Status Code: " + response.StatusCode + "\nContent: " + content);
        }
        catch (Exception ex)
        {
            EmailErrorText.Text = "SMTP error: " + GetInnermostMessage(ex);
            EmailErrorText.Visibility = Visibility.Visible;
        }
    }


    private static string GetInnermostMessage(Exception exception)
    {
        Exception current = exception;
        while (current.InnerException is not null)
        {
            current = current.InnerException;
        }

        return current.Message;
    }
}