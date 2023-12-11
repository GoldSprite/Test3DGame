using com.goldsprite.gstools.CustomRequireEssentials;
using System;
using UnityEngine;

public class EntityComponent: MonoBehaviour
{
#if UNITY_EDITOR
    [RequireEssentials(typeof(Animator))] public string RequireVal;
#endif

    //引用
    Animator anim;

    //配置
    [Header("配置属性")]
    [SerializeField] private float maxHealth = 20;
    public float MaxHealth { get=>maxHealth; protected set => maxHealth = value; }

    //实时
    [Header("实时属性")]
    [SerializeField] private float health;
    public float Health { get => health; protected set => health = value; }

    //事件
    public Action<Vector3, float> TakeDamage;


    public EntityComponent()
    {
        Init();
    }


    private void Start()
    {
        anim = GetComponent<Animator>();
    }


    protected virtual void Init()
    {
        Health = MaxHealth;

        TakeDamage = (hurtPos, damage) =>
        {
            if (health <= 0) return;  //已死亡不反馈

            //受伤
            health -= damage;
            //动画混合树
            var hurtPos2Local = transform.InverseTransformPoint(hurtPos);
            hurtPos2Local.y = 0;
            var blendVal = hurtPos2Local.normalized;
            anim.SetBool("Hurt", true);
            anim.SetFloat("HurtDirForward", blendVal.z);
            anim.SetFloat("HurtDirRight", blendVal.x);

            //死亡
            if(health <= 0)
            {
                health = 0;
                Debug.Log($"{gameObject.name}-Death.");
            }
        };
    }


    //[ContextMenu("M_TakeDamage")]
    //public void M_TakeDamage()
    //{
    //    TakeDamage?.Invoke(5);
    //}


}