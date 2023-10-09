namespace Logic.Tests;

public interface Output
{
    event Action<string> Received;
}