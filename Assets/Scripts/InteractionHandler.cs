using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionHandler : MonoBehaviour
{
    private bool canInteract;
    public bool CanInteract
    {
        get => canInteract;
        set
        {
            canInteract = value;
            if (!value && (immInteractable)) immInteractable = false;
        }
    }
    private bool immInteractable;
    public bool ImmInteractable
    {
        get => immInteractable;
        set
        {
            immInteractable = value;
            if (value) EnableInteraction(target);
            else DisableInteraction();
        }
    }
    private int layerMask;
    private GameObject target;
    private RaycastHit hit;

    private bool inAttackMode;

    [SerializeField] private GameObject bulletPrefab;

    public delegate void OnMouseClick();
    public OnMouseClick onMouseClick;

    InteractableObject interactableObject;

    private bool hasInteractedWithObject;

    AchievementManager am;

    private void Awake()
    {
        layerMask = (1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Default"));
        canInteract = false;
        immInteractable = false;
    }

    public void SetMouseClickAction(int actionCode)
    {
        switch (actionCode)
        {
            case 0:
                onMouseClick = HandleInteraction;
                inAttackMode = false;
                break;
            case 1:
                onMouseClick = Attack;
                inAttackMode = true;
                break;
            default:
                break;
        }
    }

    public void SetInteract(bool value)
    {
        CanInteract = value;
    }

    private void Update()
    {
        if (CanInteract && Physics.Raycast(transform.position, transform.forward, out hit, 5f, layerMask))
        {
            GameObject newTarget = hit.collider.gameObject;
            if (IsValidInteractable(newTarget))
            {
                interactableObject?.EndGlow();
                target = newTarget;
                if (!ImmInteractable)
                {
                    ImmInteractable = true;
                }
                return;
            }
        }
        if (ImmInteractable) ImmInteractable = false;
    }
    private bool IsValidInteractable(GameObject target)
    {
        return target.layer == LayerMask.NameToLayer("Interactable")
        && target.GetComponent<IInteractable>() != null
        && target.GetComponent<IInteractable>().IsInteractable();
    }

    private void DisableInteraction()
    {
        HideInteractableUI();
        if (interactableObject == null) return;
        interactableObject?.EndGlow();
        interactableObject = null;
        target = null;
    }

    private void EnableInteraction(GameObject newTarget)
    {
        ShowInteractableUI(newTarget);
        interactableObject = newTarget.GetComponent<InteractableObject>();
        interactableObject.StartGlow();
        target = newTarget;
    }

    public void HandleInteraction()
    {
        if (GameManager.GetInstance().GetState() != GameState.Playing || !ImmInteractable) return;

        GameObject target = hit.transform.gameObject;
        var interactable = target.GetComponent<IInteractable>();
        interactable?.Interact(target);
        interactableObject?.EndGlow();
        interactableObject = null;
        hasInteractedWithObject = true;
    }

    private void ShowInteractableUI(GameObject newTarget)
    {
        GameManager.GetInstance().um.ShowInteractionInfo(newTarget);
    }

    private void HideInteractableUI()
    {
        GameManager.GetInstance().um.HideInteractionInfo();
    }

    public void Attack()
    {
        Vector3 direction = transform.forward;
        float degree = Mathf.Rad2Deg * Mathf.Atan2(-direction.z, direction.x);
        float degree2 = -transform.localRotation.eulerAngles.x;
        Debug.Log(degree2);
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(new Vector3(0, degree, degree2)));
        if (am == null) am = GameManager.GetInstance().am;
        am.Shoot();
        BulletBehaviour bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
        bulletBehaviour.Shoot(direction);
    }

    public bool HasInteractedWithObject()
    {
        return hasInteractedWithObject;
    }
}
