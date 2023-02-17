namespace Battleships.Core.Models
{
  internal abstract class Ship
  {
    private readonly List<Point> _points;

    internal Ship(List<Point> points)
    {
      _points = points ?? new List<Point>();
    }

    // to be considered if needed in derived class
    //protected IReadOnlyList<Point> Points => _points.AsReadOnly();

    internal void Hit()
    {
      // It is better to use Any instead of All due to performance
      if (_points.Any(x => !x.Hit))
        _points.ForEach(p => p.MarkAsDestroyed());
    }
  }
}
