namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
  public sealed class BallUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      Vector testinVector = new Vector(0.0, 0.0);
      Ball newInstance = new(testinVector, testinVector);
      Assert.AreEqual<IVector>(testinVector, newInstance.Position);
      Assert.AreEqual<IVector>(testinVector, newInstance.Velocity);
    }

    [TestMethod]
    public async Task MoveTestMethod()
    {
      Vector initialPosition = new(10.0, 10.0);
      Ball newInstance = new(initialPosition, new Vector(0.0, 0.0));
      IVector curentPosition = new Vector(0.0, 0.0);
      int numberOfCallBackCalled = 0;
      newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); curentPosition = position; numberOfCallBackCalled++; };
      newInstance.StartMoving();
      await Task.Delay(TimeSpan.FromMilliseconds(100));
      lock (newInstance.AcquireLock())
      {
        Assert.IsTrue(numberOfCallBackCalled > 0);
        Assert.AreEqual<IVector>(initialPosition, curentPosition);
      }
    }
  }
}