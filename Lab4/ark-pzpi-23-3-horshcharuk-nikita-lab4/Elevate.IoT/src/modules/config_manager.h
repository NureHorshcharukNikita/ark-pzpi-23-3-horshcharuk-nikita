#pragma once

#include "constants.h"

// ============================================================================
// ConfigManager - Управління налаштуваннями
// ============================================================================
class ConfigManager {
public:
    enum Mode { SCAN_MODE, DASHBOARD_MODE };
    
    static const char* WIFI_SSID;
    static const char* WIFI_PASSWORD;
    static const char* API_BASE_URL;
    static const char* DEVICE_KEY;
    static Mode currentMode;
    static unsigned long dashboardUpdateInterval;
    
    static void initialize() {
        currentMode = SCAN_MODE;
        dashboardUpdateInterval = Timing::DASHBOARD_UPDATE_INTERVAL_MS;
    }
};

