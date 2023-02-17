namespace Battleships.Core.Models
{
  internal class Battleship : Ship
  {
    internal Battleship(Point p1, Point p2, Point p3, Point p4, Point p5)
      : base(new List<Point>(new[] { p1, p2, p3, p4, p5 }))
    {

    }
  }
}
