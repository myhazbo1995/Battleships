using Battleships.Core.Extensions;
using Battleships.Core.Models;

namespace Battleships.Tests
{
  [TestClass]
  public class PointTests
  {
    [TestMethod]
    public void TestConstructor()
    {
      // Arrange
      int x = 3;
      int y = 4;

      // Act
      Point point = new Point(x, y);

      // Assert
      Assert.AreEqual(x, point.X);
      Assert.AreEqual(y, point.Y);
      Assert.IsFalse(point.Hit);
      Assert.IsFalse(point.IsAssignedToShip);
      Assert.AreEqual(PointState.NotHit, point.PointState);
      Assert.ThrowsException<ArgumentOutOfRangeException>(() => point.GetHitSuccessType());
    }

    [TestMethod]
    public void TestTryHit_HitMissedPoint_ReturnsTrue()
    {
      // Arrange
      Point point = new Point(2, 2);

      // Act
      bool result = point.TryHit();

      // Assert
      Assert.IsTrue(result);
      Assert.IsTrue(point.Hit);
      Assert.IsFalse(point.IsAssignedToShip);
      Assert.AreEqual(PointState.Missed, point.PointState);
      Assert.AreEqual(Core.Models.Dtos.HitSuccessType.Missed, point.GetHitSuccessType());
    }

    [TestMethod]
    public void TestTryHit_HitInjuredPoint_ReturnsFalse()
    {
      // Arrange
      Point point = new Point(2, 2);
      point.MarkAsAssigned();
      point.TryHit();

      // Act
      bool result = point.TryHit();

      // Assert
      Assert.IsFalse(result);
      Assert.IsTrue(point.Hit);
      Assert.IsTrue(point.IsAssignedToShip);
      Assert.AreEqual(PointState.Injured, point.PointState);
      Assert.AreEqual(Core.Models.Dtos.HitSuccessType.Injured, point.GetHitSuccessType());
    }

    [TestMethod]
    public void TestMarkAsAssigned()
    {
      // Arrange
      Point point = new Point(2, 2);

      // Act
      point.MarkAsAssigned();

      // Assert
      Assert.IsTrue(point.IsAssignedToShip);
    }

    [TestMethod]
    public void TestMarkAsDestroyed()
    {
      // Arrange
      Point point = new Point(2, 2);
      point.MarkAsAssigned();

      // Act
      point.MarkAsDestroyed();

      // Assert
      Assert.AreEqual(PointState.Destroyed, point.PointState);
      Assert.AreEqual(Core.Models.Dtos.HitSuccessType.Destroyed, point.GetHitSuccessType());
    }


  }
}