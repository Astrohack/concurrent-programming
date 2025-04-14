using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.Presentation.Model.Test
{
  [TestClass]
  public class ModelBallUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      ModelBall ball = new ModelBall(0.0, 0.0, new BusinessLogicIBallFixture());
      Assert.AreEqual<double>(0.0, ball.Top);
      Assert.AreEqual<double>(0.0, ball.Top);
    }

    [TestMethod]
    public void PositionChangeNotificationTestMethod()
    {
      int notificationCounter = 0;
      var logicLayer = new BusinessLogicIBallFixture();
      ModelBall ball = new ModelBall(0, 0.0, logicLayer) { Diameter = 8 };
      ball.PropertyChanged += (sender, args) => notificationCounter++;
      Assert.AreEqual(0, notificationCounter);
      ball.SetLeft(2.0);
      Assert.AreEqual<int>(1, notificationCounter);
      Assert.AreEqual<double>(2.0, ball.Left);
      Assert.AreEqual<double>(0.0, ball.Top);
      ball.SettTop(3.0);
      Assert.AreEqual(2, notificationCounter);
      Assert.AreEqual<double>(2.0, ball.Left);
      Assert.AreEqual<double>(3.0, ball.Top);

      logicLayer.InvokePositionChangeNotification(new PositionFixture(4, 6));

      Assert.AreEqual<double>(20, ball.Top);
      Assert.AreEqual<double>(12, ball.Left);
    }

    #region testing instrumentation

    private class BusinessLogicIBallFixture : BusinessLogic.IBall
    {
      public double Radius => 1;

      public event EventHandler<IPosition>? NewPositionNotification;

      public void InvokePositionChangeNotification(IPosition position) 
      {
        NewPositionNotification.Invoke(this, position);
      }

      public void Dispose()
      {
        throw new NotImplementedException();
      }
    }

    private class PositionFixture : BusinessLogic.IPosition
    {
      internal PositionFixture(double X, double Y)
      {
        x = X; y = Y;
      }

      public double x { get; init; }
      public double y { get; init; }
    }

    #endregion testing instrumentation
  }
}