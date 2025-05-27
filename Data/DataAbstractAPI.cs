using System.Collections.ObjectModel;

namespace TP.ConcurrentProgramming.Data
{
  public abstract class DataAbstractAPI : IDisposable
  {
    #region Layer Factory

    public static DataAbstractAPI GetDataLayer()
    {
      return modelInstance.Value;
    }

    #endregion Layer Factory

    #region public API

    public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);
    
    #endregion public API

    #region IDisposable

    public abstract void Dispose();

    #endregion IDisposable

    #region private

    private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

    #endregion private
  }

  public interface IVector
  {
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    double x { get; init; }

    /// <summary>
    /// The y component of the vector.
    /// </summary>
    double y { get; init; }

    public static IVector Right = new Vector(1,0);
    public static IVector Left = new Vector(-1,0);
    public static IVector Up = new Vector(0,1);
    public static IVector Down = new Vector(0,-1);

    public static IVector operator +(IVector a, IVector b) =>
        new Vector(a.x + b.x, a.y + b.y);

    public static IVector operator -(IVector a, IVector b) =>
        new Vector(a.x - b.x, a.y - b.y);

    public static IVector operator *(IVector v, double scalar) =>
        new Vector(v.x * scalar, v.y * scalar);

    public static IVector operator /(IVector v, double scalar) =>
     new Vector(v.x / scalar, v.y / scalar);

    public static double Dot(IVector a, IVector b) =>
        a.x * b.x + a.y * b.y;

    public IVector Normalize();

    public double Magnitude();

    public double MagnitudeSquared();
  }

  public interface IBall
  {
    event EventHandler<IVector> NewPositionNotification;

    void StartMoving();

    double Radius { get; }
    double Mass { get; }
    double Id { get; }
    IVector Position { get; }

    IVector Velocity { get; }

    void SetVelocity(double x, double y);
  }
}