#include <Arduino_MKRIoTCarrier.h>
#include <SPI.h>
#include <WiFi101.h>
MKRIoTCarrier carrier;

// Connection to wifi
char ssid[] = "H3Gruppe1";   // your network SSID (name)
char pass[] = "Merc1234";    // your network password (use for WPA, or use as key for WEP)

IPAddress subnet(255, 255, 255, 0);
IPAddress gateway(192, 168, 1, 1);
IPAddress local_IP(192, 168, 1, 200);

int status = WL_IDLE_STATUS;
unsigned int localPort = 4000;

WiFiServer server(80);
// Connection to wifi end

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
const int dry = 1023; // Value for dry sensor
const int wet = 0; // Value for wet sensor

// Convert the measured values to a percentage with map()
int percentageHumididySensor;
int percentageHumididySensor2;

void setup() {
    // Initialize carrier and display
    CARRIER_CASE = false;
    carrier.begin();
    carrier.display.setRotation(2); // Adjust rotation to your setup
    Serial.begin(9600); // Initialize serial communication for debugging¨

    while(!Serial) {
      ; //Wait for the serial port to connect
    }

   if (WiFi.status() == WL_NO_SHIELD) {
    Serial.println("WIFi101 shield not present");
    // Don't continue
    while(true);
   }
   
   WiFi.config(local_IP, gateway, subnet);

   while(status != WL_CONNECTED) {
    Serial.print("Attempting to connect to SSID: ");
    Serial.println("place ssid name here.");

    status = WiFi.begin(ssid, pass);

    delay(10000);
   }

   server.begin();

   Serial.println("Connected");
}

void loop() {
    carrier.Buttons.update();

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
