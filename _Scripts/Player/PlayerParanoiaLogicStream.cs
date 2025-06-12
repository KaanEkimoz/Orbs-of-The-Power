using com.absence.attributes;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.player
{
    [RequireComponent(typeof(PlayerParanoiaLogic))]
    public class PlayerParanoiaLogicStream : MonoBehaviour
    {
        [SerializeField, Readonly] private PlayerParanoiaLogic m_owner;
        [SerializeField] private List<PlayerParanoiaLogicStreamEntry> m_entries = new();

        private void Awake()
        {
            m_owner.OnParanoiaSegmentChange += OnParanoiaSegmentChange;
        }

        private void Update()
        {
            foreach (var entry in m_entries)
            {
                entry.Target.Value.OnFetchParanoiaAffectionValue(entry.GetValue(m_owner));
            }
        }

        private void OnParanoiaSegmentChange()
        {
            foreach (var entry in m_entries) 
            {
                entry.Target.Value.OnParanoiaSegmentChange(m_owner.SegmentIndex);
            }
        }

        private void Reset()
        {
            m_owner = GetComponent<PlayerParanoiaLogic>();
        }
    }
}
