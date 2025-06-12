using System;
using UnityEditor;
using UnityEngine;

namespace com.game.subconditionsystem.editor
{
    public static class SubconditionEditorHelpers
    {
        public static SubconditionProfileBase CreateSubconditionProfile(Type type, UnityEngine.Object root = null)
        {
            bool isSubAsset = root != null;

            SubconditionProfileBase createdSO = (SubconditionProfileBase)ScriptableObject.CreateInstance(type);
            createdSO.IsSubAsset = isSubAsset;

            if (root != null)
                AssetDatabase.AddObjectToAsset(createdSO, root);

            Undo.RegisterCreatedObjectUndo(createdSO, "Subcondition Profile Created");

            return createdSO;
        }
    }
}
