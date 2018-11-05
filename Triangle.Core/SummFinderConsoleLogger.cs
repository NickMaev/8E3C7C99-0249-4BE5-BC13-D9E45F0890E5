using System;
using System.Diagnostics.CodeAnalysis;

namespace Triangle.Core
{
    /// <summary>
    /// Logger which sinks to console with colors.
    /// </summary>
    public class SummFinderConsoleLogger : ISummFinderLogger
    {
        public void Write(string text, ConsoleColor? backgroundColor = null, ConsoleColor? fontColor = null)
        {
            SetColors(backgroundColor, fontColor);
            Console.Write(text);
            ResetColors();
        }
        
        public void WriteLine(string text = null, ConsoleColor? backgroundColor = null, ConsoleColor? fontColor = null)
        {
            SetColors(backgroundColor, fontColor);
            Console.WriteLine(text);
            ResetColors();
        }
        
        protected void ResetColors()
        {
            Console.ResetColor();
        }

        protected void SetColors(ConsoleColor? backgroundColor = null, ConsoleColor? fontColor = null)
        {
            if(backgroundColor.HasValue)
                Console.BackgroundColor = backgroundColor.Value;
            if(fontColor.HasValue)
                Console.ForegroundColor = fontColor.Value;
        }
    }
}
