using UnityEngine;

namespace com.game.generics.interfaces
{
    public interface IVisible
    {
        ISpark Spark { get; }
        Transform transform { get; }
    }
}
