using UnityEngine;

namespace com.game.miscs
{
    public interface IGatherable
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }

        public bool IsGatherable { get; }
        public bool TryGather(IGatherer sender);

        //public void CommitSeen(IGatherer sender);
        //public void CommitUnseen(IGatherer sender);
    }
}
