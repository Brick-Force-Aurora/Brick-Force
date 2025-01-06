using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _Emulator
{
    public class DebugConsole : MonoBehaviour
    {
        public static DebugConsole instance;
        struct Log
        {
            public string message;
            public string stackTrace;
            public LogType type;
        }

        public KeyCode toggleKey = KeyCode.BackQuote;

        List<Log> logs = new List<Log>();
        Vector2 scrollPosition;
        bool hidden = true;
        bool collapse = true;
        private const string logFileName = "ClientLog.log";

        static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
    {
        { LogType.Assert, Color.white },
        { LogType.Error, Color.red },
        { LogType.Exception, Color.red },
        { LogType.Log, Color.white },
        { LogType.Warning, Color.yellow },
    };

        const int margin = 20;

        Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
        Rect titleBarRect = new Rect(0, 0, 10000, 20);
        GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
        GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

        void Awake()
        {
            // Clear the log file at the start of the program
            try
            {
                if (File.Exists(logFileName))
                {
                    File.WriteAllText(logFileName, string.Empty); // Clear the file
                }
            }
            catch (IOException ex)
            {
                Debug.LogError($"Failed to clear log file: {ex.Message}");
            }
        }

        void OnEnable()
        {
            Application.RegisterLogCallback(HandleLog);
        }

        void OnDisable()
        {
            Application.RegisterLogCallback(null);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
                hidden = !hidden;

            if (Input.GetKey(KeyCode.DownArrow))
                scrollPosition.y += 5f;

            if (Input.GetKey(KeyCode.UpArrow))
                scrollPosition.y -= 5f;
        }

        public void OnGUI()
        {
            if (!hidden)
                windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
        }

        void ConsoleWindow(int windowID)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < logs.Count; i++)
            {
                var log = logs[i];

                if (collapse)
                {
                    var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

                    if (messageSameAsPrevious)
                    {
                        continue;
                    }
                }

                GUI.contentColor = logTypeColors[log.type];
                GUILayout.Label(log.message);
                if (log.stackTrace != string.Empty)
                    GUILayout.Label(log.stackTrace);
            }

            GUILayout.EndScrollView();

            GUI.contentColor = Color.white;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(clearLabel))
            {
                logs.Clear();
            }

            collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

            GUILayout.EndHorizontal();

            GUI.DragWindow(titleBarRect);
        }

        void HandleLog(string message, string stackTrace, LogType type)
        {
            var logEntry = new Log()
            {
                message = message,
                stackTrace = stackTrace,
                type = type,
            };

            logs.Add(logEntry);

            // Append the log entry to the file
            try
            {
                using (StreamWriter writer = new StreamWriter(logFileName, true))
                {
                    writer.WriteLine($"[{System.DateTime.Now}] [{type}] {message}");
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        writer.WriteLine(stackTrace);
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.LogError($"Failed to write log to file: {ex.Message}");
            }
        }
    }
}