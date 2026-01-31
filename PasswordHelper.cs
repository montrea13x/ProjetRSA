using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjetRSA;

class PasswordHelper
{
    public static string ReadPassword()
    {
        StringBuilder password = new StringBuilder();
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Length -= 1;
                }
            }
            else
            {
                password.Append(key.KeyChar);
            }
        }
        return password.ToString();
    }

}