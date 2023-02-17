using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Core.Models.Dtos
{
  public class PointResult
  {
    public PointResult(int x, int y, PointState pointState)
    {
      X = x;
      Y = y;
      PointState = pointState;
    }

    public int X { get; private set; }
    public int Y { get; private set; }
    public PointState PointState { get; private set; }
  }
}
