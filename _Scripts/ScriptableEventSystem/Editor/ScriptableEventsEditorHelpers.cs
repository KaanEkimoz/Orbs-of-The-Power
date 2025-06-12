using System;
using UnityEditor;
using UnityEngine;

namespace com.game.scriptableeventsystem.editor
{
    public static class ScriptableEventsEditorHelpers
    {
        public static ScriptableEventProfileBase CreateScriptableEvent(Type type, UnityEngine.Object root = null)
        {
            bool isSubAsset = root != null;

            ScriptableEventProfileBase createdSO = (ScriptableEventProfileBase)ScriptableObject.CreateInstance(type);
            createdSO.IsSubAsset = isSubAsset;

            if (root != null)
                AssetDatabase.AddObjectToAsset(createdSO, root);

            Undo.RegisterCreatedObjectUndo(createdSO, "Scriptable Event Created");

            return createdSO;
        }
    }
}
