using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleMenu
{
    public class ConsoleMenuUtility
    {
        private readonly List<(int, string, MethodInfo)> options;

        public ConsoleMenuUtility()
        {
            // Initialize options with the Exit option
            options = new List<(int, string, MethodInfo)>
            {
                (0, "Exit", null) // Adding Exit as the first option
            };

            // Append methods annotated with ConsoleOption
            options.AddRange(GetConsoleOptions());
        }

        public void DisplayMenuAndHandleInput()
        {
            StringBuilder inputBuffer = new StringBuilder();
            int currentSelection = -1;
            ConsoleKeyInfo keyInfo;

            do
            {
                Console.Clear();
                DisplayMenu(currentSelection);
                Console.WriteLine($"\nYour Selection: {inputBuffer}");
                keyInfo = Console.ReadKey();

                if (char.IsDigit(keyInfo.KeyChar))
                {
                    inputBuffer.Append(keyInfo.KeyChar);
                    if (int.TryParse(inputBuffer.ToString(), out currentSelection) && options.Any(o => o.Item1 == currentSelection))
                    {
                        // Valid selection
                    }
                    else
                    {
                        inputBuffer.Clear();
                        currentSelection = -1;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter && currentSelection >= 0)
                {
                    InvokeSelectedMethod(currentSelection);
                    inputBuffer.Clear();
                    currentSelection = -1;
                }
                else
                {
                    inputBuffer.Clear();
                    currentSelection = -1;
                }

            } while (currentSelection != 0);

            Console.WriteLine("\nExiting...");
        }

        private void DisplayMenu(int currentSelection)
        {
            Console.WriteLine("Select an option:");
            foreach (var option in options)
            {
                if (option.Item1 == currentSelection)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }

                Console.WriteLine($"{option.Item1}. {option.Item2}");

                if (option.Item1 == currentSelection)
                {
                    Console.ResetColor();
                }
            }
        }

        private void InvokeSelectedMethod(int selection)
        {
            var selectedOption = options.FirstOrDefault(o => o.Item1 == selection);
            Console.WriteLine($"Executing: {selectedOption.Item2}");
            selectedOption.Item3?.Invoke(null, null);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private IEnumerable<(int, string, MethodInfo)> GetConsoleOptions()
        {
            var callingAssembly = Assembly.GetEntryAssembly();
            var consoleOptionMethods = new List<(int, string, MethodInfo)>();

            foreach (var type in callingAssembly.GetTypes())
            {
                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<ConsoleOptionAttribute>();
                    if (attr != null)
                    {
                        consoleOptionMethods.Add((attr.Number, attr.Description, method));
                    }
                }
            }

            return consoleOptionMethods.OrderBy(o => o.Item1);
        }
    }

}