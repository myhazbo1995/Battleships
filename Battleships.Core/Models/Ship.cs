namespace Battleships.Core.Models
{
    public abstract class Ship
    {
        public List<Point> Points { get; private set; } = new List<Point>();

        public abstract int Size { get; }

        public void Assign(Point point)
        {
            if (Points.Count >= Size)
            {
                throw new InvalidOperationException("Cannot assign more points than the ship's size.");
            }
            Points.Add(point);
            point.AssignToShip();
        }

        public bool IsDestroyed()
        {
            return Points.All(x => x.Hit);
        }
    }

    public class Battleship : Ship
    {
        public override int Size => 5;
    }

    public class Destroyer : Ship
    {
        public override int Size => 3;
    }
}
