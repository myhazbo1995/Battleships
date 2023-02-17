using Battleships.Core.Models;
using Battleships.Core.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Core.Extensions
{
  internal static class PointExtensions
  {
    internal static HitSuccessType GetHitSuccessType(this Point point)
    {
      if (point.PointState == PointState.Injured)
        return HitSuccessType.Injured;
      else if (point.PointState == PointState.Destroyed)
        return HitSuccessType.Destroyed;
      else if (point.PointState == PointState.Missed)
        return HitSuccessType.Missed;
      else
        throw new ArgumentOutOfRangeException();
    }
  }
}
