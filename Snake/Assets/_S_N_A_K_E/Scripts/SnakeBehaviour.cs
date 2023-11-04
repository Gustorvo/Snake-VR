using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gustorvo.Snake
{
    public interface ISnake
    {
        public Transform TailTransform { get; }

        public Transform HeadTransform { get; }

        IMover Mover { get; }
        bool HasReachedTarget { get; }
        ITarget Target { get; set; }
        void Move();
    }

    public class SnakeBehaviour : MonoBehaviour, ISnake
    {
        public List<Transform> snakeParts = new List<Transform>();
        public IMover Mover { get; private set; } = new Mover();

        public ITarget Target
        {
            get => target;
            set => target = value;
        }

        private ITarget target;

        readonly INavigator navigator = new Navigator();
        private Vector3 currentPosition;

        public bool HasReachedTarget =>
            Vector3.Distance(TailTransform.position, Target.Position) < Mover.MoveStep;


        private void Awake()
        {
            snakeParts = GetComponentsInChildren<Transform>().ToList();
            HeadTransform = snakeParts.First();
            TailTransform = snakeParts.Last();
        }

        public void Move()
        {
            if (navigator.Navigate(in currentPosition, in target, out var navData))
                Mover.Move(GetNextPartToMove(), navData.Direction);
        }


        private int tailIndex = 0;

        public Transform GetNextPartToMove()
        {
            tailIndex = (tailIndex + 1) % snakeParts.Count;
            TailTransform = snakeParts[tailIndex];
            return TailTransform;
        }

        private Transform tail;

        public Transform TailTransform
        {
            get => tail;
            set { tail = value; }
        }

        public Transform HeadTransform { get; private set; }
    }
}