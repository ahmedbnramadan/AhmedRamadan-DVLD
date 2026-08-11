using System;

public static class clsInputHelper
{
    public static void PrintHeader(string Title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n========================================");
        Console.WriteLine("    " + Title.ToUpper());
        Console.WriteLine("========================================");
        Console.ResetColor();
    }

    public static void NotifySuccess(string Message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n[+] Success: " + Message);
        Console.ResetColor();
    }

    public static void NotifyError(string Message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n[-] Error: " + Message);
        Console.ResetColor();
    }

    public static string ReadString(string Prompt)
    {
        Console.Write(Prompt + ": ");
        return Console.ReadLine();
    }

    public static int ReadInt(string Prompt)
    {
        int Value;
        Console.Write(Prompt + ": ");
        while (!int.TryParse(Console.ReadLine(), out Value))
        {
            NotifyError("Invalid Number, try again.");
            Console.Write(Prompt + ": ");
        }
        return Value;
    }
}