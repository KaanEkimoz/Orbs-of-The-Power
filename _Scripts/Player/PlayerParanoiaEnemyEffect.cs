using com.absence.attributes;
using com.game.events;
using UnityEngine;

namespace com.game.player
{
    [RequireComponent(typeof(PlayerParanoiaLogic))]
    public class PlayerParanoiaEnemyEffect : MonoBehaviour
    {
        [SerializeField, Readonly] private PlayerParanoiaLogic m_paranoia;
        [SerializeField] private float m_enemyKillEffect;

        private void Awake()
        {
            PlayerEventChannel.OnEnemyKill += OnEnemyKilled;
        }

        private void OnEnemyKilled()
        {
            m_paranoia.Modify(m_enemyKillEffect);
        }

        private void Reset()
        {
            m_paranoia = GetComponent<PlayerParanoiaLogic>();
        }
    }
}
