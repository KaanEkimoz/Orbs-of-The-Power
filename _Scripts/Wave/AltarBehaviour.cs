using com.game.interactionsystem;

namespace com.game.altarsystem
{
    public class AltarBehaviour : InteractableBase
    {
        public override bool Interactable
        {
            get
            {
                return GameManager.Instance.State == GameState.BetweenWaves;
            }

            set
            {

            }
        }

        public override bool OnInteract(IInteractor interactor)
        {
            SceneManager.Instance.EnterShop();
            return true;
        }
    }

}