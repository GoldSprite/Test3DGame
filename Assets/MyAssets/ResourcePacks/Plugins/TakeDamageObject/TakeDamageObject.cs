using System;
using UnityEngine;

public class TakeDamageObject: MonoBehaviour
{
    public float Damage = 5;
    Transform damageOriginPoint => transform.parent;


    private void OnTriggerEnter(Collider other)
    {
        if(TryGetHurtable(other.gameObject, out IHurtable ent))
        {
            ent.TakeDamage?.Invoke(damageOriginPoint.position, Damage);
        }
    }


    private bool TryGetHurtable(GameObject gameObject, out IHurtable ent)
    {
        if ((ent = gameObject.GetComponent<IHurtable>()) != null) 
            return true;
        return false;
    }


}
