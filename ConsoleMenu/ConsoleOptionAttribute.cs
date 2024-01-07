using System;

namespace ConsoleMenu
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ConsoleOptionAttribute : Attribute
    {
        public int Number { get; }
        public string Description { get; }

        public ConsoleOptionAttribute(int number, string description)
        {
            Number = number;
            Description = description;
        }
    }

}