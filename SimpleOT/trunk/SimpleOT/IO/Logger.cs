using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SimpleOT.IO
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public static class Logger
    {
        private static StreamWriter streamWriter;

        private static void Write(string message)
        {
            try
            {
                if (streamWriter == null)
                    streamWriter = new StreamWriter("log.txt", true);

                streamWriter.WriteLine(message);
                streamWriter.Flush();
            }
            catch (Exception)
            {
                streamWriter = null;
            }
        }

        public static void Log(LogLevel level, string message, Exception exception = null)
        {
            var builder = new StringBuilder();

            builder.Append(DateTime.Now);
            builder.Append(" [").Append(level.ToString().ToUpper()).Append("] ");
            builder.Append(message);

            //vamos imprimir para tela sem o stack track.
            switch (level)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine(builder.ToString());
            Console.ResetColor();


            if (exception != null)
            {
                builder.Append('\n');
                builder.Append("Caused by: ").Append(exception.Message).Append('\n');
                builder.Append("Stack trace:\n");
                builder.Append(exception.StackTrace);
            }

            Write(builder.ToString());
        }

        public static void Debug(string message, Exception exception = null)
        {
            Log(LogLevel.Debug, message, exception);
        }

        public static void Info(string message, Exception exception = null)
        {
            Log(LogLevel.Info, message, exception);
        }

        public static void Warn(string message, Exception exception = null)
        {
            Log(LogLevel.Warn, message, exception);
        }

        public static void Error(string message, Exception exception = null)
        {
            Log(LogLevel.Error, message, exception);
        }

        public static void Fatal(string message, Exception exception = null)
        {
            Log(LogLevel.Fatal, message, exception);
        }
    }
}
