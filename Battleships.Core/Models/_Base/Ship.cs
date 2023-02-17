namespace Battleships.Core.Models
{
  public abstract class Ship
  {
    public readonly Point[] Points;

    public Ship(int shipSize)
    {
      Points = new Point[shipSize];
    }

    public void Hit()
    {
      // It is better to use Any instead of All due to performance
      if (!Points.Any(x => !x.Hit))
        Array.ForEach(Points, p => p.MarkAsDestroyed());
    }

    public void Assign(int index, Point point)
    {
      Points[index] = point;
      point.MarkAsAssigned();
    }
  }
}
