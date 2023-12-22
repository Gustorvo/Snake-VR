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
        public List<Vector3> GetMovePositions();
        public Vector3 GetPositionInDirection(Vector3 direction);
    }

    public class Positioner : IPositioner
    {
        public Vector3 GetPosition()
        {
            Vector3 direction = SnakeMoveDirection.Direction;
            return GetPositionInDirection(direction);
        }

        public List<Vector3> GetMovePositions()
        {
            throw new NotImplementedException();
        }

        public Vector3 GetPositionInDirection(Vector3 direction)
        {
            Vector3 headPosition = SnakeBehaviour.Head.Position;
            return headPosition + direction * MoveStep;
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public float MoveStep => Core.CellSize;
    }

    public struct MoveDirections
    {
        private readonly bool local;
        private readonly Transform transform;
        public Vector3 Forward => local ? transform.InverseTransformDirection(Vector3.forward) : Vector3.forward;

       // public Vector3 Back => local ? transform.InverseTransformDirection(Vector3.back) : Vector3.back;

        public Vector3 Left => local ? transform.InverseTransformDirection(Vector3.left) : Vector3.left;

        public Vector3 Right => local ? transform.InverseTransformDirection(Vector3.right) : Vector3.right;

        public Vector3 Up => local ? transform.InverseTransformDirection(Vector3.up) : Vector3.up;

        public Vector3 Down => local ? transform.InverseTransformDirection(Vector3.down) : Vector3.down;

        public Vector3[] ToArray()
        {
            return new[] { Forward, Left, Right, Up, Down };
        }


        public MoveDirections(bool local, Transform transform)
        {
            this.local = local;
            this.transform = transform;
        }
    }

    public class AIPositioner : IPositioner
    {
        public IPositioner positioner = new Positioner();
        PlayBoundary boundary => Core.PlayBoundary;
        SnakeBehaviour snake => Core.Snake;

        private MoveDirections moveDirections;
        private Vector3 previousDirection;


        private bool IsSnakePosition(Vector3 pos)
        {
            bool isSnakePosition = snake.Positions.Any(p => p.AlmostEquals(pos, 0.0001f));
            return isSnakePosition;
        }

        public Vector3 GetPosition()
        {
            throw new NotImplementedException();
        }

        public List<Vector3> GetMovePositions()
        {
            Transform headTransform = snake.head.Transform;
            // Get possible directions
            moveDirections = new MoveDirections(true, headTransform);

            List<Vector3> possibleDirections = new List<Vector3>(moveDirections.ToArray());
          

            // find shortest direction
            //var shortestDirection = GetStraightDirectionToTarget(snake.Target.Position);
            //Vector3 shortestDirection = possibleDirections.

            // transform shortest direction from world to local
            //shortestDirection = headTransform.TransformDirection(shortestDirection);

            // make shortest direction first
            //possibleDirections.Remove(shortestDirection);
            //possibleDirections.Insert(0, shortestDirection);

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
            
            //sort position by distance to target
            positions = positions.OrderBy(p => Vector3.Distance(snake.Target.Position, p)).ToList();
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
}