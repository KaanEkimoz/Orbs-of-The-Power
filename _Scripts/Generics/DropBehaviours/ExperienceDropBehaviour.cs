using com.absence.soundsystem;
using com.absence.soundsystem.internals;
using com.game.miscs;
using com.game.player;
using UnityEngine;

namespace com.game.generics
{
    public class ExperienceDropBehaviour : DropBehaviour
    {
        public override bool DestroyOnGather => true;
        [Header("Sound Asset References")]
        [SerializeField] private SoundAsset m_xpGatherSoundAsset;

        protected override void Start()
        {
            if (Amount <= 0)
            {
                Destroy();
                return;
            }

            transform.localScale *= 1 + ((Amount - 1) / 10);
            base.Start();
        }

        protected override bool OnGather(IGatherer sender)
        {
            PlaySFX(m_xpGatherSoundAsset);
            Player.Instance.Hub.Leveling.GainExperience(Amount);
            PopupManager.Instance.CreateExperiencePopup(Amount, transform.position);
            return true;
        }
        void PlaySFX(ISoundAsset asset)
        {
            if (SoundManager.Instance == null)
                return;

            Sound.Create(asset)
                .AtPosition(transform.position)
                .Play();
        }
    }
}
