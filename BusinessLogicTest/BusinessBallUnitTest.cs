using System.Numerics;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void MoveTestMethod()
    {
      DataBallFixture dataBallFixture = new DataBallFixture();
      Ball newInstance = new(dataBallFixture);
      double ballRadius = 1;
      int numberOfCallBackCalled = 0;
      newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
      newPositionVector = new VectorFixture(ballRadius, ballRadius);
      dataBallFixture.Move();
      Assert.AreEqual<int>(1, numberOfCallBackCalled);

      numberOfCallBackCalled = 0;
      newPositionVector = new VectorFixture(0, 0);
      dataBallFixture.Move();
      Assert.AreEqual<int>(1, numberOfCallBackCalled);
      Assert.AreEqual<double>(dataBallFixture.Position.y, ballRadius);
      Assert.AreEqual<double>(dataBallFixture.Position.x, ballRadius);
    }

    private static VectorFixture newPositionVector;

    #region testing instrumentation

    private class DataBallFixture : Data.IBall
    {
      public Data.IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

      public double Radius => 1;

      public Data.IVector Position { get; private set; }

      public event EventHandler<Data.IVector>? NewPositionNotification;

      public void SetPosition(Data.IVector pos)
      {
        Position = new VectorFixture(pos.x, pos.y);
        NewPositionNotification?.Invoke(this, pos);
      }

      internal void Move()
      {
        NewPositionNotification?.Invoke(this, newPositionVector);
      }
    }

    private class VectorFixture : Data.IVector
    {
      internal VectorFixture(double X, double Y)
      {
        x = X; y = Y;
      }

      public double x { get; init; }
      public double y { get; init; }
    }

    #endregion testing instrumentation
  }
}