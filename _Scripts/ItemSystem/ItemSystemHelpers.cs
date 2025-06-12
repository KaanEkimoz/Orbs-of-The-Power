using com.game.itemsystem.gamedependent;
using com.game.itemsystem.scriptables;
using com.game.scriptableeventsystem;
using com.game.subconditionsystem;
using System;
using System.Text;

namespace com.game.itemsystem
{
    public static class ItemSystemHelpers
    {
        public static class Text
        {
            public static string GenerateDescription(ItemObject itemObject, bool richText)
            {
                return GenerateFullDescription_Internal(itemObject, itemObject.Profile, richText);
            }

            public static string GenerateDescription(ItemProfileBase itemProfile, bool richText)
            {
                return GenerateFullDescription_Internal(null, itemProfile, richText);
            }

            private static string GenerateFullDescription_Internal(ItemObject itemObject, ItemProfileBase itemProfile, bool richText)
            {
                string trimmedRawDesc = itemProfile.Description.Trim();

                StringBuilder sb = new(trimmedRawDesc);
                if (!string.IsNullOrWhiteSpace(trimmedRawDesc)) sb.Append("\n\n");

                string specificsDesc = GenerateSpecificsDescription(itemObject, itemProfile, richText);
                string customActionDesc = GenerateCustomActionDescription_Internal(itemObject, itemProfile, richText);
                string furtherDesc = itemProfile.GenerateFurtherDescription(itemObject, richText);

                if (specificsDesc != null) sb.Append(specificsDesc);
                if (furtherDesc != null) sb.Append(furtherDesc);
                if (customActionDesc != null) sb.Append(customActionDesc);

                if (itemObject != null)
                {
                    foreach (ItemBehaviour bhv in itemObject.Behaviours)
                    {
                        sb.Append(bhv.GenerateDataDescription(richText));
                    }
                }

                return sb.ToString();
            }

            private static string GenerateCustomActionDescription_Internal(ItemObject context, ItemProfileBase profile, bool richText)
            {
                StringBuilder sb = new();

                ItemBehaviour bhv = profile.Behaviour;
                if (bhv != null)
                {
                    sb.Append(bhv.GenerateActionDescription(richText));
                    sb.Append("\n");
                }

                profile.CustomActions.ForEach(act =>
                {
                    sb.Append(GenerateDescription_Internal(context, act, richText));
                    sb.Append("\n");
                });

                return sb.ToString();
            }

            private static string GenerateDescription_Internal(ItemObject context, ItemCustomAction action, bool richText)
            {
                return action.ItemBehaviour.GenerateActionDescription(richText);
            }

            private static string GenerateSpecificsDescription(ItemObject context, ItemProfileBase profile, bool richText)
            {
                StringBuilder sb = new();

                if (context == null)
                {
                    sb.Append(GenerateSpecificProfileDescription(profile.FirstSpecificCondition, profile.FirstSpecificEvent, richText));
                    sb.Append(GenerateSpecificProfileDescription(profile.SecondSpecificCondition, profile.SecondSpecificEvent, richText));

                    return sb.ToString();
                }

                foreach (ItemRuntimeSpecific specific in context.Specifics)
                {
                    sb.Append(specific.GenerateDescription(richText));
                }

                return sb.ToString();
            }

            private static string GenerateSpecificProfileDescription(SubconditionProfileBase subconditionProfile, ScriptableEventProfileBase evtProfile, bool richText)
            {
                StringBuilder sb = new();

                if (subconditionProfile == null)
                    return null;

                if (subconditionProfile.DisplayKeyword)
                {
                    sb.Append(subconditionProfile.Keyword.ToLower());
                    sb.Append(" ");
                }

                sb.Append(subconditionProfile.GenerateDescription(richText, null));

                if (!string.IsNullOrWhiteSpace(sb.ToString())) 
                    sb.Append("; ");

                sb.Append(evtProfile.GenerateDescription(richText, null));
                sb.Append("\n");

                return sb.ToString();
            }
        }
    }
}
