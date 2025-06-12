using com.absence.attributes;
using UnityEngine;

namespace com.game.generics
{
    public class ParentOnStart : MonoBehaviour
    {
        [SerializeField, Required] private Transform m_parent;

        private void Start()
        {
            transform.SetParent(m_parent, true);
        }
    }
}
