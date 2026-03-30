using UnityEngine;
using WebBridge;
using System;

public class RotationChanger : MonoBehaviour, IRawSubscriber
{
    [SerializeField] private Bridge bridge;
    [SerializeField] private string topic = "rotation";


    private void OnEnable()
    {
        if (bridge == null)
        {
            bridge = Bridge.Instance != null ? Bridge.Instance : FindFirstObjectByType<Bridge>();
        }

        if (bridge != null)
        {
            bridge.Register(this, topic);
        }
    }

    private void OnDisable()
    {
        if (bridge != null)
        {
            bridge.Unregister(this);
        }
    }

    public void ChangeRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void OnMessageReceived(string topic, string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            Debug.LogWarning("Received empty payload.", this);
            return;
        }

        RotationPayload payload;
        try
        {
            payload = JsonUtility.FromJson<RotationPayload>(payloadJson);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to parse rotation payload: {ex.Message}", this);
            return;
        }

        ChangeRotation(payload.angle);
    }

    [Serializable]
    private struct RotationPayload
    {
        public float angle;
    }
}
