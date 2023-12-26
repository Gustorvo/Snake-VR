using System;
using System.Collections.Generic;
using System.Linq;
using Gustorvo.Snake.Input;
using UnityEngine;

namespace Gustorvo.Snake
{
    public interface IPositioner
    {
        public bool TryGetMovePosition(out Vector3 movePosition);
        public IEnumerable<Vector3> GetMovePositions();
        public Vector3 GetPositionInDirection(Vector3 direction);
        bool IsPositionValid(Vector3 newPos);
    }

    public class Positioner : IPositioner
    {
        public bool TryGetMovePosition(out Vector3 movePosition)
        {
            Vector3 direction = SnakeMoveDirection.Direction;
            movePosition = GetPositionInDirection(direction);
            return IsPositionValid(movePosition);
        }

        public IEnumerable<Vector3> GetMovePositions()
        {
            throw new NotImplementedException();
        }

        public Vector3 GetPositionInDirection(Vector3 direction)
        {
            return SnakeBehaviour.Head.Position + direction * MoveStep;
        }

        public bool IsPositionValid(Vector3 newPos)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public float MoveStep => Core.CellSize;
    }


    public class AIPositioner : IPositioner
    {
        private List<Vector3> possibleDirections = new()
            { Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down };

        public IPositioner positioner = new Positioner();
        PlayBoundary boundary => Core.PlayBoundary;
        SnakeBehaviour snake => Core.Snake;

        private Vector3 previousDirection;


        private bool IsSnakePosition(Vector3 pos)
        {
            bool isSnakePosition = snake.Positions.Any(p => p.AlmostEquals(pos, 0.0001f));
            return isSnakePosition;
        }

        public Vector3[] GetDirections(Transform transform, bool local)
        {
            throw new NotImplementedException();
        }

        public bool TryGetMovePosition(out Vector3 movePosition)
        {
            movePosition = GetMovePositions().FirstOrDefault();
            return IsPositionValid(movePosition);
        }


        public IEnumerable<Vector3> GetMovePositions()
        {
            // Apply world rotation of Play boundary
            var rotation = Core.PlayBoundary.transform.rotation;
            var moveDirections = possibleDirections.Select(direction => rotation * direction);

            // Get positions for each direction
            var positions = moveDirections.Select(direction => GetPositionInDirection(direction));

            // Remove positions that are out of bounds
            positions = positions.Where(p => boundary.IsPositionInBounds(p));

            // Remove positions that are occupied by the snake
            positions = positions.Where(p => !IsSnakePosition(p));

            //sort position by distance to target
            positions = positions.OrderBy(p => Vector3.Distance(snake.Target.Position, p));
            if (positions.Count() == 0)
            {
                Debug.LogError("No possible positions");
            }

            return positions;
        }


        private Vector3 GetStraightDirectionToTarget(Vector3 target)
        {
            Vector3 dirVector = (target - SnakeBehaviour.Head.Position).normalized;
            int largestIndexAbs = 0;
            for (int i = 0; i < 3; i++)
            {
                largestIndexAbs = Mathf.Abs(dirVector[i]) > Mathf.Abs(dirVector[largestIndexAbs]) ? i : largestIndexAbs;
            }

            float dirAxis = dirVector[largestIndexAbs] > 0 ? 1 : -1;
            dirVector = Vector3.zero;
            dirVector[largestIndexAbs] = dirAxis;
            return dirVector;
        }

        public Vector3 GetPositionInDirection(Vector3 direction) =>
            positioner.GetPositionInDirection(direction);

        public bool IsPositionValid(Vector3 newPos)
        {
            return newPos != default
                   && boundary.IsPositionInBounds(newPos)
                   && !IsSnakePosition(newPos);
        }
    }
}