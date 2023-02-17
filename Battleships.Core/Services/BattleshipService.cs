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
    string GetPointGridString();
    string GetLegendInfo();
    IEnumerable<PointResult> GetPointsResult();
  }
  public class BattleshipService : IBattleshipService
  {
    private readonly Point[,] points = new Point[Const.ColsAmount, Const.ColsAmount];
    private readonly Dictionary<int, Ship> shipPointsDictionary = new();

    public void Init()
    {
      int rowIndex = 0;

      while (rowIndex < Const.ColsAmount)
      {
        for (int i = 0; i < Const.ColsAmount; i++)
        {
          points[rowIndex, i] = new Point(rowIndex + 1, i + 1);
        }
        rowIndex++;
      }
    }

    public HitResult Hit(string coordinates)
    {
      var result = new HitResult();

      var validationResult = ValidateCoordinates(coordinates);

      result.HitErrorType = validationResult.HitErrorType;

      if (!result.IsSuccess)
        return result;

      var point = points[validationResult.X, validationResult.Y];

      if (!point.TryHit())
      {
        result.HitErrorType = HitErrorType.AlreadyHit;

        return result;
      }

      if(point.IsAssignedToShip)
      {
        var ship = shipPointsDictionary[point.GetHashCode()];
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
          yield return new PointResult(rowIndex, i, points[rowIndex, i].PointState);
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
          sb.Append(points[rowIndex - 1, i]);
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
      return new (0, 0, HitErrorType.None);
    }
  }
}
