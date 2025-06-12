using UnityEngine;

namespace com.game.utilities
{
    public interface IObject
    {
        Transform transform { get; }
        GameObject gameObject { get; }
    }
}
