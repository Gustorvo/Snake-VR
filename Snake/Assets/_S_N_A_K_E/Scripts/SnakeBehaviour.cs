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
        public SnakeBody Head { get; }

        IMover Mover { get; }
        bool HasReachedFood { get; }
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
        public IMover Mover { get; private set; } = new Mover();

        public ITarget Food { get; set; }


        readonly INavigator navigator = new Navigator();
        private Vector3 currentPosition;

        public bool HasReachedFood => Food != null && Food.Transform != null &&
                                      Vector3.Distance(Head.Position, Food.Position) < Mover.MoveStep;


        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            // if (snakeParts.Count > 0) return;
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
            Vector3 Direction = SnakeMoveDirection.Direction;
            SnakeBody tail = GetTailPart(out int tailIndex);
            Mover.Move(tail, Direction, Head.Position);
            Head = tail;
            Tail = snakeParts[GetPreviousIndex(tailIndex)];
        }

        public void MoveInOppositeDirection()
        {
            Vector3 Direction = -SnakeMoveDirection.Direction;
            SnakeBody head = snakeParts[headIndex];
            Mover.Move(head, Direction, Tail.Position);
            var temp = head;
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

        public SnakeBody Head
        {
            get => head;
            set => head = value;
        }

        public SnakeBody head;
    }
}