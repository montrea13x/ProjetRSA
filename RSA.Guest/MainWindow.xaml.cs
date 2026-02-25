using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;

using ProjetRSA.CertificateOperations;


namespace RSA.Guest;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static readonly Regex EmailRegex = new(
        @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    private static string GenerateVerificationCode()
    {
        byte[] codeBytes = new byte[4];
        RandomNumberGenerator.Fill(codeBytes);

        int codeInt = BitConverter.ToInt32(codeBytes, 0) & 0x7FFFFFFF; // Ensure non-negative
        return (codeInt % 1000000).ToString("D6"); // Format as 6-digit code
    }

    private readonly IMemoryCache _cache;

    public MainWindow()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        InitializeComponent();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void CreateProfileEmail_Click(object sender, RoutedEventArgs e)
    {
        CreateProfilePanel.Visibility = Visibility.Visible;
        EmailErrorText.Visibility = Visibility.Collapsed;
        EmailTextBox.Focus();
    }

    private void GenerateProfileFromPanel_Click(object sender, RoutedEventArgs e)
    {
        string email = EmailTextBox.Text.Trim();
        if (!EmailRegex.IsMatch(email))
        {
            EmailErrorText.Text = "Please enter a valid email address.";
            EmailErrorText.Visibility = Visibility.Visible;
            return;
        }

        EmailErrorText.Visibility = Visibility.Collapsed;
    }
}