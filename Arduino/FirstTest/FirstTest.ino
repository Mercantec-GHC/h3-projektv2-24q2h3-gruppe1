#include <Servo.h>
#include <WiFiNINA.h> // Include WiFi library
#include <ArduinoJson.h>
#include <ArduinoHttpClient.h>
#include <Arduino_MKRIoTCarrier.h>

MKRIoTCarrier carrier;
Servo myServo1;
Servo myServo2;


// WiFi credentials
char ssid[] = "H3Gruppe1";
char pass[] = "Merc1234";

const int serverPort = 443; // Use port 443 for HTTPS
const char* endpoint = "/api/PlantOverviews/postvalue";
const char* serverAddress = "h3-projektv2-24q2h3-gruppe1-pjo3.onrender.com";

WiFiSSLClient wifi;
HttpClient client = HttpClient(wifi, serverAddress, serverPort);

// Define the pins connected to the moisture sensors
bool autoMode;
bool displayOn = true;
String modePrint = "";
bool autoUpdate = false;     // Flag to control automatic update
const int moisturePin = A6;  // Change this if needed
const int moisturePin2 = A5; // Change this if needed
const long interval = 60000; // Interval at which to update (1 minut)
bool showNotification = false;
unsigned long previousMillis = 0; // Store last update time

// Read moisture sensor values
const int wet = 0; // Value for wet sensor
const int dry = 1023; // Value for dry sensor
int moistureValue = analogRead(moisturePin);
int moistureValue2 = analogRead(moisturePin2);
int MinWaterLevelplant;
int MaxWaterLevelplant;
int MinWaterLevelplant2;
int MaxWaterLevelplant2; 

// Convert the measured values to a percentage with map()
int percentageHumididySensor;
int percentageHumididySensor2;

// Retrieving Arduino's unique ID
int uniqueArduinoID = 1;
int userID = 0;
String selectedPlant1Arduino;
String selectedPlant2Arduino;
boolean activeMotor = false;

// -------------------------------------------------------------- //

void setup() {
    // Initialize carrier and display
    carrier.noCase();
    carrier.begin();

    Serial.begin(9600); // Initialize serial communication for debugging

 

    connectToWiFi();
                sendGetRequestmaxminPlants();

    sendGetRequestArduinoID();
    sendGetRequestArduinoPlants();
      //    myServo1.attach(8); // Attaching the motor to pin 8
      // myServo2.attach(9); // Attaching the motor to pin 9
    Serial.println("Setup complete");
}

// -------------------------------------------------------------- //

void loop() {
  if(userID != 0) {
    handleButtons();
    percentageHumididySensor = map(moistureValue, wet, dry, 100, 0);
    percentageHumididySensor2 = map(moistureValue2, wet, dry, 100, 0);
    if ((moistureValue < 1000 || moistureValue2 < 1000) && !showNotification && modePrint == "Mode: manuel") {
        notification();
        showNotification = true; // Set the flag after showing notification
      
    }
    else if ((moistureValue < 1000 || moistureValue2 < 1000) && showNotification) {
        showNotification = false; // Set the flag after showing notification

    }
    if (percentageHumididySensor < MinWaterLevelplant && modePrint == "Mode: auto") {
       turnOnMotor1();

    }
     else if (percentageHumididySensor2 < MinWaterLevelplant2 && modePrint == "Mode: auto") {
       turnOnMotor2();

    }

    unsigned long currentMillis = millis();
    if (autoUpdate && displayOn && currentMillis - previousMillis >= interval) {
        previousMillis = currentMillis; // Save the last update time
        displayMoisture();
            sendGetRequestmaxminPlants();
        sendGetRequestArduinoPlants();
        sendPutSettingRequest(autoMode);
        sendPostRequest(percentageHumididySensor, 1, selectedPlant1Arduino, uniqueArduinoID);
        sendPostRequest(percentageHumididySensor2, 2, selectedPlant2Arduino, uniqueArduinoID);
    }

    delay(10); // Short delay for button responsivenes
  }
}

// -------------------------------------------------------------- //

