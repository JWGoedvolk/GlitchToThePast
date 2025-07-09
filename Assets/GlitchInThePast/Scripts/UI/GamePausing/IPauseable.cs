/// <summary>
/// Use this interface for anything you wish to spawn into the game to pause them. (freeze them)
/// </summary>

public interface IPauseable
{
    void OnPause();
    void OnUnpause();
}

#region Tips
///<summary>
///
/// Add these lines to your start and ondestroy to register and unregister your objects
/// Same thing can be done with OnEnable and OnDisable if there is no destroy
/// 
/// void Start()
///{
///   GamePauser.Instance?.RegisterPauseable(this);
///
///void OnDestroy()
///{
///    GamePauser.Instance?.UnregisterPauseable(this);
///}
///
/// Also implement these two Functions in the scripts you marked as pauseable.
/// You can mark them as pauseable by Adding , IPauseable next to MonoBehaviour
/// i.e. MonoBehaviour, IPauseable
/// 
/// public void OnPause()
///{
///    enabled = false; // or set a paused bool and check in Update
///}
///
///public void OnUnpause()
///{
///   enabled = true;
///}
/// 
/// </summary>
#endregion