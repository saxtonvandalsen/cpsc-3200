#include <iostream>
#include "MsgStream.h"

// Testing of MsgStream class
int main() {
    // Create a MsgStream with a capacity of 5
    MsgStream msgStream(5);
    
    // Append messages
    try {
        msgStream.appendMessage("Hello, World!");
        msgStream.appendMessage("Testing message stream.");
        msgStream.appendMessage("Another message.");
        msgStream.appendMessage("Yet another message.");
        msgStream.appendMessage("Last message."); // Should work
        msgStream.appendMessage("This message should fail."); // Should throw an overflow error
    } catch (const std::exception& e) {
        std::cerr << "Error while appending message: " << e.what() << std::endl;
    }

    // Read messages
    try {
        char** messages = msgStream.readMessages(0, msgStream.getMessageCount());
        for (int i = 0; i < msgStream.getMessageCount(); ++i) {
            std::cout << "Message " << i + 1 << ": " << messages[i] << std::endl;
        }
        delete[] messages; // Free the result array
    } catch (const std::exception& e) {
        std::cerr << "Error while reading messages: " << e.what() << std::endl;
    }

    // Reset the MsgStream
    msgStream.reset();
    std::cout << "Messages after reset: " << msgStream.getMessageCount() << std::endl;

    // Append messages after reset to test functionality
    try {
        msgStream.appendMessage("New message after reset.");
        std::cout << "Messages after appending post-reset: " << msgStream.getMessageCount() << std::endl;
    } catch (const std::exception& e) {
        std::cerr << "Error while appending message: " << e.what() << std::endl;
    }

    return 0;
}
