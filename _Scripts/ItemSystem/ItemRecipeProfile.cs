using com.absence.utilities.experimental.databases;
using UnityEngine;

namespace com.game.itemsystem.scriptables
{
    [CreateAssetMenu(fileName = "ItemRecipeProfile", menuName = "Game/Item System/Recipe")]
    public class ItemRecipeProfile : ScriptableObject, IDatabaseMember<string>
    {
        //[SerializeField] private bool m_allowDifferentSubtypes = true;
        [SerializeField] private string m_guid1;
        [SerializeField] private string m_guid2;
        [SerializeField] private string m_resultGuid;

        //public bool AllowDifferentSubtypes => m_allowDifferentSubtypes;
        public string LHSGuid => m_guid1;
        public string RHSGuid => m_guid2;
        public string ResultGuid => m_resultGuid;

        public bool Contains(string guid)
        {
            return m_guid1.Equals(guid) || m_guid2.Equals(guid);
        }

        public bool Results(string guid)
        {
            return m_resultGuid.Equals(guid);
        }

        public string GetDatabaseKey()
        {
            return System.Guid.NewGuid().ToString();
        }
    }
}
