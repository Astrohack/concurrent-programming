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

    public IVector Velocity { get; set; }

    public IVector Position { get; private set; }

    public double Radius { get; } = 0.5;

    #endregion IBall

    #region private

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

    internal void Move(Vector delta)
    {
      Position = new Vector(Position.x + delta.x, Position.y + delta.y);

      RaiseNewPositionChangeNotification();
    }

    public void SetPosition(IVector pos)
    {
      Position = new Vector(pos.x, pos.y);
      RaiseNewPositionChangeNotification();
    }

    #endregion private
  }
}