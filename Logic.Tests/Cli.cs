using System.Threading.Channels;

namespace Logic.Tests;

public interface Cli
{
    ChannelReader<string> Output { get; }
    Task Write(string command);
}