void handleButtons() {
    carrier.Buttons.update();
    if (carrier.Buttons.onTouchDown(TOUCH0)) {
        displayOn = false;
        autoUpdate = false; // Stop automatic updates
        carrier.display.fillScreen(ST77XX_BLACK); // Turn off all LEDs
        delay(50);
    } else if (carrier.Buttons.onTouchDown(TOUCH1)) {
        displayOn = true; // Ensure display is on
        autoUpdate = true; // Toggle auto-update state
        displayMoisture(); // Display immediately when button is pressed
        delay(50);
    } else if (carrier.Buttons.onTouchDown(TOUCH2)) {
        mode();
        delay(50);
    } else if (carrier.Buttons.onTouchDown(TOUCH3)) {
        turnOnMotor1();
    } else if (carrier.Buttons.onTouchDown(TOUCH4)) {
        turnOffMotor();
    }
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
        carrier.display.setCursor(50, 180); // Set position to start writing text
        carrier.display.setTextSize(2); // Set text size
        carrier.display.print(modePrint);
    }
    delay(10);
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
    if(moistureValue < MinWaterLevelplant && moistureValue2 < MinWaterLevelplant2) {
        carrier.display.print("Both plants are dying!");
    }
    else if(moistureValue < MinWaterLevelplant) {
        carrier.display.print("Plant 1 is dying!");
    }
    else if(moistureValue2 < MinWaterLevelplant2) {
        carrier.display.print("Plant 2 is dying!");
    }
    delay(10);
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

void sendPostRequest(int moisturePercentage, int sensorID, String plantName, int arduinoId) {
    DynamicJsonDocument doc(1024);
    doc["moistureLevel"] = moisturePercentage;
    doc["sensorId"] = sensorID;
    doc["ArduinoId"] = arduinoId;
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
    
    Serial.print("HTTP Response Code: ");
    Serial.println(statusCode);
    Serial.print("Response Body: ");
    Serial.println(response);
    delay(10);
}

void sendGetRequestArduinoID() {

    // Make the HTTP GET request
    String arduinosEndpoint = "/api/Arduino/";

    client.beginRequest();
    client.get(arduinosEndpoint + uniqueArduinoID);
    client.sendHeader(HTTP_HEADER_CONNECTION, "close");
    client.endRequest();

    // Wait for the server's response
    int statusCode = client.responseStatusCode();
    String response = client.responseBody();

    // Print the response
    Serial.println(statusCode);
    Serial.print("Response: ");
    Serial.println(response);

    // Check if the response is OK
    if (statusCode == 200) { // Status code 200 indicates success
        // Parse the JSON response
        DynamicJsonDocument doc(1024);
        DeserializationError error = deserializeJson(doc, response);

        if (!error) {
            userID = doc["userId"];
        } else {
            Serial.print("deserializeJson() failed: ");
            Serial.println(error.c_str());
        }
    } else {
        Serial.print("Error on HTTP request: ");
        Serial.println(statusCode);
    }

    // Small delay to avoid spamming the server
    delay(10);
}
void sendGetRequestmaxminPlants() {
    // Make the HTTP GET request
    String plantsEndpoint = "/api/Plants/";

    client.beginRequest();
    client.get(plantsEndpoint);
    client.sendHeader(HTTP_HEADER_CONNECTION, "close");
    client.endRequest();

    // Wait for the server's response
    int statusCode = client.responseStatusCode();
    String response = client.responseBody();

    // Print the response
    Serial.println(statusCode);
    Serial.print("Response: ");
    Serial.println(response);

    // Check if the response is OK
    if (statusCode == 200) { // Status code 200 indicates success
        // Parse the JSON response
        DynamicJsonDocument doc(1024);
        DeserializationError error = deserializeJson(doc, response);

        if (!error) {
            JsonArray plants = doc.as<JsonArray>();

            for (JsonObject plant : plants) {
                int plantUserID = plant["userID"];
                String plantID = plant["plantID"].as<String>();

                if (plantUserID == userID) {
                    if (plantID == selectedPlant1Arduino) {
                        int minWaterLevel1 = plant["MinWaterLevel"];
                        int maxWaterLevel1 = plant["MaxWaterLevel"];
                        MinWaterLevelplant = minWaterLevel1;
                        MaxWaterLevelplant = maxWaterLevel1;
                    } else if (plantID == selectedPlant2Arduino) {
                        int minWaterLevel2 = plant["MinWaterLevel"];
                        int maxWaterLevel2 = plant["MaxWaterLevel"];
                        MinWaterLevelplant2 = minWaterLevel2;
                        MaxWaterLevelplant2 = maxWaterLevel2;
                    }
                }
            }
        } else {
            Serial.print("deserializeJson() failed: ");
            Serial.println(error.c_str());
        }
    } else {
        Serial.print("Error on HTTP request: ");
        Serial.println(statusCode);
    }

    // Small delay to avoid spamming the server
    delay(10);
}
void sendGetRequestArduinoPlants() {

    // Make the HTTP GET request
    String arduinosEndpoint = "/api/Settings/";

    client.beginRequest();
    client.get(arduinosEndpoint + userID);
    client.sendHeader(HTTP_HEADER_CONNECTION, "close");
    client.endRequest();

    // Wait for the server's response
    int statusCode = client.responseStatusCode();
    String response = client.responseBody();

    // Print the response
    Serial.println(statusCode);
    Serial.print("Response: ");
    Serial.println(response);

    // Check if the response is OK
    if (statusCode == 200) { // Status code 200 indicates success
        // Parse the JSON response
        DynamicJsonDocument doc(1024);
        DeserializationError error = deserializeJson(doc, response);

        if (!error) {
          selectedPlant1Arduino = doc["selectedPlant1"].as<String>();
          selectedPlant2Arduino = doc["selectedPlant2"].as<String>();
            autoMode = doc["AutoMode"];
        } else {
            Serial.print("deserializeJson() failed: ");
            Serial.println(error.c_str());
        }
    } else {
        Serial.print("Error on HTTP request: ");
        Serial.println(statusCode);
    }

    // Small delay to avoid spamming the server
    delay(10);
}

