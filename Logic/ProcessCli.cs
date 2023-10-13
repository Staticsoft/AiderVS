using System;
using System.Diagnostics;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Logic;

public abstract class ProcessCli : Cli
{
    readonly Process Process;
    readonly Channel<string> Output = Channel.CreateUnbounded<string>();

    public ProcessCli(ProcessCliOptions options)
    {
        Process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = options.WorkingDirectory,
                FileName = options.Command,
                Arguments = options.Arguments,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        Process.OutputDataReceived += LogMessage;
        Process.ErrorDataReceived += LogMessage;

        Process.Start();

        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();

        Task.Run(DisposeOnExit);
    }

    public Task Write(string command)
    {
        Process.StandardInput.WriteLine(command);
        return Task.CompletedTask;
    }

    ChannelReader<string> Cli.Output
        => Output;

    void LogMessage(object _, DataReceivedEventArgs message)
    {
        if (message.Data != null)
        {
            Output.Writer.WriteAsync(message.Data);
        }
    }

    async Task DisposeOnExit()
    {
        if (!Process.HasExited)
        {
            Process.WaitForExit();
        }

        await Output.Writer.WriteAsync("### END OF OUTPUT ###");
        Output.Writer.Complete(new OperationCanceledException("Process has exited"));

        Process.OutputDataReceived -= LogMessage;
        Process.ErrorDataReceived -= LogMessage;

        Process.Dispose();
    }
}