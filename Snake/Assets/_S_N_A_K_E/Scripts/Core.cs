using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gustorvo.Snake
{
    public class Core : MonoBehaviour
    {
        [field: SerializeField] public PlayBoundary playBoundary { get; private set; }
        private static Core instance { get; set; }

        public static PlayBoundary PlayBoundary => instance.playBoundary;

        private void Awake()
        {
            instance = this;
        }
    }
}