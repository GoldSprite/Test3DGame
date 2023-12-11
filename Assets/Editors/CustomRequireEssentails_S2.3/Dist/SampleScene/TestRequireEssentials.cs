using com.goldsprite.gstools.CustomRequireEssentials;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRequireEssentials : MonoBehaviour
{

    [RequireEssentials(typeof(Animator), typeof(Rigidbody2D), typeof(ITestInterface))]
    //[RequireEssentials(typeof(Animator))]
    public string essentials;

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
