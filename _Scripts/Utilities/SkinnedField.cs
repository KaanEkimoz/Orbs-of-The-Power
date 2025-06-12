using System;
using UnityEngine;

namespace com.game.utilities
{
    [System.Serializable]
    public abstract class SkinnedField
    {
#if UNITY_EDITOR
        [SerializeField] protected string m_keyPropName;
        [SerializeField] protected bool m_allowSceneObjects;
#endif

        public abstract Type GetSkinType();
        public abstract Type GetRealType();
    }

    [System.Serializable]
    public class SkinnedField<T1, T2> : SkinnedField where T2 : UnityEngine.Object
    {
#if UNITY_EDITOR
        [SerializeField] protected T2 m_skinValue;
#endif

        [SerializeField] protected T1 m_realValue;
        public T1 Value => m_realValue;

        public override Type GetSkinType() => typeof(T2);
        public override Type GetRealType() => typeof(T1);
    }
}
