using Battleships.Core.Extensions;
using Battleships.Core.Models;
using Battleships.Core.Models.Dtos;
using System.Text;

namespace Battleships.Core.Services
{
  public interface IBattleshipService
  {
    void Init();
    void GenerateShips();
    HitResult Hit(string coordinates);
    string GetPointGridString();
    string GetLegendInfo();
    IEnumerable<PointResult> GetPointsResult();
  }
  public class BattleshipService : IBattleshipService
  {
    private readonly Random _random = new Random();
    private readonly List<DirectionType> _directions = Enum.GetValues<DirectionType>().ToList();

    protected readonly Point[,] Points = new Point[Const.ColsAmount, Const.ColsAmount];
    protected readonly Dictionary<int, Ship> ShipPointsDictionary = new();

    public void Init()
    {
      for (int rowIndex = 0; rowIndex < Const.ColsAmount; rowIndex++)
      {
        for (int i = 0; i < Const.ColsAmount; i++)
        {
          Points[rowIndex, i] = new Point(rowIndex + 1, i + 1);
        }
      }
    }

    public void GenerateShips()
    {
      Battleship battleship = new Battleship();
      Destroyer destroyer1 = new Destroyer();
      Destroyer destroyer2 = new Destroyer();


      GenerateShip(battleship, new List<Point>());
      GenerateShip(destroyer1, battleship.Points.ToList());
      GenerateShip(destroyer2, battleship.Points.Concat(destroyer1.Points).ToList());
    }

    public HitResult Hit(string coordinates)
    {
      var result = new HitResult();

      var validationResult = ValidateCoordinates(coordinates);

      result.HitErrorType = validationResult.HitErrorType;

      if (!result.IsSuccess)
        return result;

      var point = Points[validationResult.X - 1, validationResult.Y - 1];

      if (!point.TryHit())
      {
        result.HitErrorType = HitErrorType.AlreadyHit;

        return result;
      }

      if (point.IsAssignedToShip)
      {
        var ship = ShipPointsDictionary[point.GetHashCode()];
        ship.Hit();

        result.HitSuccessType = point.GetHitSuccessType();

        // For performance improvement there has been used Any instead of All
        if (!ShipPointsDictionary.Values.Any(x => x.Points.Any(x => !x.Hit)))
        {
          result.GameOver = true;
        }
      }

      return result;
    }

    public IEnumerable<PointResult> GetPointsResult()
    {
      for (int rowIndex = 0; rowIndex < Const.ColsAmount; rowIndex++)
      {
        for (int i = 0; i < Const.ColsAmount; i++)
        {
          yield return new PointResult(rowIndex, i, Points[rowIndex, i].PointState);
        }
      }
    }

    public string GetPointGridString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("   ");

      for (int i = 0; i < Const.ColsAmount; i++)
      {
        sb.Append(Const.Letters[i] + " ");
      }

      sb.Append(Environment.NewLine);

      for (int rowIndex = 1; rowIndex <= Const.ColsAmount; rowIndex++)
      {
        sb.Append(rowIndex.ToString("D2") + " ");

        for (int i = 0; i < Const.ColsAmount; i++)
        {
          sb.Append(Points[rowIndex - 1, i]);
        }

        sb.Append(Environment.NewLine);
      }

      return sb.ToString();
    }

    public string GetLegendInfo()
    {
      return $"{Const.NotHit}- not hit; {Const.Missed}- missed; {Const.Injured}- injured; {Const.Destroyed}- destroyed.";
    }

    protected virtual (int X, int Y, HitErrorType HitErrorType) ValidateCoordinates(string coordinates)
    {
      coordinates = coordinates.ToUpper().Trim();
      if (string.IsNullOrWhiteSpace(coordinates) || coordinates.Length < 2 ||
        coordinates.Length > 3 || !char.IsLetter(coordinates[0]) ||
        !char.IsDigit(coordinates[1]) || (coordinates.Length == 3 && !char.IsDigit(coordinates[2])))
        return new(0, 0, HitErrorType.NotValid);

      char xC = coordinates[0];

      int x = int.Parse(coordinates.Substring(1));

      if (x < 1 || x > Const.ColsAmount || !Const.Letters.Contains(xC))
        return new(0, 0, HitErrorType.OutOfRange);

      int y = Array.IndexOf(Const.Letters, xC) + 1;

      return new(x, y, HitErrorType.None);
    }

