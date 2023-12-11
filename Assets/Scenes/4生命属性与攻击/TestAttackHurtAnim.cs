using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAttackHurtAnim : MonoBehaviour
{
    public EntityComponent ent;

    public bool attack;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attack)
        {
            attack = false;
            ent.TakeDamage?.Invoke(transform.position, 5);
        }
    }


}
