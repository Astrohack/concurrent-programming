using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using TP.ConcurrentProgramming.BusinessLogic;
using UnderneathLayerAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.Presentation.Model
{
  /// <summary>
  /// Class Model - implements the <see cref="ModelAbstractApi" />
  /// </summary>
  internal class ModelImplementation : ModelAbstractApi
  {
    internal ModelImplementation() : this(null)
    { }

    internal ModelImplementation(UnderneathLayerAPI underneathLayer)
    {
      layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetBusinessLogicLayer() : underneathLayer;
      eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(this, "BallChanged");
    }

    #region ModelAbstractApi

    public override void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(Model));
      layerBellow.Dispose();
      Disposed = true;
    }

    public override IDisposable Subscribe(IObserver<IBall> observer)
    {
      return eventObservable.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
    }

    private double tableWidth { get; set; }
    private double tableHeight { get; set; }

    public override void Start(int numberOfBalls, double canvasWidth, double canvasHeight)
    {
      tableHeight = canvasHeight;
      tableWidth = canvasWidth;
      layerBellow.Start(numberOfBalls, StartHandler);
    }

    #endregion ModelAbstractApi

    #region API

    public event EventHandler<BallChaneEventArgs> BallChanged;

    #endregion API

    #region private

    private bool Disposed = false;
   
    private readonly IObservable<EventPattern<BallChaneEventArgs>> eventObservable = null;
    private readonly UnderneathLayerAPI layerBellow = null;

    private void StartHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball)
    {
      double diameter = tableHeight / UnderneathLayerAPI.GetDimensions.TableHeight * ball.Radius * 2;
      double scale = (tableWidth - 2 * 5) / UnderneathLayerAPI.GetDimensions.TableHeight;
      ModelBall newBall = new ModelBall(position.x, position.y, ball) { Diameter = diameter };
      BallChanged.Invoke(this, new BallChaneEventArgs() { Ball = newBall });
    }

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    [Conditional("DEBUG")]
    internal void CheckUnderneathLayerAPI(Action<UnderneathLayerAPI> returnNumberOfBalls)
    {
      returnNumberOfBalls(layerBellow);
    }

    [Conditional("DEBUG")]
    internal void CheckBallChangedEvent(Action<bool> returnBallChangedIsNull)
    {
      returnBallChangedIsNull(BallChanged == null);
    }

    #endregion TestingInfrastructure
  }

  public class BallChaneEventArgs : EventArgs
  {
    public IBall Ball { get; init; }
  }
}