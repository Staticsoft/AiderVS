using System.Threading.Channels;
using System.Threading.Tasks;

namespace Logic;

public interface Cli
{
    ChannelReader<string> Output { get; }
    Task Write(string command);
}
