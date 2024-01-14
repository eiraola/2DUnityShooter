using UnityEngine.Events;
using UnityEngine;
public class OnTargetDetectedEvent: UnityEvent<Transform, bool> { }
public class TargetDetector : MonoBehaviour
{
    public OnTargetDetectedEvent onTargetDetected = new OnTargetDetectedEvent();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(Constants.TAG_PLAYER))
        {
            return;
        }
        onTargetDetected?.Invoke(collision.transform, true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(Constants.TAG_PLAYER))
        {
            return;
        }
        onTargetDetected?.Invoke(collision.transform, false);
    }
}
