using com.absence.utilities;
using com.game.statsystem.presetobjects;
using System;
using System.Text;

namespace com.game.statsystem
{
    public static class StatSystemHelpers
    {
        public static class Text
        {
            public static string GetDisplayName<T>(T stat, bool richText = false) where T : Enum
            {
                string refinedLabel = Helpers.SplitCamelCase(Enum.GetName(typeof(T), stat), " ");

                if (!richText)
                {
                    return refinedLabel;
                }

                else
                {
                    StringBuilder sb = new("<b>");
                    sb.Append(refinedLabel);
                    sb.Append("</b>");
                    return sb.ToString();
                }
            }

            public static string GenerateDescription<T>(StatOverride<T> ovr, bool richText = false) where T : Enum
            {
                StringBuilder sb = new($"Sets {GetDisplayName(ovr.TargetStatType, richText)} to ");

                if (richText) sb.Append("<b>");

                sb.Append(ovr.NewValue);

                if (richText) sb.Append("</b>");

                sb.Append(".");

                return sb.ToString();
            }

            public static string GenerateDescription<T>(StatModification<T> mod, bool richText = false) where T : Enum
            {
                StringBuilder sb = new();

                if (richText) sb.Append("<b>");

                if (mod.Value > 0) sb.Append("+");
                sb.Append(mod.Value);

                if (mod.ModificationType == StatModificationType.Percentage)
                    sb.Append("%");

                if (richText) sb.Append("</b>");

                sb.Append($" to {GetDisplayName(mod.TargetStatType, richText)}.");

                return sb.ToString();
            }

            public static string GenerateDescription<T>(StatCap<T> cap, bool richText = false) where T : Enum
            {
                StringBuilder sb = new();

                if (cap.CapLow)
                {
                    sb.Append($"Caps {GetDisplayName(cap.TargetStatType, richText)} ");
                    if (richText) sb.Append($"at <b>min</b> of <b>{cap.MinValue}</b>.");
                    else sb.Append($"at min of {cap.MinValue}.");

                    if (cap.CapHigh) sb.Append("\n");
                }

                if (cap.CapHigh)
                {
                    sb.Append($"Caps {GetDisplayName(cap.TargetStatType, richText)} ");
                    if (richText) sb.Append($"at <b>max</b> of <b>{cap.MaxValue}</b>.");
                    else sb.Append($"at max of {cap.MaxValue}.");
                }

                return sb.ToString();
            }
        }
    }
}
