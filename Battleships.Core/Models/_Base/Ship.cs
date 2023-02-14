namespace Battleships.Core.Models
{
  public abstract class Ship
  {
    private readonly List<Point> _points;

    public Ship(List<Point> points)
    {
      _points = points ?? new List<Point>();
    }

    // to be considered if needed in derived class
    //protected IReadOnlyList<Point> Points => _points.AsReadOnly();

    public void Hit()
    {
      // It is better to use Any instead of All due to performance
      if (_points.Any(x => !x.Hit))
        _points.ForEach(p => p.MarkAsDestroyed());
    }
  }
}
