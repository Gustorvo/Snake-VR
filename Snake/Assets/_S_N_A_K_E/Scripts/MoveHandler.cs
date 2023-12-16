using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;
using UnityEditor;

namespace Gustorvo.Snake
{
    public class MoveHandler : MonoBehaviour
    {
        [SerializeField, Range(1, 120)] int movesPerSecond = 1;
        [SerializeField] private SnakeBehaviour snakeBehaviourBehaviour;
        [SerializeField] private SnakeTarget snakeTargetBehaviour;
        [SerializeField] Coroutine snakeMoveCoroutine;

        private Vector3 snakeCurrentPosition;


        public ISnake Snake { get; private set; }

        private void Awake()
        {
            Init();
        }


        private void Start()
        {
            Assert.IsNotNull(snakeBehaviourBehaviour, "SnakeMoverBehaviour is not set");
            snakeMoveCoroutine = StartCoroutine(MoveSnake());
        }

        void Init()
        {
            Snake = snakeBehaviourBehaviour;
            Snake.Food = snakeTargetBehaviour;
        }

        void InitAllEditor()
        {
            Init();
            Snake.Init();
        }

        [Button]
        private void UndoMove()
        {
            
        }

        [Button]
        public bool TryMove()
        {
            // InitAllEditor();

            if (Snake.HasReachedFood)
            {
                Snake.EatFood();
            }
            else
            {
                Snake.Move();
            }


#if UNITY_EDITOR
            if (EditorApplication.isPaused && !Snake.CanMove)
            {
                Debug.LogError("Snake can't move");
            }
#endif

            return Snake.CanMove;
        }


        [Button]
        public void MoveOpposite()
        {
            InitAllEditor();
            Snake.MoveInOppositeDirection();
        }

        IEnumerator MoveSnake()
        {
            bool canMove = true;
            while (canMove)
            {
                canMove = TryMove();
                yield return new WaitForSeconds(1f / movesPerSecond);
            }

            Debug.Log("Snake died");
        }
    }
}