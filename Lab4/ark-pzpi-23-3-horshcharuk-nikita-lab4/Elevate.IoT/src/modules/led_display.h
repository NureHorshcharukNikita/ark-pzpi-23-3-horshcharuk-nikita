#pragma once

#include "constants.h"
#include "types.h"
#include "display.h"
#include "modules/config_manager.h"
#include "modules/wifi_manager.h"

// ============================================================================
// LedDisplay - Модуль відображення
// ============================================================================
class LedDisplay {
private:
    static bool isDisplayInitialized;
    static unsigned long startTime;
    static int successfulScans;
    static int failedScans;
    
    static void initDisplay() {
        if (isDisplayInitialized) return;
        display.begin();
        display.setRotation(1);
        display.fillScreen(ILI9341_BLACK);
        display.setTextColor(ILI9341_WHITE);
        isDisplayInitialized = true;
    }
    
    static String truncateString(const String& str, int maxLen) {
        return (str.length() > maxLen) ? str.substring(0, maxLen) + "..." : str;
    }
    
public:
    static void setDisplayInitialized(bool state) {
        isDisplayInitialized = state;
    }
    
    static void showUserProfile(const ScanResult& result) {
        initDisplay();
        
        if (isDisplayInitialized) {
            display.fillScreen(ILI9341_BLACK);
            display.setTextColor(ILI9341_WHITE);
            display.setTextSize(2);
            display.setCursor(10, 10);
            display.println("PROFILE");
            
            display.setTextSize(1);
            display.setCursor(10, 40);
            display.println(truncateString(result.fullName, Display::MAX_NAME_LENGTH));
            
            display.setCursor(10, 60);
            display.print("Points: ");
            display.println(result.teamPoints);
            
            display.setCursor(10, 80);
            display.print("Level: ");
            display.println(truncateString(result.teamLevelName, Display::MAX_LEVEL_LENGTH));
            
            if (result.badgeCount > 0) {
                display.setCursor(10, 100);
                display.print("Badge: ");
                display.println(truncateString(result.recentBadges[0], Display::MAX_BADGE_LENGTH));
            }
        }
    }
    
    static void showError(const String& message) {
        initDisplay();
        
        if (isDisplayInitialized) {
            display.fillScreen(ILI9341_BLACK);
            display.setTextColor(ILI9341_RED);
            display.setCursor(10, 10);
            display.setTextSize(2);
            display.println("ERROR");
            
            display.setTextColor(ILI9341_WHITE);
            display.setTextSize(1);
            
            String errorMsg = message;
            int yPos = 40;
            
            while (errorMsg.length() > 0 && yPos < 220) {
                String line = errorMsg;
                if (line.length() > Display::MAX_ERROR_LINE_LENGTH) {
                    line = line.substring(0, Display::MAX_ERROR_LINE_LENGTH);
                    errorMsg = errorMsg.substring(Display::MAX_ERROR_LINE_LENGTH);
                } else {
                    errorMsg = "";
                }
                display.setCursor(10, yPos);
                display.println(line);
                yPos += 20;
            }
        }
    }
    
    static void showLeaderboard(const LeaderboardEntry* entries, int count) {
        initDisplay();
        
        if (isDisplayInitialized) {
            display.fillScreen(ILI9341_BLACK);
            display.setTextColor(ILI9341_WHITE);
            display.setCursor(10, 10);
            display.setTextSize(2);
            display.println("LEADERBOARD");
            
            display.setTextSize(1);
            int yPos = 40;
            int validEntries = 0;
            
            for (int i = 0; i < count && validEntries < Display::MAX_LEADERBOARD_ENTRIES; i++) {
                if (entries[i].userId > 0 && entries[i].teamPoints > 0) {
                    display.setCursor(10, yPos);
                    display.print(entries[i].rank);
                    display.print(". ");
                    display.print(truncateString(entries[i].fullName, Display::MAX_LEADERBOARD_NAME_LENGTH));
                    display.print(" ");
                    display.print(entries[i].teamPoints);
                    display.println("pt");
                    yPos += 20;
                    validEntries++;
                }
            }
            
            while (validEntries < Display::MAX_LEADERBOARD_ENTRIES) {
                display.setCursor(10, yPos);
                display.print(validEntries + 1);
                display.print(". ");
                display.println("---");
                yPos += 20;
                validEntries++;
            }
        }
    }
    
