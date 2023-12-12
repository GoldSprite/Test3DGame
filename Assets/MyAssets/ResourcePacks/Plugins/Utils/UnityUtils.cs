using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityUtils
{

    public static Vector2 P3To2(Vector3 origin)
    {
        var point = origin;
        point.y = origin.z;
        return point;
    }
    public static Vector3 P2To3(Vector2 origin, float ny)
    {
        Vector3 point = origin;
        point.z = point.y;
        point.y = ny;
        return point;
    }


    /// <summary>
    /// 获取指定动画是否播放完成: 有播放动画, 是目标动画, 播放时长超过100%
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="animName"></param>
    /// <param name="layerIndex"></param>
    /// <returns></returns>
    public static bool IsAnimEnd(Animator anim, string animName, int layerIndex = 0)
    {
        var clip = anim.GetCurrentAnimatorClipInfo(layerIndex);
        var time = anim.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
        return clip.Length != 0 && clip[0].clip.name == animName && time >= 1;
    }


    /// <summary>
    /// boxCollider碰撞检测
    /// </summary>
    /// <param name="selfPos"></param>
    /// <param name="targetColl"></param>
    /// <param name="mask"></param>
    /// <returns></returns>
    public static bool IsOverlapBoxTargetCollider(Vector3 selfPos, Collider targetColl, LayerMask mask = default)
    {
        var center = selfPos;
        var halfExtents = targetColl.bounds.size / 2f;
        Collider[] colls = null;
        if (mask == default(LayerMask))
            colls = Physics.OverlapBox(center, halfExtents, Quaternion.identity);
        else
            colls = Physics.OverlapBox(center, halfExtents, Quaternion.identity, mask);

        return colls.Length != 0;
    }


    /// <summary>
    /// 根据两坐标转向X
    /// </summary>
    /// <param name="selfTrans"></param>
    /// <param name="targetPos"></param>
    public static void AutoFacingX(Transform selfTrans, Vector3 targetPos)
    {
        var facex = targetPos.x - selfTrans.position.x;
        if (facex == 0) return;
        var facexNormalized = facex > 0 ? 1 : -1;
        var oldScale = selfTrans.localScale;
        oldScale.x = Mathf.Abs(oldScale.x) * facexNormalized;
        selfTrans.localScale = oldScale;
    }


    /// <summary>
    /// 根据向量转向
    /// </summary>
    /// <param name="selfTrans"></param>
    /// <param name="dir"></param>
    public static void AutoFacingX(Transform selfTrans, float dir)
    {
        if (dir == 0) return;
        var dirNormalized = dir > 0 ? 1 : -1;
        var ls = selfTrans.localScale;
        ls.x = dirNormalized * Mathf.Abs(ls.x);
        selfTrans.localScale = ls;
    }


    public static float GetFacingX(Transform transform)
    {
        return transform.localScale.x>=0?1:-1;
    }


    public static bool CompareLayerMaskByName(GameObject obj, string layerName)
    {
        var layer = obj.layer;
        var layerOrMask = LayerMask.NameToLayer(layerName);
        return (layer & layerOrMask) != 0;
    }


}
