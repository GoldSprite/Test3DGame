using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace com.goldsprite.gstools.CustomRequireEssentials
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple =false)]
    public class RequireEssentialsAttribute : PropertyAttribute
    {
        public Type[] type { get; }
        public RequireEssentialsAttribute(params Type[] type)
        {
            this.type = type;
        }
    }


}