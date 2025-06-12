using System;
using UnityEngine;

namespace com.game.statsystem.internals
{
    public interface IDefaultStats
    {
        Type GetEnumType();
        void Reinitialize();
        string GetTitle();
    }

    public abstract class DefaultStats : ScriptableObject, IDefaultStats
    {
        public abstract void Reinitialize();
        public abstract string GetTitle();
        public abstract Type GetEnumType();
    }
}
