using com.game.miscs;
using com.game.player;

namespace com.game.generics
{
    public class MoneyDropBehaviour : DropBehaviour
    {
        public override bool DestroyOnGather => true;

        protected override void Start()
        {
            if (Amount <= 0)
            {
                Destroy();
                return;
            }

            transform.localScale *= 1 + ((Amount - 1) / 10);
            base.Start();
        }

        protected override bool OnGather(IGatherer sender)
        {
            Player.Instance.Hub.Money.Gain(Amount);
            PopupManager.Instance.CreateMoneyPopup(Amount, transform.position);
            return true;
        }
    }
}
