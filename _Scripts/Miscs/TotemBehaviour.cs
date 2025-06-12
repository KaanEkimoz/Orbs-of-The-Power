using com.absence.attributes;
using com.game.generics.entities;
using com.game.interactionsystem;
using System;
using UnityEngine;

namespace com.game.miscs
{
    public class TotemBehaviour : MonoBehaviour
    {
        [SerializeField, Required] private InteractableBase m_interactableScript;
        [SerializeField] private Door m_doorScript;

        public event Action OnDispose;

        private void Start()
        {
            m_interactableScript.Hidden = true;

            if (m_doorScript != null)
                m_doorScript.Close(OnTotemFullyVisible);
            else
                m_interactableScript.Hidden = false;
        }

        private void OnTotemFullyVisible()
        {
            m_interactableScript.Hidden = false;
        }

        public void Dispose()
        {
            m_interactableScript.Hidden = true;

            if (m_doorScript != null)
                m_doorScript.Open(OnTotemFullyInvisible);
            else
                Destroy(gameObject);

            OnDispose?.Invoke();
        }

        private void OnTotemFullyInvisible()
        {
            Destroy(gameObject);
        }
    }
}