    static void showWaitingMessage() {
        initDisplay();
        
        if (isDisplayInitialized) {
            display.fillScreen(ILI9341_BLACK);
            display.setTextColor(ILI9341_WHITE);
            display.setCursor(10, 80);
            display.setTextSize(2);
            display.println("Waiting");
            display.setCursor(10, 110);
            display.setTextSize(1);
            display.println("for scan...");
        }
    }
    
    static void showLoadingStep(const String& step, int progress = -1) {
        initDisplay();
        
        if (isDisplayInitialized) {
            display.fillScreen(ILI9341_BLACK);
            display.setTextColor(ILI9341_WHITE);
            display.setCursor(10, 50);
            display.setTextSize(2);
            display.println("Elevate");
            display.setCursor(10, 80);
            display.setTextSize(1);
            display.println(step);
            
            if (progress >= 0 && progress <= 100) {
                constexpr int barWidth = 200;
                constexpr int barHeight = 10;
                constexpr int barX = 20;
                constexpr int barY = 120;
                
                display.drawRect(barX, barY, barWidth, barHeight, ILI9341_WHITE);
                int fillWidth = (barWidth * progress) / 100;
                display.fillRect(barX, barY, fillWidth, barHeight, ILI9341_GREEN);
                
                display.setCursor(barX + barWidth + 10, barY - 2);
                display.print(progress);
                display.print("%");
            }
        }
    }
    
    static void showOfflineInfo() {
        initDisplay();
        
        if (isDisplayInitialized) {
            display.fillScreen(ILI9341_BLACK);
            display.setTextColor(ILI9341_WHITE);
            
            display.setCursor(10, 10);
            display.setTextSize(1);
            display.println("SERVER UNAVAILABLE");
            
            display.setCursor(10, 30);
            display.print("Wi-Fi: ");
            if (WiFiManager::isConnected()) {
                display.println("OK");
                display.setCursor(10, 50);
                display.print("IP: ");
                IPAddress ip = WiFi.localIP();
                display.println(String(ip[0]) + "." + String(ip[1]) + "." + 
                               String(ip[2]) + "." + String(ip[3]));
            } else {
                display.println("NOT CONN.");
            }
            
            unsigned long uptime = (millis() - startTime) / 1000;
            unsigned long hours = uptime / 3600;
            unsigned long minutes = (uptime % 3600) / 60;
            unsigned long seconds = uptime % 60;
            
            display.setCursor(10, 70);
            display.print("Time: ");
            if (hours > 0) {
                display.print(hours);
                display.print("h ");
            }
            if (minutes > 0 || hours > 0) {
                display.print(minutes);
                display.print("m");
            } else {
                display.print(seconds);
                display.print("s");
            }
            
            display.setCursor(10, 90);
            display.print("Scans: ");
            display.print(successfulScans);
            display.print("/");
            display.print(successfulScans + failedScans);
            
            display.setCursor(10, 110);
            display.print("Mode: ");
            display.println((ConfigManager::currentMode == ConfigManager::SCAN_MODE) ? "SCAN" : "DASHBOARD");
        }
    }
    
    static void incrementSuccessfulScan() { successfulScans++; }
    static void incrementFailedScan() { failedScans++; }
    
    static void initializeStats() {
        startTime = millis();
        successfulScans = 0;
        failedScans = 0;
    }
    
