using Battleships.Core.Models;

namespace Battleships.Tests
{
    [TestClass]
    public class PointTests
    {
        [TestMethod]
        public void AssignToShip_Once_IsAssignedToShipIsTrue()
        {
            // Arrange
            Point point = new Point(1, 1);

            // Act
            point.AssignToShip();

            // Assert
            Assert.IsTrue(point.IsAssignedToShip);
        }

        [TestMethod]
        public void TryHit_OnNewPoint_ReturnsTrue()
        {
            // Arrange
            Point point = new Point(1, 1);

            // Act
            bool wasHitSuccessful = point.TryHit();

            // Assert
            Assert.IsTrue(wasHitSuccessful);
            Assert.IsTrue(point.Hit);
        }

        [TestMethod]
        public void TryHit_OnAlreadyHitPoint_ReturnsFalse()
        {
            // Arrange
            Point point = new Point(1, 1);
            point.TryHit();

            // Act
            bool wasHitSuccessful = point.TryHit();

            // Assert
            Assert.IsFalse(wasHitSuccessful);
        }
    }
}