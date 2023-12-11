using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pico_ChanAnimBehaviour : BaseRoleAnimBehaviour
{

    protected override void InitFullAnimNames()
    {
        FullAnimNames = new List<MyKVPair<string, string>>()
        {
            new MyKVPair<string, string>(){ Key="Attack", Value="Base Layer.Lucy_Kick04_Root" },
            new MyKVPair<string, string>(){ Key="Hurt", Value="Base Layer.HurtBlend Tree" },
        };
    }


}
