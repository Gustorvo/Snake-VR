using UnityEngine;

namespace Gustorvo.Snake
{
    public interface IMover
    {
        public void Move(Transform transform, Vector3 direction);
        public float MoveStep { get; set; }
    }

    public class Mover : IMover
    {
        public void Move(Transform transform, Vector3 direction)
        {
            Vector3 moveTo = direction * MoveStep + transform.position;
            transform.position = moveTo;
        }

        public float MoveStep { get; set; } = 0.1f;
    }
}