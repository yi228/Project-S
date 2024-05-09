using System;
using System.IO;
using System.Text;

public class ConsoleLogManager
{
    private static StringBuilder _stringBuilder = new StringBuilder();
    public static ConsoleLogManager Instance { get; } = new ConsoleLogManager();
    ConsoleLogManager()
    {
        Console.WriteLine("Start SaveLogs");
        AppDomain.CurrentDomain.ProcessExit -= SaveLogs;
        AppDomain.CurrentDomain.ProcessExit += SaveLogs;
    }
    public void Log(string message)
    {
        string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        message = $"[{currentDateTime}]: {message}";
        Console.WriteLine(message);
        _stringBuilder.AppendLine(message);
    }
    public void Log(Exception e)
    {
        string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string message = e.ToString();
        message = $"[{currentDateTime}]: {message}";
        Console.WriteLine(message);
        _stringBuilder.AppendLine(message);
    }
    private static void SaveLogs(object sender, EventArgs e)
    {
        string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"ConsoleLog{currentDateTime}.txt";
        string directory = "../../../../Logs";

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string fullPath = Path.Combine(directory, fileName);

        File.WriteAllText(fullPath, _stringBuilder.ToString());
    }
}
