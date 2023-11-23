using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gustorvo.Snake.Input
{
    public class SnakeMoveDirection : MonoBehaviour
    {
        public Vector3 direction { get; set; } = Vector3.right;
        public static Vector3 Direction {get; set; } = Vector3.forward;

        private static SnakeMoveDirection instance;

        public static SnakeMoveDirection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SnakeMoveDirection>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("SnakeMoveDirection");
                        go.AddComponent<SnakeMoveDirection>();
                    }
                }

                return instance;
            }
        }
    }
}