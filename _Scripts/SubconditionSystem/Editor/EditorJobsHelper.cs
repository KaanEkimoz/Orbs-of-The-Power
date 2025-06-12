using System.Text;
using UnityEditor;
using UnityEngine;

namespace com.game.subconditionsystem.editor
{
    public static class EditorJobsHelper
    {
        public static void PrintTypeCache()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<b>[SUBCONDITIONS] Types found in cache: </b>");

            SubconditionTypeCache.FoundTypes.ForEach(type =>
            {
                sb.Append("\n\t");

                sb.Append("<color=white>");
                sb.Append("-> ");
                sb.Append(type.Name);
                sb.Append("</color>");
            });

            Debug.Log(sb.ToString());
        }

        [MenuItem("Game/Subcondition System/Refresh Type Cache")]
        static void Refresh_MenuItem()
        {
            SubconditionTypeCache.Refresh(true);
        }
    }
}
