using com.absence.attributes;
using com.game.player.statsystemextensions;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.player.scriptables
{
    [CreateAssetMenu(fileName = "New PlayerCharacterProfile", menuName = "Game/Player/Player Character Profile", order = int.MinValue)]
    public class PlayerCharacterProfile : ScriptableObject
    {
        [Header1("Character Profile for Player")]

        [Space]
        [Header3("Initial")]

        [Range(0, Constants.Gameplay.MAX_ORBS_CAN_BE_HELD)] public int OrbCount;

        [Space]
        [Header3("Stat-Based")]

        public List<PlayerStatOverride> Overrides = new();
        public List<PlayerStatModification> Modifications = new();
        public List<PlayerStatCap> Caps = new();

        [Space]
        [Header3("Custom Actions")]
        [SerializeField] private float m_notReadyYet;
    }
}