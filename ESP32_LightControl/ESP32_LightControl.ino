// ESP32 Light Control for IDKI Personal Project
// Potentiometer controls directional light brightness in Unity
// Built-in LED lights up on game over (receives LED_INTENSITY from Unity)
//
// Requires: 'WebSockets' by Markus Sattler and 'Adafruit NeoPixel' libraries
//
// Wiring:
//   - Potentiometer middle pin -> GPIO 9 (POT_PIN)
//   - Potentiometer outer pins -> 3.3V and GND

#include <WiFi.h>
#include <WebSocketsClient.h>
#include <Adafruit_NeoPixel.h>
#include <header.h>

// --------------------------------------------------
// WiFi / Server Settings — UPDATE THESE
// --------------------------------------------------
const char *ssid       = "dsv-extrality-lab";
const char *password   = "expiring-unstuck-slider";
const char* serverIP   = "SERVER_IP_HERE"; // Replace with your Python server's IP
const uint16_t serverPort = 8081;

// --------------------------------------------------
// Hardware pins
// --------------------------------------------------
#define POT_PIN 9

// --------------------------------------------------
// WebSocket events
// --------------------------------------------------
void webSocketEvent(WStype_t type, uint8_t* payload, size_t length) {
  switch(type) {
    case WStype_CONNECTED: {
      Serial.printf("WebSocket connected to %s:%u\n", serverIP, serverPort);
      String message = String("Device: ") + BOARD_NAME + " ... MAC: " + WiFi.macAddress();
      webSocket.sendTXT(message);
      break;
    }
    case WStype_DISCONNECTED:
      Serial.println("Not connected to WebSocket server... Retrying in 5 seconds");
      break;
    case WStype_TEXT:
      payload[length] = '\0';
      Serial.print("WebSocket received: ");
      Serial.println((char*)payload);
      handleMessage(String((char*)payload));
      break;
  }
}

// --------------------------------------------------
// Handle incoming messages from Unity (via server)
// --------------------------------------------------
void handleMessage(const String& message) {
  int sep = message.indexOf(':');
  if (sep == -1) return;

  String type = message.substring(0, sep);
  int value = message.substring(sep + 1).toInt();

  if (type.equalsIgnoreCase("LED_INTENSITY")) {
    value = constrain(value, 0, 255);
    ledSet(value);
    Serial.printf("LED intensity -> %d\n", value);
  }
}

// --------------------------------------------------
// Setup
// --------------------------------------------------
void setup() {
  Serial.begin(115200);
  delay(100);

  Serial.println();
  Serial.println("=================================");
  Serial.print("Compiled for board: ");
  Serial.println(BOARD_NAME);
  Serial.println("=================================");

  #ifdef LED_TYPE_GPIO
    ledcAttach(LED_PIN, LEDC_FREQUENCY, LEDC_RESOLUTION);
    ledcWrite(LED_PIN, 0);
  #endif

  #ifdef LED_TYPE_RGB
    rgbLed.begin();
    rgbLed.clear();
    rgbLed.show();
  #endif

  WiFi.begin(ssid, password);
  Serial.printf("Connecting to WiFi: %s", ssid);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nWiFi connected");
  Serial.printf("Attempting to connect to WebSocket Server on %s\n", serverIP);

  webSocket.begin(serverIP, serverPort, "/");
  webSocket.onEvent(webSocketEvent);
  webSocket.setReconnectInterval(5000);
}

// --------------------------------------------------
// Loop — reads potentiometer and sends value
// --------------------------------------------------
void loop() {
  webSocket.loop();

  int rawPotVal = analogRead(POT_PIN);
  potVal = map(rawPotVal, 0, 8191, 0, 100);

  unsigned long currentTime = millis();
  if (lastPotValue != potVal && (currentTime - lastSendTime >= 100)) {
    webSocket.sendTXT("potVal:" + String(potVal));
    Serial.print("Pot Value: ");
    Serial.println(potVal);
    lastPotValue = potVal;
    lastSendTime = currentTime;
  }
}
