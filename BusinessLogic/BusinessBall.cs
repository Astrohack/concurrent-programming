using System;
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

    #endregion IBall

    #region private

    private Data.IBall ball;

    public double Radius => ball.Radius;

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
      double radius = Radius;
      var board = BusinessLogicAbstractAPI.GetDimensions;
      if (e.x + radius > board.TableWidth)
      {
        ball.SetPosition(new Position(board.TableWidth - radius, e.y));
      }
      else if (e.x < radius)
      {
        ball.SetPosition(new Position(radius, e.y));
      }
      else if (e.y + radius > board.TableHeight)
      {
        ball.SetPosition(new Position(e.x, board.TableHeight - radius));
      }
      else if (e.y < radius)
      {
         ball.SetPosition(new Position(e.x, radius));
      }
      else 
      {
        NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
      }
    }

    #endregion private
  }
}