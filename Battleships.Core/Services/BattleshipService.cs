using Battleships.Core.Models;
using Battleships.Core.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Point = Battleships.Core.Models.Point;

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
    private readonly Point[,] _points = new Point[Const.ColsAmount, Const.ColsAmount];
    private readonly Dictionary<int, Ship> _shipPointsDictionary = new();
    private readonly Random _random = new Random();
    private readonly List<DirectionType> _directions = Enum.GetValues<DirectionType>().ToList();

    public void Init()
    {
      int rowIndex = 0;

      while (rowIndex < Const.ColsAmount)
      {
        for (int i = 0; i < Const.ColsAmount; i++)
        {
          _points[rowIndex, i] = new Point(rowIndex + 1, i + 1);
        }
        rowIndex++;
      }
    }

    public void GenerateShips()
    {
      Battleship battleship = new Battleship();
      Destroyer destroyer1 = new Destroyer();
      Destroyer destroyer2 = new Destroyer();

      Point battleshipStartingPoint;
      do
      {
        battleshipStartingPoint = _points[_random.Next(0, Const.ColsAmount), _random.Next(0, Const.ColsAmount)];
      }
      while (!TryGenerateShip(battleshipStartingPoint, new List<Point>(), battleship));

      Point destroyer1StartingPoint;
      do
      {
        destroyer1StartingPoint = _points[_random.Next(0, Const.ColsAmount), _random.Next(0, Const.ColsAmount)];
      }
      while (!TryGenerateShip(destroyer1StartingPoint, battleship.Points.ToList(), destroyer1));

      Point destroyer2StartingPoint;
      do
      {
        destroyer2StartingPoint = _points[_random.Next(0, Const.ColsAmount), _random.Next(0, Const.ColsAmount)];
      }
      while (!TryGenerateShip(destroyer2StartingPoint, battleship.Points.Concat(destroyer1.Points).ToList(), destroyer2));
    }

    public HitResult Hit(string coordinates)
    {
      var result = new HitResult();

      var validationResult = ValidateCoordinates(coordinates);

      result.HitErrorType = validationResult.HitErrorType;

      if (!result.IsSuccess)
        return result;

      var point = _points[validationResult.X - 1, validationResult.Y - 1];

      if (!point.TryHit())
      {
        result.HitErrorType = HitErrorType.AlreadyHit;

        return result;
      }

      if(point.IsAssignedToShip)
      {
        var ship = _shipPointsDictionary[point.GetHashCode()];
        ship.Hit();
      }

      return result;
    }

    public IEnumerable<PointResult> GetPointsResult()
    {
      int rowIndex = 0;

      while (rowIndex < Const.ColsAmount)
      {
        for (int i = 0; i < Const.ColsAmount; i++)
        {
          yield return new PointResult(rowIndex, i, _points[rowIndex, i].PointState);
        }
        rowIndex++;
      }
    }

    public string GetPointGridString()
    {
      StringBuilder sb = new StringBuilder();
      int rowIndex = 1;
      sb.Append("   ");

      for (int i = 0; i < Const.ColsAmount; i++)
      {
        sb.Append(Const.Letters[i] + " ");
      }

      sb.Append(Environment.NewLine);

      while (rowIndex <= Const.ColsAmount)
      {
        sb.Append(rowIndex.ToString("D2") + " ");

        for (int i = 0; i < Const.ColsAmount; i++)
        {
          sb.Append(_points[rowIndex - 1, i]);
        }

        sb.Append(Environment.NewLine);
        rowIndex++;
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

      return new (x, y, HitErrorType.None);
    }

    internal virtual bool TryGenerateShip(Point startPoint, List<Point> pointsOccupiedByOtherGroups, Ship shipToGenerate)
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

    internal virtual bool TryGeneratePoints(Point startPoint, DirectionType direction, List<Point> pointsOccupiedByOtherGroups, Ship shipToGenerate)
    {
      int newX = startPoint.X;
      int newY = startPoint.Y;
      for (int i = 0; i < shipToGenerate.Points.Length - 1; i++)
      {
        if (direction == DirectionType.Up)
        {
          newY--;

          if (newX < 1 || newY < 1 || newX > Const.ColsAmount || newY > Const.ColsAmount ||
            pointsOccupiedByOtherGroups.Any(p => p.X == newX && p.Y == newY) ||
            pointsOccupiedByOtherGroups.Any(p => (Math.Abs(p.X - newX) <= 1 && Math.Abs(p.Y - newY) <= 1)))
            return false;
        }
        else if (direction == DirectionType.Down)
        {
          newY++;

          if (newX < 1 || newY < 1 || newX > Const.ColsAmount || newY > Const.ColsAmount ||
            pointsOccupiedByOtherGroups.Any(p => p.X == newX && p.Y == newY) ||
            pointsOccupiedByOtherGroups.Any(p => (Math.Abs(p.X - newX) <= 1 && Math.Abs(p.Y - newY) <= 1)))
            return false;
        }
        else if (direction == DirectionType.Left)
        {
          newX--;

          if (newX < 1 || newY < 1 || newX > Const.ColsAmount || newY > Const.ColsAmount ||
            pointsOccupiedByOtherGroups.Any(p => p.X == newX && p.Y == newY) ||
            pointsOccupiedByOtherGroups.Any(p => (Math.Abs(p.X - newX) <= 1 && Math.Abs(p.Y - newY) <= 1)))
            return false;
        }
        else if (direction == DirectionType.Right)
        {
          newX++;

          if (newX < 1 || newY < 1 || newX > Const.ColsAmount || newY > Const.ColsAmount ||
            pointsOccupiedByOtherGroups.Any(p => p.X == newX && p.Y == newY) ||
            pointsOccupiedByOtherGroups.Any(p => (Math.Abs(p.X - newX) <= 1 && Math.Abs(p.Y - newY) <= 1)))
            return false;
        }
      }

      for (int i = 0; i < shipToGenerate.Points.Length - 1; i++)
      {
        if (direction == DirectionType.Up)
        {
          var point = _points[newX - 1, newY + i - 1];
          shipToGenerate.Assign(i, point);
          _shipPointsDictionary[point.GetHashCode()] = shipToGenerate;
        }
        else if (direction == DirectionType.Down)
        {
          var point = _points[newX - 1, newY - i - 1];
          shipToGenerate.Assign(i, point);
          _shipPointsDictionary[point.GetHashCode()] = shipToGenerate;
        }
        else if (direction == DirectionType.Left)
        {
          var point = _points[newX + i - 1, newY - 1];
          shipToGenerate.Assign(i, point);
          _shipPointsDictionary[point.GetHashCode()] = shipToGenerate;
        }
        else if (direction == DirectionType.Right)
        {
          var point = _points[newX - i - 1, newY - 1];
          shipToGenerate.Assign(i, point);
          _shipPointsDictionary[point.GetHashCode()] = shipToGenerate;
        }
      }

      shipToGenerate.Assign(shipToGenerate.Points.Length - 1, startPoint);
      _shipPointsDictionary[startPoint.GetHashCode()] = shipToGenerate;

      return true;
    }
  }

  internal enum DirectionType : int
  {
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
  }
}
