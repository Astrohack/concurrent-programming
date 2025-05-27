using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
  internal interface ILogger
  {
    void Log(IBall ball);
    void Stop();
  }
  internal class DiagnosticsLogger : ILogger
  {
    private readonly BlockingCollection<string> _logQueue = new(1024);
    private readonly string _filePath;
    private readonly Thread _workerThread;
    private volatile bool _isRunning = true;

    private static readonly object _singletonLock = new();
    private static DiagnosticsLogger? _instance;
    public static DiagnosticsLogger GetInstance(string? filePath = null)
    {
      lock (_singletonLock)
      {
        _instance ??= new DiagnosticsLogger(filePath);
        return _instance;
      }
    }

    public DiagnosticsLogger(string? filePath = null)
    {
      _filePath = filePath ?? "logs.txt";
      _workerThread = new Thread(WriteLoop);
      _workerThread.Start();
    }

    public void Log(IBall ball)
    {
      if (!_isRunning) return;
      var pos = ball.Position;
      var vel = ball.Velocity;
      var id = ball.Id;

      string line = $"{DateTime.Now:HH:mm:ss.ffffff};ID={id};X={pos.x:F3};Y={pos.y:F3};VX={vel.x:F3};VY={vel.y:F3}";
      _logQueue.TryAdd(line);
    }

    private void WriteLoop()
    {
      using (var writer = new StreamWriter(_filePath, append: true, Encoding.ASCII))
      {
        foreach (var line in _logQueue.GetConsumingEnumerable())
        {
          if (!_isRunning) break;
          writer.WriteLine(line);
          writer.Flush();
        }
      }
    }

    public void Stop()
    {
      _isRunning = false;
      _logQueue.CompleteAdding();
      _instance = null;
    }
  }
}
