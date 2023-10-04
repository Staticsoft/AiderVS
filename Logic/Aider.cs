using System;
using System.Diagnostics;

namespace Logic;

public class Aider : IDisposable
{
    readonly AiderOptions Options;
    readonly Logger Output;
    readonly Process process;

    public Aider(AiderOptions options, Logger output)
    {
        Options = options;
        Output = output;

        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "aider",
                Arguments = "--no-pretty --no-git",
                WorkingDirectory = options.WorkingDirectory,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
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
            process.StandardInput.WriteLine("/exit");
            process.WaitForExit();
        }

        process.OutputDataReceived -= LogMessage;
        process.ErrorDataReceived -= LogMessage;

        process.Dispose();
    }

    void LogMessage(object _, DataReceivedEventArgs message)
        => Output.Log(message.Data);
}
