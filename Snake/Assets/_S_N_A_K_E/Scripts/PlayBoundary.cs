using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Gustorvo.Snake
{
    public class PlayBoundary : MonoBehaviour
    {
        [SerializeField] private Transform debugCube;
        [SerializeField] bool debug;
        [SerializeField] Transform debugCubeParent;
        [SerializeField] private float unitSize = 0.1f; // size of 1 body unit (of snake)
        [SerializeField] int gridSize = 1;
        [SerializeField] private BoxCollider collider;
        private Bounds gameBounds => collider.bounds;
        private int itemsInRow;
        private Vector3[] positionsInCube;

        private void Awake()
        {
            FormACube();
        }

        private void FormACube()
        {
            itemsInRow = (int)(gridSize / unitSize);

            int itemsInCube = itemsInRow * itemsInRow * itemsInRow;
            positionsInCube = new Vector3[itemsInCube];

            for (int i = 0; i < itemsInCube; i++)
            {
                int j = i % itemsInRow;
                int k = (i / itemsInRow) % itemsInRow;
                int l = i / (itemsInRow * itemsInRow);
                Vector3 pos = new Vector3(
                    gameBounds.min.x + unitSize * j,
                    gameBounds.min.y + unitSize * k,
                    gameBounds.min.z + unitSize * l
                );

                positionsInCube[i] = pos;
            }
        }

        [Button]
        void InstantiateCubes()
        {
            for (int i = 0; i < positionsInCube.Length; i++)
            {
                Instantiate(debugCube, position: positionsInCube[i], rotation: Quaternion.identity,
                    parent: debugCubeParent).localScale = Vector3.one * unitSize;
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

            if (excludePositions.Length >= positionsInCube.Length)
            {
                Debug.LogError("Too many positions to exclude");
                return false;
            }

            int i = 0;
            do
            {
                i++;
                int randomIndex = Random.Range(0, positionsInCube.Length);
                randomPosition = positionsInCube[randomIndex];
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
    }
}