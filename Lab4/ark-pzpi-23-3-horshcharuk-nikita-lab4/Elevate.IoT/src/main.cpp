/*
 * Elevate IoT Client
 * 
 * Модульний IoT-клієнт для системи гейміфікації Elevate
 * Платформа: ESP32
 * 
 * Архітектура:
 * - ConfigManager: управління налаштуваннями
 * - WiFiManager: підключення до Wi-Fi
 * - ApiClient: HTTP комунікація з сервером
 * - BadgeReader: зчитування бейджів (кнопки)
 * - LedDisplay: відображення інформації (TFT ILI9341)
 * - CoreLogic: головна бізнес-логіка
 */

#include <SPI.h>
#include <WiFi.h>
#include "constants.h"
#include "display.h"
#include "modules/config_manager.h"
#include "modules/wifi_manager.h"
#include "modules/api_client.h"
#include "modules/badge_reader.h"
#include "modules/leaderboard_button.h"
#include "modules/led_display.h"
#include "modules/core_logic.h"

// ============================================================================
// Глобальні змінні
// ============================================================================
Adafruit_ILI9341 display(Hardware::TFT_CS, Hardware::TFT_DC, Hardware::TFT_RST);

// Статичні змінні модулів
const char* ConfigManager::WIFI_SSID = Config::WIFI_SSID;
const char* ConfigManager::WIFI_PASSWORD = Config::WIFI_PASSWORD;
const char* ConfigManager::API_BASE_URL = Config::API_BASE_URL;
const char* ConfigManager::DEVICE_KEY = Config::DEVICE_KEY;
ConfigManager::Mode ConfigManager::currentMode = ConfigManager::SCAN_MODE;
unsigned long ConfigManager::dashboardUpdateInterval = Timing::DASHBOARD_UPDATE_INTERVAL_MS;

unsigned long WiFiManager::lastConnectionAttempt = 0;
bool WiFiManager::connectionStatus = false;

bool BadgeReader::isInitialized = false;
unsigned long BadgeReader::lastButtonPressTime = 0;
int BadgeReader::lastReadUserId = 0;

bool LeaderboardButton::isInitialized = false;
unsigned long LeaderboardButton::lastButtonPressTime = 0;
bool LeaderboardButton::wasPressed = false;

bool LedDisplay::isDisplayInitialized = false;
unsigned long LedDisplay::startTime = 0;
int LedDisplay::successfulScans = 0;
int LedDisplay::failedScans = 0;

unsigned long CoreLogic::lastDashboardUpdate = 0;
unsigned long CoreLogic::lastWaitingMessage = 0;

HTTPClient ApiClient::http;

// ============================================================================
// Arduino setup() та loop()
// ============================================================================
void setup() {
    Serial.begin(115200);
    delay(100);
    
    LedDisplay::initializeStats();
    LedDisplay::showLoadingStep("Initializing...", 10);
    delay(200);
    
    LedDisplay::showLoadingStep("Configuring...", 20);
    ConfigManager::initialize();
    delay(200);
    
    LedDisplay::showLoadingStep("Display...", 40);
    SPI.begin();
    display.begin();
    display.setRotation(1);
    LedDisplay::setDisplayInitialized(true);
    delay(200);
    
    LedDisplay::showLoadingStep("Buttons...", 50);
    BadgeReader::initialize();
    LeaderboardButton::initialize();
    delay(200);
    
    LedDisplay::showLoadingStep("Wi-Fi...", 80);
    WiFi.begin(ConfigManager::WIFI_SSID, ConfigManager::WIFI_PASSWORD);
    
    int attempts = 0;
    while (WiFi.status() != WL_CONNECTED && attempts < Timing::WIFI_MAX_ATTEMPTS) {
        delay(Timing::WIFI_CONNECT_DELAY_MS);
        attempts++;
        
        int progress = 80 + (attempts * 15 / Timing::WIFI_MAX_ATTEMPTS);
        if (progress > 95) progress = 95;
        LedDisplay::showLoadingStep("Wi-Fi...", progress);
    }
    
    if (WiFi.status() == WL_CONNECTED) {
        WiFiManager::setConnectionStatus(true);
        LedDisplay::showLoadingStep("Ready!", 100);
        delay(500);
    } else {
        WiFiManager::setConnectionStatus(false);
        LedDisplay::showLoadingStep("Wi-Fi error", 100);
        delay(1000);
    }
    
    LedDisplay::showSystemStatus();
    delay(5000);
    
    if (ConfigManager::currentMode == ConfigManager::SCAN_MODE) {
        LedDisplay::showWaitingMessage();
    } else {
        LedDisplay::showOfflineInfo();
    }
}

void loop() {
    if (!WiFiManager::isConnected()) {
        static unsigned long lastCheck = 0;
        unsigned long now = millis();
        if (now - lastCheck > Timing::WIFI_CHECK_INTERVAL_MS) {
            WiFiManager::connect();
            lastCheck = now;
        }
    }
    
    CoreLogic::run();
    delay(Timing::LOOP_DELAY_MS);
}
