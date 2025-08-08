using UnityEngine;
using UnityEngine.Events;

namespace JW.Objects.Interactibles
{
    public class DoorInteractible : Interactible
    {
        [Header("Door Interactable", order = 0)]
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite closedSprite;
        [SerializeField] private Sprite openSprite;
        [SerializeField] private bool startOpen = false;
        
        [SerializeField] private UnityEvent onOpen;
        [SerializeField] private UnityEvent onClose;

        void OnEnable()
        {
            boxCollider.isTrigger = startOpen; // Switch the door's collision on/off (closed/open)
            
            if (!boxCollider.isTrigger) // We are not a trigger so we are closed
            {
                spriteRenderer.sprite = closedSprite;
            }
            else
            {
                spriteRenderer.sprite = openSprite;
            }
        }
        
        public override void Interact()
        {
            base.Interact(); // Trigger the event
            
            boxCollider.isTrigger = !boxCollider.isTrigger; // Switch the door's collision on and off
            Debug.Log("Door Interact");
            if (!boxCollider.isTrigger) // We are not a trigger so we are closed
            {
                spriteRenderer.sprite = closedSprite;
                onClose?.Invoke();
            }
            else
            {
                spriteRenderer.sprite = openSprite;
                onOpen?.Invoke();
            }
        }
    }
}