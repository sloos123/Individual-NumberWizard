/// 
/// Handles parsing and execution of console commands, as well as collecting log output.
/// Copyright (c) 2014-2015 Eliot Lash
/// 
/*
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Text;

public delegate void CommandHandler(string[] args);

//consolecontroller ausgeben
public class ConsoleController
{

    #region Event declarations
    // Used to communicate with ConsoleView
    public delegate void LogChangedHandler(string[] log);
    public event LogChangedHandler logChanged;

    public delegate void VisibilityChangedHandler(bool visible);
    public event VisibilityChangedHandler visibilityChanged;
    #endregion

    public GameLogic gameLogic = new GameLogic();
    bool mydebug = false;

    /// 
    /// Object to hold information about each command
    /// 
    class CommandRegistration
    {
        public string command { get; private set; }
        public CommandHandler handler { get; private set; }
        public string help { get; private set; }

        public CommandRegistration(string command, CommandHandler handler, string help)
        {
            this.command = command;
            this.handler = handler;
            this.help = help;
        }
    }

    /// 
    /// How many log lines should be retained?
    /// Note that strings submitted to appendLogLine with embedded newlines will be counted as a single line.
    /// 
    const int scrollbackSize = 20;

    Queue<string> scrollback = new Queue<string>(scrollbackSize);
    List<string> commandHistory = new List<string>();
    Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>();

	public string[] log { get; private set; } //Copy of scrollback as an array for easier use by ConsoleView

    const string repeatCmdName = "!!"; //Name of the repeat command, constant since it needs to skip these if they are in the command history

    public ConsoleController()
    {
        //When adding commands, you must add a call below to registerCommand() with its name, implementation method, and help text.
        registerCommand("game", game, "Starts the game: game [start] min=1, max=1000, game [start] [min] [max] start and set min and max, game [lower], game [higher], game [end]. ");
        registerCommand("babble", babble, "Example command that demonstrates how to parse arguments. babble [word] [# of times to repeat]");
        registerCommand("echo", echo, "echoes arguments back as array (for testing argument parser)");
        registerCommand("help", help, "Print this help.");
        registerCommand("hide", hide, "Hide the console.");
        registerCommand(repeatCmdName, repeatCommand, "Repeat last command.");
        registerCommand("reload", reload, "Reload game.");
        registerCommand("resetprefs", resetPrefs, "Reset & saves PlayerPrefs.");
    }

    void registerCommand(string command, CommandHandler handler, string help)
    {
        commands.Add(command, new CommandRegistration(command, handler, help));
    }

    public void appendLogLine(string line)
    {
        Debug.Log(line);

        if (scrollback.Count >= ConsoleController.scrollbackSize)
        {
            scrollback.Dequeue();
        }
        scrollback.Enqueue(line);

        log = scrollback.ToArray();
        if (logChanged != null)
        {
            logChanged(log);
        }
    }

    public void runCommandString(string commandString)
    {
        appendLogLine("$ " + commandString);

        string[] commandSplit = parseArguments(commandString);
        string[] args = new string[0];
        if (commandSplit.Length < 1)
        {
            appendLogLine(string.Format("Unable to process command '{0}'", commandString));
            return;
        }
        else if (commandSplit.Length >= 2) {
			int numArgs = commandSplit.Length - 1;
            args = new string[numArgs];
			Array.Copy(commandSplit, 1, args, 0, numArgs);
		}

        runCommand(commandSplit[0].ToLower(), args);
        commandHistory.Add(commandString);
	}
	
	public void runCommand(string command, string[] args)
{
    CommandRegistration reg = null;
    if (!commands.TryGetValue(command, out reg))
    {
        appendLogLine(string.Format("Unknown command '{0}', type 'help' for list.", command));
    }
    else
    {
        if (reg.handler == null)
        {
            appendLogLine(string.Format("Unable to process command '{0}', handler was null.", command));
        }
        else
        {
            reg.handler(args);
        }
    }
}

static string[] parseArguments(string commandString)
{
    LinkedList<char> parmChars = new LinkedList<char>(commandString.ToCharArray());
    bool inQuote = false;
    var node = parmChars.First;
    while (node != null)
    {
        var next = node.Next;
        if (node.Value == '"')
        {
            inQuote = !inQuote;
            parmChars.Remove(node);
        }
        if (!inQuote && node.Value == ' ')
        {
            node.Value = '\n';
        }
        node = next;
    }
    char[] parmCharsArr = new char[parmChars.Count];
    parmChars.CopyTo(parmCharsArr, 0);
    return (new string(parmCharsArr)).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
}

    #region Command handlers
    //Implement new commands in this region of the file.

    void game(string[] args)
    {
        string msg = "";

        if (args.Length == 0)
        {
            msg = "game [start] -- starts the game in default mode (min=1, max=1000 \n" +
                  "game [start] [min] [max] -- starts the game in user specified mode \n" +
                  "game [lower] -- use if your number is lower than the guess \n" +
                  "game [higher] -- use if your number is higher than the guess \n " +
                  "game [end] -- use if your number equals the guess";
        }
        else if (args.Length == 1)
        {
            if (args[0].Equals("end"))
            {
                gameLogic.EndGame();
                msg = "Erraten :)";

                MyDebug("console End");
            }
            else if (args[0].Equals("start"))
            {
                msg = gameLogic.StartGame();

                MyDebug("console Start");
            }

            else if (args[0].Equals("lower"))
            {
                gameLogic.AdaptMax();
                msg = gameLogic.NextGuess();

                MyDebug("console Lower");
            }
            else if (args[0].Equals("higher"))
            {
                gameLogic.AdaptMin();
                msg = gameLogic.NextGuess();

                MyDebug("console Higher");
            }
            else
            {
                msg = "richtiger Befehl? Tippe game für Hilfe";
            }
        }
        else if (args.Length == 3)
        {
            gameLogic.SetValues(Int32.Parse(args[1]), Int32.Parse(args[2]));
            MyDebug("args: [0]" + args[0] + " [1]" + args[1] + " [2] " + args[2]);

            msg = gameLogic.StartGame();
        }
        else
        {
            msg= "Ein Satz mit X das war wohl nix! Befehl richtig?";
        }

        appendLogLine(msg);
    }


    /// 
    /// A test command to demonstrate argument checking/parsing.
    /// Will repeat the given word a specified number of times.
    /// 

    void babble(string[] args)
    {
        if (args.Length < 2)
        {
            appendLogLine("Expected 2 arguments.");
            return;
        }

        string text = args[0];
        if (string.IsNullOrEmpty(text))
        {
            appendLogLine("Expected arg1 to be text.");
        }
        else
        {
            int repeat = 0;
            if (!Int32.TryParse(args[1], out repeat))
            {
                appendLogLine("Expected an integer for arg2.");
            }
            else
            {
                for (int i = 0; i < repeat; ++i)
                {
                    appendLogLine(string.Format("{0} {1}", text, i));
                }
            }
        }
    }

    void echo(string[] args)
    {
        StringBuilder sb = new StringBuilder();
        foreach (string arg in args)
        {
            sb.AppendFormat("{0},", arg);
        }
        sb.Remove(sb.Length - 1, 1);
        appendLogLine(sb.ToString());
    }

    void help(string[] args)
    {
        foreach (CommandRegistration reg in commands.Values)
        {
            appendLogLine(string.Format("{0}: {1}", reg.command, reg.help));
        }
    }

    void hide(string[] args)
    {
        if (visibilityChanged != null)
        {
            visibilityChanged(false);
        }
    }

    void repeatCommand(string[] args)
    {
        for (int cmdIdx = commandHistory.Count - 1; cmdIdx >= 0; --cmdIdx)
        {
            string cmd = commandHistory[cmdIdx];
            if (String.Equals(repeatCmdName, cmd))
            {
                continue;
            }
            runCommandString(cmd);
            break;
        }
    }

    void reload(string[] args)
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    void resetPrefs(string[] args)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    #endregion

    void MyDebug(string msg)
    {
        if (mydebug)
        {
            Debug.Log(msg);
        }
    }
}
*/