using System;
using System.Collections.Generic;
using System.Linq;
using Gustorvo.Snake.Input;
using UnityEngine;

namespace Gustorvo.Snake
{
    public interface IPositioner
    {
        public Vector3[] GetDirections(Transform transform, bool local);
        public Vector3 GetPosition();
        public List<Vector3> GetMovePositions();
        public Vector3 GetPositionInDirection(Vector3 direction, bool local);
    }

    public class Positioner : IPositioner
    {
        public Vector3[] GetDirections(Transform transform, bool local)
        {
            throw new NotImplementedException();
        }

        public Vector3 GetPosition()
        {
            Vector3 direction = SnakeMoveDirection.Direction;
            return GetPositionInDirection(direction);
        }

        public List<Vector3> GetMovePositions()
        {
            throw new NotImplementedException();
        }

        public Vector3 GetPositionInDirection(Vector3 direction, bool local = false)
        {
            if (local)
                direction = SnakeBehaviour.Head.transform.TransformDirection(direction);
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
        Quaternion rotation => Core.PlayBoundary.transform.rotation;

        public Vector3 forward =>
            rotation * (local ? transform.InverseTransformDirection(transform.forward) : transform.forward);

        public Vector3 left =>
            rotation * (local ? transform.InverseTransformDirection(-transform.right) : -transform.right);

        public Vector3 right =>
            rotation * (local ? transform.InverseTransformDirection(transform.right) : transform.right);

        public Vector3 up => rotation * (local ? transform.InverseTransformDirection(transform.up) : transform.up);

        public Vector3 down => rotation * (local ? transform.InverseTransformDirection(-transform.up) : -transform.up);

        public Vector3[] ToArray()
        {
            return new[] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down };
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

        public Vector3[] GetDirections(Transform transform, bool local) =>
            new MoveDirections(local, transform).ToArray();


        public List<Vector3> GetMovePositions()
        {
            Transform headTransform = snake.head.Transform;
            // Get possible directions
            moveDirections = new MoveDirections(true, headTransform);

            List<Vector3> possibleDirections = new List<Vector3>(moveDirections.ToArray());

            // Apply world rotation of Play boundary
            var rotation = Core.PlayBoundary.transform.rotation;
            possibleDirections = possibleDirections.Select(direction => rotation * direction).ToList();

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

        public Vector3 GetPositionInDirection(Vector3 direction, bool local = false) =>
            positioner.GetPositionInDirection(direction, local);
    }
}