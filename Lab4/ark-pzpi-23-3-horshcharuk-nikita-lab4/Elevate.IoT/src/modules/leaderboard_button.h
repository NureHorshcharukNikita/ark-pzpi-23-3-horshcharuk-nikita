#pragma once

#include <Arduino.h>
#include "constants.h"

// ============================================================================
// LeaderboardButton - Обробка кнопки лідерборду
// ============================================================================
class LeaderboardButton {
private:
    static bool isInitialized;
    static unsigned long lastButtonPressTime;
    static bool wasPressed;
    
public:
    static void initialize() {
        if (isInitialized) return;
        
        pinMode(Hardware::BUTTON_LEADERBOARD, INPUT_PULLUP);
        lastButtonPressTime = 0;
        wasPressed = false;
        isInitialized = true;
    }
    
    static bool isPressed() {
        if (!isInitialized) initialize();
        
        unsigned long now = millis();
        if (now - lastButtonPressTime < Timing::BUTTON_DEBOUNCE_MS) {
            return false;
        }
        
        if (digitalRead(Hardware::BUTTON_LEADERBOARD) == LOW) {
            if (!wasPressed) {
                delay(Timing::BUTTON_PRESS_CHECK_DELAY_MS);
                if (digitalRead(Hardware::BUTTON_LEADERBOARD) == LOW) {
                    lastButtonPressTime = now;
                    wasPressed = true;
                    
                    while (digitalRead(Hardware::BUTTON_LEADERBOARD) == LOW) {
                        delay(10);
                    }
                    
                    return true;
                }
            }
        } else {
            wasPressed = false;
        }
        
        return false;
    }
};

