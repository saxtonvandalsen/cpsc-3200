// Saxton Van Dalsen
// 11/14/2024

#include "MsgStream.h"
#include <string>
#include <memory>

using namespace std;

MsgStream::MsgStream(int initialCapacity) : messageCount(0), operationCount(0)
{
    capacity = calculateCapacity(initialCapacity);
    maxOperations = calculateMaxOperations(initialCapacity);
    messages = make_unique<string[]>(capacity);
}

MsgStream::MsgStream(const MsgStream& other)
{
    capacity = other.capacity;
    maxOperations = other.maxOperations;
    messageCount = other.messageCount;
    operationCount = other.operationCount;

    messages = make_unique<string[]>(capacity);

    for (int i = 0; i < capacity; i++)
    {
        messages[i] = other.messages[i];
    }
}

int MsgStream::calculateMaxOperations(int capacity)
{
    return capacity * 2;
}

bool MsgStream::isFull() const
{
    return messageCount >= capacity;
}

bool MsgStream::operationLimit() const
{
    return operationCount >= maxOperations;
}

bool MsgStream::isValidMessage(const string& message) const
{
    return message != "" && message.length() <= MAX_STRING_LENGTH;
}

int MsgStream::calculateCapacity(int capacity)
{
    if (capacity > MAX_CAPACITY) {
        capacity = MAX_CAPACITY;
    }
    if (capacity <= 0) {
        capacity = 1;
    }
    return capacity;
}

bool MsgStream::isInvalidRange(int startRange, int endRange) const
{
    return (startRange < 0 || endRange <= startRange || endRange > messageCount || startRange >= messageCount);
}

void MsgStream::reset()
{
    messageCount = 0;
    operationCount = 0;

    messages = make_unique<string[]>(capacity);
}

int MsgStream::getMessageCount() const
{
    return messageCount;
}

int MsgStream::getMaxOperations() const{
    return maxOperations;
}

int MsgStream::getCapacity() const
{
    return capacity;
}