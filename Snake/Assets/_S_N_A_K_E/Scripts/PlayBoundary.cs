using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gustorvo.Snake
{
    public class PlayBoundary : MonoBehaviour
    {
        [SerializeField] private Transform debugCube;
        [SerializeField] bool debug;
        [SerializeField] Transform debugCubeParent;
        [SerializeField] private float snakeBodySize = 0.1f;
        [SerializeField] int gridSize = 1;
        [SerializeField] private BoxCollider collider;
        private Bounds gameBounds => collider.bounds;
        private int unitLength;


        private Vector3[][][] gameBoundsPositions;

        private void Awake()
        {
          
            unitLength = (int)(gridSize / snakeBodySize);

            int numOfPositions = unitLength * 3;
            gameBoundsPositions = new Vector3[numOfPositions][][];

            for (int i = 0; i < numOfPositions; i++)
            {
                gameBoundsPositions[i] = new Vector3[unitLength][];
                for (int j = 0; j < unitLength; j++)
                {
                    gameBoundsPositions[i][j] = new Vector3[unitLength];
                    for (int k = 0; k < unitLength; k++)
                    {
                        Vector3 pos = new Vector3(
                            gameBounds.min.x + snakeBodySize * j,
                            gameBounds.min.y + snakeBodySize * k,
                            gameBounds.min.z + snakeBodySize * i
                        );
                        gameBoundsPositions[i][j][k] = pos;
                        if (debug)
                        {
                            var cube = Instantiate(debugCube, position: pos, rotation: Quaternion.identity,
                                parent: debugCubeParent);
                            cube.localScale = Vector3.one * snakeBodySize;
                        }
                    }
                }
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
            if (excludePositions.Length >= unitLength * unitLength * unitLength)
            {
                return false;
            }

            int i = 0;
            do
            {
                i++;
                int indexX = Random.Range(0, unitLength);
                int indexY = Random.Range(0, unitLength);
                int indexZ = Random.Range(0, unitLength);
                randomPosition = gameBoundsPositions[indexX][indexY][indexZ];
            } while (excludePositions.Contains(randomPosition) && i < 100);

            if (i >= 100)
            {
                return false;
            }

            return true;
        }
    }
}