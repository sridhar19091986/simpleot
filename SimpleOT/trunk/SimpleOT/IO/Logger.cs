using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.IO
{
    public static class Logger
    {
        public static void Debug(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("{0} [DEBUG] {1}", DateTime.Now, message);
            Console.ResetColor();
        }

        public static void Info(string message, Exception exception = null)
        {
            Console.WriteLine("{0} [INFO] {1}", DateTime.Now, message);
        }

        public static void Warn(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0} [WARN] {1}", DateTime.Now, message);
            Console.ResetColor();
        }

        public static void Error(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0} [ERROR] {1}", DateTime.Now, message);
            Console.ResetColor();
        }

        public static void Fatal(string message, Exception exception = null)
        {
            Console.WriteLine("{0} [FATAL] {1}", DateTime.Now, message);
        }
    }
}
