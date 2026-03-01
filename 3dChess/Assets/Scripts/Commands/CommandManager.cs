using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    private readonly JsonSerializerSettings settings = new()
    {
        Converters = new List<JsonConverter> { 
            new StringEnumConverter(), 
            new CommandConverter()          
        }
    };

    public List<CommandBase> commands = new();
    [TextArea(5, 10)]
    public List<string> serializedCommands = new();

    public void AddCommandLocal(CommandBase command)
    {
        commands.Add(command);
        var serializedCommand = JsonConvert.SerializeObject(command, settings);
        serializedCommands.Add(serializedCommand);
        command.Execute();

        if(!ChessManager.Local)
        {
            Ref.OnlineManager.SendCommand(serializedCommand);
        }
    }

    public void AddCommandExternal(string serializedCommand)
    {
        var command = JsonConvert.DeserializeObject<CommandBase>(serializedCommand, settings);
        commands.Add(command);
        serializedCommands.Add(serializedCommand);
        command.Execute();
    }

    public void AddCommandExternal(CommandBase command)
    {
        commands.Add(command);
        serializedCommands.Add(JsonConvert.SerializeObject(command, settings));
        command.Execute();
    }
}