    protected virtual void GenerateShip(Ship ship, List<Point> pointsToAvoid)
    {
      Point startingPoint;
      do
      {
        startingPoint = Points[_random.Next(0, Const.ColsAmount), _random.Next(0, Const.ColsAmount)];
      }
      while (!TryGenerateShip(startingPoint, pointsToAvoid, ship));
    }

    protected virtual bool TryGenerateShip(Point startPoint, List<Point> pointsOccupiedByOtherGroups, Ship shipToGenerate)
    {
      if (pointsOccupiedByOtherGroups.Contains(startPoint))
        return false;

      var tempDirections = new List<DirectionType>(_directions);
      //First of all try to pick direction automatically
      DirectionType direction = (DirectionType)_random.Next(0, 4);

      while (tempDirections.Count > 0)
      {
        // once automatically picked direction is not fitting - pick sequentially
        if (tempDirections.Count < 4)
          direction = tempDirections.First();

        if (TryGeneratePoints(startPoint, direction, pointsOccupiedByOtherGroups, shipToGenerate))
          return true;

        tempDirections.Remove(direction);
      }

      return false;
    }

    protected virtual bool TryGeneratePoints(Point startPoint, DirectionType direction, List<Point> pointsOccupiedByOtherGroups, Ship shipToGenerate)
    {
      int newX = startPoint.X;
      int newY = startPoint.Y;
      for (int i = 0; i < shipToGenerate.Points.Length - 1; i++)
      {
        if (direction == DirectionType.Up)
        {
          newY--;

          if (IsGeneratedPointNotValid(newX, newY, pointsOccupiedByOtherGroups))
            return false;
        }
        else if (direction == DirectionType.Down)
        {
          newY++;

          if (IsGeneratedPointNotValid(newX, newY, pointsOccupiedByOtherGroups))
            return false;
        }
        else if (direction == DirectionType.Left)
        {
          newX--;

          if (IsGeneratedPointNotValid(newX, newY, pointsOccupiedByOtherGroups))
            return false;
        }
        else if (direction == DirectionType.Right)
        {
          newX++;

          if (IsGeneratedPointNotValid(newX, newY, pointsOccupiedByOtherGroups))
            return false;
        }
        else
        {
          throw new ArgumentOutOfRangeException();
        }
      }

      AssignGeneratedPointsToShip(startPoint, direction, shipToGenerate, newX, newY);

      return true;
    }

    protected virtual void AssignGeneratedPointsToShip(Point startPoint, DirectionType direction, Ship shipToGenerate, int newX, int newY)
    {
      for (int i = 0; i < shipToGenerate.Points.Length - 1; i++)
      {
        if (direction == DirectionType.Up)
        {
          var point = Points[newX - 1, newY + i - 1];
          shipToGenerate.Assign(i, point);
          ShipPointsDictionary[point.GetHashCode()] = shipToGenerate;
        }
        else if (direction == DirectionType.Down)
        {
          var point = Points[newX - 1, newY - i - 1];
          shipToGenerate.Assign(i, point);
          ShipPointsDictionary[point.GetHashCode()] = shipToGenerate;
        }
        else if (direction == DirectionType.Left)
        {
          var point = Points[newX + i - 1, newY - 1];
          shipToGenerate.Assign(i, point);
          ShipPointsDictionary[point.GetHashCode()] = shipToGenerate;
        }
        else if (direction == DirectionType.Right)
        {
          var point = Points[newX - i - 1, newY - 1];
          shipToGenerate.Assign(i, point);
          ShipPointsDictionary[point.GetHashCode()] = shipToGenerate;
        }
      }

      shipToGenerate.Assign(shipToGenerate.Points.Length - 1, startPoint);
      ShipPointsDictionary[startPoint.GetHashCode()] = shipToGenerate;
    }

    protected virtual bool IsGeneratedPointNotValid(int x, int y, List<Point> pointsOccupiedByOtherGroups)
    {
      return x < 1 || y < 1 || 
        x > Const.ColsAmount || y > Const.ColsAmount ||
            pointsOccupiedByOtherGroups.Any(p => p.X == x && p.Y == y) ||
            pointsOccupiedByOtherGroups.Any(p => (Math.Abs(p.X - x) <= 1 && Math.Abs(p.Y - y) <= 1));
    }
  }

  public enum DirectionType : int
  {
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
  }
}
