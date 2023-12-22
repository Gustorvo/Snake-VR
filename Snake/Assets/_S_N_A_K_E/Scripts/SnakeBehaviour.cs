using System;
using System.Collections.Generic;
using System.Linq;
using Gustorvo.Snake.Input;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gustorvo.Snake
{
    public interface ISnake
    {
        public Vector3[] Positions { get; }
        public Vector3 Direction { get; }
        public SnakeBody Tail { get; }
        public static SnakeBody Head { get; }

        IPositioner Positioner { get; }
        bool HasReachedTarget { get; }
        bool CanMove { get; }
        ITarget Target { get; set; }
        void Move();
        void MoveInOppositeDirection();
        void TakeTarget();
        void Init();
    }

    public class SnakeBehaviour : MonoBehaviour, ISnake
    {
        [SerializeField] private bool drawGizmos = false;
        [SerializeField] private SnakeBody snakeBodyPrefab;
        [SerializeField] SnakeBody headPointer;
        [SerializeField] SnakeBody tailPointer;
        public List<SnakeBody> snakeParts = new();
        public IPositioner Positioner { get; private set; } = new AIPositioner();

        public bool CanMove { get; private set; } = true;
        public ITarget Target { get; set; }


        // readonly INavigator navigator = new Navigator();
        private Vector3 currentPosition;

        private float distanceToTarget => Vector3.Distance(Head.Position, Target.Position);
        private bool targetValid => Target != null && Target.Transform != null;

        public bool HasReachedTarget =>
            targetValid && (distanceToTarget - Core.CellSize) <= Core.DistanceTolerance;


        private List<Vector3> nextPositions = new List<Vector3>();

        private void Awake()
        {
            Init();
           // AlignToGrid();
        }

        private void Start()
        {
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

        public void TakeTarget()
        {
            Vector3 targetPosition = Target.Position;
            Target.Reposition();
            SnakeBody newHead = Instantiate(snakeBodyPrefab, parent: transform, position: targetPosition,
                rotation: Quaternion.identity);
            snakeParts.Insert(headIndex, newHead);
            Head.ApplyBodyMaterial();
            Vector3 headDirection = Head.Position - targetPosition;
            Head = newHead;
            Head.Transform.forward = headDirection;
            Head.ApplyHeadMaterial();
            Debug.Log("target taken");
        }

        public void Move()
        {
            nextPositions = Positioner.GetMovePositions();
            CanMove = nextPositions.Count > 0;
            if (!CanMove) return;
            Tail.TryMoveTo(nextPositions[0], out bool hasCollided);
            CanMove = !hasCollided;
            if (CanMove)
            {
                Head.ApplyBodyMaterial();
            }

            Vector3 headDirection = Tail.Position - Head.Position;
            Head = Tail;
            Head.Transform.forward = headDirection;
            Tail = snakeParts[GetPreviousIndex(tailIndex)];
            Tail.ApplyTailMaterial();
        }

        public void MoveInOppositeDirection()
        {
            var nextPosition = Positioner.GetPosition();
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
        public Vector3 Direction => Head.Transform.forward;
        public Vector3 DirectionLocal => Head.Transform.InverseTransformDirection(Direction);

        public SnakeBody Tail
        {
            get => tail;
            set { tail = value; }
        }

        public static SnakeBody Head
        {
            get => instance?.head;
            set => instance.head = value;
        }

        private static SnakeBehaviour instance { get; set; }

        public SnakeBody head;

        [Button]
        private void AlignToGrid()
        {
            for (int i = 0; i < snakeParts.Count; i++)
            {
                Vector3 neatestPositionInGrid = Core.PlayBoundary.GetNearestPositionInGrid(snakeParts[i].Position);
                snakeParts[i].MoveTo(neatestPositionInGrid);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos) return;
            // draw possible next positions
            Gizmos.color = Color.green;
            for (int i = 0; i < nextPositions.Count; i++)
            {
                Vector3 pos = nextPositions[i];
                // draw a green sphere at the position
                Gizmos.DrawWireSphere(pos, Core.CellSize * 0.5f);
            }


            // draw head direction
            if (Head != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(Head.Position, Head.Position + Head.Transform.forward);
            }
        }
    }
}