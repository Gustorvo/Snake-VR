using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gustorvo.Snake
{
    public class PlayBoundary : MonoBehaviour
    {
        [SerializeField] private BoxCollider collider;
        private Bounds bounds => collider.bounds;

        public Vector3 GetRandomPosition()
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
            return randomPosition;
        }
    }
}