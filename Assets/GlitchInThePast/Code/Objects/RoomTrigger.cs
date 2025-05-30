using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public AnvilDropping anvilRef;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anvilRef.PlayerEnteredRoom(other.gameObject); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anvilRef.PlayerLeftRoom();
        }
    }
}