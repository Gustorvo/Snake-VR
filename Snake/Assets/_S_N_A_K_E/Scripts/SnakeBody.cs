using System;
using System.Linq;
using UnityEngine;

namespace Gustorvo.Snake
{
    [Serializable]
    public class SnakeBody : SnakeBodyComponent
    {
        //public SnakeBodyComponent c;
        public Vector3 Position => transform.position;
        public Transform Transform => transform;
        public Material Material { get; set; }
        //private Transform transform;

        // public SnakeBody(Transform transform)
        // {
        //     this.transform = transform;
        // }

        private void Init()
        {
            Material = transform.GetComponent<Renderer>().material;
        }

        public void MoveTo(Vector3 moveTo)
        {
            transform.position = moveTo;
        }

        public void TryMoveTo(Vector3 moveToPosition, out bool bodyCollidesWithItself)
        {
            bodyCollidesWithItself = Core.Snake.Positions.Contains(moveToPosition);
            MoveTo(moveToPosition);
        }
    }
}