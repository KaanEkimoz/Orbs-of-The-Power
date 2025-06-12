using com.game.generics.interfaces;
using UnityEngine;

namespace com.game.generics
{
    [DefaultExecutionOrder(-1)]
    public class LightSystemDependent : MonoBehaviour
    {
        public enum DisableMode
        {
            SetActive,
            DestroyImmediate,
        }

        [SerializeField] private DisableMode m_disableMode = DisableMode.SetActive;

        private void Awake()
        {
            if (!Application.isPlaying)
                return;

            if (SceneManager.Instance == null)
            {
                Disable();
                return;
            }

            if (!SceneManager.Instance.LightSystemInUse)
            {
                Disable();
                return;
            }
        }

        void Disable()
        {
            switch (m_disableMode)
            {
                case DisableMode.SetActive:
                    gameObject.SetActive(false);
                    break;
                case DisableMode.DestroyImmediate:
                    if (TryGetComponent(out ICustomDestroy customDestroy)) customDestroy.CustomDestroy();
                    else DestroyImmediate(gameObject);
                    break;
                default:
                    break;
            }
        }
    }
}
