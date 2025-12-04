#pragma once

// ============================================================================
// Константи та налаштування
// ============================================================================
namespace Hardware {
    constexpr int TFT_DC = 2;
    constexpr int TFT_CS = 15;
    constexpr int TFT_RST = 4;
    
    constexpr int BUTTON_USER1 = 32;
    constexpr int BUTTON_USER2 = 33;
    constexpr int BUTTON_USER3 = 25;
    constexpr int BUTTON_LEADERBOARD = 26;
}

namespace Timing {
    constexpr unsigned long BUTTON_DEBOUNCE_MS = 200;
    constexpr unsigned long BUTTON_PRESS_CHECK_DELAY_MS = 50;
    constexpr unsigned long WIFI_RETRY_INTERVAL_MS = 5000;
    constexpr unsigned long WIFI_CONNECT_DELAY_MS = 300;
    constexpr unsigned long WIFI_MAX_ATTEMPTS = 15;
    constexpr unsigned long HTTP_TIMEOUT_MS = 10000;
    constexpr unsigned long SCAN_RESULT_DISPLAY_MS = 7000;
    constexpr unsigned long WAITING_MESSAGE_INTERVAL_MS = 5000;
    constexpr unsigned long WIFI_CHECK_INTERVAL_MS = 10000;
    constexpr unsigned long LOOP_DELAY_MS = 100;
    constexpr unsigned long DASHBOARD_UPDATE_INTERVAL_MS = 10000;
}

namespace Display {
    constexpr int MAX_NAME_LENGTH = 20;
    constexpr int MAX_LEVEL_LENGTH = 15;
    constexpr int MAX_BADGE_LENGTH = 15;
    constexpr int MAX_ERROR_LINE_LENGTH = 25;
    constexpr int MAX_LEADERBOARD_NAME_LENGTH = 12;
    constexpr int MAX_LEADERBOARD_ENTRIES = 5;
    constexpr int MAX_RECENT_BADGES = 5;
}

namespace Config {
    constexpr const char* WIFI_SSID = "Wokwi-GUEST";
    constexpr const char* WIFI_PASSWORD = "";
    constexpr const char* API_BASE_URL = "http://192.168.0.77:5181";
    constexpr const char* DEVICE_KEY = "device-backend-001";
}

namespace Users {
    constexpr int USER1_ID = 1;
    constexpr int USER2_ID = 2;
    constexpr int USER3_ID = 3;
    
    constexpr const char* USER1_NAME = "User 1";
    constexpr const char* USER2_NAME = "User 2";
    constexpr const char* USER3_NAME = "User 3";
}

