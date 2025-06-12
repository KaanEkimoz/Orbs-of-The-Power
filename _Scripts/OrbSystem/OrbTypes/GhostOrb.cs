using UnityEngine;

namespace com.game
{
    public class GhostOrb : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float speedMultiplier = 1f;
        private Vector3 currentTargetPos;
        private bool teleportToSpawn = false;
        private void OnEnable()
        {
            teleportToSpawn = false;
        }
        void Update()
        {
            MoveTargetPos();
        }
        public void SetNewDestination(Vector3 newPos)
        {
            currentTargetPos = newPos;
            if(!teleportToSpawn)
            {
                transform.position = newPos;
                teleportToSpawn = true;
            }
        }
        private void MoveTargetPos()
        {
            Vector3 posToMove = currentTargetPos;
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPos, movementSpeed * Time.deltaTime);
        }
    }
}
