namespace com.game.itemsystem
{
    public interface IInventory<T>
    {
        public bool Add(T target);
        public void Remove(T target);
    }
}
