using System;
using UnityEngine;

public interface IHurtable
{
    public Action<Vector3, float> TakeDamage { get;}
}