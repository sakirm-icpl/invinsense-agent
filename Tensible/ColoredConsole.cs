using System;

namespace Tensible
{
    public static class ColoredConsole
    {
        public static void Write(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void WriteLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        //todo: Need to create helper class for this
        public static void ConsoleColorTest()
        {
            Console.WriteLine("\x1b[31mThis text is red\x1b[0m");

            Console.WriteLine("\x1b[32mThis text is green\x1b[0m");

            Console.WriteLine("\x1b[33mThis text is yellow\x1b[0m");

            Console.WriteLine("\x1b[34mThis text is blue\x1b[0m");

            Console.WriteLine("\x1b[35mThis text is magenta\x1b[0m");

            Console.WriteLine("\x1b[36mThis text is cyan\x1b[0m");

            Console.WriteLine("\x1b[37mThis text is white\x1b[0m");

            Console.WriteLine("\x1b[1mThis text is bold\x1b[0m");

            Console.WriteLine("\x1b[4mThis text is underlined\x1b[0m");

            Console.WriteLine("\x1b[7mThis text is reversed\x1b[0m");

            Console.WriteLine("\x1b[8mThis text is invisible\x1b[0m");

            Console.WriteLine("\x1b[9mThis text is strikethrough\x1b[0m");

            Console.WriteLine("\x1b[31mThis text is red\x1b[0m \x1b[32mThis text is green\x1b[0m");
        }
    }
}
