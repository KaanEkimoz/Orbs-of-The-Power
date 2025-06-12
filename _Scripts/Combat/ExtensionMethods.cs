using System.Collections;
using UnityEngine;

namespace com.game
{
    public static class ExtensionMethods
    {
        public static void TakeDamage(this IDamageable damageable, float damage)
        {
            damageable.TakeDamage(damage, out _);
        }

        // !!!
        public static void TakeDamageInSeconds(this IDamageable damageable, MonoBehaviour sender, float damage, float durationInSeconds, float intervalInSeconds)
        {
            sender.StartCoroutine(TakeDamageOverTime(damageable, damage, durationInSeconds, intervalInSeconds));
        }

        private static IEnumerator TakeDamageOverTime(IDamageable damageable, float totalDamage, float durationInSeconds, float intervalInSeconds)
        {
            float elapsedTime = 0f;
            float damageDivider = durationInSeconds / intervalInSeconds;

            while (elapsedTime < durationInSeconds)
            {
                damageable.TakeDamage(totalDamage / damageDivider, out DamageEvent evt);

                if (evt.CausedDeath)
                    yield break;

                elapsedTime += intervalInSeconds;
                yield return new WaitForSeconds(intervalInSeconds);
            }
        }
    }
}
