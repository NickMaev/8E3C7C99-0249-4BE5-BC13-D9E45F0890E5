using System;

namespace Triangle.Core
{
    public interface ISummFinderLogger
    {
        void WriteLine(string text = null, ConsoleColor? backgroundColor = null, ConsoleColor? fontColor = null);
        void Write(string text, ConsoleColor? backgroundColor = null, ConsoleColor? fontColor = null);
    }
}
