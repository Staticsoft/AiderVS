using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using System;
using System.Threading.Tasks;

namespace AiderVS;

public class OutputWindowLogger : Logger
{
    readonly IVsOutputWindowPane Pane;

    public OutputWindowLogger(IVsOutputWindowPane pane)
        => Pane = pane;

    public static async Task<OutputWindowLogger> InitializeAsync(AsyncPackage package, IVsOutputWindow window, Guid paneGuid, string paneTitle)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

        if (window.GetPane(ref paneGuid, out var pane) != VSConstants.S_OK)
        {
            window.CreatePane(ref paneGuid, paneTitle, fInitVisible: 1, fClearWithSolution: 0);
            window.GetPane(ref paneGuid, out pane);
        }
        return new(pane);
    }

    public void Log(string message)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        Pane.OutputStringThreadSafe(message + Environment.NewLine);
    }

    public void Activate()
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        Pane.Activate();
    }
}
