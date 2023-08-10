using Battleships.Core.Models;
using Battleships.Core.Models.Dtos;
using System.Text;

namespace Battleships.Core.Services
{
    public interface IBattleshipService
    {
        void GenerateShips();
        HitResult Hit(string coordinates);
        string GetPointGridString();
        string GetLegendInfo();
    }
    public class BattleshipService : IBattleshipService
    {
        private readonly Random _random = new();

        public readonly Point[,] Points = new Point[Const.ColsAmount, Const.ColsAmount];
        public readonly Dictionary<int, Ship> ShipPointsDictionary = new();

        public BattleshipService()
        {
            Init();
        }

        public void GenerateShips()
        {
            Battleship battleship = new();
            Destroyer destroyer1 = new();
            Destroyer destroyer2 = new();

            GenerateShip(battleship);
            GenerateShip(destroyer1);
            GenerateShip(destroyer2);
        }

        public HitResult Hit(string coordinates)
        {
            var result = new HitResult();

            var validationResult = ValidateCoordinates(coordinates);

            result.HitErrorType = validationResult.HitErrorType;

            if (!result.IsSuccess)
                return result;

            var point = Points[validationResult.X - 1, validationResult.Y - 1];

            if (!point.TryHit())
            {
                result.HitErrorType = HitErrorType.AlreadyHit;

                return result;
            }

            if (!point.IsAssignedToShip)
            {
                result.HitSuccessType = HitSuccessType.Missed;

                return result;
            }

            var ship = ShipPointsDictionary[point.GetHashCode()];

            if (ship.IsDestroyed())
            {
                ship.Points.ForEach(p => p.MarkAsPointOfDestroyedShip());

                result.HitSuccessType = HitSuccessType.Destroyed;

                if (ShipPointsDictionary.Values.All(x => x.IsDestroyed()))
                {
                    result.GameOver = true;
                }

                return result;
            }

            result.HitSuccessType = HitSuccessType.Injured;

            return result;
        }

        public string GetPointGridString()
        {
            StringBuilder sb = new();
            sb.Append("   ");

            for (int i = 0; i < Const.ColsAmount; i++)
            {
                sb.Append(Const.Letters[i] + " ");
            }

            sb.Append(Environment.NewLine);

            for (int rowIndex = 1; rowIndex <= Const.ColsAmount; rowIndex++)
            {
                sb.Append(rowIndex.ToString("D2") + " ");

                for (int i = 0; i < Const.ColsAmount; i++)
                {
                    sb.Append(Points[rowIndex - 1, i]);
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public string GetLegendInfo()
        {
            return $"{Const.NotHit}- not hit; {Const.Missed}- missed; {Const.Injured}- injured; {Const.Destroyed}- destroyed.";
        }

        private void GenerateShip(Ship ship)
        {
            Point startingPoint;
            do
            {
                startingPoint = Points[_random.Next(0, Const.ColsAmount), _random.Next(0, Const.ColsAmount)];
            }
            while (!IsPointValidForShip(startingPoint, ship));
        }

        private void Init()
        {
            for (int rowIndex = 0; rowIndex < Const.ColsAmount; rowIndex++)
            {
                for (int i = 0; i < Const.ColsAmount; i++)
                {
                    Points[rowIndex, i] = new Point(rowIndex + 1, i + 1);
                }
            }
        }  

        private void AssignPointsToShip(IEnumerable<Point> points, Ship ship)
        {
            foreach (var point in points)
            {
                ship.Assign(point);
                ShipPointsDictionary[point.GetHashCode()] = ship;
            }
        }

        private bool IsPointValidForShip(Point startPoint, Ship ship)
        {
            List<Point> potentialPoints;

            if (CanPlaceShipRight(startPoint, ship.Size))
            {
                potentialPoints = GetPointsRight(startPoint, ship.Size);

                if (potentialPoints.Count < ship.Size)
                    return false;

                if (SurroundingPointsAreFree(potentialPoints))
                {
                    AssignPointsToShip(potentialPoints, ship);
                    return true;
                }
            }

            if (CanPlaceShipDown(startPoint, ship.Size))
            {
                potentialPoints = GetPointsDown(startPoint, ship.Size);

                if (potentialPoints.Count < ship.Size)
                    return false;

                if (SurroundingPointsAreFree(potentialPoints))
                {
                    AssignPointsToShip(potentialPoints, ship);
                    return true;
                }
            }

            return false;
        }

        private bool CanPlaceShipRight(Point startPoint, int size)
        {
            return startPoint.Y + size - 1 <= Const.ColsAmount;
        }

        private bool CanPlaceShipDown(Point startPoint, int size)
        {
            return startPoint.X + size - 1 <= Const.ColsAmount;
        }

        private List<Point> GetPointsRight(Point startPoint, int size)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < size; i++)
            {
                int newY = startPoint.Y - 1 + i;
                if (newY < Const.ColsAmount)
                {
                    points.Add(Points[startPoint.X - 1, newY]);
                }
            }
            return points;
        }

        private List<Point> GetPointsDown(Point startPoint, int size)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < size; i++)
            {
                int newX = startPoint.X - 1 + i;
                if (newX < Const.ColsAmount)
                {
                    points.Add(Points[newX, startPoint.Y - 1]);
                }
            }
            return points;
        }

        private bool SurroundingPointsAreFree(List<Point> shipPoints)
        {
            foreach (var point in shipPoints)
            {
                // Check above
                if (point.X > 0 && IsWithinGrid(point.X - 2, point.Y - 1) && Points[point.X - 2, point.Y - 1].IsAssignedToShip)
                {
                    return false;
                }

                // Check below
                if (point.X < Const.ColsAmount && IsWithinGrid(point.X, point.Y - 1) && Points[point.X, point.Y - 1].IsAssignedToShip)
                {
                    return false;
                }

                // Check left
                if (point.Y > 0 && IsWithinGrid(point.X - 1, point.Y - 2) && Points[point.X - 1, point.Y - 2].IsAssignedToShip)
                {
                    return false;
                }

                // Check right
                if (point.Y < Const.ColsAmount && IsWithinGrid(point.X - 1, point.Y) && Points[point.X - 1, point.Y].IsAssignedToShip)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsWithinGrid(int x, int y)
        {
            return x >= 0 && x < Const.ColsAmount && y >= 0 && y < Const.ColsAmount;
        }

        private static (int X, int Y, HitErrorType HitErrorType) ValidateCoordinates(string coordinates)
        {
            coordinates = coordinates.ToUpper().Trim();
            if (string.IsNullOrWhiteSpace(coordinates) || coordinates.Length < 2 ||
              coordinates.Length > 3 || !char.IsLetter(coordinates[0]) ||
              !char.IsDigit(coordinates[1]) || (coordinates.Length == 3 && !char.IsDigit(coordinates[2])))
                return new(0, 0, HitErrorType.NotValid);

            char xC = coordinates[0];

            int x = int.Parse(coordinates[1..]);

            if (x < 1 || x > Const.ColsAmount || !Const.Letters.Contains(xC))
                return new(0, 0, HitErrorType.OutOfRange);

            int y = Array.IndexOf(Const.Letters, xC) + 1;

            return new(x, y, HitErrorType.None);
        }
    }
}
