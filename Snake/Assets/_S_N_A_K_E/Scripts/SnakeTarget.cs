using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gustorvo.Snake
{
    public interface ITarget
    {
        Vector3 Position { get; }
    }

    public class SnakeTarget : MonoBehaviour, ITarget
    {
        public Vector3 Position => transform.position;
    }
}