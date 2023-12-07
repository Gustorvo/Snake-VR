using System;
using System.Collections.Generic;
using System.Linq;
using Gustorvo.Snake.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gustorvo.Snake
{
    public interface ISnake
    {
        public Vector3[] Positions { get; }
        public SnakeBody Tail { get; }
        public static SnakeBody Head { get; }

        IPositioner Positioner { get; }
        bool HasReachedFood { get; }
        bool HasCollidedWithItself { get; }
        ITarget Food { get; set; }
        void Move();
        void MoveInOppositeDirection();
        void EatFood();
        void Init();
    }

    public class SnakeBehaviour : MonoBehaviour, ISnake
    {
        [SerializeField] private SnakeBody snakeBodyPrefab;
        [SerializeField] SnakeBody headPointer;
        [SerializeField] SnakeBody tailPointer;
        public List<SnakeBody> snakeParts = new();
        public IPositioner Positioner { get; private set; } = new AIPositioner();

        public bool HasCollidedWithItself { get; private set; } 
        public ITarget Food { get; set; }


        readonly INavigator navigator = new Navigator();
        private Vector3 currentPosition;

        public bool HasReachedFood => Food != null && Food.Transform != null &&
                                      Vector3.Distance(Head.Position, Food.Position) < Positioner.MoveStep;


        private void Awake()
        {
            Init();

            SnakeBodyComponent.OnSnakeCollidedWithItself -= SnakeDead;
            SnakeBodyComponent.OnSnakeCollidedWithItself += SnakeDead;
        }

        private void SnakeDead()
        {
            Debug.Log("Snake collided with itself. Dead!");
        }

        public void Init()
        {
            instance = this;

            snakeParts.Clear();
            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent(out SnakeBody body))
                {
                    body = child.gameObject.AddComponent<SnakeBody>();
                }

                snakeParts.Add(body);
            }

            Head = headPointer;
            Tail = tailPointer;
        }

        public void EatFood()
        {
            SnakeBody newHead = Instantiate(snakeBodyPrefab, parent: transform, position: Food.Position,
                rotation: Quaternion.identity);
            snakeParts.Insert(tailIndex, newHead);
            Head = newHead;
            Food.Reposition();
        }

        public void Move()
        {
            bool canMove = true;
            var nextPosition = Positioner.GetNextPosition();
            Tail.TryMoveTo(nextPosition, out canMove);
            HasCollidedWithItself = !canMove;
            Head = Tail;
            Tail = snakeParts[GetPreviousIndex(tailIndex)];
        }

        public void MoveInOppositeDirection()
        {
            var nextPosition = Positioner.GetNextPosition();
            Head.MoveTo(nextPosition);
            Tail = head;
            Head = snakeParts[GetNextIndex(tailIndex)];
        }

        public int GetPreviousIndex(int index)
        {
            return (index - 1 + snakeParts.Count) % snakeParts.Count;
        }

        public int GetNextIndex(int index)
        {
            return (index + 1 + snakeParts.Count) % snakeParts.Count;
        }


        private int tailIndex => snakeParts.IndexOf(Tail);
        private int headIndex => snakeParts.IndexOf(Head);

        public SnakeBody GetTailPart(out int tailIndex)
        {
            tailIndex = snakeParts.IndexOf(Tail);
            return Tail;
        }

        public SnakeBody tail;

        public Vector3[] Positions => snakeParts.Select(x => x.Position).ToArray();

        public SnakeBody Tail
        {
            get => tail;
            set { tail = value; }
        }

        public static SnakeBody Head
        {
            get => instance.head;
            set => instance.head = value;
        }

        private static SnakeBehaviour instance { get; set; }

        public SnakeBody head;
    }
}