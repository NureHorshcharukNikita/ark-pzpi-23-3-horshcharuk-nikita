#pragma once

#include <HTTPClient.h>
#include <ArduinoJson.h>
#include "constants.h"
#include "types.h"
#include "modules/config_manager.h"
#include "modules/wifi_manager.h"

// ============================================================================
// ApiClient - HTTP клієнт
// ============================================================================
class ApiClient {
private:
    static HTTPClient http;
    
    static String buildScanUrl() {
        return String(ConfigManager::API_BASE_URL) + "/api/iot/scan";
    }
    
    static String buildLeaderboardUrl() {
        return String(ConfigManager::API_BASE_URL) + "/api/iot/leaderboard?deviceKey=" + 
               String(ConfigManager::DEVICE_KEY);
    }
    
    static bool handleRedirect(int& httpCode, const String& requestBody = "") {
        if (httpCode != 307 && httpCode != 301) return false;
        
        String location = http.header("Location");
        if (location.length() == 0) {
            location = http.header("location");
        }
        
        http.end();
        
        if (location.length() == 0) return false;
        
        http.begin(location);
        http.setTimeout(Timing::HTTP_TIMEOUT_MS);
        if (requestBody.length() > 0) {
            http.addHeader("Content-Type", "application/json");
            httpCode = http.POST(requestBody);
        } else {
            httpCode = http.GET();
        }
        
        return (httpCode != 307 && httpCode != 301);
    }
    
    static void parseErrorResponse(int httpCode, String& errorMessage) {
        String response = http.getString();
        
        if (response.length() == 0) {
            if (httpCode == HTTP_CODE_BAD_REQUEST) {
                errorMessage = "Bad request (400)";
            } else if (httpCode == HTTP_CODE_UNAUTHORIZED) {
                errorMessage = "Unauthorized (401)";
            } else {
                errorMessage = "Server error: " + String(httpCode);
            }
            return;
        }
        
        JsonDocument errorDoc;
        if (deserializeJson(errorDoc, response) == DeserializationError::Ok) {
            if (errorDoc["message"].is<String>()) {
                errorMessage = errorDoc["message"].as<String>();
            } else if (errorDoc["error"].is<String>()) {
                errorMessage = errorDoc["error"].as<String>();
            } else if (errorDoc["title"].is<String>()) {
                errorMessage = errorDoc["title"].as<String>();
            } else {
                errorMessage = "Server error: " + String(httpCode);
            }
        } else {
            errorMessage = response.substring(0, min(50, (int)response.length()));
        }
    }
    
    static void parseConnectionError(int httpCode, String& errorMessage) {
        if (httpCode == HTTPC_ERROR_CONNECTION_REFUSED) {
            errorMessage = "Connection refused";
        } else if (httpCode == HTTPC_ERROR_CONNECTION_LOST) {
            errorMessage = "Connection lost";
        } else if (httpCode == HTTPC_ERROR_READ_TIMEOUT) {
            errorMessage = "Connection timeout";
        } else {
            errorMessage = "Server unavailable (error: " + String(httpCode) + ")";
        }
    }
    
public:
    static ScanResult scanUser(int userId) {
        ScanResult result;
        
        if (!WiFiManager::ensureConnection()) {
            result.errorMessage = "No network";
            return result;
        }
        
        String url = buildScanUrl();
        JsonDocument doc;
        doc["deviceKey"] = ConfigManager::DEVICE_KEY;
        doc["userId"] = userId;
        
        String requestBody;
        serializeJson(doc, requestBody);
        
        http.begin(url);
        http.setTimeout(Timing::HTTP_TIMEOUT_MS);
        http.addHeader("Content-Type", "application/json");
        
        int httpCode = http.POST(requestBody);
        
        if (httpCode == 307 || httpCode == 301) {
            if (!handleRedirect(httpCode, requestBody)) {
                result.errorMessage = "Redirect error";
                http.end();
                return result;
            }
        }
        
        if (httpCode == HTTP_CODE_OK) {
            String response = http.getString();
            JsonDocument responseDoc;
            
            if (deserializeJson(responseDoc, response) == DeserializationError::Ok) {
                result.success = true;
                result.userId = responseDoc["userId"] | 0;
                result.teamId = responseDoc["teamId"] | 0;
                result.fullName = responseDoc["fullName"].as<String>();
                result.teamPoints = responseDoc["teamPoints"] | 0;
                result.teamLevelName = responseDoc["teamLevelName"].as<String>();
                
                JsonArray badges = responseDoc["recentBadges"].as<JsonArray>();
                result.badgeCount = min((int)badges.size(), Display::MAX_RECENT_BADGES);
                for (int i = 0; i < result.badgeCount; i++) {
                    result.recentBadges[i] = badges[i].as<String>();
                }
            } else {
                result.errorMessage = "Response parsing error";
            }
        } else if (httpCode >= 400) {
            parseErrorResponse(httpCode, result.errorMessage);
        } else if (httpCode < 0) {
            parseConnectionError(httpCode, result.errorMessage);
        } else {
            result.errorMessage = "Server error: " + String(httpCode);
        }
        
        http.end();
        return result;
    }
    
    static bool getLeaderboard(LeaderboardEntry* entries, int maxEntries) {
        if (!WiFiManager::ensureConnection()) {
            return false;
        }
        
        String url = buildLeaderboardUrl();
        http.begin(url);
        http.setTimeout(Timing::HTTP_TIMEOUT_MS);
        
        int httpCode = http.GET();
        
        if (httpCode == 307 || httpCode == 301) {
            if (!handleRedirect(httpCode)) {
                http.end();
                return false;
            }
        }
        
        if (httpCode == HTTP_CODE_OK) {
            String response = http.getString();
            JsonDocument doc;
            
            if (deserializeJson(doc, response) == DeserializationError::Ok) {
                JsonArray leaderboard = doc.as<JsonArray>();
                int count = min((int)leaderboard.size(), maxEntries);
                
                for (int i = 0; i < count; i++) {
                    JsonObject entry = leaderboard[i];
                    entries[i].rank = entry["rank"] | (i + 1);
                    entries[i].userId = entry["userId"] | 0;
                    entries[i].fullName = entry["fullName"].as<String>();
                    entries[i].teamPoints = entry["teamPoints"] | 0;
                    entries[i].teamLevel = entry["teamLevel"].as<String>();
                }
                
                http.end();
                return true;
            }
        }
        
        http.end();
        return false;
    }
};

