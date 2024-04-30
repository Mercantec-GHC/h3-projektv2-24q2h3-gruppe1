#include <Servo.h>
#include <WiFiNINA.h> // Include WiFi library
#include <ArduinoJson.h>
#include <ArduinoHttpClient.h>
#include <Arduino_MKRIoTCarrier.h>

Servo myServo1;
Servo myServo2;
MKRIoTCarrier carrier;

// WiFi credentials
char ssid[] = "H3Gruppe1";
char pass[] = "Merc1234";

const int serverPort = 443; // Use port 443 for HTTPS
const char* endpoint = "/api/PlantOverviews";
const char* serverAddress = "h3-projektv2-24q2h3-gruppe1-sqve.onrender.com";

WiFiSSLClient wifi;
HttpClient client = HttpClient(wifi, serverAddress, serverPort);

// Define the pins connected to the moisture sensors
bool autoMode = true;
bool displayOn = true;
String modePrint = "";
bool autoUpdate = false;     // Flag to control automatic update
const int moisturePin = A6;  // Change this if needed
const int moisturePin2 = A5; // Change this if needed
const long interval = 10000; // Interval at which to update (10 seconds)
bool showNotification = false;
unsigned long lastPostTime = 0;
unsigned long previousMillis = 0; // Store last update time
const unsigned long postInterval = 10000; // 10 seconds between posts

// Read moisture sensor values
const int wet = 0; // Value for wet sensor
const int dry = 1023; // Value for dry sensor
int moistureValue = analogRead(moisturePin);
int moistureValue2 = analogRead(moisturePin2);

// Convert the measured values to a percentage with map()
int percentageHumididySensor;
int percentageHumididySensor2;

// -------------------------------------------------------------- //

void setup() {
    // Initialize carrier and display
    carrier.begin();
    carrier.display.setRotation(2); // Adjust rotation to your setup
    Serial.begin(9600); // Initialize serial communication for debugging

    myServo1.attach(8); // Attaching the motor to pin 8
    myServo2.attach(9); // Attaching the motor to pin 9

    connectToWiFi();
    Serial.println("Setup complete");
}

// -------------------------------------------------------------- //

void loop() {
    carrier.Buttons.update();

    percentageHumididySensor = map(moistureValue, wet, dry, 100, 0);
    percentageHumididySensor2 = map(moistureValue2, wet, dry, 100, 0);

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

    unsigned long currentMillis = millis();
    if (autoUpdate && displayOn && currentMillis - previousMillis >= interval) {
        previousMillis = currentMillis; // Save the last update time
        displayMoisture();
    }

    if (currentMillis - lastPostTime >= postInterval) {
        lastPostTime = currentMillis;
        sendPostRequest(percentageHumididySensor, "s1", "sol");
        sendPostRequest(percentageHumididySensor2, "s2", "sne");
    }

    delay(10); // Short delay for button responsiveness
}

// -------------------------------------------------------------- //

void displayMoisture() {
    moistureValue = analogRead(moisturePin);
    moistureValue2 = analogRead(moisturePin2);

    percentageHumididySensor = map(moistureValue, wet, dry, 100, 0);
    percentageHumididySensor2 = map(moistureValue2, wet, dry, 100, 0);

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
    }
}

// -------------------------------------------------------------- //

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

// -------------------------------------------------------------- //

void notification() {
    carrier.display.fillScreen(ST77XX_RED); // Clear the screen with red
    carrier.display.setTextSize(2); // Set text size
    carrier.display.setTextColor(ST77XX_WHITE); // Set text color
    carrier.display.setCursor(10, 100); // Set position to start writing text
    if(moistureValue < 1000 && moistureValue2 < 1000) {
        carrier.display.print("Both plants are dying!");
    }
    else if(moistureValue < 1000) {
        carrier.display.print("Plant 1 is dying!");
    }
    else if(moistureValue2 < 1000) {
        carrier.display.print("Plant 2 is dying!");
    }
}

// -------------------------------------------------------------- //

void connectToWiFi() {
    Serial.println("Attempting to connect to WiFi...");
    while (WiFi.begin(ssid, pass) != WL_CONNECTED) {
        Serial.println("Connection failed, retrying...");
        delay(5000);
    }
    Serial.println("Connected to WiFi!");
    Serial.print("IP Address: ");
    Serial.println(WiFi.localIP());
}

// -------------------------------------------------------------- //

void sendPostRequest(int moisturePercentage, const char* sensorName, const char* plantName) {
    DynamicJsonDocument doc(1024);
    doc["moistureLevel"] = moisturePercentage;
    doc["SensorName"] = sensorName;
    doc["PlantName"] = plantName;

    String payload;
    serializeJson(doc, payload);

    Serial.println("Sending POST request");
    client.beginRequest();
    client.post(endpoint);
    client.sendHeader("Content-Type", "application/json");
    client.sendHeader("Content-Length", payload.length());
    client.endRequest();
    client.print(payload);

    int statusCode = client.responseStatusCode();
    String response = client.responseBody();
//makes a json object
    DynamicJsonDocument doc(1024);
    float minWaterLevel = doc["minWaterLevel"];
    float maxWaterLevel = doc["maxWaterLevel"];
    Serial.print("HTTP Response Code: ");
    Serial.println(statusCode);
    Serial.print("Response Body: ");
    Serial.println(response);
}
void sendGetRequest() {
    Serial.println(statusCode);
    Serial.print("Response: ");
    Serial.println(response);
//makes a json object
    DynamicJsonDocument doc(1024);
    float minWaterLevel = doc["minWaterLevel"];
    float maxWaterLevel = doc["maxWaterLevel"];
    delay(10000);
}