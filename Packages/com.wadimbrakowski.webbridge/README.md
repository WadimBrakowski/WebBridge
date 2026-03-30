# Web Bridge

A small runtime messaging bridge for Unity components.

## Installation

Add this package in Unity Package Manager using a Git URL:

- `https://github.com/<owner>/<repo>.git?path=/Packages/com.wadimbrakowski.webbridge#0.1.0`

You can also target a branch (for development only), for example:

- `https://github.com/<owner>/<repo>.git?path=/Packages/com.wadimbrakowski.webbridge#main`

## Quick Start

1. Add `Bridge` to a scene object.
2. Add `ColorChanger` to a renderer object.
3. Publish a message from browser or test code using `Bridge.Publish(string envelopeJson)`.

Example envelope:

```json
{
	"topic": "color",
	"payload": "{\"colorHex\":\"#2dd4bf\"}"
}
```

## Browser / WebGL Call

Unity WebGL `SendMessage` can only call public, non-generic instance methods by name.
Use this call from JavaScript:

```js
unityInstance.SendMessage("Bridge", "Publish", JSON.stringify({
	topic: "color",
	payload: JSON.stringify({
		colorHex: "#2dd4bf"
	})
}));
```

`Bridge.Publish(string envelopeJson)` parses the envelope and routes the raw payload string by topic.
Subscribers deserialize their own payload model.

## Play Mode Test

You can test the same flow in Play Mode with a helper component (for example `Tester`) that does:

```csharp
bridgeObject.SendMessage("Publish", envelopeJson, SendMessageOptions.RequireReceiver);
```

This mirrors the browser behavior.

## Notes

- Topic matching is exact (`StringComparison.Ordinal`).
- Empty subscriber topic matches every message topic.
- Bridge routing is raw-only: subscribers receive `topic` and raw `payload` and deserialize their own model.
