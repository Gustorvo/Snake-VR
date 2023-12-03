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
            Vector3 direction = SnakeMoveDirection.Direction;
            Vector3 nextPosition = positioner.GetNextPosition();
            if (boundary.IsPositionInBounds(nextPosition))
            {
                previousDirection = direction;
                return nextPosition;
            }

            List<Vector3> possibleDirections = new List<Vector3>();
            possibleDirections.AddRange(moveDirections.ToArray());
            possibleDirections.Remove(direction);
            for (int i = 0; i < possibleDirections.Count; i++)
            {
                direction = possibleDirections[i];
                if (direction == previousDirection)
                    continue;
                nextPosition = GetNextPositionInDirection(direction);
                if (boundary.IsPositionInBounds(nextPosition))
                {
                    SnakeMoveDirection.Direction = direction;
                    previousDirection = direction;
                    return nextPosition;
                }
            }

            Debug.Log("No possible positions found! Returning 'false' position");
            return nextPosition;
        }

        public Vector3 GetNextPositionInDirection(Vector3 direction) =>
            positioner.GetNextPositionInDirection(direction);


        public float MoveStep { get; set; } = 0.1f;
    }
}