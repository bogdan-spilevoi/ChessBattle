using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    private readonly JsonSerializerSettings settings = new()
    {
        Converters = new List<JsonConverter> { new StringEnumConverter() }
    };

    public List<CommandBase> commands = new();
    [TextArea(5, 10)]
    public List<string> serializedCommands = new();

    public void AddCommandLocal(CommandBase command)
    {
        commands.Add(command);
        serializedCommands.Add(JsonConvert.SerializeObject(command, settings));
        command.Execute();

        if(!ChessManager.Local)
        {
            Ref.OnlineManager.SendCommand(command);
        }
    }

    public void AddCommandExternal(CommandBase command)
    {
        commands.Add(command);
        serializedCommands.Add(JsonConvert.SerializeObject(command, settings));
        command.Execute();
    }
}
