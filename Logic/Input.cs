using System;

namespace Logic;

public interface Input
{
    event Action<string> Sent;
}