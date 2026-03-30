using System;
using System.Collections.Generic;
using UnityEngine;

namespace WebBridge
{
    public class Bridge : MonoBehaviour
    {
        private static Bridge instance;

        private readonly List<Subscription> subscriptions = new();

        public static Bridge Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning("More than one Bridge found. Destroying duplicate instance.", this);
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        public void Register(IRawSubscriber subscriber, string topic = null)
        {
            if (subscriber == null)
            {
                return;
            }

            for (var i = 0; i < subscriptions.Count; i++)
            {
                if (ReferenceEquals(subscriptions[i].Subscriber, subscriber) && IsSameTopic(subscriptions[i].Topic, topic))
                {
                    return;
                }
            }

            subscriptions.Add(new Subscription(subscriber, topic));
        }

        public void Unregister(IRawSubscriber subscriber)
        {
            if (subscriber == null)
            {
                return;
            }

            subscriptions.RemoveAll(subscription => ReferenceEquals(subscription.Subscriber, subscriber));
        }

        public void PublishRaw(string topic, string payloadJson)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                Debug.LogWarning("Publish topic is empty.", this);
                return;
            }

            Dispatch(topic, payloadJson ?? string.Empty);
        }

        // This overload is used by Unity WebGL SendMessage("Bridge", "Publish", envelopeJson).
        public void Publish(string envelopeJson)
        {
            if (string.IsNullOrWhiteSpace(envelopeJson))
            {
                Debug.LogWarning("Publish envelope is empty.", this);
                return;
            }

            PublishEnvelope envelope;
            try
            {
                envelope = JsonUtility.FromJson<PublishEnvelope>(envelopeJson);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to parse publish envelope: {ex.Message}", this);
                return;
            }

            if (string.IsNullOrWhiteSpace(envelope.topic))
            {
                Debug.LogWarning("Publish envelope does not contain 'topic'.", this);
                return;
            }

            PublishRaw(envelope.topic, envelope.payload);
        }

        private void Dispatch(string topic, string payloadJson)
        {
            for (var i = 0; i < subscriptions.Count; i++)
            {
                if (!IsTopicMatch(subscriptions[i].Topic, topic))
                {
                    continue;
                }

                subscriptions[i].Subscriber.OnMessageReceived(topic, payloadJson);
            }
        }

        private static bool IsTopicMatch(string subscriptionTopic, string messageTopic)
        {
            if (string.IsNullOrEmpty(subscriptionTopic))
            {
                return true;
            }

            return string.Equals(subscriptionTopic, messageTopic, StringComparison.Ordinal);
        }

        private static bool IsSameTopic(string left, string right)
        {
            return string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal);
        }

        private readonly struct Subscription
        {
            public Subscription(IRawSubscriber subscriber, string topic)
            {
                Subscriber = subscriber;
                Topic = topic ?? string.Empty;
            }

            public IRawSubscriber Subscriber { get; }
            public string Topic { get; }
        }

        [Serializable]
        private struct PublishEnvelope
        {
            public string topic;
            public string payload;
        }
    }

}