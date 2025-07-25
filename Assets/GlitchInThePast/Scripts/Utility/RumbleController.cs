using UnityEngine;
using UnityEngine.InputSystem; // Import the Input System namespace

public class RumbleController : MonoBehaviour
{
    public float rumbleDuration = 0.5f; // Duration of the rumble
    public float lowFrequencyIntensity = 0.5f; // Intensity of low-frequency motor
    public float highFrequencyIntensity = 0.8f; // Intensity of high-frequency motor

    public void TriggerRumble()
    {
        if (Gamepad.current != null)
        {
            // Set motor speeds
            Gamepad.current.SetMotorSpeeds(lowFrequencyIntensity, highFrequencyIntensity);

            // Stop rumble after a duration
            Invoke("StopRumble", rumbleDuration); 
        }
    }

    private void StopRumble()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f); // Stop all vibration
        }
    }
}
