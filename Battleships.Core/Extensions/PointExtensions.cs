using Battleships.Core.Models;
using Battleships.Core.Models.Dtos;

namespace Battleships.Core.Extensions
{
  public static class PointExtensions
  {
    public static HitSuccessType GetHitSuccessType(this Point point)
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
