#include <iostream>
#include "MsgStream.h"

// Testing of MsgStream class
int main() {
    try {
        // Create a message stream
        MsgStream stream(5);
        stream.appendMessage("Hello, World!");
        stream.appendMessage("Testing message stream.");
        stream.appendMessage("Another message.");
        stream.appendMessage("Yet another message.");
        stream.appendMessage("Last message.");

        // Test copy constructor
        MsgStream copyStream = stream;
        std::cout << "Messages from copyStream after using copy constructor:" << std::endl;
        for (int i = 0; i < copyStream.getMessageCount(); ++i) {
            std::cout << "Message " << i + 1 << ": " << copyStream.readMessages(i, i + 1)[0] << std::endl;
        }

        // Test assignment operator
        MsgStream assignedStream(3);
        assignedStream = stream;
        std::cout << "Messages from assignedStream after using assignment operator:" << std::endl;
        for (int i = 0; i < assignedStream.getMessageCount(); ++i) {
            std::cout << "Message " << i + 1 << ": " << assignedStream.readMessages(i, i + 1)[0] << std::endl;
        }

        // Reset and append new messages to check behavior
        stream.reset();
        stream.appendMessage("New message after reset.");
        std::cout << "Messages after reset and append to original stream:" << std::endl;
        std::cout << "Message 1: " << stream.readMessages(0, 1)[0] << std::endl;

    } catch (const std::exception& e) {
        std::cerr << "Error: " << e.what() << std::endl;
    }

    return 0;
}
