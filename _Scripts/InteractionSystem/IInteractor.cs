using com.game.utilities;

namespace com.game.interactionsystem
{
    public interface IInteractor : IObject
    {
        bool IsPlayer { get; }

        string GenerateInteractorMessage(IInteractable interactable);
    }
}
