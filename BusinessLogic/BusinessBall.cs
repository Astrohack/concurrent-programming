using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class Ball : IBall
  {
    public Ball(Data.IBall ball)
    {
      ball.NewPositionNotification += RaisePositionChangeEvent;
      this.ball = ball;
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    internal Data.IBall ball;

    #endregion IBall

    #region private

    public double Radius => ball.Radius;

    private static readonly object _ballLock = new object();

    private void RaisePositionChangeEvent(object? sender, IVector e)
    {
      lock (_ballLock) {
        foreach (Ball other in BusinessLogicImplementation.BallsList)
        {
          if (other == this) continue;
          CalculateBallCollisions(ball, other.ball);
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

      var aVel = velA + impulseVectorA;
      var bVel = velB + impulseVectorB;

      a.SetVelocity(aVel.x, aVel.y);
      b.SetVelocity(bVel.x, bVel.y);
      return;
    }

    private void CalculateWallCollisions(Data.IBall ball)
    {
      double radius = ball.Radius;
      var board = BusinessLogicAbstractAPI.GetDimensions;
      var pos = ball.Position;
      var vel = ball.Velocity;
      if (pos.x + radius > board.TableWidth && IVector.Dot(vel, IVector.Right) > 0)
      {
        ball.SetVelocity(-vel.x, vel.y);
      }
      else if (pos.x < radius && IVector.Dot(vel, IVector.Left) > 0)
      {
        ball.SetVelocity(-vel.x, vel.y);
      }
      if (pos.y + radius > board.TableHeight && IVector.Dot(vel, IVector.Up) > 0)
      {
        ball.SetVelocity(vel.x, -vel.y);
      }
      else if (pos.y < radius && IVector.Dot(vel, IVector.Down) > 0)
      {
        ball.SetVelocity(vel.x, -vel.y);
      }
    }

    #endregion private
  }
}