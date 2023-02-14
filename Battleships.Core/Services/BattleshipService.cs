using Battleships.Core.Models;
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
  }
  public class BattleshipService : IBattleshipService
  {
    Point[,] points = new Point[Const.ColsAmount, Const.ColsAmount];
    Dictionary<int, Ship> shipPointsDictionary = new();

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
  }
}