    static bool testApiConnection() {
        if (!WiFiManager::isConnected()) {
            return false;
        }
        
        // Швидка перевірка доступності API
        HTTPClient http;
        String testUrl = String(ConfigManager::API_BASE_URL) + "/api/iot/leaderboard?deviceKey=" + 
                        String(ConfigManager::DEVICE_KEY);
        http.begin(testUrl);
        http.setTimeout(3000); // Короткий таймаут для швидкої перевірки
        int httpCode = http.GET();
        http.end();
        
        return (httpCode == HTTP_CODE_OK || httpCode == HTTP_CODE_BAD_REQUEST || httpCode == HTTP_CODE_UNAUTHORIZED);
    }
    
    static void showSystemStatus() {
        initDisplay();
        
        if (isDisplayInitialized) {
            display.fillScreen(ILI9341_BLACK);
            display.setTextColor(ILI9341_WHITE);
            
            // Заголовок
            display.setCursor(10, 5);
            display.setTextSize(1);
            display.println("=== SYSTEM STATUS ===");
            
            int yPos = 25;
            const int lineHeight = 18;
            const int maxLineWidth = 38; // Збільшена ширина для дисплею 320px
            
            // Wi-Fi SSID
            display.setTextColor(ILI9341_WHITE);
            display.setCursor(5, yPos);
            display.print("WiFi: ");
            String ssid = String(ConfigManager::WIFI_SSID);
            if (ssid.length() > maxLineWidth - 7) {
                ssid = ssid.substring(0, maxLineWidth - 10) + "...";
            }
            display.println(ssid);
            yPos += lineHeight;
            
            // Wi-Fi Status
            display.setCursor(5, yPos);
            display.print("WiFi Status: ");
            bool wifiConnected = WiFiManager::isConnected();
            if (wifiConnected) {
                display.setTextColor(ILI9341_GREEN);
                display.print("OK");
                display.setTextColor(ILI9341_WHITE);
                IPAddress ip = WiFi.localIP();
                display.print(" (");
                display.print(ip[0]);
                display.print(".");
                display.print(ip[1]);
                display.print(".");
                display.print(ip[2]);
                display.print(".");
                display.print(ip[3]);
                display.println(")");
            } else {
                display.setTextColor(ILI9341_RED);
                display.println("DISCONNECTED");
                display.setTextColor(ILI9341_WHITE);
            }
            yPos += lineHeight;
            
            // API URL (без переносу)
            display.setCursor(5, yPos);
            display.print("API: ");
            String apiUrl = String(ConfigManager::API_BASE_URL);
            if (apiUrl.length() > maxLineWidth - 6) {
                apiUrl = apiUrl.substring(0, maxLineWidth - 9) + "...";
            }
            display.println(apiUrl);
            yPos += lineHeight;
            
            // API Status
            display.setCursor(5, yPos);
            display.print("API Status: ");
            bool apiOk = testApiConnection();
            if (apiOk) {
                display.setTextColor(ILI9341_GREEN);
                display.println("OK");
            } else {
                display.setTextColor(ILI9341_RED);
                display.println("FAIL");
            }
            display.setTextColor(ILI9341_WHITE);
            yPos += lineHeight;
            
            // Device Key
            display.setCursor(5, yPos);
            display.print("Device: ");
            String deviceKey = String(ConfigManager::DEVICE_KEY);
            if (deviceKey.length() > maxLineWidth - 9) {
                deviceKey = deviceKey.substring(0, maxLineWidth - 12) + "...";
            }
            display.println(deviceKey);
            yPos += lineHeight;
            
            // Mode
            display.setCursor(5, yPos);
            display.print("Mode: ");
            display.print((ConfigManager::currentMode == ConfigManager::SCAN_MODE) ? "SCAN" : "DASH");
            display.print(" | Int: ");
            display.print(ConfigManager::dashboardUpdateInterval / 1000);
            display.println("s");
        }
    }
};

