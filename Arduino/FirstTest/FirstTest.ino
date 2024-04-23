
#include <Arduino_MKRIoTCarrier.h>
#include <WiFiNINA.h> // Include WiFi library
#include <ArduinoHttpClient.h>

MKRIoTCarrier carrier;


// WiFi credentials
char ssid[] = "H3Gruppe1";
char pass[] = "Merc1234";

const char* serverAddress = "192.168.1.138";
const int serverPort = 80;
const char* endpoint = "/API/PlantSensor/";

WiFiClient wifi;
HttpClient client = HttpClient(wifi, serverAddress, serverPort);

// Define the pins connected to the moisture sensors
const int moisturePin = A6;  // Change this if needed
const int moisturePin2 = A5; // Change this if needed
bool displayOn = true;
bool autoUpdate = false;     // Flag to control automatic update
unsigned long previousMillis = 0; // Store last update time
const long interval = 10000;  // Interval at which to update (10 seconds)
bool autoMode = true;
String modePrint = "";
bool showNotification = false;

// Read moisture sensor values
int moistureValue = analogRead(moisturePin);
int moistureValue2 = analogRead(moisturePin2);
const int dry = 1000; // Value for dry sensor
const int wet = 239; // Value for wet sensor

// Convert the measured values to a percentage with map()
int percentageHumididySensor;
int percentageHumididySensor2;

void setup() {
    // Initialize carrier and display
    CARRIER_CASE = false;
    carrier.begin();
    carrier.display.setRotation(2); // Adjust rotation to your setup
    Serial.begin(9600); // Initialize serial communication for debugging

     connectToWiFi();
     Serial.println("Setup complete");

}

void loop() {
    carrier.Buttons.update();
    testServerConnection();

    //WiFiClient client = server.available(); // Listen for incoming clients

    percentageHumididySensor = map(moistureValue, wet, dry, 100, 0);
    percentageHumididySensor2 = map(moistureValue2, wet, dry, 100, 0);
    
    // Check if TOUCH0 is pressed to turn off the lights
    if (carrier.Buttons.onTouchDown(TOUCH0)) {
        displayOn = false;
        autoUpdate = false; // Stop automatic updates
        carrier.display.fillScreen(ST77XX_BLACK); // Turn off all LEDs
        delay(50);
    } else if (carrier.Buttons.onTouchDown(TOUCH1)) {
        displayOn = true; // Ensure display is on
        autoUpdate = !autoUpdate; // Toggle auto-update state
        displayMoisture(); // Display immediately when button is pressed
        delay(50);
    } else if (carrier.Buttons.onTouchDown(TOUCH2)) {
      mode();
      delay(50);
    } else if (carrier.Buttons.onTouchDown(TOUCH4)) { // Reset notification on TOUCH3
      showNotification = false;
      delay(50);
    }

    if ((moistureValue < 1000 || moistureValue2 < 1000) && !showNotification && modePrint == "Mode: manuel") {
      notification();
      showNotification = true; // Set the flag after showing notification
    }
    if ((moistureValue < 1000 && moistureValue2 < 1000) && !showNotification && modePrint == "Mode: manuel") {
      notification();
      showNotification = true; // Set the flag after showing notification
    }
    if ((moistureValue < 1000 || moistureValue2 < 1000) && showNotification) {
      showNotification = false; // Set the flag after showing notification
    }

    // Check if it's time to update the display
    unsigned long currentMillis = millis();
    if (autoUpdate && displayOn && currentMillis - previousMillis >= interval) {
        previousMillis = currentMillis; // Save the last update time
        displayMoisture();
    }
    

    delay(10); // Short delay for button responsiveness
}

