using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] private string bridgeObjectName = "Bridge";
    [SerializeField] private string topic = "color";
    [SerializeField] private string colorHex = "#2dd4bf";

    [ContextMenu("Test Publish (wie Browser)")]
    public void TestPublishLikeBrowser()
    {
        var bridgeObject = GameObject.Find(bridgeObjectName);
        if (bridgeObject == null)
        {
            Debug.LogError($"Bridge object '{bridgeObjectName}' not found.", this);
            return;
        }

        var payload = JsonUtility.ToJson(new ColorPayload
        {
            colorHex = colorHex
        });

        var envelope = JsonUtility.ToJson(new BridgePublishEnvelope
        {
            topic = topic,
            payload = payload
        });

        bridgeObject.SendMessage("Publish", envelope, SendMessageOptions.RequireReceiver);
        Debug.Log($"SendMessage to '{bridgeObjectName}.Publish' sent: {envelope}", this);
    }

    [System.Serializable]
    private struct BridgePublishEnvelope
    {
        public string topic;
        public string payload;
    }

    [System.Serializable]
    private struct ColorPayload
    {
        public string colorHex;
    }
}
