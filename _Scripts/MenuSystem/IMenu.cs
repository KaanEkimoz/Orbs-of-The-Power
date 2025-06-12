using com.game.orbsystem.itemsystemextensions;
using com.game.shopsystem;

namespace com.game.menusystem
{
    public interface IMenu
    {
        public void SetVisibility(IShop<OrbItemProfile> shop, bool visibility);
        public void Show(IShop<OrbItemProfile> shop);
        public void Hide(bool clear = false);
    }
}
