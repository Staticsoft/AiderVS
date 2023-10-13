using System;

namespace Logic;

public interface Output
{
    event Action<string> Received;
}