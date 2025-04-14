//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Windows;
using TP.ConcurrentProgramming.Presentation.ViewModel;

namespace TP.ConcurrentProgramming.PresentationView
{
  /// <summary>
  /// View implementation
  /// </summary>
  public partial class MainWindow : Window
  {

    private int BallsQuantity = 30;

    public MainWindow()
    {
      InitializeComponent();
      double screenWidth = SystemParameters.PrimaryScreenWidth;
      double screenHeight = SystemParameters.PrimaryScreenHeight;
      TableBorder.Height = screenHeight * 0.6;
      TableBorder.Width = TableBorder.Height;
      BallsQuantityTextBlock.Text = BallsQuantity.ToString();
    }

    private void Increment_Click(object sender, RoutedEventArgs e)
    {
      BallsQuantity++;
      BallsQuantityTextBlock.Text = BallsQuantity.ToString();
      DecrementBallsBtn.IsEnabled = true;
    }

    private void Decrement_Click(object sender, RoutedEventArgs e)
    {
      if (BallsQuantity <= 1) return;
      BallsQuantity--;
      BallsQuantityTextBlock.Text = BallsQuantity.ToString();
      if (BallsQuantity == 1)
      {
        DecrementBallsBtn.IsEnabled = false;
      }
    }

    private void GenerateBalls_Click(object sender, RoutedEventArgs e)
    {
      MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
      double width = Canvas.ActualWidth;
      double height = Canvas.ActualHeight;
      viewModel.Start(BallsQuantity, width, height);
      GenerateBallsBtn.IsEnabled = false;
    }

    /// <summary>
    /// Raises the <seealso cref="System.Windows.Window.Closed"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    protected override void OnClosed(EventArgs e)
    {
      if (DataContext is MainWindowViewModel viewModel)
        viewModel.Dispose();
      base.OnClosed(e);
    }
  }
}