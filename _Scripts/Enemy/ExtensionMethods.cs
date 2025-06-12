namespace com.game.enemysystem
{
    public static class ExtensionMethods
    {
        public static void ReinitializeAsFake(this EnemyCombatant enemyCombatant)
        {
            enemyCombatant.ReinitializeStatsAsFake();
            enemyCombatant.Owner.ReinitializeAsFake();
        }

        public static void ReinitializeStatsAsFake(this EnemyCombatant enemyCombatant)
        {
            enemyCombatant.MaxHealth = 1;
            enemyCombatant.Health = 1;
        }

        public static void ReinitializeAsFake(this Enemy enemy)
        {
            enemy.IsFake = true;
            Enemy.DoFakify(enemy);
        }
    }
}
