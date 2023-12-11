using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliciyaAnimBehaviour : BaseRoleAnimBehaviour
{

    protected override void InitFullAnimNames()
    {
        FullAnimNames = new List<MyKVPair<string, string>>()
        {
            new MyKVPair<string, string>(){ Key="Attack", Value="Base Layer.Lucy_Kick02_Root" },
            new MyKVPair<string, string>(){ Key="Hurt", Value="Base Layer.HurtBlend Tree" },
        };
    }


}
