using Battleships.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Tests
{
  class TestShip : Ship
  {
    public TestShip(int size)
    : base(size)
    {

    }
  }

  [TestClass]
  public class ShipTests
  {
    [TestMethod]
    public void TestConstructor()
    {
      // Arrange
      int shipSize = 3;

      // Act
      Ship ship = new TestShip(shipSize);

      // Assert
      Assert.IsNotNull(ship.Points);
      Assert.AreEqual(shipSize, ship.Points.Length);
    }

    [TestMethod]
    public void TestAssign()
    {
      // Arrange
      int index = 2;
      Point point = new Point(3, 4);
      Ship ship = new TestShip(3);

      // Act
      ship.Assign(index, point);

      // Assert
      Assert.AreEqual(point, ship.Points[index]);
      Assert.IsTrue(point.IsAssignedToShip);
    }

    [TestMethod]
    public void TestHit_NotAllPointsHit_NoPointsMarkedAsDestroyed()
    {
      // Arrange
      Point[] points = new Point[]
      {
            new Point(1, 1),
            new Point(1, 2),
            new Point(1, 3)
      };
      Ship ship = new TestShip(points.Length);

      for (int i = 0; i < points.Length; i++)
      {
        ship.Assign(i, points[i]);
      }

      // Act
      ship.Hit();

      // Assert
      foreach (Point point in points)
      {
        Assert.IsFalse(point.PointState == PointState.Destroyed);
      }
    }

    [TestMethod]
    public void TestHit_AllPointsHit_AllPointsMarkedAsDestroyed()
    {
      // Arrange
      Point[] points = new Point[]
      {
            new Point(1, 1),
            new Point(1, 2),
            new Point(1, 3)
      };
      Ship ship = new TestShip(points.Length);

      for (int i = 0; i < points.Length; i++)
      {
        ship.Assign(i, points[i]);
        points[i].TryHit();
      }

      // Act
      ship.Hit();

      // Assert
      foreach (Point point in points)
      {
        Assert.IsTrue(point.PointState == PointState.Destroyed);
      }
    }
  }
}
