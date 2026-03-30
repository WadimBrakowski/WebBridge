using UnityEngine;
using WebBridge;

public class ColorChanger : MonoBehaviour, IRawSubscriber
{
    [SerializeField] private Bridge bridge;
    [SerializeField] private string topic = "color";

    private Renderer cachedRenderer;

    private void Awake()
    {
        cachedRenderer = GetComponent<Renderer>();
        if (cachedRenderer == null)
        {
            Debug.LogWarning("ColorChanger requires a Renderer component.", this);
        }
    }

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

    public void ChangeColor(string colorHex)
    {
        if (cachedRenderer == null)
        {
            return;
        }

        if (ColorUtility.TryParseHtmlString(colorHex, out var newColor))
        {
            cachedRenderer.material.color = newColor;
            return;
        }

        Debug.LogWarning($"Invalid color payload '{colorHex}'.", this);
    }

    public void OnMessageReceived(string receivedTopic, string payloadJson)
    {
        if (!string.Equals(receivedTopic, topic, System.StringComparison.Ordinal))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            Debug.LogWarning("ColorChanger received empty payload.", this);
            return;
        }

        ColorChangePayload payload;
        try
        {
            payload = JsonUtility.FromJson<ColorChangePayload>(payloadJson);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"ColorChanger failed to parse payload: {ex.Message}", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(payload.colorHex))
        {
            Debug.LogWarning("ColorChanger payload does not contain 'colorHex'.", this);
            return;
        }

        ChangeColor(payload.colorHex);
    }

    [System.Serializable]
    private struct ColorChangePayload
    {
        public string colorHex;
    }
}