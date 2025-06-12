using com.absence.utilities.experimental.databases;
using com.absence.utilities.experimental.databases.editor;
using com.game.itemsystem.scriptables;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace com.game.itemsystem.editor
{
    public static class ItemDatabase
    {
        public const bool DEBUG_MODE = true;
        private static IDatabaseInstance<string, ItemProfileBase> s_instance;

        [MenuItem("Game/Item System/Refresh Item Database")]
        static void Refresh_MenuItem()
        {
            Refresh(true);
        }

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            s_instance = new EditorMemberDatabaseInstance<string, ItemProfileBase>();
            Refresh(DEBUG_MODE);
        }

        public static void Refresh(bool debugMode = false)
        {
            s_instance.Refresh();

            if (debugMode) Print();
        }
        public static void Print()
        {
            StringBuilder sb = new("<b>[ITEMSYSTEM] Found Items: </b>");

            foreach (ItemProfileBase item in s_instance)
            {
                sb.Append("\n\t");
                sb.Append("-> <color=white>");
                sb.Append(item.DisplayName);
                sb.Append("</color>");
                sb.Append(" [");
                sb.Append(item.Guid);
                sb.Append("]");
            }

            sb.Append("\n");
            Debug.Log(sb.ToString());
        }

        public static ItemProfileBase GetItem(string guid)
        {
            return s_instance[guid];
        }
        public static bool TryGetItem(string guid, out ItemProfileBase result)
        {
            bool success = s_instance.TryGet(guid, out result);
            return success;
        }
    }
}
