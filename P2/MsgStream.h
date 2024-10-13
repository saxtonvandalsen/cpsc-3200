// Saxton Van Dalsen
// 10/15/2024

#ifndef MSGSTREAM_H
#define MSGSTREAM_H
#include <string>

using namespace std;

class MsgStream
{
    // Class invariant:
    // - The "messages" array may only contain valid, non-null, and non-empty strings, each adhering to the maximum length defined by MAX_STRING_LENGTH.
    // - The total number of operations performed on the MsgStream instance must not exceed the limit established by MAX_OPERATIONS, which is set as a multiple of the object's capacity.
    // - The number of messages appended to the array cannot exceed the fixed capacity of the message stream, which must be between 1 and MAX_CAPACITY, as determined at initialization.
    // - Once established, the capacity remains unchanged throughout the lifetime of the object unless reset by the client.
    // - Clients must be prepared to handle exceptions, particularly those related to invalid message length and capacity limits, to ensure robust error handling.

    private:
        static const int MAX_CAPACITY = 200;
        const int MAX_STRING_LENGTH = 150;
        const int MAX_OPERATIONS_MULTIPLIER = 2;

        char** messages;
        int capacity;
        int operationCount;
        int messageCount;

        int getMaxOperations() const;
        bool isFull() const;
        bool operationLimit() const;
        bool isValidMessage(const char* message) const;
        int validateCapacity(int capacity);

        
    public:
        MsgStream(int capacity);
        ~MsgStream();

        string* readMessages(int startRange, int endRange);
        void appendMessage(const string& message);
        void reset();
        int getMessageCount() const;
        int getCapacity() const;
};

#endif