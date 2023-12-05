using System;
using System.Collections.Generic;
using System.Linq;
using Gustorvo.Snake.Input;
using UnityEngine;

namespace Gustorvo.Snake
{
    public interface IPositioner
    {
        public Vector3 GetNextPosition();
        public Vector3 GetNextPositionInDirection(Vector3 direction);
        public float MoveStep { get; set; }
    }

    public class Positioner : IPositioner
    {
        public Vector3 GetNextPosition()
        {
            Vector3 direction = SnakeMoveDirection.Direction;
            return GetNextPositionInDirection(direction);
        }

        public Vector3 GetNextPositionInDirection(Vector3 direction)
        {
            Vector3 headPosition = SnakeBehaviour.Head.Position;
            return direction * MoveStep + headPosition;
        }

        public float MoveStep { get; set; } = 0.1f;
    }

    public class AIPositioner : IPositioner
    {
        public IPositioner positioner = new Positioner();
        PlayBoundary boundary => Core.PlayBoundary;
        SnakeBehaviour snake => Core.Snake;

        private Vector3[] moveDirections =
            { Vector3.forward, Vector3.back, Vector3.left, Vector3.right, Vector3.up, Vector3.down };

        private Vector3 previousDirection;

        public Vector3 GetNextPosition()
        {
            // Vector3 direction = SnakeMoveDirection.Direction;
            Vector3 direction = GetStraightDirectionToTarget(snake.Food.Position);
            Vector3 nextPosition = positioner.GetNextPositionInDirection(direction);

            if (boundary.IsPositionInBounds(nextPosition) && !IsNextMoveCollideWithSnakeBody(direction))
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
            } while (!IsNextMoveWithinBounds(direction) && !IsNextMoveCollideWithSnakeBody(direction) &&
                     possibleDirections.Count > 0);

            if (possibleDirections.Count == 0)
                Debug.LogError("No possible directions");
            SnakeMoveDirection.Direction = direction;
            previousDirection = direction;
            return GetNextPositionInDirection(direction);

            bool IsNextMoveWithinBounds(Vector3 dir)
            {
                Vector3 nextPos = GetNextPositionInDirection(dir);
                return boundary.IsPositionInBounds(nextPos);
            }

            bool IsNextMoveCollideWithSnakeBody(Vector3 dir)
            {
                Vector3 pos = positioner.GetNextPositionInDirection(dir);
                return Core.Snake.Positions.Contains(pos);
            }
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

        public Vector3 GetNextPositionInDirection(Vector3 direction) =>
            positioner.GetNextPositionInDirection(direction);


        public float MoveStep { get; set; } = 0.1f;
    }
}