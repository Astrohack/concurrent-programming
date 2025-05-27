using System;
using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity)
    {
      Position = initialPosition;
      Velocity = initialVelocity;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity
    {
      get
      {
        lock (_lock)
        {
          return _velocity;
        }
      }
      set
      {
        lock (_lock)
        {
          _velocity = value;
        }
      }
    }

    public IVector Position
    {
      get
      {
        lock (_lock)
        {
          return _position;
        }
      }
      set
      {
        lock (_lock)
        {
          _position = value;
        }
      }
    }

    public double Radius
    {
      get => _radius; 
      init
      {
        Mass = value * value * Math.PI;
        _radius = value;
      }
    }

    public double Id { get; init; }

    public double Mass { get; init; }
    #endregion IBall

    #region private

    private IVector _velocity;
    private IVector _position;
    private readonly double _radius;

    private bool _isBallMoverRunning = false;
    private readonly object _lock = new object();

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

    public void StartMoving()
    {
      if (_isBallMoverRunning) return;
      Thread thread = new Thread(Move);
      thread.IsBackground = true;
      _isBallMoverRunning = true;
      thread.Start();
    }

    private void Move()
    {
      Stopwatch stopwatch = new();
      stopwatch.Start();
      float startingTime = 0f;
      float frameDuration = 1f / 60f;
      var _loggeer = DiagnosticsLogger.GetInstance();

      while (_isBallMoverRunning)
      {
        float currentTime = stopwatch.ElapsedMilliseconds;
        float delta = currentTime - startingTime;

        if (delta >= frameDuration)
        {
          var vel = Velocity;
          Position = new Vector(Position.x + vel.x * delta, Position.y + vel.y * delta);
          RaiseNewPositionChangeNotification();
          _loggeer.Log(this);
          startingTime = currentTime;
          Thread.Sleep(TimeSpan.FromSeconds(frameDuration/10));
        }
      }
    }

    public void SetVelocity(double x, double y)
    {
      Velocity = new Vector(x, y);
    }
    #endregion private
  }
}