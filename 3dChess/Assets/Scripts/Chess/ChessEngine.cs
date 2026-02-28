using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class ChessEngine : MonoBehaviour, IDisposable
{
    private Process engine;
    private StreamWriter input;
    private StreamReader output;

    public void StartEngine()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "stockfish.exe");

        engine = new Process();
        engine.StartInfo.FileName = path;
        engine.StartInfo.UseShellExecute = false;
        engine.StartInfo.RedirectStandardInput = true;
        engine.StartInfo.RedirectStandardOutput = true;
        engine.StartInfo.CreateNoWindow = true;

        engine.Start();

        input = engine.StandardInput;
        output = engine.StandardOutput;

        Send("uci");
        Send("isready");
    }

    public async Task<string> GetBestMove(string fen, int moveTimeMs)
    {
        if (engine == null || engine.HasExited)
            throw new InvalidOperationException("Stockfish process is not running.");
        if (input == null || output == null)
            throw new InvalidOperationException("Engine not started (input/output null).");

        var fields = fen.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (fields.Length != 6)
            throw new ArgumentException($"Malformed FEN (expected 6 fields, got {fields.Length}): {fen}");

        Send("stop");
        Send("isready");
        await ReadUntilAsync("readyok", timeoutMs: 2000);

        Send($"position fen {fen}");
        Send($"go movetime {moveTimeMs}");

        var deadline = Environment.TickCount + moveTimeMs + 3000;

        while (Environment.TickCount < deadline)
        {
            string line = await output.ReadLineAsync();
            if (line == null)
            {
                UnityEngine.Debug.LogError($"[SF] stdout closed. HasExited={engine?.HasExited}, ExitCode={(engine != null && engine.HasExited ? engine.ExitCode.ToString() : "n/a")}");
                throw new InvalidOperationException("Stockfish stdout closed (engine exited).");
            }

            if (line.StartsWith("bestmove ", StringComparison.Ordinal))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return parts.Length >= 2 ? parts[1] : null;
            }
        }

        throw new TimeoutException("Timed out waiting for bestmove.");
    }

    private async Task ReadUntilAsync(string prefix, int timeoutMs)
    {
        var deadline = Environment.TickCount + timeoutMs;
        while (Environment.TickCount < deadline)
        {
            var line = await output.ReadLineAsync();
            if (line == null)
                throw new InvalidOperationException("Stockfish stdout closed during ReadUntil.");
            if (line.StartsWith(prefix, StringComparison.Ordinal))
                return;
        }
        throw new TimeoutException($"Timed out waiting for {prefix}");
    }

    private void Send(string command)
    {
        input.WriteLine(command);
        input.Flush();
    }

    public void Dispose()
    {
        if (engine != null && !engine.HasExited)
        {
            Send("quit");
            engine.WaitForExit(500);
            engine.Kill();
        }

        engine?.Dispose();
    }
}
