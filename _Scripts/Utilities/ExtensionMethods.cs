using System.Collections.Generic;
using UnityEngine;

namespace com.game.utilities
{
    public static class ExtensionMethods
    {
        public static Vector2 RotateByAngle(this Vector2 target, float angleInDegrees)
        {
            float deg2rad = Mathf.Deg2Rad;
            float sin = Mathf.Sin(angleInDegrees * deg2rad);
            float cos = Mathf.Cos(angleInDegrees * deg2rad);
            float x = target.x;
            float y = target.y;
            return new Vector2(cos * x - sin * y, sin * x + cos * y);
        }

        public static Vector3 ProjectXZ(this Vector2 target)
        {
            return new Vector3(target.x, 0f, target.y);
        }

        public static Vector3 ProjectZX(this Vector2 target)
        {
            return new Vector3(target.y, 0f, target.x);
        }

        public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict,
                                           TKey oldKey, TKey newKey)
        {
            if (!dict.Remove(oldKey, out TValue value))
                return false;

            if (dict.ContainsKey(newKey))
                return false;

            dict.Add(newKey, value);
            return true;
        }

        public static Transform GetRealTransform(this Collider collider)
        {
            if (collider.attachedRigidbody != null)
                return collider.attachedRigidbody.transform;

            return collider.transform;
        }

        public static Vector3 CalculateKnockbackDirection(this IKnockbackable knockbackable, Vector3 source, KnockbackSourceUsage usage)
        {
            return CalculateKnockbackDirection(knockbackable, source, usage, knockbackable.transform);
        }

        public static Vector3 CalculateKnockbackDirection(this IKnockbackable knockbackable, Vector3 source, KnockbackSourceUsage usage, Transform self)
        {
            return usage switch
            {
                KnockbackSourceUsage.KnockSource => new Vector3(self.position.x - source.x, 0, self.position.z - source.z).normalized,
                KnockbackSourceUsage.Final => source.normalized,
                _ => Vector3.up,
            };
        }

        public static void SlowForSeconds(this ISlowable slowable, float slowPercent, float duration, CCSource source = CCSource.Unset)
        {
            slowable.SlowForSeconds(slowPercent, duration, source, CCBreakFlags.None);
        }

        public static void Stun(this IStunnable stunnable, float duration, CCSource source = CCSource.Unset)
        {
            stunnable.Stun(duration, source, CCBreakFlags.None);
        }
    }
}
