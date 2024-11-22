using Microsoft.Win32;
using System;

namespace L3_console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Task_Queue\Tasks"))
            {
                object value = key.GetValue("Tasks_Progress");
                if (value is string[] tasksProgress)
                {
                    string message = string.Join(Environment.NewLine, tasksProgress);
                    Console.WriteLine("Tasks_Progress:");
                    Console.WriteLine(message);
                }
            }
            Console.ReadKey();
        }
    }
}
