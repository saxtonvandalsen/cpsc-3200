// Saxton Van Dalsen
// 11/14/2024

#include "MsgStream.h"

using namespace std;

MsgStream::MsgStream(int initialCapacity) : messageCount(0), operationCount(0)
{
    capacity = calculateCapacity(initialCapacity);
    maxOperations = calculateMaxOperations(initialCapacity);


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

bool MsgStream::isValidMessage(string message) const
{

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
    for (int i = 0; i < messageCount; i++)
        delete[] messages[i];

    messageCount = 0;
    operationCount = 0;
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