using com.game.interactionsystem;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.player
{
    public class PlayerInteractor : MonoBehaviour, IInteractor
    {
        [SerializeField] private PlayerInputHandler m_inputHandler;
        [SerializeField] private LayerMask m_obstacleMask;

        List<IInteractable> m_seenInteractables = new();

        int m_pickedIndex = 0;

        public bool IsPlayer => true;

        private void Awake()
        {
            RefreshPickedIndex();
        }

        private void Update()
        {
            if (m_inputHandler.ChooseNextInteractableButtonPressed)
                PickNext();

            if (m_inputHandler.InteractButtonPressed)
                Interact();

            CommitPicked();
        }

        private void LateUpdate()
        {
            m_seenInteractables.RemoveAll(interactable => interactable == null || interactable.Disposed);

            RefreshPickedIndex();
        }

        public void Interact()
        {
            if (Game.Paused)
                return;

            if (m_pickedIndex == -1)
                return;

            //RefreshPickedIndex();

            IInteractable target = GetAtOrNull(m_pickedIndex);

            if (target == null) 
                return;

            if (Blocked(target))
                return;

            target.Interact(this);
        }

        public string GenerateInteractorMessage(IInteractable interactable)
        {
            return $"Press <b>G</b> to interact";
        }

        public void PickNext()
        {
            Pick(m_pickedIndex + 1);
        }

        public void PickPrevious()
        {
            Pick(m_pickedIndex - 1);
        }

        public void Pick(int index)
        {
            if (m_pickedIndex == -1)
                return;

            CommitUnpicked();

            m_pickedIndex = index;

            RefreshPickedIndex();

            CommitPicked();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable))
                return;

            if ((!m_seenInteractables.Contains(interactable)) && (!interactable.Hidden))
                m_seenInteractables.Add(interactable);

            RefreshPickedIndex(false);

            interactable.CommitSeen(this, !Blocked(interactable));
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable))
                return;

            if (m_seenInteractables.Contains(interactable))
                m_seenInteractables.Remove(interactable);

            interactable.OnDispose -= OnInteractableDispose;

            RefreshPickedIndex(false);

            interactable.CommitPicked(this, false);
            interactable.CommitSeen(this, false);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable))
                return;

            if ((!m_seenInteractables.Contains(interactable)) && (!interactable.Hidden))
            {
                m_seenInteractables.Add(interactable);
                RefreshPickedIndex(false);

                interactable.CommitSeen(this, true);
            }
        }

        private void OnInteractableDispose(IInteractable interactable)
        {
            if (m_seenInteractables.Contains(interactable))
                m_seenInteractables.Remove(interactable);

            RefreshPickedIndex();
        }

        void RefreshPickedIndex(bool wrap = true)
        {
            int count = m_seenInteractables.Count;

            if (count == 0)
            {
                CommitUnpicked();
                m_pickedIndex = -1;
                return;
            }

            //if (count == 1)
            //{
            //    CommitUnpicked();
            //    m_pickedIndex = 0;
            //    CommitPicked();
            //    return;
            //}

            if (wrap)
            {
                if (m_pickedIndex < 0f)
                {
                    m_pickedIndex += count;
                    CommitPicked();
                }

                else if (m_pickedIndex >= count)
                {
                    m_pickedIndex -= count;
                    CommitPicked();
                }

                return;
            }

            if (m_pickedIndex < 0f)
            {
                m_pickedIndex = 0;
                CommitPicked();
            }

            else if (m_pickedIndex >= count)
            {
                m_pickedIndex = count - 1;
                CommitPicked();
            }
        }

        void CommitPicked(bool ignoreObstacles = false)
        {
            DoCommitPicked(true, ignoreObstacles);
        }

        void CommitUnpicked(bool ignoreObstacles = true)
        {
            DoCommitPicked(false, ignoreObstacles);
        }

        void DoCommitPicked(bool value, bool ignoreObstacles)
        {
            IInteractable pickedInteractable = GetAtOrNull(m_pickedIndex);

            if (pickedInteractable == null)
                return;

            if (ignoreObstacles || (!Blocked(pickedInteractable)))
            {
                pickedInteractable.CommitPicked(this, value);
            }
        }

        bool Blocked(IInteractable interactable)
        {
            Vector3 targetPosition = interactable.transform.position;
            Vector3 position = transform.position;

            return Physics.Raycast(position, (targetPosition - position).normalized,
                Vector3.Distance(position, targetPosition), m_obstacleMask);
        }

        IInteractable GetAtOrNull(int index)
        {
            if (index < 0) return null;
            if (index >= m_seenInteractables.Count) return null;

            return m_seenInteractables[index];
        }
    }
}
