namespace Battleships.Core.Models
{
  internal class Destroyer : Ship
  {
    internal Destroyer(Point p1, Point p2, Point p3, Point p4)
      : base(new List<Point>(new[] { p1, p2, p3, p4 }))
    {

    }
  }
}