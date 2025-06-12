using com.absence.attributes;
using com.game.itemsystem.scriptables;
using com.game.scriptableeventsystem.editor;
using com.game.subconditionsystem.editor;
using UnityEditor;

namespace com.game.itemsystem.editor
{
    public static class FieldButtonMethods
    {
        [FieldButtonId(2392, priority = int.MaxValue)]
        static void CreateFirstSpecificSubcondition(object sender)
        {
            ItemProfileBase item = sender as ItemProfileBase;
            SubconditionSelectionWindow.Initiate((createdSO) =>
            {
                Undo.SetCurrentGroupName("Item Profile (Create Specific Subcondition)");
                int undoGroup = Undo.GetCurrentGroup();

                Undo.RecordObject(item, "Item Profile (First Specific Subcondition Set)");

                createdSO.name = $"{item.name} (First Specific Subcondition)";
                item.FirstSpecificCondition = createdSO;

                Undo.CollapseUndoOperations(undoGroup);

                EditorUtility.SetDirty(createdSO);
                EditorUtility.SetDirty(item);
            }, item);
        }

        [FieldButtonId(2393, priority = int.MaxValue)]
        static void DestroyFirstSpecificSubcondition(object sender)
        {
            ItemProfileBase item = sender as ItemProfileBase;

            Undo.RecordObject(item, "Item (Delete First Specific Subcondition)");

            UnityEngine.Object objectWillGetDeleted = item.FirstSpecificCondition;

            item.FirstSpecificCondition = null;

            if (objectWillGetDeleted == null)
                return;

            Undo.DestroyObjectImmediate(objectWillGetDeleted);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [FieldButtonId(2394, priority = int.MaxValue)]
        static void CreateFirstSpecificEvent(object sender)
        {
            ItemProfileBase item = sender as ItemProfileBase;
            ScriptableEventSelectionWindow.Initiate((createdSO) =>
            {
                Undo.SetCurrentGroupName("Item Profile (Create Specific Event)");
                int undoGroup = Undo.GetCurrentGroup();

                Undo.RecordObject(item, "Item Profile (First Specific Event Set)");

                createdSO.name = $"{item.name} (First Specific Event)";
                item.FirstSpecificEvent = createdSO;

                Undo.CollapseUndoOperations(undoGroup);

                EditorUtility.SetDirty(createdSO);
                EditorUtility.SetDirty(item);
            }, item);
        }

        [FieldButtonId(2395, priority = int.MaxValue)]
        static void DestroyFirstSpecificEvent(object sender)
        {
            ItemProfileBase item = sender as ItemProfileBase;

            Undo.RecordObject(item, "Item (Delete First Specific Event)");

            UnityEngine.Object objectWillGetDeleted = item.FirstSpecificEvent;

            item.FirstSpecificEvent = null;

            if (objectWillGetDeleted == null)
                return;

            Undo.DestroyObjectImmediate(objectWillGetDeleted);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [FieldButtonId(2396, priority = int.MaxValue)]
        static void CreateSecondSpecificSubcondition(object sender)
        {
            ItemProfileBase item = sender as ItemProfileBase;
            SubconditionSelectionWindow.Initiate((createdSO) =>
            {
                Undo.SetCurrentGroupName("Item Profile (Create Specific Subcondition)");
                int undoGroup = Undo.GetCurrentGroup();

                Undo.RecordObject(item, "Item Profile (Second Specific Subcondition Set)");

                createdSO.name = $"{item.name} (Second Specific Subcondition)";
                item.SecondSpecificCondition = createdSO;

                Undo.CollapseUndoOperations(undoGroup);

                EditorUtility.SetDirty(createdSO);
                EditorUtility.SetDirty(item);
            }, item);
        }

        [FieldButtonId(2397, priority = int.MaxValue)]
        static void DestroySecondSpecificSubcondition(object sender)
        {
            ItemProfileBase item = sender as ItemProfileBase;

            Undo.RecordObject(item, "Item (Delete Second Specific Subcondition)");

            UnityEngine.Object objectWillGetDeleted = item.SecondSpecificCondition;

            item.SecondSpecificCondition = null;

            if (objectWillGetDeleted == null)
                return;

            Undo.DestroyObjectImmediate(objectWillGetDeleted);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [FieldButtonId(2398, priority = int.MaxValue)]
        static void CreateSecondSpecificEvent(object sender)
        {
            ItemProfileBase item = sender as ItemProfileBase;
            ScriptableEventSelectionWindow.Initiate((createdSO) =>
            {
                Undo.SetCurrentGroupName("Item Profile (Create Specific Event)");
                int undoGroup = Undo.GetCurrentGroup();

                Undo.RecordObject(item, "Item Profile (Second Specific Event Set)");

                createdSO.name = $"{item.name} (Second Specific Event)";
                item.SecondSpecificEvent = createdSO;

                Undo.CollapseUndoOperations(undoGroup);

                EditorUtility.SetDirty(createdSO);
                EditorUtility.SetDirty(item);
            }, item);
        }

        [FieldButtonId(2399, priority = int.MaxValue)]
        static void DestroySecondSpecificEvent(object sender)
        {
            ItemProfileBase item = sender as ItemProfileBase;

            Undo.RecordObject(item, "Item (Delete Second Specific Event)");

            UnityEngine.Object objectWillGetDeleted = item.SecondSpecificEvent;

            item.SecondSpecificEvent = null;

            if (objectWillGetDeleted == null)
                return;

            Undo.DestroyObjectImmediate(objectWillGetDeleted);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
