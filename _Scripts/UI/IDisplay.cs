using UnityEngine;

namespace com.game.ui
{
    public interface IDisplay<T>
    {
        public void Initialize(T target);
    }
}
