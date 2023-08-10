using Battleships.Core.Models.Dtos;
using Battleships.Core.Services;
using Battleships.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleships.Core.Models;

namespace Battleships.Tests
{
    [TestClass]
    public class BattleshipServiceTests
    {
        private BattleshipService _service;

        [TestInitialize]
        public void SetUp()
        {
            _service = new BattleshipService();
        }

        [TestMethod]
        public void GenerateShips_PlacesAllShips_WithoutOverlap()
        {
            // Act
            _service.GenerateShips();

            // Arrange
            var occupiedPoints = _service.ShipPointsDictionary.Keys;

            // Assert
            Assert.AreEqual(occupiedPoints.Count(), occupiedPoints.Distinct().Count());
        }

        [TestMethod]
        public void Hit_UnoccupiedPoint_ReturnsMissed()
        {
            // Arrange
            _service.GenerateShips();
            var unoccupiedPoint = _service.Points.Cast<Point>().First(p => !p.IsAssignedToShip);
            string coordinates = $"{Const.Letters[unoccupiedPoint.Y - 1]}{unoccupiedPoint.X}";

            // Act
            var result = _service.Hit(coordinates);

            // Assert
            Assert.AreEqual(HitSuccessType.Missed, result.HitSuccessType);
        }

        [TestMethod]
        public void Hit_OccupiedPoint_ReturnsInjured()
        {
            // Arrange
            _service.GenerateShips();
            var occupiedPoint = _service.Points.Cast<Point>().First(p => p.IsAssignedToShip);
            string coordinates = $"{Const.Letters[occupiedPoint.Y - 1]}{occupiedPoint.X}";

            // Act
            var result = _service.Hit(coordinates);

            // Assert
            Assert.AreEqual(HitSuccessType.Injured, result.HitSuccessType);
        }

        [TestMethod]
        public void Hit_AllPointsOfShip_ReturnsDestroyed()
        {
            // Arrange
            var ship = new Destroyer();
            for (int i = 0; i < ship.Size; i++)
            {
                var point = _service.Points[i, 0];
                point.AssignToShip();
                ship.Assign(point);
                _service.ShipPointsDictionary[point.GetHashCode()] = ship;
            }

            // Act & Assert
            foreach (var point in ship.Points)
            {
                string coordinates = $"{Const.Letters[point.Y - 1]}{point.X}";
                var result = _service.Hit(coordinates);
                if (point == ship.Points.Last())
                {
                    Assert.AreEqual(HitSuccessType.Destroyed, result.HitSuccessType);
                }
                else
                {
                    Assert.AreEqual(HitSuccessType.Injured, result.HitSuccessType);
                }
            }
        }

        [TestMethod]
        public void Hit_InvalidCoordinate_ReturnsOutOfRangeError()
        {
            // Arrange
            _service.GenerateShips();
            string invalidCoordinates = "Z99";

            // Act
            var result = _service.Hit(invalidCoordinates);

            // Assert
            Assert.AreEqual(HitErrorType.OutOfRange, result.HitErrorType);
        }

        [TestMethod]
        public void Hit_InvalidCoordinate_ReturnsNotValidError()
        {
            // Arrange
            _service.GenerateShips();
            string invalidCoordinates = "Z999";

            // Act
            var result = _service.Hit(invalidCoordinates);

            // Assert
            Assert.AreEqual(HitErrorType.NotValid, result.HitErrorType);
        }

        [TestMethod]
        public void Hit_AlreadyHitPoint_ReturnsAlreadyHitError()
        {
            // Arrange
            _service.GenerateShips();
            var occupiedPoint = _service.Points.Cast<Point>().First(p => p.IsAssignedToShip);
            string coordinates = $"{Const.Letters[occupiedPoint.Y - 1]}{occupiedPoint.X}";

            _service.Hit(coordinates);

            // Act
            var result = _service.Hit(coordinates);

            // Assert
            Assert.AreEqual(HitErrorType.AlreadyHit, result.HitErrorType);
        }

        [TestMethod]
        public void GetPointGridString_ReturnsAccurateBoardState()
        {
            // Act
            var boardRepresentation = _service.GetPointGridString();

            // Assert
            Assert.IsTrue(boardRepresentation.Contains(Const.NotHit));
        }
    }
}
