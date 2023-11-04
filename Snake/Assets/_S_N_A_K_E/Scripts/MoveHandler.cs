using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

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


        private void Start()
        {
            Assert.IsNotNull(snakeBehaviourBehaviour, "SnakeMoverBehaviour is not set");

            Snake = snakeBehaviourBehaviour;
            Snake.Target = snakeTargetBehaviour;

            snakeMoveCoroutine = StartCoroutine(MoveSnake());
        }

        IEnumerator MoveSnake()
        {
            while (isActiveAndEnabled && !Snake.HasReachedTarget)
            {
                Snake.Move();
                yield return new WaitForSeconds(1f / movesPerSecond);
            }
        }
    }
}