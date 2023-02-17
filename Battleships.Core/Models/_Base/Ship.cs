namespace Battleships.Core.Models
{
  internal abstract class Ship
  {
    internal readonly Point[] Points;

    public Ship(int shipSize)
    {
      Points = new Point[shipSize];
    }

    // to be considered if needed in derived class
    //protected IReadOnlyList<Point> Points => _points.AsReadOnly();

    internal void Hit()
    {
      // It is better to use Any instead of All due to performance
      if (Points.Any(x => !x.Hit))
        Array.ForEach(Points, p => p.MarkAsDestroyed());
    }

    internal void Assign(int index, Point point)
    {
      Points[index] = point;
      point.MarkAsAssigned();
    }
  }
}
