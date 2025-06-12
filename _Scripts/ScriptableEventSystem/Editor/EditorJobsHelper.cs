using System.Text;
using UnityEditor;
using UnityEngine;

namespace com.game.scriptableeventsystem.editor
{
    public static class EditorJobsHelper
    {
        public static void PrintTypeCache()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<b>[SCRIPTABLEEVENTS] Types found in cache: </b>");

            ScriptableEventTypeCache.FoundTypes.ForEach(type =>
            {
                sb.Append("\n\t");

                sb.Append("<color=white>");
                sb.Append("-> ");
                sb.Append(type.Name);
                sb.Append("</color>");
            });

            Debug.Log(sb.ToString());
        }

        [MenuItem("Game/Scriptable Event System/Refresh Type Cache")]
        static void Refresh_MenuItem()
        {
            ScriptableEventTypeCache.Refresh(true);
        }
    }
}