void displayMoisture() {
    moistureValue = analogRead(moisturePin);
    moistureValue2 = analogRead(moisturePin2);

    // Print the moisture values to the serial monitor
    Serial.print("Moisture Level: ");
    Serial.println(percentageHumididySensor);
    Serial.print("Moisture 2 Level: ");
    Serial.println(percentageHumididySensor2);

    // Display the moisture values on the carrier's OLED
    if (displayOn) {
        carrier.display.fillScreen(ST77XX_BLUE); // Clear the screen with blue
        carrier.display.setTextSize(2); // Set text size
        carrier.display.setTextColor(ST77XX_WHITE); // Set text color
        carrier.display.setCursor(20, 80); // Set position to start writing text
        carrier.display.print("Moisture 1: ");
        carrier.display.println(percentageHumididySensor);
        carrier.display.setCursor(20, 100); // Change y coordinate for second line
        carrier.display.print("Moisture 2: ");
        carrier.display.println(percentageHumididySensor2);
          // Create JSON payload
          //  String jsonPayload = "{\"SensorController\": " + String(moistureValue) + "}";
          //  String jsonPayload2 = "{\"SensorController\": " + String(moistureValue2) + "}";
          //  // Send POST request
          // sendPostRequest(jsonPayload);
          // sendPostRequest(jsonPayload2);

    }
}

void mode() {
  if(autoMode) {
    autoMode = false;
    carrier.display.fillScreen(ST77XX_BLUE); // Clear the screen with blue
    carrier.display.setTextSize(2); // Set text size
    carrier.display.setTextColor(ST77XX_WHITE); // Set text color
    carrier.display.setCursor(60, 100); // Set position to start writing text
    carrier.display.print("Mode: manuel");
    modePrint = "Mode: manuel";
  }
  else if(!autoMode) {
    autoMode = true;
    carrier.display.fillScreen(ST77XX_BLUE); // Clear the screen with blue
    carrier.display.setTextSize(2); // Set text size
    carrier.display.setTextColor(ST77XX_WHITE); // Set text color
    carrier.display.setCursor(60, 100); // Set position to start writing text
    carrier.display.print("Mode: auto");
    modePrint = "Mode: auto";
  }
}

void notification() {
    carrier.display.fillScreen(ST77XX_RED); // Clear the screen with blue
    carrier.display.setTextSize(2); // Set text size
    carrier.display.setTextColor(ST77XX_WHITE); // Set text color
    carrier.display.setCursor(10, 100); // Set position to start writing text
    if(moistureValue < 1000 && moistureValue2 < 1000) {
      carrier.display.print("Both plants is dying!");
    }
    else if(moistureValue < 1000) {
      carrier.display.print("Plant 1 dying!");
    }
    else if(moistureValue2 < 1000) {
      carrier.display.print("Plant 2 dying!");
    }
}

void connectToWiFi() {
  Serial.println("Attempting to connect to WiFi...");
    while (WiFi.begin(ssid, pass) != WL_CONNECTED) {
        // If connection failed, retry
        Serial.println("Connection failed, retrying...");
        delay(5000);
    }
    Serial.println("Connected to WiFi!");
    Serial.print("IP Address: ");
    Serial.println(WiFi.localIP());
}

  // void sendPostRequest(String payload) {
  //  Serial.println("Sending POST request");

  //     client.beginRequest();
  //     client.post(endpoint);
  //     client.sendHeader("Content-Type", "application/json");
  //     client.sendHeader("Content-Length", payload.length());
  //    client.endRequest();

  //     // Send request body
  //     client.print(payload);

  //     // Check HTTP response
  //     int statusCode = client.responseStatusCode();
  //     String response = client.responseBody();

  //     Serial.print("HTTP Response Code: ");
  //     Serial.println(statusCode);
  //     Serial.print("Response Body: ");
  //     Serial.println(response);
  // }

void testServerConnection() {
    Serial.println("Testing server connection...");

    if (wifi.connect(serverAddress, serverPort)) {
        Serial.println("Connected to server!");

        // Specify the ID for the request
        int id = 1; // Change this to the ID you want to test

        // Send a GET request to the endpoint with the specified ID
        wifi.print(String("GET ") + endpoint  + " HTTP/1.1\r\n" +
                   "Host: " + serverAddress + "\r\n" +
                   "Connection: close\r\n\r\n");

        // Wait for the server to respond
        while (wifi.connected()) {
            if (wifi.available()) {
                // Print the server's response to the Serial Monitor
                Serial.write(wifi.read());
            }
        }

        // Close the connection to the server
        wifi.stop();
    } else {
        Serial.println("Failed to connect to server!");
    }
    delay(10000);
}