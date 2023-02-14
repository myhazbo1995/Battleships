﻿namespace Battleships.Core.Models
{
  public class Destroyer : Ship
  {
    public Destroyer(Point p1, Point p2, Point p3, Point p4)
      : base(new List<Point>(new[] { p1, p2, p3, p4 }))
    {

    }
  }
}