using System.Collections;
using TMPro;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField]
    Transform _interactionRadiusTransform;
    [SerializeField]
    float _interactionRadius;
    [SerializeField]
    bool _showInteractRadius;
    [SerializeField]
    TextMeshProUGUI _interactUI;


    IInteractable _interactable;

    private void Start()
    {
        StartCoroutine(TickRoutine());
    }

    public void Interact()
    {
        _interactable?.Interact(this);
    }

    IEnumerator TickRoutine()
    {
        WaitForSeconds wait = new(0.1f);

        while (true)
        {
            yield return wait;
            Detection();

            #region UI Prompt
            if (_interactable != null)
            {
                _interactUI.enabled = true;
                _interactUI.text = _interactable?.GetInteractPrompt();
            }
            else
            {
                _interactUI.enabled = false;
                _interactUI.text = "";
            }
            #endregion
        }
    }

    /// <summary>
    /// Runs very often to detect interactable objects in the player's radius.
    /// Returns the closest interactable to the field _interactable.
    /// </summary>
    void Detection()
    {
        Collider[] hits = Physics.OverlapSphere(_interactionRadiusTransform.position, _interactionRadius);
        IInteractable[] interactables = new IInteractable[hits.Length];
        int validTargets = 0;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent<IInteractable>(out var interactable))
            {
                interactables[i] = interactable;
                validTargets++;
            }
        }

        if (validTargets > 0)
        {
            float alpha = Mathf.Infinity;
            IInteractable closest = null;
            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i] == null) continue;

                float dist = Vector3.Distance(transform.position, hits[i].transform.position);
                if (dist < alpha)
                {
                    closest = interactables[i];
                    alpha = dist;
                }
            }
            _interactable = closest;
        }
        else
        {
            _interactable = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_showInteractRadius == false) return;

        Gizmos.color = new Color(0, 200, 0, 0.2f);
        Gizmos.DrawSphere(_interactionRadiusTransform.position, _interactionRadius);
    }
}
