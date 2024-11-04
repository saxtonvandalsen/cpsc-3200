// Saxton Van Dalsen
// 11/14/2024

#ifndef MSGSTREAM_H
#define MSGSTREAM_H

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

        char** messages;
        int capacity;
        int maxOperations;
        int operationCount;
        int messageCount;

        int calculateMaxOperations(int capacity);
        bool isFull() const;
        bool operationLimit() const;
        bool isValidMessage(const char* message) const;
        int calculateCapacity(int capacity);
        bool isInvalidRange(int startRange, int endRange) const;

        
    public:
        // Precondition:
        // - Capacity must be between 1 and MAX_CAPACITY.
        // Postcondition:
        // - Capacity is initialized and the "messages" array is created.
        MsgStream(int capacity);
        MsgStream(const MsgStream& other);
        MsgStream& operator=(const MsgStream& other);
        MsgStream(MsgStream&& src) noexcept;
        MsgStream& operator=(MsgStream&& src) noexcept;

        // Precondition:
        // - Operation count must not exceed MAX_OPERATIONS.
        // - The start and end ranges must be acceptable within the bounds of the message array.
        // Postcondition:
        // - Returns the messages between the specified start and end range.
        char** readMessages(int startRange, int endRange);

        // Precondition:
        // - Message stream must not be full.
        // - Operation count must not exceed MAX_OPERATIONS.
        // - The message must be non-null, non-empty, and within the MAX_STRING_LENGTH.
        // Postcondition:
        // - Message is appended to the stream; the message and operation counts are updated.
        void appendMessage(const char* message);
        void reset();
        int getMessageCount() const;
        int getMaxOperations() const;
        int getCapacity() const;
};

// Implementation invariant:
// - The message stream must always maintain its message count and operation count within the defined limits.
// - The "messages" array should always contain valid messages that meet the set constraints.

#endif