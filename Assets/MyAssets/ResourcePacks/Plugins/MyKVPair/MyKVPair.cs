using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class MyKVPair<K, V>
{
    public K Key;
    public V Value;


    public static V TryGetVal(List<MyKVPair<K, V>> data, K key)
    {
        var ret = data.FirstOrDefault(p=>ReferenceEquals(p.Key, key));
        return ret==null?default:ret.Value;
    }


}