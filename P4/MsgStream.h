// Saxton Van Dalsen
// 11/14/2024

#ifndef MSGSTREAM_H
#define MSGSTREAM_H

#include <memory>
#include <string>
#include <stdexcept>

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
        int capacity;
        int maxOperations;
        int operationCount;

        int calculateMaxOperations(int capacity);
        int calculateCapacity(int capacity);
        bool isInvalidRange(int startRange, int endRange) const;

    protected:
        static const int MAX_CAPACITY = 200;
        const int MAX_STRING_LENGTH = 150;

        unique_ptr<string[]> messages;
        int messageCount;

        bool virtual isFull() const;
        bool virtual operationLimit() const;
        bool virtual isValidMessage(const string& message) const;
        
    public:
        // Preconditions:
        // - Capacity must be between 1 and MAX_CAPACITY.
        // Postconditions:
        // - Capacity is initialized and the "messages" array is created.
        MsgStream(int capacity);

        // Postcondition:
        // - MsgStream object created with all variables set to 0 and nullptr.
        MsgStream();
        
        // Preconditions:
        // - passed in object must be valid and initialized.
        // - its messages should contain no null or invalid elements within capacity.
        // Postconditions:
        // - Deep copying of the passed in object is created with all resources copied.
        // - New object is independent of other object.
        MsgStream(const MsgStream& other);

        // Preconditions:
        // - passed in object must be valid and initialized.
        // - its messages should contain no null or invalid elements within capacity.
        // Postconditions:
        // - Current MsgStream object is updated to be a deep copy of passed in object.
        // - Existing resources in current object are replaced.
        // - Current MsgStream is independent of the passed in object with no shared resources.
        MsgStream& operator=(const MsgStream& other);

        // Preconditions:
        // - passed in object must be valid and initialized.
        // Postconditions:
        // - current MsgStream object takes ownership of resources from passed in object.
        // - passed in object is left in a valid empty state, with resources safely transferred.
        MsgStream(MsgStream&& other) noexcept;

        // Preconditions:
        // - passed in object must be valid and initialized.
        // - passed in objects must not be the same as the current MsgStream object.
        // Postconditions:
        // - Current MsgStream object takes ownership of all resources.
        // - passed in object is left in a valid empty state.
        MsgStream& operator=(MsgStream&& other) noexcept;

        // Preconditions:
        // - Operation count must not exceed MAX_OPERATIONS.
        // - The start and end ranges must be acceptable within the bounds of the message array.
        // Postconditions:
        // - Returns the messages between the specified start and end range.
        unique_ptr<string[]> virtual readMessages(int startRange, int endRange);

        // Preconditions:
        // - Message stream must not be full.
        // - Operation count must not exceed MAX_OPERATIONS.
        // - The message must be non-null, non-empty, and within the MAX_STRING_LENGTH.
        // Postconditions:
        // - Message is appended to the stream; the message and operation counts are updated.
        void virtual appendMessage(const string& message);

        // Preconditions:
        // - MsgStream object must be valid and initialized.
        // Postconditions:
        // - All messages in MsgStream are cleared and replaced with an empty array.
        // - message count and operation count are reset to 0.
        void virtual reset();
        
        // Preconditions:
        // - MsgStream object is valid and initialized.
        // Postconditions:
        // - will return true if the MsgStream is empty (messageCount == 0), otherwise false.
        bool operator!() const ;

        // Preconditions:
        // - Both MsgStream objects are valid and initialized.
        // - The total combined capacity of the two MsgStreams does not exceed the maximum allowable size.
        // Postconditions:
        // - Returns a new MsgStream object containing all messages from both MsgStreams.
        // - The original MsgStream objects remain unchanged.
        MsgStream operator+(const MsgStream& other) const;

        // Preconditions:
        // - Both MsgStream objects are valid and initialized.
        // - The capacity and message counts of both MsgStreams are accurately set.
        // Postconditions:
        // - Returns true if the two MsgStreams are identical in capacity, message count, and content; otherwise false.
        bool operator==(const MsgStream& other) const;

        // Preconditions:
        // - Both MsgStream objects are valid and initialized.
        // Postconditions:
        // - Returns true if the two MsgStreams are not equal; otherwise false.
        bool operator!=(const MsgStream& other) const;

        // Preconditions:
        // - Both MsgStream objects are valid and initialized.
        // - The total combined message count does not exceed the capacity of the current MsgStream.
        // Postconditions:
        // - Appends all messages from the other MsgStream to the current MsgStream.
        // - Throws a runtime error if the combined message count exceeds the capacity.
        MsgStream& operator+=(const MsgStream& other);

        int getMessageCount() const;
        int getMaxOperations() const;
        int getCapacity() const;
};

// Implementation invariant:
// - The message stream must always maintain its message count and operation count within the defined limits.
// - The "messages" array should always contain valid messages that meet the set constraints.
// - Messages are stored sequentially in the messages array without any gaps or uninitialized entries within the valid range.
// - The capacity must not be exceeded; attempting to append beyond capacity should throw an appropriate error.
// - The operation count must accurately reflect the total number of client operations performed on the stream.
// - overloaded operator! provides a quick way to check if the stream is empty, improving readability.
// - overloaded operator+ allows merging two stream into a new stream for clear abstraction.
// - overloaded operator== enables comparison of two streams to enhance usability for equality checks.
// - overloaded operator!= enables comparison of two stream but returns the negation of ==.
// - overloaded operator+= helps in combining messages of two objects into one.

#endif