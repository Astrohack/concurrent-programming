using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor

    public DataImplementation() {}


    #endregion ctor

    #region DataAbstractAPI

    public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));
      Random random = new Random();
      for (int i = 0; i < numberOfBalls; i++)
      {
        Vector startingPosition = new(random.NextDouble() * 19 + 0.5, random.NextDouble() * 19 + 0.5);

        double direction = random.NextDouble() * 2 * Math.PI;
        double speed = 0.005;
        Vector startingVelocity = new(Math.Cos(direction) * speed, Math.Sin(direction) * speed);
        double radius = random.NextDouble() * 0.5 + 0.5;
        Ball newBall = new(startingPosition, startingVelocity) { Radius = radius };
        upperLayerHandler(startingPosition, newBall);
        BallsList.Add(newBall);
      }
      foreach (IBall ball in BallsList)
      {
        ball.StartMoving();
      }
    }
    public override ReadOnlyCollection<IBall> GetBalls()
    {
      return BallsList.AsReadOnly();
    }

    #endregion DataAbstractAPI

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          BallsList.Clear();
        }
        Disposed = true;
      }
      else
        throw new ObjectDisposedException(nameof(DataImplementation));
    }

    public override void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    private bool Disposed = false;

    private List<IBall> BallsList = [];
  
    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
    {
      returnBallsList(BallsList);
    }

    [Conditional("DEBUG")]
    internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
    {
      returnNumberOfBalls(BallsList.Count);
    }

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }


    #endregion TestingInfrastructure
  }
}