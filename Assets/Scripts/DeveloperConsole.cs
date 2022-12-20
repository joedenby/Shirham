using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Object = UnityEngine.Object;
using System.IO;
using System.Linq;

public class DeveloperConsole : MonoBehaviour
{

    // Flag to track whether the console is open or closed
    private bool isOpen = false;

    // The input field where users can type their commands
    [SerializeField]
    private TMP_InputField inputField;
    private string inputText => inputField.text;

    // The list of available commands
    private Dictionary<string, Command> commands = new Dictionary<string, Command>();

    // Static instance of the singleton
    private static DeveloperConsole instance;

    // Property to get the instance of the singleton
    public static DeveloperConsole Instance
    {
        get
        {
            // If the instance is null, find the singleton object in the scene
            if (instance == null)
                instance = FindObjectOfType<DeveloperConsole>();

            return instance;
        }
    }


    void Start() => BuildCommandDictionary();

    void Update()
    {
        // Toggle the console when the '~' key is pressed
        if (Input.GetKeyDown(KeyCode.BackQuote))
            EnableConsole(!isOpen);

        // Process command when 'return' is pressed
        if (Input.GetKeyDown(KeyCode.Return) && isOpen)
            CallCommand(inputText);
    }

    void BuildCommandDictionary() {
        // Find all types that derive from the Command class
        Type commandType = typeof(Command);
        Type[] commandTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsSubclassOf(commandType)).ToArray();

        // Add an instance of each command type to the commands dictionary
        foreach (Type type in commandTypes) {
            Command command = (Command)Activator.CreateInstance(type);
            commands.Add(type.Name.ToLower(), command);
        }
    }

    public void EnableConsole(bool active = true) {
        isOpen = active;
        inputField.gameObject.SetActive(isOpen);
        Time.timeScale = isOpen ? 0f : 1f;

        if (!isOpen) return;

        // Set focus if console is open
        inputField.Select();
        inputField.ActivateInputField();
    }

    public void CallCommand(string input)
    {
        if (!isOpen) return;

        // Split the input into words, separated by spaces
        string[] words = Array.ConvertAll(input.Split(' '), s => s.Replace("_", " "));

        // The first word is the command name
        string commandName = words[0];

        // The rest of the words are the arguments
        string[] args = new string[words.Length - 1];
        for (int i = 1; i < words.Length; i++)
        {
            args[i - 1] = words[i];
        }

        // Look up the command by name and execute it
        Command command;
        if (commands.TryGetValue(commandName, out command))
        {
            command.Execute(args);
        }
        else
        {
            Debug.Log("Unknown command: " + commandName);
            EnableConsole(false);
        }

        // Clear the input field
        inputField.text = string.Empty;
    }

}

#region Commands

// An abstract base class for commands that can be executed from the console
public abstract class Command
{
    public abstract void Execute(string[] args);
}

public class Move : Command
{
    public override void Execute(string[] args)
    {
        if (args.Length != 3)
        {
            Debug.Log($"Usage: move 'name' 'x' 'y'" +
                $"\nargs[{args.Length}]");
            return;
        }

        GameObject obj = GameObject.Find(args[0]);
        if (!obj) {
            Debug.LogWarning($"Object '{args[0]}' was not found.");
            return;
        }

        float x = float.Parse(args[1]);
        float y = float.Parse(args[2]);

        obj.transform.position = new Vector2(Mathf.Floor(x) + .5f, Mathf.Floor(y) + .5f);
    }
}

public class Batch : Command
{
    public override void Execute(string[] args)
    {
        if (args.Length != 1) 
        {
            Debug.Log($"Usage: batch 'filename'" +
                    $"\nargs[{args.Length}]");
            return;
        }

        // Get the path to the file
        string path = Application.dataPath + $"/Resources/Batch/{args[0]}.txt";

        // Check if the file exists
        if (File.Exists(path))
        {
            // Read the contents of the file into an array of strings
            string[] lines = File.ReadAllLines(path).Where(
                line => !string.IsNullOrEmpty(line) && !line.TrimStart().StartsWith("//")).ToArray();
            string log = $"Loading batch {args[0]}.txt";

            // Iterate through the array and print each line to the console
            foreach (string line in lines) {
                DeveloperConsole.Instance.CallCommand(line);
                log += $"\n> {line}";
            }
            Debug.Log(log);
        }
        else
        {
            // File does not exist, print an error message to the console
            Debug.LogError("File does not exist: " + path);
        }
    }
}

public class Cam : Command
{
    public override void Execute(string[] args)
    {
        if (args.Length == 0) return;

        switch (args[0]) {
            case "size":
                if (args.Length != 3) {
                    Debug.Log("Usage: cam size 'x' 'y'");
                    return;
                }
                var size = new Vector2Int(int.Parse(args[1]), int.Parse(args[2]));
                CameraController.main.Camera.refResolutionX = size.x;
                CameraController.main.Camera.refResolutionY = size.y;
                return;
            case "target":
                if (args.Length != 2) {
                    Debug.Log("Usage: cam target 'name'");
                    return;
                }

                var target = GameObject.Find(args[1]);
                if (!target && !args[1].Equals("null")) {
                    Debug.Log($"Could not find target '{args[1]}'");
                }

                CameraController.main.target = args[1].Equals("null") ? null : target.transform;
                return;
            case "move":
                if (args.Length != 3) {
                    Debug.Log("Usage: cam move 'x' 'y'");
                    return;
                }

                var move = new Vector2(float.Parse(args[1]), float.Parse(args[2]));
                CameraController.main.SetPosition(move);
                return;
            default:
                Debug.Log("Unknown command: " + args[0]);
                return;
        }
    }
}

public class Destroy : Command
{
    public override void Execute(string[] args)
    {
        if (args.Length != 1)
        {
            Debug.Log($"Usage: destroy 'name' \nargs[{args.Length}]");
            return;
        }

        GameObject obj = GameObject.Find(args[0]);
        if (!obj) {
            Debug.LogWarning($"Object '{args[0]}' was not found.");
            return;
        }

        Object.Destroy(obj);
    }
}

public class Unit : Command
{
    public override void Execute(string[] args)
    {
        if (args.Length == 0) return;

        switch (args[0])
        {
            case "follow":
                if (args.Length != 3)
                {
                    Debug.Log("Usage: unit follow 'leader' 'follower'");
                    return;
                }

                var leader = GameObject.Find(args[1]);
                var follower = GameObject.Find(args[2])?.GetComponent<AIPathFinder>();

                if (!leader)
                {
                    follower?.Clear();
                    return;
                }

                follower.GoTo(leader.transform, true);
                follower.nextToTarget = true;
                return;
            case "spawn":
                if (args.Length != 4) {
                    Debug.Log($"Usage: unit spawn 'name' 'x' 'y'" +
                        $"\nargs[{args.Length}]");
                    return;
                }

                // Load the unit from Resources
                GameObject prefab = Resources.Load<GameObject>($"Units/{args[1]}");
                if (!prefab)
                {
                    Debug.LogWarning($"Unit '{args[1]}' was not found.");
                    return;
                }

                float x = float.Parse(args[2]);
                float y = float.Parse(args[3]);

                // Instantiate the unit at the given position
                var obj = Object.Instantiate(prefab, new Vector2(Mathf.Floor(x) + .5f, Mathf.Floor(y) + .5f),
                    Quaternion.identity);

                obj.name = args[1];
                return;
            default:
                Debug.Log("Unknown command: " + args[0]);
                return;
        }
    }
}

#endregion