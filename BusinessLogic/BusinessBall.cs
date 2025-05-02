using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;
using Newtonsoft.Json.Linq;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class Ball : IBall
  {
    public Ball(Data.IBall ball, GetBallsFn getBalls)
    {
      ball.NewPositionNotification += RaisePositionChangeEvent;
      this.ball = ball;
      this.getBalls = getBalls;
    }

    public delegate ReadOnlyCollection<Data.IBall> GetBallsFn();

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    #endregion IBall

    #region private

    private Data.IBall ball;

    public double Radius => ball.Radius;

    private static readonly object _ballLock = new object();

    private GetBallsFn getBalls;

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
      lock (_ballLock) {
        var balls = getBalls();
        foreach (Data.IBall other in balls)
        {
          if (other == ball) continue;
          lock (other.AcquireLock()) {
            CalculateBallCollisions(ball, other);
          }
        }
        CalculateWallCollisions(ball);
      }
      NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

    private void CalculateBallCollisions(Data.IBall a, Data.IBall b)
    {
      var velA = a.Velocity;
      var velB = b.Velocity;

      var delta = a.Position - b.Position;
      var distance = delta.Magnitude();

      if (distance == 0 || distance > a.Radius + b.Radius) return;

      var normal = delta.Normalize();
      var relativeVelocity = velA - velB;

      double velocityAlongNormal = IVector.Dot(relativeVelocity, normal);
      if (velocityAlongNormal > 0) return;

      double m1 = a.Mass;
      double m2 = b.Mass;

      double impulse = (2 * velocityAlongNormal) / (m1 + m2);

      var impulseVectorA = normal * (-impulse * m2);
      var impulseVectorB = normal * (impulse * m1);

      a.Velocity = velA + impulseVectorA;
      b.Velocity = velB + impulseVectorB;
      return;
    }

    private void CalculateWallCollisions(Data.IBall ball)
    {
      double radius = ball.Radius;
      var board = BusinessLogicAbstractAPI.GetDimensions;
      Data.IVector pos = ball.Position;
      Data.IVector vel = ball.Velocity;
      if (pos.x + radius > board.TableWidth && IVector.Dot(vel, IVector.Right) > 0)
      {
        ball.Velocity = new Position(-vel.x, vel.y);
      }
      else if (pos.x < radius && IVector.Dot(vel, IVector.Left) > 0)
      {
        ball.Velocity = new Position(-vel.x, vel.y);
      }
      if (pos.y + radius > board.TableHeight && IVector.Dot(vel, IVector.Up) > 0)
      {
        ball.Velocity = new Position(vel.x, -vel.y);
      }
      else if (pos.y < radius && IVector.Dot(vel, IVector.Down) > 0)
      {
        ball.Velocity = new Position(vel.x, -vel.y);
      }
    }

    #endregion private
  }
}