using UnityEngine;
using NativeWebSocket;

public class ESP32Controller : MonoBehaviour
{
    [Header("WebSocket")]
    [SerializeField] private string serverIP = "10.204.0.29";
    [SerializeField] private int serverPort = 8081;

    [Header("Lighting")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 3f;

    private WebSocket websocket;
    private float targetIntensity;
    private bool connected;

    async void Start()
    {
        if (directionalLight != null)
            targetIntensity = directionalLight.intensity;

        websocket = new WebSocket("ws://" + serverIP + ":" + serverPort);

        websocket.OnOpen += () =>
        {
            Debug.Log("[ESP32] Connected to WebSocket server");
            connected = true;
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            ParseMessage(message);
        };

        websocket.OnClose += (code) =>
        {
            Debug.Log("[ESP32] WebSocket closed");
            connected = false;
        };

        websocket.OnError += (error) =>
        {
            Debug.LogWarning("[ESP32] WebSocket error: " + error);
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (websocket != null)
            websocket.DispatchMessageQueue();
#endif

        if (directionalLight != null)
            directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, targetIntensity, Time.unscaledDeltaTime * 10f);
    }

    private void ParseMessage(string message)
    {
        int sep = message.IndexOf(':');
        if (sep == -1) return;

        string type = message.Substring(0, sep);
        string valueStr = message.Substring(sep + 1);

        if (type == "potVal" && int.TryParse(valueStr, out int potValue))
        {
            // Map 0-100 potentiometer to min-max light intensity
            targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, potValue / 100f);
        }
    }

    public async void SendLedOn()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("LED_INTENSITY:255");
            Debug.Log("[ESP32] Sent LED ON");
        }
    }

    public async void SendLedOff()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("LED_INTENSITY:0");
            Debug.Log("[ESP32] Sent LED OFF");
        }
    }

    async void OnDestroy()
    {
        if (websocket != null)
        {
            SendLedOff();
            await websocket.Close();
        }
    }
}
