namespace com.game.entitysystem
{
    public interface IEntityStatProvider
    {
        public float Health { get; }
        public float Armor { get; }
        public float Damage { get; }
    }
}
