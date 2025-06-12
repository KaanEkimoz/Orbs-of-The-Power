using System;
using UnityEngine;

namespace com.game.utilities
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SkinnedFieldAttribute : PropertyAttribute
    {
        public string keyPropertyName {  get; protected set; }
        public bool allowSceneObjects { get; protected set; } = false;

        public SkinnedFieldAttribute(string keyPropertyName)
        {
            this.keyPropertyName = keyPropertyName;
        }
    }
}
