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
        public Vector3 Position => transform.position;
        public Transform Transform => transform;
        public void Reposition() => transform.position = Core.PlayBoundary.GetRandomPosition();
    }
}