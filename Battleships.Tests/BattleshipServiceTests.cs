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
  class BattleshipServiceTest : BattleshipService
  {
    public Point[,] PointsStub => Points;
    public Dictionary<int, Ship> ShipPointsDictionaryStub => ShipPointsDictionary;

    public new (int X, int Y, HitErrorType HitErrorType) ValidateCoordinates(string coordinates)
    {
      return base.ValidateCoordinates(coordinates);
    }

    public new bool TryGeneratePoints(Point startPoint, DirectionType direction, List<Point> pointsOccupiedByOtherGroups, Ship shipToGenerate)
    {
      return base.TryGeneratePoints(startPoint, direction, pointsOccupiedByOtherGroups, shipToGenerate);
    }

    public new bool TryGenerateShip(Point startPoint, List<Point> pointsOccupiedByOtherGroups, Ship shipToGenerate)
    {
      return base.TryGenerateShip(startPoint, pointsOccupiedByOtherGroups, shipToGenerate);
    }

    public new bool IsGeneratedPointNotValid(int x, int y, List<Point> pointsOccupiedByOtherGroups)
    {
      return base.IsGeneratedPointNotValid(x, y, pointsOccupiedByOtherGroups);
    }

    public new void AssignGeneratedPointsToShip(Point startPoint, DirectionType direction, Ship shipToGenerate, int newX, int newY)
    {
      base.AssignGeneratedPointsToShip(startPoint, direction, shipToGenerate, newX, newY);
    }
  }

  [TestClass]
  public class BattleshipServiceTests
  {
    [TestMethod]
    public void TestInit()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();

      // Act
      service.Init();

      // Assert
      for (int i = 0; i < Const.ColsAmount; i++)
      {
        for (int j = 0; j < Const.ColsAmount; j++)
        {
          Assert.IsNotNull(service.PointsStub[i, j]);
          Assert.AreEqual(i + 1, service.PointsStub[i, j].X);
          Assert.AreEqual(j + 1, service.PointsStub[i, j].Y);
        }
      }
    }

    [TestMethod]
    public void TestHit_MissedPoint_ReturnsSuccess()
    {
      // Arrange
      BattleshipService service = new BattleshipService();
      service.Init();

      // Act
      HitResult result = service.Hit("A1");

      // Assert
      Assert.IsTrue(result.IsSuccess);
      Assert.AreEqual(HitSuccessType.Missed, result.HitSuccessType);
    }

    [TestMethod]
    public void TestHit_AlreadyHitPoint_ReturnsError()
    {
      // Arrange
      BattleshipService service = new BattleshipService();
      service.Init();

      // Act
      HitResult result = service.Hit("A1");
      result = service.Hit("A1");

      // Assert
      Assert.IsFalse(result.IsSuccess);
      Assert.AreEqual(HitErrorType.AlreadyHit, result.HitErrorType);
    }

    [TestMethod]
    public void TestHit_OutOfRangePoint_ReturnsError()
    {
      // Arrange
      BattleshipService service = new BattleshipService();
      service.Init();

      // Act
      HitResult result = service.Hit("A11");

      // Assert
      Assert.IsFalse(result.IsSuccess);
      Assert.AreEqual(HitErrorType.OutOfRange, result.HitErrorType);
    }

    [TestMethod]
    public void TestHit_InjuredPoint_ReturnsSuccess()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();
      service.Init();
      service.GenerateShips();
      var pointToHit = service.ShipPointsDictionaryStub.Values.First().Points.First();

      // Act
      var result = service.Hit($"{Const.Letters[pointToHit.Y - 1]}{pointToHit.X}");

      // Assert
      Assert.IsTrue(result.IsSuccess);
      Assert.AreEqual(HitSuccessType.Injured, result.HitSuccessType);
    }

    [TestMethod]
    public void TestHit_DestroyedPoint_ReturnsSuccess()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();
      service.Init();
      service.GenerateShips();
      var pointsToHit = service.ShipPointsDictionaryStub.Values.First().Points;
      HitResult result;

      for (int i = 0; i < pointsToHit.Length; i++)
      {
        var pointToHit = pointsToHit[i];
        // Act
        result = service.Hit($"{Const.Letters[pointToHit.Y - 1]}{pointToHit.X}");

        // Assert
        Assert.IsTrue(result.IsSuccess);

        if(i < pointsToHit.Length - 1)
          Assert.AreEqual(HitSuccessType.Injured, result.HitSuccessType);
      }

      // Assert
      Assert.IsTrue(pointsToHit.All(x => x.PointState == PointState.Destroyed));
    }

    [TestMethod]
    public void TestHit_GameOver_ReturnsTrue()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();
      service.Init();
      service.GenerateShips();
      var ships = service.ShipPointsDictionaryStub.Values.Distinct().ToList();
      HitResult result = new HitResult();

      foreach (var ship in ships)
      {
        for (int i = 0; i < ship.Points.Length; i++)
        {
          var pointToHit = ship.Points[i];
          // Act
          result = service.Hit($"{Const.Letters[pointToHit.Y - 1]}{pointToHit.X}");

          // Assert
          Assert.IsTrue(result.IsSuccess);
        }
      }

      // Assert
      Assert.IsTrue(result.GameOver);
    }

    [TestMethod]
    public void TestGetPointsResult()
    {
      // Arrange
      BattleshipService service = new BattleshipService();
      service.Init();
      service.GenerateShips();

      // Act
      IEnumerable<PointResult> results = service.GetPointsResult();

      // Assert
      Assert.IsNotNull(results);
      Assert.AreEqual(Const.ColsAmount * Const.ColsAmount, results.Count());
    }

    [TestMethod]
    public void TestValidateCoordinates_ValidCoordinates_ReturnsNone()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();

      // Act
      var result = service.ValidateCoordinates("A1");

      // Assert
      Assert.AreEqual(HitErrorType.None, result.HitErrorType);
      Assert.AreEqual(1, result.X);
      Assert.AreEqual(1, result.Y);
    }

    [TestMethod]
    public void TestValidateCoordinates_InvalidLength_ReturnsNotValid()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();

      // Act
      var result = service.ValidateCoordinates("A");

      // Assert
      Assert.AreEqual(HitErrorType.NotValid, result.HitErrorType);
    }

    [TestMethod]
    public void TestGenerateShips()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();
      service.Init();

      // Act
      service.GenerateShips();

      // Assert
      int shipsCount = service.ShipPointsDictionaryStub.Values.Distinct().Count();
      Assert.AreEqual(3, shipsCount);

      int battleshipSize = 5;
      int destroyerSize = 4;

      int battleshipsCount = service.ShipPointsDictionaryStub.Values.Distinct().Count(x => x.Points.Length == battleshipSize);
      int destroyersCount = service.ShipPointsDictionaryStub.Values.Distinct().Count(x => x.Points.Length == destroyerSize);

      Assert.AreEqual(1, battleshipsCount);
      Assert.AreEqual(2, destroyersCount);
    }



    [TestMethod]
    public void TestTryGenerateShip_WithValidStartingPoint_ReturnsTrue()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();
      service.Init();
      Point startPoint = service.PointsStub[0, 0];
      List<Point> pointsOccupiedByOtherGroups = new List<Point>();
      Ship shipToGenerate = new Destroyer();

      // Act
      bool result = service.TryGenerateShip(startPoint, pointsOccupiedByOtherGroups, shipToGenerate);

      // Assert
      Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestTryGenerateShip_WithInvalidStartingPoint_ReturnsFalse()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();
      service.Init();
      Point startPoint = new Point(0, 0);
      List<Point> pointsOccupiedByOtherGroups = new List<Point>() { startPoint };
      Ship shipToGenerate = new Destroyer();

      // Act
      bool result = service.TryGenerateShip(startPoint, pointsOccupiedByOtherGroups, shipToGenerate);

      // Assert
      Assert.IsFalse(result);
    }

    [TestMethod]
    public void TestTryGeneratePoints_WithValidDirection_ReturnsTrue()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();
      service.Init();
      Point startPoint = service.PointsStub[0, 0];
      List<Point> pointsOccupiedByOtherGroups = new List<Point>();
      Ship shipToGenerate = new Destroyer();

      // Act
      bool result = service.TryGeneratePoints(startPoint, DirectionType.Right, pointsOccupiedByOtherGroups, shipToGenerate);

      // Assert
      Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestTryGeneratePoints_WithValidDirection_ReturnsFalse()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();
      service.Init();
      Point startPoint = service.PointsStub[0, 0];
      List<Point> pointsOccupiedByOtherGroups = new List<Point>();
      Ship shipToGenerate = new Destroyer();

      // Act
      bool result = service.TryGeneratePoints(startPoint, DirectionType.Left, pointsOccupiedByOtherGroups, shipToGenerate);

      // Assert
      Assert.IsFalse(result);
    }

    [TestMethod]
    public void TestAssignGeneratedPointsToShip_AssignsPointsToShip()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();
      service.Init();
      Point startPoint = service.PointsStub[4, 4];
      DirectionType direction = DirectionType.Right;
      Ship shipToGenerate = new Destroyer();
      int newX = startPoint.X;
      int newY = startPoint.Y;

      // Act
      service.AssignGeneratedPointsToShip(startPoint, direction, shipToGenerate, newX, newY);

      // Assert
      Assert.IsTrue(shipToGenerate.Points.All(x => x != null));
      Assert.IsTrue(shipToGenerate.Points.All(x => x.IsAssignedToShip));
      Assert.IsTrue(shipToGenerate.Points.Contains(startPoint));

      Assert.IsTrue(shipToGenerate.Points.All(x => service.ShipPointsDictionaryStub.ContainsKey(x.GetHashCode())));
    }

    [TestMethod]
    public void TestIsGeneratedPointNotValid_WithValidPoint_ReturnsFalse()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();
      int x = 1;
      int y = 1;
      List<Point> pointsOccupiedByOtherGroups = new List<Point>();

      // Act
      bool result = service.IsGeneratedPointNotValid(x, y, pointsOccupiedByOtherGroups);

      // Assert
      Assert.IsFalse(result);
    }

    [TestMethod]
    public void TestIsGeneratedPointNotValid_WithPointOccupiedByOtherGroups_ReturnsTrue()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();
      service.Init();
      var point = service.PointsStub[0, 0];
      int x = point.X;
      int y = point.Y;
      List<Point> pointsOccupiedByOtherGroups = new List<Point> { point };

      // Act
      bool result = service.IsGeneratedPointNotValid(x, y, pointsOccupiedByOtherGroups);

      // Assert
      Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestIsGeneratedPointNotValid_WithPointAdjacentToOtherGroups_ReturnsTrue()
    {
      // Arrange
      BattleshipServiceTest service = new BattleshipServiceTest();
      service.Init();
      var point = service.PointsStub[0, 0];
      int x = point.X + 1;
      int y = point.Y + 1;
      List<Point> pointsOccupiedByOtherGroups = new List<Point> { point };

      // Act
      bool result = service.IsGeneratedPointNotValid(x, y, pointsOccupiedByOtherGroups);

      // Assert
      Assert.IsTrue(result);
    }
  }
}
