namespace Battleships.Core.Models
{
  class Point
  {
    private readonly int _x, _y;
    private bool _hit, _isAssignedToShip;
    private PointState _pointState;

    private Point()
    {
      _pointState = PointState.NotHit;
    }

    internal Point(int x, int y)
      : this()
    {
      _x = x;
      _y = y;

    }

    public int X => _x;
    public int Y => _y;
    internal bool Hit => _hit;
    internal bool IsAssignedToShip => _isAssignedToShip;
    internal PointState PointState => _pointState;

    internal bool TryHit()
    {
      if (_hit)
        return false;

      _hit = true;
      _pointState = _isAssignedToShip ? PointState.Injured : PointState.Missed;

      return true;
    }

    internal void MarkAsAssigned()
    {
      _isAssignedToShip = true;
    }

    internal void MarkAsDestroyed()
    {
      _pointState = PointState.Destroyed;
    }

    public override string ToString()
    {
      switch (_pointState)
      {
        case PointState.NotHit:
          return Const.NotHit;
        case PointState.Missed:
          return Const.Missed;
        case PointState.Injured:
          return Const.Injured;
        case PointState.Destroyed:
          return Const.Destroyed;
        default:
          throw new ArgumentOutOfRangeException("PointState not implemented");
      }
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(_x, _y);
    }
  }
}

