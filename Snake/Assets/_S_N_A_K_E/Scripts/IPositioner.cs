using System;
using System.Collections.Generic;
using System.Linq;
using Gustorvo.Snake.Input;
using UnityEngine;

namespace Gustorvo.Snake
{
    public interface IPositioner
    {
        public Vector3 GetPosition();
        public List<Vector3> GetPositions();
        public Vector3 GetPositionInDirection(Vector3 direction);
    }

    public class Positioner : IPositioner
    {
        public Vector3 GetPosition()
        {
            Vector3 direction = SnakeMoveDirection.Direction;
            return GetPositionInDirection(direction);
        }

        public List<Vector3> GetPositions()
        {
            throw new NotImplementedException();
        }

        public Vector3 GetPositionInDirection(Vector3 direction)
        {
            Vector3 headPosition = SnakeBehaviour.Head.Position;
            return direction * MoveStep + headPosition;
        }

        public float MoveStep => Core.CellSize;
    }

    public class AIPositioner : IPositioner
    {
        public IPositioner positioner = new Positioner();
        PlayBoundary boundary => Core.PlayBoundary;
        SnakeBehaviour snake => Core.Snake;

        private Vector3[] moveDirections =
            { Vector3.forward, Vector3.back, Vector3.left, Vector3.right, Vector3.up, Vector3.down };

        private Vector3 previousDirection;

        public Vector3 GetPosition()
        {
            // Vector3 direction = SnakeMoveDirection.Direction;
            Vector3 direction = GetStraightDirectionToTarget(snake.Target.Position);
            Vector3 nextPosition = GetPositionInDirection(direction);

            if (boundary.IsPositionInBounds(nextPosition) && !IsSnakePosition(nextPosition))
            {
                previousDirection = direction;
                return nextPosition;
            }

            List<Vector3> possibleDirections = new List<Vector3>(moveDirections);

            possibleDirections.Remove(-direction);
            do
            {
                possibleDirections.Remove(direction);
                int randomPositionIndex = UnityEngine.Random.Range(0, possibleDirections.Count);
                direction = possibleDirections[randomPositionIndex];
                nextPosition = GetPositionInDirection(direction);
            } while (!IsNextMoveWithinBounds(direction) && !IsSnakePosition(nextPosition) &&
                     possibleDirections.Count > 0);

            if (possibleDirections.Count == 0)
                Debug.LogError("No possible directions");
            SnakeMoveDirection.Direction = direction;
            previousDirection = direction;
            return nextPosition;

            bool IsNextMoveWithinBounds(Vector3 dir)
            {
                Vector3 nextPos = GetPositionInDirection(dir);
                return boundary.IsPositionInBounds(nextPos);
            }
        }

       private bool IsSnakePosition(Vector3 pos)
        {
            bool isSnakePosition = snake.Positions.Any(p => p.AlmostEquals(pos, 0.0001f));
            return isSnakePosition;
        }

        public List<Vector3> GetPositions()
        {
            // Get possible directions
            List<Vector3> possibleDirections = new List<Vector3>(moveDirections);
            possibleDirections.Remove(-Core.Snake.Direction);
            
            // find shortest direction
            var shortestDirection = GetStraightDirectionToTarget(snake.Target.Position);
           
            // make shortest direction first
            possibleDirections.Remove(shortestDirection);
            possibleDirections.Insert(0, shortestDirection);

            // Get positions for each direction
            List<Vector3> positions = new List<Vector3>(possibleDirections.Count);
            for (int i = 0; i < possibleDirections.Count; i++)
            {
                positions.Add(GetPositionInDirection(possibleDirections[i]));
            }

            // Remove positions that are out of bounds
            positions = positions.Where(p => boundary.IsPositionInBounds(p)).ToList();

            // Remove positions that are occupied by the snake
            positions = positions.Where(p => !IsSnakePosition(p)).ToList();

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

    }


    public struct PositionData
    {
        Vector3 direction;
        Vector3 nextPosition;
    }
}