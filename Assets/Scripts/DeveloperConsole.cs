using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Object = UnityEngine.Object;

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


    void Start()
    {
        // Add some sample commands to the list of available commands
        commands.Add("spawn", new SpawnCommand());
        commands.Add("kill", new KillCommand());
        commands.Add("move", new MoveCommand());
    }

    void Update()
    {
        // Toggle the console when the '~' key is pressed
        if (Input.GetKeyDown(KeyCode.BackQuote))
            EnableConsole(!isOpen);


        // Process command when 'return' is pressed
        if (Input.GetKeyDown(KeyCode.Return) && isOpen)
            CallCommand();
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

    public void CallCommand()
    {
        if (!isOpen) return;

        // Split the input into words, separated by spaces
        string[] words = Array.ConvertAll(inputText.Split(' '), s => s.Replace("_", " "));

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

// An abstract base class for commands that can be executed from the console
public abstract class Command
{
    public abstract void Execute(string[] args);
}

// A sample command that spawns a prefab at a given position
public class SpawnCommand : Command
{
    public override void Execute(string[] args)
    {
        if (args.Length != 3)
        {
            Debug.Log($"Usage: spawn 'name' 'x' 'y'" +
                $"\nargs[{args.Length}]");
            return;
        }

        // Load the unit from Resources
        GameObject prefab = Resources.Load<GameObject>($"Units/{args[0]}");
        if (!prefab) {
            Debug.LogWarning($"Unit '{args[0]}' was not found.");
            return;
        }

        float x = float.Parse(args[1]);
        float y = float.Parse(args[2]);

        // Instantiate the unit at the given position
        Object.Instantiate(prefab, new Vector2(Mathf.Floor(x) + .5f, Mathf.Floor(y) + .5f), 
            Quaternion.identity);
    }
}

public class MoveCommand : Command
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

public class KillCommand : Command
{
    public override void Execute(string[] args)
    {
        if (args.Length != 1)
        {
            Debug.Log($"Usage: kill 'unit name' \nargs[{args.Length}]");
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