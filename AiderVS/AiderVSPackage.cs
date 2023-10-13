using EnvDTE;
using Logic;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace AiderVS;

/// <summary>
/// This is the class that implements the package exposed by this assembly.
/// </summary>
/// <remarks>
/// <para>
/// The minimum requirement for a class to be considered a valid package for Visual Studio
/// is to implement the IVsPackage interface and register itself with the shell.
/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
/// to do it: it derives from the Package class that provides the implementation of the
/// IVsPackage interface and uses the registration attributes defined in the framework to
/// register itself and its components with the shell. These attributes tell the pkgdef creation
/// utility what data to put into .pkgdef file.
/// </para>
/// <para>
/// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
/// </para>
/// </remarks>
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[Guid(PackageGuidString)]
[ProvideMenuResource("Menus.ctmenu", 1)]
public sealed class AiderVSPackage : AsyncPackage
{
    /// <summary>
    /// AiderVSPackage GUID string.
    /// </summary>
    public const string PackageGuidString = "1c8a04a1-7e9f-413d-b258-7303da93b38e";

    /// <summary>
    /// Initialization of the package; this method is called right after the package is sited, so this is the place
    /// where you can put all the initialization code that rely on services provided by VisualStudio.
    /// </summary>
    /// <param name="cancellation">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
    /// <param name="progress">A provider for progress updates.</param>
    /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
    protected override async Task InitializeAsync(CancellationToken cancellation, IProgress<ServiceProgressData> progress)
    {
        // When initialized asynchronously, the current thread may be a background thread at this point.
        // Do any initialization that requires the UI thread after switching to the UI thread.
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellation);
        var aider = await LoadAider();
        if (aider == null) return;

        await AddToChatCommand.Initialize(this);
        await RemoveFromChatCommand.Initialize(this);
    }

    async Task<Aider> LoadAider()
    {
        var output = await CreateOutputWindow();

        var solutionPath = await GetSolutionPath();
        if (solutionPath == null)
        {
            output.Log("Unable to find active solution path. Aider cannot be initialized.");
            return null;
        }

        var aiderConfigPath = Path.Combine(solutionPath, ".aider.conf.yml");
        if (!File.Exists(aiderConfigPath))
        {
            output.Log($"No aider config found at {aiderConfigPath}");
            return null;
        }

        var cliCommandExecutor = new CliCommandExecutor(new AiderCli(new AiderOptions(solutionPath)));
        var aider = new Aider(
            exit: new ExitCommand(cliCommandExecutor)
        );
        output.Log($"Using aider config at {aiderConfigPath}");
        return aider;
    }

    async Task<OutputWindowLogger> CreateOutputWindow()
    {
        var window = await Get<SVsOutputWindow, IVsOutputWindow>();
        var logger = await OutputWindowLogger.InitializeAsync(this, window, new Guid("75b97a3e-005b-448b-bca5-180710799bac"), "Aider");
        logger.Activate();
        return logger;
    }

    async Task<string> GetSolutionPath()
    {
        var dte = await GetServiceAsync(typeof(DTE)) as DTE;
        if (dte == null || dte.Solution == null || !dte.Solution.IsOpen) return null;

        return Path.GetDirectoryName(dte.Solution.FullName);
    }

    public Task<Casted> Get<Resolved, Casted>()
        where Casted : class
        => this.GetServiceAsync<Resolved, Casted>();
}
