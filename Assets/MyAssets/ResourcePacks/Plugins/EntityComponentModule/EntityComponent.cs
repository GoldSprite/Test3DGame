using System;
using UnityEngine;

public class EntityComponent: MonoBehaviour
{

    //配置
    [Header("配置属性")]
    [SerializeField] private float maxHealth = 20;
    public float MaxHealth { get=>maxHealth; protected set => maxHealth = value; }

    //实时
    [Header("实时属性")]
    [SerializeField] private float health;
    public float Health { get => health; protected set => health = value; }

    //事件
    public Action<float> TakeDamage;


    public EntityComponent()
    {
        Init();
    }


    protected virtual void Init()
    {
        Health = MaxHealth;

        TakeDamage = (damage) =>
        {
            if (health <= 0) return;

            health -= damage;
            if(health <= 0)
            {
                health = 0;
                Debug.Log($"{gameObject.name}-Death.");
            }
        };
    }


    [ContextMenu("M_TakeDamage")]
    public void M_TakeDamage()
    {
        TakeDamage?.Invoke(5);
    }


}