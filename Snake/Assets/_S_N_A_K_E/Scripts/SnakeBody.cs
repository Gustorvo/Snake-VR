using System;
using System.Linq;
using UnityEngine;

namespace Gustorvo.Snake
{
    [Serializable]
    public class SnakeBody : SnakeBodyComponent
    {
        [SerializeField] Material headMaterial;
        [SerializeField] Material bodyMaterial;
        [SerializeField] Material tailMaterial;
        public Vector3 Position => transform.position;
        public Transform Transform => transform;
        private Renderer renderer;

        private void Init()
        {
            renderer = transform.GetComponentInChildren<Renderer>();
        }

        private void Awake()
        {
            Init();
        }

       
        public void MoveTo(Vector3 moveTo)
        {
            transform.position = moveTo;
        }

        public void TryMoveTo(Vector3 moveToPosition, out bool bodyCollidesWithItself)
        {
            bodyCollidesWithItself = Core.Snake.Positions.Contains(moveToPosition);
            MoveTo(moveToPosition);
            ApplyHeadMaterial();
        }

        public void ApplyHeadMaterial()
        {
            renderer.material = headMaterial;
        }
        
        public void ApplyBodyMaterial() => renderer.material = bodyMaterial;
        
        public void ApplyTailMaterial() => renderer.material = tailMaterial;
    }
}