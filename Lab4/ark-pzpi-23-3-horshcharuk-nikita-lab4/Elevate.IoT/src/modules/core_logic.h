#pragma once

#include "constants.h"
#include "types.h"
#include "modules/config_manager.h"
#include "modules/badge_reader.h"
#include "modules/api_client.h"
#include "modules/led_display.h"
#include "modules/leaderboard_button.h"

// ============================================================================
// CoreLogic - Головна бізнес-логіка
// ============================================================================
class CoreLogic {
private:
    static unsigned long lastDashboardUpdate;
    static unsigned long lastWaitingMessage;
    
    static bool isNetworkError(const String& error) {
        return error.indexOf("server") >= 0 || 
               error.indexOf("unavailable") >= 0 ||
               error.indexOf("HTTP") >= 0 ||
               error.indexOf("connection") >= 0 ||
               error.indexOf("timeout") >= 0;
    }
    
    static void showLeaderboardOnDemand() {
        LeaderboardEntry entries[5];
        if (ApiClient::getLeaderboard(entries, 5)) {
            LedDisplay::showLeaderboard(entries, 5);
        } else {
            LedDisplay::showOfflineInfo();
        }
    }
    
public:
    static void handleScanMode() {
        if (LeaderboardButton::isPressed()) {
            showLeaderboardOnDemand();
            delay(2000);
            return;
        }
        
        if (BadgeReader::hasNewScan()) {
            int userId = BadgeReader::getLastUserId();
            ScanResult result = ApiClient::scanUser(userId);
            
            if (result.success) {
                LedDisplay::incrementSuccessfulScan();
                LedDisplay::showUserProfile(result);
            } else {
                LedDisplay::incrementFailedScan();
                String errorMsg = "User ID " + String(userId) + ": " + result.errorMessage;
                
                if (isNetworkError(result.errorMessage)) {
                    LedDisplay::showOfflineInfo();
                } else {
                    LedDisplay::showError(errorMsg);
                }
            }
            
            delay(Timing::SCAN_RESULT_DISPLAY_MS);
        } else {
            unsigned long now = millis();
            if (now - lastWaitingMessage >= Timing::WAITING_MESSAGE_INTERVAL_MS) {
                LedDisplay::showWaitingMessage();
                lastWaitingMessage = now;
            }
        }
    }
    
    static void handleDashboardMode() {
        unsigned long now = millis();
        
        if (now - lastDashboardUpdate >= ConfigManager::dashboardUpdateInterval) {
            lastDashboardUpdate = now;
            
            LeaderboardEntry entries[5];
            if (ApiClient::getLeaderboard(entries, 5)) {
                LedDisplay::showLeaderboard(entries, 5);
            } else {
                LedDisplay::showOfflineInfo();
            }
        }
    }
    
    static void run() {
        if (ConfigManager::currentMode == ConfigManager::SCAN_MODE) {
            handleScanMode();
        } else {
            handleDashboardMode();
        }
    }
};

