using Battleships.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Tests
{
    [TestClass]
    public class ShipTests
    {
        [TestMethod]
        public void Assign_WithValidPoint_IncreasesShipPointsCount()
        {
            // Arrange
            Ship battleship = new Battleship();
            Point point = new Point(1, 1);

            // Act
            battleship.Assign(point);

            // Assert
            Assert.AreEqual(1, battleship.Points.Count);
            Assert.IsTrue(point.IsAssignedToShip);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Assign_OverLimit_ThrowsInvalidOperationException()
        {
            // Arrange
            Ship destroyer = new Destroyer();
            Point[] points = {
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3),
                new Point(1, 4)
            };

            // Act
            foreach (var point in points)
            {
                destroyer.Assign(point);
            }

            // Assert by attribute
        }

        [TestMethod]
        public void IsDestroyed_WithAllPointsHit_ReturnsTrue()
        {
            // Arrange
            Ship destroyer = new Destroyer();
            Point[] points = {
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3)
            };

            foreach (var point in points)
            {
                destroyer.Assign(point);
                point.TryHit();
            }

            // Act
            bool isDestroyed = destroyer.IsDestroyed();

            // Assert
            Assert.IsTrue(isDestroyed);
        }
    }
}
