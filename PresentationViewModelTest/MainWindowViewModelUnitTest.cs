using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using TP.ConcurrentProgramming.Presentation.Model;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel.Test
{
  [TestClass]
  public class MainWindowViewModelUnitTest
  {
    [TestMethod]
    public void ConstructorTest()
    {
      ModelNullFixture nullModelFixture = new();
      Assert.AreEqual<int>(0, nullModelFixture.Disposed);
      Assert.AreEqual<int>(0, nullModelFixture.Started);
      Assert.AreEqual<int>(0, nullModelFixture.Subscribed);
      using (MainWindowViewModel viewModel = new(nullModelFixture))
      {
        Random random = new Random();
        viewModel.Start(100, 100);
        Assert.IsNotNull(viewModel.Balls);
        Assert.AreEqual<int>(0, nullModelFixture.Disposed);
        Assert.AreEqual<int>(1, nullModelFixture.Subscribed);
      }
      Assert.AreEqual<int>(1, nullModelFixture.Disposed);
    }

    [TestMethod]
    public void BehaviorTestMethod()
    {
      ModelSimulatorFixture modelSimulator = new();
      MainWindowViewModel viewModel = new(modelSimulator);
      Assert.IsNotNull(viewModel.Balls);
      Assert.AreEqual<int>(0, viewModel.Balls.Count);
      Random random = new Random();
      viewModel.Start(100, 100);
      viewModel.Dispose();
      Assert.IsTrue(modelSimulator.Disposed);
      Assert.AreEqual<int>(0, viewModel.Balls.Count);
    }

    private class FakeModelApi : ModelAbstractApi
    {
      public bool Started { get; private set; }
      public int StartBalls { get; private set; }
      public double StartWidth { get; private set; }
      public double StartHeight { get; private set; }
      public int SubscribeCount { get; private set; }

      public override void Start(int balls, double width, double height)
      {
        Started = true;
        StartBalls = balls;
        StartWidth = width;
        StartHeight = height;
      }

      public override void Dispose() { }

      public override IDisposable Subscribe(IObserver<ModelIBall> observer)
      {
        SubscribeCount++;
        return new DummyDisposable();
      }

      private class DummyDisposable : IDisposable
      {
        public void Dispose() { }
      }
    }

    [TestMethod]
    public void Constructor_InitializesCommandsAndProperties()
    {
      var api = new FakeModelApi();
      var vm = new MainWindowViewModel(api);

      // Initial label
      Assert.AreEqual("15", vm.BallsQunatityLabel);
      // Commands should be available
      Assert.IsTrue(vm.IncreaseBallsQuantity.CanExecute(null));
      Assert.IsTrue(vm.DecreaseBallsQuantity.CanExecute(null));
      Assert.IsTrue(vm.StartSimulation.CanExecute(null));
      // Subscribe should have been called once
      Assert.AreEqual(1, api.SubscribeCount);
    }

    [TestMethod]
    public void IncreaseBallsQuantity_ExecutesAndUpdatesLabelAndCanExecute()
    {
      var api = new FakeModelApi();
      var vm = new MainWindowViewModel(api);

      // Increase up to limit
      for (int i = 0; i < 5; i++)
      {
        Assert.IsTrue(vm.IncreaseBallsQuantity.CanExecute(null));
        vm.IncreaseBallsQuantity.Execute(null);
      }
      Assert.AreEqual("20", vm.BallsQunatityLabel);
      // Now at max, cannot increase
      Assert.IsFalse(vm.IncreaseBallsQuantity.CanExecute(null));
      Assert.IsTrue(vm.DecreaseBallsQuantity.CanExecute(null));
    }

    [TestMethod]
    public void DecreaseBallsQuantity_ExecutesAndUpdatesLabelAndCanExecute()
    {
      var api = new FakeModelApi();
      var vm = new MainWindowViewModel(api);

      // Decrease down to zero
      for (int i = 0; i < 15; i++)
      {
        Assert.IsTrue(vm.DecreaseBallsQuantity.CanExecute(null));
        vm.DecreaseBallsQuantity.Execute(null);
      }
      Assert.AreEqual("0", vm.BallsQunatityLabel);
      // Now at min, cannot decrease
      Assert.IsFalse(vm.DecreaseBallsQuantity.CanExecute(null));
      Assert.IsTrue(vm.IncreaseBallsQuantity.CanExecute(null));
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void Start_AfterDispose_ThrowsObjectDisposedException()
    {
      var api = new FakeModelApi();
      var vm = new MainWindowViewModel(api);
      vm.Dispose();
      vm.Start(10, 10);
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void Dispose_Twice_ThrowsObjectDisposedException()
    {
      var api = new FakeModelApi();
      var vm = new MainWindowViewModel(api);
      vm.Dispose();
      vm.Dispose();
    }

    #region testing infrastructure

    private class ModelNullFixture : ModelAbstractApi
    {
      #region Test

      internal int Disposed = 0;
      internal int Started = 0;
      internal int Subscribed = 0;

      #endregion Test

      #region ModelAbstractApi

      public override void Dispose()
      {
        Disposed++;
      }

      public override void Start(int numberOfBalls, double canvasWidth, double canvasHeight)
      {
        Started = numberOfBalls;
      }

      public override IDisposable Subscribe(IObserver<ModelIBall> observer)
      {
        Subscribed++;
        return new NullDisposable();
      }

      #endregion ModelAbstractApi

      #region private

      private class NullDisposable : IDisposable
      {
        public void Dispose()
        { }
      }

      #endregion private
    }

    private class ModelSimulatorFixture : ModelAbstractApi
    {
      #region Testing indicators

      internal bool Disposed = false;

      #endregion Testing indicators

      #region ctor

      public ModelSimulatorFixture()
      {
        eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(this, "BallChanged");
      }

      #endregion ctor

      #region ModelAbstractApi fixture

      public override IDisposable? Subscribe(IObserver<ModelIBall> observer)
      {
        return eventObservable?.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
      }

      public override void Start(int numberOfBalls, double canvasWidth, double canvasHeight)
      {
        for (int i = 0; i < numberOfBalls; i++)
        {
          ModelBall newBall = new ModelBall(0, 0) { };
          BallChanged?.Invoke(this, new BallChaneEventArgs() { Ball = newBall });
        }
      }

      public override void Dispose()
      {
        Disposed = true;
      }

      #endregion ModelAbstractApi

      #region API

      public event EventHandler<BallChaneEventArgs> BallChanged;

      #endregion API

      #region private

      private IObservable<EventPattern<BallChaneEventArgs>>? eventObservable = null;

      private class ModelBall : ModelIBall
      {
        public ModelBall(double top, double left)
        { }

        #region IBall

        public double Diameter => throw new NotImplementedException();

        public double Top => throw new NotImplementedException();

        public double Left => throw new NotImplementedException();

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion INotifyPropertyChanged

        #endregion IBall
      }

      #endregion private
    }

    #endregion testing infrastructure
  }
}