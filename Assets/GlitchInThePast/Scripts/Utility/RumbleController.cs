using UnityEngine;
using UnityEngine.InputSystem; // Import the Input System namespace

public class RumbleController : MonoBehaviour
{
    public static RumbleController Instance;
    
    public float rumbleDuration = 0.5f; // Duration of the rumble
    public float lowFrequencyIntensity = 0.5f; // Intensity of low-frequency motor
    public float highFrequencyIntensity = 0.8f; // Intensity of high-frequency motor

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(this);
            }
        }
    }
    
    public void TriggerRumble(float duration = 0.5f, float lfIntensity = 0.5f, float hfIntensity = 0.8f)
    {
        if (Gamepad.current != null)
        {
            // Set motor speeds
            Gamepad.current.SetMotorSpeeds(lfIntensity, hfIntensity);

            // Stop rumble after a duration
            Invoke("StopRumble", duration); 
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
