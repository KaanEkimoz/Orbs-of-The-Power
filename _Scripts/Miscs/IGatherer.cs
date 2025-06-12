using UnityEngine;

namespace com.game.miscs
{
    public interface IGatherer
    {
        public bool IsPlayer { get; }

        public GameObject gameObject { get; }
        public Transform transform { get; }
    }
}
