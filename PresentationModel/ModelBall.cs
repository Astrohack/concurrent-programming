using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TP.ConcurrentProgramming.BusinessLogic;
using TP.ConcurrentProgramming.Data;
using LogicIBall = TP.ConcurrentProgramming.BusinessLogic.IBall;
using BusinessLogicAbstractAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.Presentation.Model
{
  internal class ModelBall : IBall
  {
    public ModelBall(double top, double left, LogicIBall underneathBall)
    {
      TopBackingField = top;
      LeftBackingField = left;
      underneathBall.NewPositionNotification += NewPositionNotification;
      logicBall = underneathBall;
    }

    #region IBall

    public double Top
    {
      get { return TopBackingField; }
      private set
      {
        if (TopBackingField == value)
          return;
        TopBackingField = value;
        RaisePropertyChanged();
      }
    }

    public double Left
    {
      get { return LeftBackingField; }
      private set
      {
        if (LeftBackingField == value)
          return;
        LeftBackingField = value;
        RaisePropertyChanged();
      }
    }

    public double Diameter { get; init; } = 0;

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion INotifyPropertyChanged

    #endregion IBall

    #region private

    private double TopBackingField;
    private double LeftBackingField;
    private LogicIBall logicBall;

    private void NewPositionNotification(object sender, IPosition e)
    {
      double radius = Diameter / 2;
      double scale = radius / logicBall.Radius;
      Top = e.y * scale - radius;
      Left = e.x * scale - radius;
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion private

    #region testing instrumentation

    [Conditional("DEBUG")]
    internal void SetLeft(double x)
    { Left = x; }

    [Conditional("DEBUG")]
    internal void SettTop(double x)
    { Top = x; }

    #endregion testing instrumentation
  }
}