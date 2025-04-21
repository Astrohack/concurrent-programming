using System.Numerics;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BallUnitTest
  {
    private class TestVector : IVector
    {
      public double x { get; init; }
      public double y { get; init; }

      public TestVector(double x, double y)
      {
        this.x = x;
        this.y = y;
      }

      public double MagnitudeSquared() => x * x + y * y;
      public double Magnitude() => Math.Sqrt(MagnitudeSquared());

      public IVector Normalize()
      {
        double mag = Magnitude();
        return mag == 0 ? new TestVector(0, 0) : new TestVector(x / mag, y / mag);
      }

      public static double Dot(IVector a, IVector b) => a.x * b.x + a.y * b.y;

      public static IVector Right => new TestVector(1, 0);
      public static IVector Left => new TestVector(-1, 0);
      public static IVector Up => new TestVector(0, 1);
      public static IVector Down => new TestVector(0, -1);
      public static TestVector operator +(TestVector a, TestVector b) => new TestVector(a.x + b.x, a.y + b.y);
      public static TestVector operator -(TestVector a, TestVector b) => new TestVector(a.x - b.x, a.y - b.y);
      public static TestVector operator *(TestVector a, double scalar) => new TestVector(a.x * scalar, a.y * scalar);
      public static TestVector operator *(double scalar, TestVector a) => a * scalar;
    }
    private class StubDataBall : Data.IBall
    {
      public event EventHandler<Data.IVector>? NewPositionNotification;

      public Data.IVector Velocity { get; set; }
      public Data.IVector Position { get; set; }
      public double Radius { get; init; }
      public double Mass { get; init; }

      private readonly object _lock = new object();

      public StubDataBall(double radius, TestVector position, TestVector velocity)
      {
        Radius = radius;
        Mass = radius * radius;
        Position = position;
        Velocity = velocity;
      }

      public object AcquireLock() => _lock;

      public void RaisePositionChange()
      {
        NewPositionNotification?.Invoke(this, Position);
      }

      public void StartMoving()
      {
        throw new NotImplementedException();
      }
    }

    private List<Data.IBall> dataBalls;
    private List<IBall> logicBalls;

    [TestInitialize]
    public void SetUp()
    {
      dataBalls = new List<Data.IBall>();
      logicBalls = new List<IBall>();
    }
    private BusinessLogic.Ball CreateLogicBall(StubDataBall stub)
    {
      var logic = new BusinessLogic.Ball(stub, () => dataBalls);
      dataBalls.Add(stub);
      logicBalls.Add(logic);
      return logic;
    }

    [TestMethod]
    public void NoCollision_WhenBallsFarApart_VelocitiesUnchanged()
    {
      var stubA = new StubDataBall(1.0, new TestVector(0, 0), new TestVector(1, 0));
      var stubB = new StubDataBall(1.0, new TestVector(5, 0), new TestVector(-1, 0));
      CreateLogicBall(stubA);
      CreateLogicBall(stubB);

      stubA.RaisePositionChange();

      Assert.AreEqual(1, stubA.Velocity.x);
      Assert.AreEqual(0, stubA.Velocity.y);
      Assert.AreEqual(-1, stubB.Velocity.x);
      Assert.AreEqual(0, stubB.Velocity.y);
    }

    [TestMethod]
    public void ElasticCollision_HeadOnEqualMass_SwapsVelocities()
    {
      var stubA = new StubDataBall(1.0, new TestVector(2, 0), new TestVector(1, 0));
      var stubB = new StubDataBall(1.0, new TestVector(4, 0), new TestVector(-1, 0));
      CreateLogicBall(stubA);
      CreateLogicBall(stubB);

      stubA.RaisePositionChange();

      Assert.AreEqual(-1, stubA.Velocity.x);
      Assert.AreEqual(0, stubA.Velocity.y);
      Assert.AreEqual(1, stubB.Velocity.x);
      Assert.AreEqual(0, stubB.Velocity.y);
    }

    [TestMethod]
    public void WallCollision_RightWall_ReversesXVelocity()
    {
      var stub = new StubDataBall(1.0, new TestVector(BusinessLogicAbstractAPI.GetDimensions.TableWidth - 0.5, BusinessLogicAbstractAPI.GetDimensions.TableHeight /2), new TestVector(1.0, 0.0));
      CreateLogicBall(stub);

      stub.RaisePositionChange();

      Assert.AreEqual(-1.0, stub.Velocity.x);
      Assert.AreEqual(0.0, stub.Velocity.y);
    }
  }
}