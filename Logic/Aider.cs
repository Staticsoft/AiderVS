using System;
using System.Diagnostics;

namespace Logic;

public class Aider : IDisposable
{
    readonly Logger Output;
    readonly Process process;

    public Aider(AiderOptions options, Logger output)
    {
        Output = output;

        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = options.WorkingDirectory,
                FileName = @"python",
                Arguments = @"-m aider.main --no-git --no-pretty --programmatic-access",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += LogMessage;
        process.ErrorDataReceived += LogMessage;

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }

    public void Dispose()
    {
        if (!process.HasExited)
        {
            WriteInput("/exit");
            process.WaitForExit();
        }

        process.OutputDataReceived -= LogMessage;
        process.ErrorDataReceived -= LogMessage;

        process.Dispose();
    }

    void WriteInput(string input)
    {
        process.StandardInput.WriteLine(input);
        LogMessage($"> {input}");
    }

    void LogMessage(object _, DataReceivedEventArgs message)
        => LogMessage(message.Data);

    void LogMessage(string message)
        => Output.Log(message);
}