void sendPutSettingRequest(bool mode) {
   String arduinosEndpoint = "/api/Settings/mode/";
    DynamicJsonDocument doc(1024);
    doc["AutoMode"] = mode;
 
    String payload;
    serializeJson(doc, payload);
 
    Serial.println("Sending PUT request");

    // Make a HTTP request:
    client.beginRequest();
    client.put(arduinosEndpoint + userID, "application/json", payload);
    client.endRequest();
 
    int statusCode = client.responseStatusCode();
    String response = client.responseBody();
 
    Serial.print("HTTP Response Code: ");
    Serial.println(statusCode);
    Serial.print("Response Body: ");
    Serial.println(response);
    delay(10);
}
        // ----------------------------------------------------------------------------- //

void turnOnMotor1() {
    activeMotor = true;

    if (activeMotor) {
        // Rotate motor clockwise
        myServo1.write(180); // Set motor to maximum angle (clockwise)
        delay(3000); // Wait for 1 second

        // Stop the motor
        myServo1.write(90); // Set motor to stop position
        delay(3000); // Wait for 1 second

        // Rotate motor counterclockwise
        myServo1.write(0); // Set motor to minimum angle (counterclockwise)
        delay(3000); // Wait for 1 second

        // Stop the motor
        myServo1.write(90); // Set motor to stop position
        delay(1000); // Wait for 1 second
    }
}
void turnOnMotor2() {
    activeMotor = true;

    if (activeMotor) {
        // Rotate motor clockwise
        myServo2.write(180); // Set motor to maximum angle (clockwise)
        delay(3000); // Wait for 1 second

        // Stop the motor
        myServo2.write(90); // Set motor to stop position
        delay(3000); // Wait for 1 second

        // Rotate motor counterclockwise
        myServo2.write(0); // Set motor to minimum angle (counterclockwise)
        delay(3000); // Wait for 1 second

        // Stop the motor
        myServo2.write(90); // Set motor to stop position
        delay(3000); // Wait for 1 second
    }
}
void turnOffMotor() {
    activeMotor = false;

    if (!activeMotor) {
        // Rotate motor counterclockwise
        myServo1.write(0); // Set motor to minimum angle (counterclockwise)

        // ----------------------------------------------------------------------------- //

        // Rotate motor counterclockwise
        myServo2.write(0); // Set motor to minimum angle (counterclockwise)
    }
}

void activateServos() {
    myServo1.write(90);
    delay(1000);
    myServo1.write(0);

    myServo2.write(90);
    delay(1000);
    myServo2.write(0);
}