using System;
using System.Collections.ObjectModel;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gustorvo.Snake
{
    public class PlayBoundary : MonoBehaviour
    {
        [SerializeField] private Transform debugCube;
        [SerializeField] bool showGrid;
        [SerializeField] Transform debugCubeParent;


        // private Vector3[] cellArray;
        private ReadOnlyCollection<Vector3> readOnlyCellArray;

        public ReadOnlyCollection<Vector3> CellPositions
        {
            get
            {
                if (readOnlyCellArray == null)
                {
                    readOnlyCellArray = BuildGridOfCellsWithinBounds();
                }

                return readOnlyCellArray;
            }
        }


        private void Awake()
        {
            BuildGridOfCellsWithinBounds();
        }

        [SerializeField, Range(1, 2)] private float boxSize;
        [ShowNativeProperty] Bounds gameBounds => new Bounds(Vector3.zero, Vector3.one * boxSize);
        [ShowNativeProperty] int itemsInRow => Mathf.CeilToInt(gameBounds.size.x / cellSize);

        [ShowNativeProperty] private float cellSize => Core.CellSize;

        private ReadOnlyCollection<Vector3> BuildGridOfCellsWithinBounds()
        {
            float halfCellSize = cellSize / 2;
            Vector3 offset = new Vector3(halfCellSize, halfCellSize, halfCellSize);
            int totalItemsInBoundingBox = itemsInRow * itemsInRow * itemsInRow;
            Vector3[] cellArray = new Vector3[totalItemsInBoundingBox];
            for (int i = 0; i < totalItemsInBoundingBox; i++)
            {
                int j = i % itemsInRow;
                int k = (i / itemsInRow) % itemsInRow;
                int l = i / (itemsInRow * itemsInRow);

                float x = gameBounds.min.x + cellSize * j;
                float y = gameBounds.min.y + cellSize * k;
                float z = gameBounds.min.z + cellSize * l;

                cellArray[i] = new Vector3(x, y, z) + offset;
            }

            return Array.AsReadOnly(cellArray);
        }

        [Button]
        void InstantiateCubes()
        {
            for (int i = 0; i < CellPositions.Count; i++)
            {
                Instantiate(debugCube, position: CellPositions[i], rotation: Quaternion.identity,
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
            Vector3 tempPosition = Vector3.zero;

            if (excludePositions.Length >= CellPositions.Count)
            {
                Debug.LogError("Too many positions to exclude");
                return false;
            }

            int i = 0;
            do
            {
                i++;
                int randomIndex = Random.Range(0, CellPositions.Count());
                tempPosition = CellPositions[randomIndex];
            } while (excludePositions.Any(x => x.AlmostEquals(tempPosition, 0.0001f)) && i < 100);

            if (i >= 100)
            {
                Debug.LogError("Failed to get random position");
                return false;
            }

            randomPosition = tempPosition;
            PrintSomeDebug();


            return true;

            void PrintSomeDebug()
            {
                // find the number of all items in cell array that are almost equal to excludePositions array
                int count = CellPositions.Count(cell =>
                    excludePositions.Any(excludePosition => excludePosition.AlmostEquals(cell, 0.0001f)));
                Debug.Log($"Number of items in cell array that are almost equal to excludePositions array: {count}");
            }
        }

        public bool IsPositionInBounds(Vector3 postion)
        {
            return gameBounds.Contains(postion);
        }
        public Vector3 GetNearestPositionInGrid(Vector3 position)
        {
            return CellPositions.OrderBy(cell => (cell - position).sqrMagnitude).First();
        }

        private void OnDrawGizmos()
        {
            // draw bounds
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(gameBounds.center, gameBounds.size);
        }

    }
}