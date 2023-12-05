using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gustorvo.Snake
{
    public class SnakeBodyComponent : MonoBehaviour
    {
        public static event Action OnSnakeCollidedWithItself; 

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == gameObject.layer)
                OnSnakeCollidedWithItself?.Invoke();
                
        }

        private void OnCollisionEnter(Collision other)
        {
            throw new NotImplementedException();
        }
    }
}
