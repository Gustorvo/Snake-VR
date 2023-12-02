using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gustorvo.Snake
{
    public class Core : MonoBehaviour
    {
        [field: SerializeField] public PlayBoundary playBoundary { get; private set; }
        [field: SerializeField] public SnakeBehaviour snake { get; private set; }
        private static Core instance { get; set; }

        public static PlayBoundary PlayBoundary => instance.playBoundary;
        public static SnakeBehaviour Snake => instance.snake;

        private void Awake()
        {
            instance = this;
        }
    }
}