#pragma once

#include <Arduino.h>
#include "constants.h"

// ============================================================================
// Структури даних
// ============================================================================
struct ScanResult {
    int userId = 0;
    int teamId = 0;
    String fullName;
    int teamPoints = 0;
    String teamLevelName;
    String recentBadges[Display::MAX_RECENT_BADGES];
    int badgeCount = 0;
    bool success = false;
    String errorMessage;
};

struct LeaderboardEntry {
    int userId = 0;
    String fullName;
    int teamPoints = 0;
    String teamLevel;
    int rank = 0;
};

