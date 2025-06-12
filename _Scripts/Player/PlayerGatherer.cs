using com.game.miscs;
using UnityEngine;

namespace com.game.player
{
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerGatherer : MonoBehaviour, IGatherer
    {
        public bool IsPlayer => true;
    }
}
