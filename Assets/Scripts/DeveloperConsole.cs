using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeveloperConsole : MonoBehaviour
{
    // Flag to track whether the console is open or closed
    private bool isOpen = false;

    // The input field where users can type their commands
    [SerializeField]
    private TextMeshProUGUI inputField;
    private string inputText => inputField.text;

    // The list of available commands
    private Dictionary<string, Command> commands = new Dictionary<string, Command>();


    void Start()
    {
        // Add some sample commands to the list of available commands
        commands.Add("spawn", new SpawnCommand());
        commands.Add("kill", new KillCommand());
    }

    void Update()
    {
        // Toggle the console when the '~' key is pressed
        if (Input.GetKeyDown(KeyCode.BackQuote)) 
            isOpen = !isOpen;

        if (Input.GetKeyDown(KeyCode.Return) && isOpen)
            CallCommand();
    }

    public void CallCommand()
    {
        if (!isOpen) return;

        // Split the input into words, separated by spaces
        string[] words = inputText.Split(' ');

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
            Debug.Log("Usage: spawn x y z");
            return;
        }

        float x = float.Parse(args[0]);
        float y = float.Parse(args[1]);
        float z = float.Parse(args[2]);

        Vector3 position = new Vector3(x, y, z);

        // Load the prefab from Resources
        GameObject prefab = Resources.Load<GameObject>("PrefabName");

        // Instantiate the prefab at the given position
        Object.Instantiate(prefab, position, Quaternion.identity);
    }
}

public class KillCommand : Command
{
    public override void Execute(string[] args)
    {
        if (args.Length != 1)
        {
            Debug.Log("Usage: kill 'unit name'");
            return;
        }

        GameObject obj = GameObject.Find(args[0]);
        if (!obj) {
            Debug.LogWarning($"Object of name {args[0]} was found.");
            return;
        }

        Object.Destroy(obj);
    }
}