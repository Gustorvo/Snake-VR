using UnityEngine;

namespace Gustorvo.Snake
{
    // public struct NavData
    // {
    //     public Vector3 Direction;
    // }
    //
    // public interface INavigator
    // {
    //     public ITarget Target { get; set; }
    //
    //     public bool Navigate(in Vector3 currentPosition, in ITarget target, out NavData navData);
    // }
    //
    // internal class Navigator : INavigator
    // {
    //     public bool Navigate(in Vector3 currentPosition, in ITarget target, out NavData navData)
    //     {
    //         Target = target;
    //         navData = new NavData()
    //         {
    //             Direction = (target.Position - currentPosition).normalized
    //         };
    //         return navData.Direction != Vector3.zero;
    //     }
    //
    //     public ITarget Target { get; set; }
    // }
}