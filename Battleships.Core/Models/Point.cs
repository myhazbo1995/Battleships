using Battleships.Core.Models.Dtos;

namespace Battleships.Core.Models
{
    public class Point
    {
        public int X { get; }
        public int Y { get; }

        public bool Hit { get; private set; }

        public bool IsPointOfDestroyedShip { get; private set; }

        public bool IsAssignedToShip { get; private set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool TryHit()
        {
            if (Hit)
            {
                return false;
            }

            Hit = true;

            return true;
        }

        public void AssignToShip() => IsAssignedToShip = true;

        public void MarkAsPointOfDestroyedShip() => IsPointOfDestroyedShip = true;

        public override string ToString()
        {
            if (!Hit)
                return Const.NotHit;

            if (Hit && !IsAssignedToShip)
                return Const.Missed;

            if (Hit && IsAssignedToShip && IsPointOfDestroyedShip)
                return Const.Destroyed;

            if (Hit && IsAssignedToShip && !IsPointOfDestroyedShip)
                return Const.Injured;

            throw new Exception("Invalid point state");
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}

