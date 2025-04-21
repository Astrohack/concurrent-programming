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
      _isBallMoverRunning = true;
      thread.Start();
    }

    private async void Move()
    {
      Stopwatch stopwatch = new();
      stopwatch.Start();
      float startingTime = 0f;
      

      while (_isBallMoverRunning)
      {
        float currentTime = stopwatch.ElapsedMilliseconds;
        float delta = currentTime - startingTime;

        if (delta >= 1f / 60f)
        {
          lock(_lock)
          {
            Position = new Vector(Position.x + Velocity.x, Position.y + Velocity.y);
          }
          RaiseNewPositionChangeNotification();
          startingTime = currentTime;
          await Task.Delay(TimeSpan.FromSeconds(1f / 60f));
        }
      }
    }

    public object AcquireLock()
    {
      return _lock;
    }

    #endregion private
  }
}