using System;
using NaughtyAttributes;
using UnityEngine;

namespace Gustorvo.Snake
{
    public interface ITarget
    {
        Vector3 Position { get; }
        Transform Transform { get; }

        void Reposition();
    }

    public class SnakeTarget : MonoBehaviour, ITarget
    {
        [SerializeField] bool repositionOnStart = true;
        public Vector3 Position => transform.position;
        public Transform Transform => transform;

        private void Start()
        {
            if (repositionOnStart)
                Reposition();
        }

        [Button]
        public void Reposition()
        {
            var snakePositions = Core.Snake.Positions;
            if (Core.PlayBoundary.TryGetRandomPositionExcluding(snakePositions, out var randomPosition))
            {
                transform.position = randomPosition;
            }
        }
    }
}