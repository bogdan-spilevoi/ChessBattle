using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public sealed class CommandConverter : JsonConverter<CommandBase>
{
    // Map enum discriminator -> concrete command instance factory
    private static readonly IReadOnlyDictionary<CommandType, Func<CommandBase>> Factory =
        new Dictionary<CommandType, Func<CommandBase>>
        {
            { CommandType.Move,              () => new MoveCommand() },
            { CommandType.BattleUseMove,     () => new UseMoveCommand() },
            { CommandType.BattleSwitchPiece, () => new SwitchPieceCommand() },
            { CommandType.BattleUsePotion,   () => new UsePotionCommand() },
            { CommandType.BattleFlee,        () => new FleeBattleCommand() },

            // Add the rest when you implement them:
            // { CommandType.StartBattle,     () => new StartBattleCommand() },
            // { CommandType.BattleEnd,       () => new EndBattleCommand() },
        };

    public override CommandBase ReadJson(
        JsonReader reader,
        Type objectType,
        CommandBase existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var jo = JObject.Load(reader);

        if (!jo.TryGetValue(nameof(CommandBase.Type), out var typeToken))
            throw new JsonSerializationException($"Missing '{nameof(CommandBase.Type)}' field in command JSON.");

        // Works whether Type is written as string ("Move") or int (0)
        var type = typeToken.ToObject<CommandType>(serializer);

        if (!Factory.TryGetValue(type, out var ctor))
            throw new JsonSerializationException($"Unknown command Type '{type}'.");

        var cmd = ctor();

        // Populate all fields (including Side, PieceInd, etc.)
        serializer.Populate(jo.CreateReader(), cmd);

        // Ensure discriminator is consistent
        cmd.Type = type;

        return cmd;
    }

    public override void WriteJson(JsonWriter writer, CommandBase value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        // Avoid infinite recursion: use a serializer that does NOT include this converter
        var safeSerializer = JsonSerializer.CreateDefault();
        safeSerializer.ContractResolver = serializer.ContractResolver;

        foreach (var conv in serializer.Converters)
            if (conv is not CommandConverter)
                safeSerializer.Converters.Add(conv);

        // Serialize the object without this converter
        var jo = JObject.FromObject(value, safeSerializer);

        // Force discriminator to exist and match the enum
        jo[nameof(CommandBase.Type)] = JToken.FromObject(value.Type, safeSerializer);

        jo.WriteTo(writer);
    }
}