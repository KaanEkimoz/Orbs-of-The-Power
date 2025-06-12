using com.game.utilities;
using System;

namespace com.game.interactionsystem
{
    public interface IInteractable : IObject, IDisposable
    {
        string RichInteractionMessage { get; }
        bool Interactable { get; }
        bool Hidden { get; }
        bool Disposed { get; }

        event Action<IInteractable, IInteractor, bool> OnInteraction;
        event Action<IInteractor, bool> OnSeen;
        event Action<IInteractor, bool> OnPicked;
        event Action<IInteractable> OnDispose;

        bool Interact(IInteractor interactor);

        void CommitSeen(IInteractor sender, bool seen);
        void CommitPicked(IInteractor sender, bool picked);
        string GenerateInteractionPopupText(IInteractor interactor, bool success);
    }
}
