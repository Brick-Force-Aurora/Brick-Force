using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Emulator
{
    public static class ClientDebugger
    {
        public struct Entry
        {
            public string Message;
            public string StackTrace;
            public LogType Type;
        }

        private static readonly Queue<Entry> _queue = new Queue<Entry>();
        private static readonly object _lock = new object();

        public static void Log(string msg)
        {
            Enqueue(LogType.Log, "[Client] " + msg, null);
        }

        public static void Log(long msg)
        {
            Enqueue(LogType.Log, "[Client] " + msg.ToString(), null);
        }

        public static void LogVerbose(string msg)
        {
            Enqueue(LogType.Log, "[Verbose/Client] " + msg, null);
        }

        public static void LogWarning(string msg)
        {
            Enqueue(LogType.Warning, "[Warning/Client] " + msg, null);
        }

        public static void LogWarning(bool b)
        {
            Enqueue(LogType.Warning, "[Warning/Client] " + b.ToString(), null);
        }

        public static void LogError(string msg)
        {
            Enqueue(LogType.Error, "[Error/Client] " + msg, "");
        }

        public static void LogError(Exception ex)
        {
            if (ex == null)
            {
                Enqueue(LogType.Exception, "[Error/Client] Exception was null", "");
                return;
            }

            Enqueue(
                LogType.Exception,
                "[Error/Client]" + ex.Message,
                ex.StackTrace ?? ""
            );
        }

        private static void Enqueue(LogType type, string msg, string stack)
        {
            // Optional console output
            //Console.WriteLine("[" + type + "] " + msg);

            lock (_lock)
            {
                _queue.Enqueue(new Entry
                {
                    Message = msg ?? "",
                    StackTrace = stack ?? "",
                    Type = type
                });
            }
        }

        public static bool TryDequeue(out Entry entry)
        {
            lock (_lock)
            {
                if (_queue.Count > 0)
                {
                    entry = _queue.Dequeue();
                    return true;
                }
            }

            entry = default(Entry);
            return false;
        }
    }
}