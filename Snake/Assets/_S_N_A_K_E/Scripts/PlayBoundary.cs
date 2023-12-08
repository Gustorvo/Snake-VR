using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gustorvo.Snake
{
    public class PlayBoundary : MonoBehaviour
    {
        [SerializeField] private Transform debugCube;
        [SerializeField] bool debug;
        [SerializeField] Transform debugCubeParent;

        // private int itemsInRow;
        private Vector3[] cellList;

        private void Awake()
        {
            CreateCellsWithinBounds();
        }

        [SerializeField, Range(1, 2)] private float boxSize;
        [ShowNativeProperty] Bounds gameBounds => new Bounds(Vector3.zero, Vector3.one * boxSize);
        [ShowNativeProperty] int itemsInRow => Mathf.CeilToInt(gameBounds.size.x / cellSize);

        [SerializeField, Range(0.05f, 1)] private float cellSize = 0.1f;

        private void CreateCellsWithinBounds()
        {
            float halfCellSize = cellSize / 2;
            Vector3 offset = new Vector3(halfCellSize, halfCellSize, halfCellSize);
            int totalItemsInBoundingBox = itemsInRow * itemsInRow * itemsInRow;
            cellList = new Vector3[totalItemsInBoundingBox];
            for (int i = 0; i < totalItemsInBoundingBox; i++)
            {
                int j = i % itemsInRow;
                int k = (i / itemsInRow) % itemsInRow;
                int l = i / (itemsInRow * itemsInRow);

                float x = gameBounds.min.x + cellSize * j;
                float y = gameBounds.min.y + cellSize * k;
                float z = gameBounds.min.z + cellSize * l;

                cellList[i] = new Vector3(x, y, z) + offset;
            }
        }

        [Button]
        void InstantiateCubes()
        {
            for (int i = 0; i < cellList.Length; i++)
            {
                Instantiate(debugCube, position: cellList[i], rotation: Quaternion.identity,
                    parent: debugCubeParent).localScale = Vector3.one * cellSize;
            }
        }


        public Vector3 GetRandomPosition(bool randomX = true, bool randomY = true, bool randomZ = true)
        {
            Vector3 randomPosition = new Vector3(
                randomX ? Random.Range(gameBounds.min.x, gameBounds.max.x) : 0f,
                randomY ? Random.Range(gameBounds.min.y, gameBounds.max.y) : 0f,
                randomZ ? Random.Range(gameBounds.min.z, gameBounds.max.z) : 0f
            );
            //if (randomPosition.y 
            return randomPosition;
        }

        public bool TryGetRandomPositionExcluding(Vector3[] excludePositions, out Vector3 randomPosition)
        {
            randomPosition = Vector3.zero;

            if (excludePositions.Length >= cellList.Length)
            {
                Debug.LogError("Too many positions to exclude");
                return false;
            }

            int i = 0;
            do
            {
                i++;
                int randomIndex = Random.Range(0, cellList.Length);
                randomPosition = cellList[randomIndex];
            } while (excludePositions.Contains(randomPosition) && i < 100);

            if (i >= 100)
            {
                Debug.LogError("Failed to get random position");
                return false;
            }

            return true;
        }

        public bool IsPositionInBounds(Vector3 postion)
        {
            return gameBounds.Contains(postion);
        }

        private void OnDrawGizmos()
        {
            // draw bounds
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(gameBounds.center, gameBounds.size);
        }
    }
}