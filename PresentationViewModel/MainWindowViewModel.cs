using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
  public class MainWindowViewModel : ViewModelBase, IDisposable
  {
    #region ctor

    public MainWindowViewModel() : this(null)
    { }

    internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
    {
      ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
      Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));

      StartSimulation = new RelayCommand(ExecuteStartSimulation, CanStartSimulation);
      IncreaseBallsQuantity = new RelayCommand(ExecuteIncreaseBallsQuantity, CanIncreaseBallsQuantity);
      DecreaseBallsQuantity = new RelayCommand(ExecuteDecreaseBallsQuantity, CanDecreaseBallsQuantity);
    }

    #endregion ctor

    #region public API

    private int _ballsQuantity = 15;
    private bool canStartSimulation = true;
    private double canvasWidth;
    private double canvasHeight;
    public string BallsQunatityLabel
    {
      get => _ballsQuantity.ToString();
      set
      {
        _ballsQuantity = int.Parse(value);
        RaisePropertyChanged();
      }
    }

    public ICommand StartSimulation { get; }
    public ICommand IncreaseBallsQuantity { get; }
    public ICommand DecreaseBallsQuantity { get; }

    public void Start(double canvasWidth, double canvasHeight)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      this.canvasWidth = canvasWidth;
      this.canvasHeight = canvasHeight;
    }


    public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

    #endregion public API

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          Balls.Clear();
          Observer.Dispose();
          ModelLayer.Dispose();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        Disposed = true;
      }
    }

    public void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    private IDisposable Observer = null;
    private ModelAbstractApi ModelLayer;
    private bool Disposed = false;
    private void ExecuteStartSimulation()
    {
      ModelLayer.Start(_ballsQuantity, canvasWidth, canvasHeight);
      Observer.Dispose();
      canStartSimulation = false;
      (StartSimulation as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private bool CanStartSimulation()
    {
      return canStartSimulation;
    }
    private void ExecuteIncreaseBallsQuantity()
    {
      _ballsQuantity++;
      BallsQunatityLabel = _ballsQuantity.ToString();
      (IncreaseBallsQuantity as RelayCommand)?.RaiseCanExecuteChanged();
      (DecreaseBallsQuantity as RelayCommand)?.RaiseCanExecuteChanged();
    }
    private bool CanIncreaseBallsQuantity() => _ballsQuantity < 20;
    private void ExecuteDecreaseBallsQuantity()
    {
      _ballsQuantity--;
      BallsQunatityLabel = _ballsQuantity.ToString();
      (DecreaseBallsQuantity as RelayCommand)?.RaiseCanExecuteChanged();
      (IncreaseBallsQuantity as RelayCommand)?.RaiseCanExecuteChanged();
    }
    private bool CanDecreaseBallsQuantity() => _ballsQuantity > 0;

    #endregion private
  }
}