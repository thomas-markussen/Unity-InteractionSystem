public interface IInteractable
{
    void Interact(InteractionController host);
    string GetInteractPrompt();
}