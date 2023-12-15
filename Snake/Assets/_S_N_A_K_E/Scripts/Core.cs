using UnityEngine;

namespace Gustorvo.Snake
{
    public class Core : MonoBehaviour
    {
        [SerializeField, Range(0.05f, 0.2f)] private float cellSize = 0.1f;
        [field: SerializeField] public PlayBoundary playBoundary { get; private set; }
        [field: SerializeField] public SnakeBehaviour snake { get; private set; }


        public static PlayBoundary PlayBoundary => Instance.playBoundary;
        public static SnakeBehaviour Snake => Instance.snake;
        public static float CellSize => Instance.cellSize;

        #region Singleton

        private static Core instance { get; set; }

        public static Core Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<Core>();
                return instance;
            }
            set => instance = value;
        }
        #endregion


        private void Awake()
        {
            Instance = this;
        }
    }
}