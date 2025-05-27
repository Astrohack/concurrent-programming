using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
  public sealed class DiagnosticsLoggerUnitTest
  {
    public void Cleanup(string LogFileName)
    {
      if (File.Exists(LogFileName))
        File.Delete(LogFileName);
    }

    [TestMethod]
    public void Logger_CreatesLogFile()
    {
      const string LogFileName = "diagnostics_test_log_1.txt";
      Cleanup(LogFileName);
      var logger = new DiagnosticsLogger(LogFileName);
      Ball dummyBall = new(new Vector(1.0, 2.0), new Vector(0.5, -0.3)) { Radius = 1.0 };

      logger.Log(dummyBall);
      Thread.Sleep(400);
      logger.Stop();
      Thread.Sleep(500);

      string[] lines = File.ReadAllLines(LogFileName);
      Assert.AreEqual(1, lines.Length);
      Assert.IsTrue(lines[0].Contains("ID="));
      Cleanup(LogFileName);
    }

    [TestMethod]
    public void Logger_AppendsMultipleLogs()
    {
      const string LogFileName = "diagnostics_test_log_2.txt";
      Cleanup(LogFileName);
      var logger = new DiagnosticsLogger(LogFileName);
      Ball b1 = new(new Vector(0.0, 0.0), new Vector(1.0, 1.0)) { Radius = 0.5, Id = 1 };
      Ball b2 = new(new Vector(1.0, 1.0), new Vector(-1.0, -1.0)) { Radius = 0.7, Id = 2 };

      logger.Log(b1);
      logger.Log(b2);
      Thread.Sleep(100);
      logger.Stop();
      Thread.Sleep(300);

      string[] lines = File.ReadAllLines(LogFileName);
      Assert.AreEqual(2, lines.Length);
      Assert.IsTrue(lines[0].Contains("ID=" + b1.Id));
      Assert.IsTrue(lines[1].Contains("ID=" + b2.Id));
      Cleanup(LogFileName);
    }

    [TestMethod]
    public void Logger_DoesNotCrashOnMultipleCalls()
    {
      const string LogFileName = "diagnostics_test_log_3.txt";
      Cleanup(LogFileName);
      var logger = new DiagnosticsLogger(LogFileName);
      Ball ball = new(new Vector(5.0, 5.0), new Vector(1.0, 0.0)) { Radius = 1.0 };

      for (int i = 0; i < 10; i++)
        logger.Log(ball);

      Thread.Sleep(200);
      logger.Stop();
      Thread.Sleep(300);

      string[] lines = File.ReadAllLines(LogFileName);
      Assert.AreEqual(10, lines.Length);
      Cleanup(LogFileName);
    }
  }
}
