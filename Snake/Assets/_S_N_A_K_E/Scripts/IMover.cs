using UnityEngine;

namespace Gustorvo.Snake
{
    public interface IMover
    {
        public void Move(SnakeBody body, Vector3 direction, Vector3 headPosition);
        public float MoveStep { get; set; }
    }

    public class Mover : IMover
    {
        public void Move(SnakeBody body, Vector3 direction, Vector3 headPosition)
        {
            Vector3 position = direction * MoveStep + headPosition;
            body.MoveTo(position);
        }

        public float MoveStep { get; set; } = 0.1f;
    }
}