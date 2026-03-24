using System.Numerics;
using UnityEngine;

public interface IPickeable
{
    void Pick(Transform holdPoint);
    void Drop();
}
