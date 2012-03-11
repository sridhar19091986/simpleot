using System;

namespace SimpleOT.Domain
{
    public class Position : IEquatable<Position>
    {
        public static readonly Position Invalid = new Position(-1, -1, -1);
		
		private int _x;
		private int _y;
		private int _z;
		private int _stack;

        public Position(int x, int y, int z, int stack = -1)
        {
			_x = x;
			_y = y;
			_z = z;
			_stack = stack;
        }

        public Position(Position position) : this(position._x, position._y, position._z, position._stack)
        {
        }

        public bool Equals(Position other)
        {
            return other._x == _x && other._y == _y && other._z == _z && other._stack == _stack;
        }

        public bool IsAdjacentTo(Position position)
        {
            return position._z == _z && Math.Max(Math.Abs(_x - position._x), Math.Abs(_y - position._y)) <= 1;
        }

        public double DistanceTo(Position position)
        {
            int xDist = _x - position._x;
            int yDist = _y - position._y;

            return Math.Sqrt(xDist*xDist + yDist*yDist);
        }

        public bool IsNextTo(Position second)
        {
            return IsNextTo(this, second);
        }

        public static bool IsNextTo(Position first, Position second)
        {
            if (first._z != second._z)
                return false;

            int dx = first._x - second._x;
            int dy = first._y - second._y;
            return dx <= 1 && dx >= -1 && dy <= 1 && dy >= -1;
        }

        public Position Offset(Direction direction)
        {
            return Offset(direction, 1);
        }

        public Position Offset(Direction direction, int amount)
        {
            int x = _x, y = _y, z = _z;

            switch (direction)
            {
                case Direction.North:
                    y -= amount;
                    break;
                case Direction.South:
                    y += amount;
                    break;
                case Direction.West:
                    x -= amount;
                    break;
                case Direction.East:
                    x += amount;
                    break;
                case Direction.NorthWest:
                    x -= amount;
                    y -= amount;
                    break;
                case Direction.SouthWest:
                    x -= amount;
                    y += amount;
                    break;
                case Direction.NorthEast:
                    x += amount;
                    y -= amount;
                    break;
                case Direction.SouthEast:
                    x += amount;
                    y += amount;
                    break;
            }

            return new Position(x, y, z, _stack);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (obj.GetType() != typeof (Position))
                return false;
            return Equals((Position) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _x;
                result = (result*397) ^ _y;
                result = (result*397) ^ _z;
                result = (result*397) ^ _stack;
                return result;
            }
        }
		
		public bool IsValid
        {
			get{return _x < 0 && _x >= 0xFFFF && _y < 0 && _y >= 0xFFFF && _z < 0 && _z >= Constants.MapMaxLayers;}
        }
		
		public int X{get{return _x;}set{_x = value;}}
		public int Y{get{return _y;}set{_y = value;}}
		public int Z{get{return _z;}set{_z = value;}}
		public int Stack{get{return _stack;}set{_stack = value;}}
		
        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !left.Equals(right);
        }

        public static bool AreInRange(int deltax, int deltay, int deltaz, Position p1, Position p2)
        {
            return Math.Abs(p1._x - p2._x) <= deltax && Math.Abs(p1._y - p2._y) <= deltay && Math.Abs(p1._z - p2._z) <= deltaz;
        }

        public static bool AreInRange(int deltax, int deltay, Position p1, Position p2)
        {
            return Math.Abs(p1._x - p2._x) <= deltax && Math.Abs(p1._y - p2._y) <= deltay;
        }
    }
}

