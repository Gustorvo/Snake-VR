using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;

namespace Gustorvo.Snake
{
    public class MoveHandler : MonoBehaviour
    {
        [SerializeField] int movesPerSecond = 1;
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
        public void Move()
        {
           // InitAllEditor();
            if (Snake.HasReachedFood)
                Snake.EatFood();
            Snake.Move();
        }

        [Button]
        public void MoveOpposite()
        {
            InitAllEditor();
            Snake.MoveInOppositeDirection();
        }

        IEnumerator MoveSnake()
        {
            while (isActiveAndEnabled)
            {
                Move();
                yield return new WaitForSeconds(1f / movesPerSecond);
            }
        }
    }
}