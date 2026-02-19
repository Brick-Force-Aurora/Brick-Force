using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Emulator
{

    public class CommandHandler
    {
        private static readonly CommandHandler _handler = new CommandHandler();
        public static CommandHandler Instance { get { return _handler; } }
        
        public CommandHistory History { get; private set; }

        private CommandNode rootNode = new CommandNode();

        public CommandHandler()
        {
            History = new CommandHistory();
        }

        public void Register(string name, ICommand command)
        {
            Register(new string[]
            {
                name
            }, command);
        }

        public void Register(string name1, string name2, ICommand command)
        {
            Register(new string[]
            {
                name1,
                name2
            }, command);
        }

        public void Register(string name1, string name2, string name3, ICommand command)
        {
            Register(new string[]
            {
                name1,
                name2,
                name3,
            }, command);
        }

        public void Register(string[] aliases, ICommand command)
        {
            foreach (string alias in aliases)
            {
                rootNode.CreateNode(ToNodePath(alias), command);
            }
        }

        public ICommand FindCommand(string command)
        {
            CommandReader reader = new CommandReader(command);
            reader.SkipWhitespace();
            if (reader.Peek() != '/')
            {
                return null;
            }
            reader.Skip();
            int lastIndex = reader.Index;
            string lastToken;
            StringBuilder fullCommand = new StringBuilder();
            CommandNode node = rootNode, tmp;
            while (reader.HasNext())
            {
                lastToken = reader.ReadToken();
                tmp = node.Node(lastToken);
                if (tmp == null)
                {
                    reader.Index = lastIndex;
                    break;
                }
                if (fullCommand.Length != 0)
                {
                    fullCommand.Append(' ');
                }
                fullCommand.Append(lastToken);
                lastIndex = reader.Index;
                node = tmp;
            }
            if (node == rootNode || node.Command == null)
            {
                return null;
            }
            return node.Command;
        }

        public bool Execute(string command)
        {
            CommandReader reader = new CommandReader(command);
            reader.SkipWhitespace();
            if (reader.Peek() != '/')
            {
                return false;
            }
            reader.Skip();
            int lastIndex = reader.Index;
            string lastToken;
            StringBuilder fullCommand = new StringBuilder();
            CommandNode node = rootNode, tmp;
            while (reader.HasNext())
            {
                lastToken = reader.ReadToken();
                tmp = node.Node(lastToken);
                if (tmp == null)
                {
                    reader.Index = lastIndex;
                    break;
                }
                if (fullCommand.Length != 0)
                {
                    fullCommand.Append(' ');
                }
                fullCommand.Append(lastToken);
                lastIndex = reader.Index;
                node = tmp;
            }
            if (node == rootNode)
            {
                Actor.Instance.SendChat($"Unknown command '{command}'");
                return true;
            }
            reader.SkipWhitespace();
            if (node.Command == null)
            {
                Actor.Instance.SendChat($"Unknown command '{command}'");
                return true;
            }
            History.Log(command);
            try
            {
                Debug.Log($"Executing command '{command}'");
                node.Command.Execute(new CommandContext(fullCommand.ToString(), reader));
            }
            catch (IndexOutOfRangeException e)
            {
                Actor.Instance.SendChat($"Failed to execute command '{command}': {e.Message}");
                Debug.LogWarning($"Command reader couldn't read: {e.Message}");
            }
            catch (Exception e)
            {
                Actor.Instance.SendChat($"Failed to execute command '{command}': {e.Message}");
                Debug.LogWarning($"Failed to execute command '{command}':");
                Debug.LogWarning(e);
            }
            return true;
        }

        public ICommand GetCommand(string commandPath)
        {
            string[] path = ToNodePath(commandPath);
            CommandNode node = rootNode;
            foreach (string part in path)
            {
                node = node.Node(part);
                if (node == null)
                {
                    return null;
                }
            }
            return node.Command;
        }

        private string[] ToNodePath(string commandPath)
        {
            List<string> path = commandPath.Split(' ').ToList();
            for (int i = 0; i < path.Count; i++)
            {
                if (path[i].Length == 0)
                {
                    path.RemoveAt(i--);
                    continue;
                }
            }
            return path.ToArray();
        }

    }

    public class CommandHistory
    {
        const int MAX_HISTORY = 100;
        private List<string> history = new List<string>();

        public void Log(string command)
        {
            command = Sanatize(command);
            for (int i = 0; i < history.Count; i++)
            {
                if (history[i] != command)
                {
                    continue;
                }
                if (i != 0)
                {
                    string tmp = history[0];
                    history[0] = command;
                    history[i] = tmp;
                }
                return;
            }
            if (history.Count >= MAX_HISTORY)
            {
                history.RemoveAt(history.Count - 1);
            }
            history.Insert(0, command);
        }

        public string Next(string command)
        {
            if (history.Count == 0)
            {
                return string.Empty;
            }
            command = Sanatize(command);
            if (command.Length != 0)
            {
                int index = history.LastIndexOf(command);
                if (index >= 0)
                {
                    if (index + 1 >= history.Count)
                    {
                        return command;
                    }
                    return history[index + 1];
                }
            }
            return history[0];
        }

        public string Previous(string command)
        {
            if (history.Count == 0)
            {
                return string.Empty;
            }
            command = Sanatize(command);
            if (command.Length != 0)
            {
                int index = history.LastIndexOf(command);
                if (index == 0)
                {
                    return command;
                } else if (index > 0)
                {
                    return history[index - 1];
                }
            }
            return string.Empty;
        }

        private string Sanatize(string command)
        {
            if (command.Length == 0)
            {
                return command;
            }
            CommandReader reader = new CommandReader(command);
            StringBuilder builder = new StringBuilder();
            while (reader.HasNext())
            {
                if (builder.Length != 0)
                {
                    builder.Append(' ');
                }
                builder.Append(reader.ReadToken());
            }
            return builder.ToString();
        }

    }

    internal class CommandNode
    {

        private Dictionary<string, CommandNode> commands = new Dictionary<string, CommandNode>();
        public ICommand Command { get; internal set; }

        public void CreateNode(string[] path, ICommand command)
        {
            if (path.Length == 0)
            {
                throw new ArgumentException("Empty path");
            }
            CommandNode node = this;
            for (int i = 0; i < path.Length; i++)
            {
                if (!commands.ContainsKey(path[i]))
                {
                    commands[path[i]] = new CommandNode();
                }
                node = commands[path[i]];
            }
            if (node.Command != null)
            {
                throw new ArgumentException($"Command with path '{path}' already exists");
            }
            node.Command = command;
        }

        public CommandNode Node(string name)
        {
            if (commands.TryGetValue(name, out var cmd))
            {
                return cmd;
            }
            return null;
        }

    }

    public class CommandReader
    {

        private int index;
        private string buffer;

        public int Index { 
            get { return index; } 
            set {
                if (value < 0)
                {
                    throw new IndexOutOfRangeException("Only 0+");
                }
                else if (value >= buffer.Length)
                {
                    throw new IndexOutOfRangeException($"Index can't be higher than {buffer.Length}");
                }
                this.index = value;
            } 
        }

        public CommandReader(string buffer)
        {
            this.index = 0;
            this.buffer = buffer;
        }

        public bool HasNext()
        {
            return index < buffer.Length;
        }

        public char Peek()
        {
            return buffer[index];
        }

        public CommandReader Skip()
        {
            if (HasNext())
                index++;
            return this;
        }

        public CommandReader SkipWhitespace()
        {
            while (HasNext() && char.IsWhiteSpace(buffer[index]))
            {
                index++;
            }
            return this;
        }

        public string GetUnread()
        {
            if (!HasNext())
            {
                return string.Empty;
            }
            return buffer.Substring(index);
        }

        public void ReadTokens(int min, int max, out string[] tokens)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < max; i++)
            {
                if (SkipWhitespace().HasNext())
                {
                    list.Add(ReadToken());
                    continue;
                }
                if (i >= min)
                {
                    break;
                }
                if (min == max)
                {
                    throw new ArgumentException("Not enough arguments, expected " + max);
                }
                throw new ArgumentException("Not enough arguments, expected " + min + " - " + max);
            }
            tokens = list.ToArray();
        }

        public string ReadToken()
        {
            if (!SkipWhitespace().HasNext())
            {
                throw new IndexOutOfRangeException("End of command");
            }
            char ch = buffer[index];
            if (ch == '\'' || ch == '"')
            {
                return ReadQuotedToken(ch);
            }
            StringBuilder builder = new StringBuilder();
            bool escaped = false;
            while (HasNext())
            {
                ch = buffer[index];
                if (!escaped)
                {
                    if (!IsValidUnquoted(ch))
                    {
                        break;
                    }
                    if (ch == '\\')
                    {
                        escaped = true;
                        index++;
                        continue;
                    }
                }
                escaped = false;
                builder.Append(ch);
                index++;
            }
            return builder.ToString();
        }


        private bool IsValidUnquoted(char ch)
        {
            if (char.IsLetter(ch) || char.IsDigit(ch))
            {
                return true;
            }
            if (ch == '_' || ch == '-' || ch == '.' || ch == '/' || ch == ',')
            {
                return true;
            }
            return false;
        }

        private string ReadQuotedToken(char quote)
        {
            StringBuilder builder = new StringBuilder();
            bool escaped = false;
            index++;
            char ch;
            while (HasNext())
            {
                ch = buffer[index];
                if (!escaped)
                {
                    if (ch == quote)
                    {
                        index++;
                        return builder.ToString();
                    }
                    if (ch == '\\')
                    {
                        escaped = true;
                        index++;
                        continue;
                    }
                }
                escaped = false;
                builder.Append(ch);
                index++;
            }
            throw new IndexOutOfRangeException("Quoted token was not closed");
        }

    }
}
