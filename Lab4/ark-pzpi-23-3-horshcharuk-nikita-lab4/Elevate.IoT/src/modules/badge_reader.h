#pragma once

#include <Arduino.h>
#include "constants.h"

// ============================================================================
// BadgeReader - Модуль зчитування бейджів
// ============================================================================
class BadgeReader {
private:
    static bool isInitialized;
    static unsigned long lastButtonPressTime;
    static int lastReadUserId;
    
    static const int BUTTON_COUNT = 3;
    
    static int getButtonPin(int index) {
        static const int pins[] = {
            Hardware::BUTTON_USER1,
            Hardware::BUTTON_USER2,
            Hardware::BUTTON_USER3
        };
        return pins[index];
    }
    
    static int getUserId(int index) {
        static const int ids[] = {
            Users::USER1_ID,
            Users::USER2_ID,
            Users::USER3_ID
        };
        return ids[index];
    }
    
    static const char* getUserName(int userId) {
        switch (userId) {
            case Users::USER1_ID:
                return Users::USER1_NAME;
            case Users::USER2_ID:
                return Users::USER2_NAME;
            case Users::USER3_ID:
                return Users::USER3_NAME;
            default:
                return "Unknown";
        }
    }
    
public:
    static void initialize() {
        if (isInitialized) return;
        
        for (int i = 0; i < BUTTON_COUNT; i++) {
            pinMode(getButtonPin(i), INPUT_PULLUP);
        }
        
        lastButtonPressTime = 0;
        lastReadUserId = 0;
        isInitialized = true;
    }
    
    static int readUserId() {
        if (!isInitialized) initialize();
        
        unsigned long now = millis();
        if (now - lastButtonPressTime < Timing::BUTTON_DEBOUNCE_MS) {
            return 0;
        }
        
        for (int i = 0; i < BUTTON_COUNT; i++) {
            int pin = getButtonPin(i);
            if (digitalRead(pin) == LOW) {
                delay(Timing::BUTTON_PRESS_CHECK_DELAY_MS);
                if (digitalRead(pin) == LOW) {
                    lastButtonPressTime = now;
                    int userId = getUserId(i);
                    lastReadUserId = userId;
                    
                    while (digitalRead(pin) == LOW) {
                        delay(10);
                    }
                    
                    return userId;
                }
            }
        }
        
        return 0;
    }
    
    static bool hasNewScan() {
        if (!isInitialized) initialize();
        return readUserId() > 0;
    }
    
    static int getLastUserId() {
        return lastReadUserId;
    }
};

