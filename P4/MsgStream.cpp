// Saxton Van Dalsen
// 11/14/2024

#include "MsgStream.h"
#include <string>
#include <memory>
#include <stdexcept>

using namespace std;

MsgStream::MsgStream(int initialCapacity) : operationCount(0), messageCount(0)
{
    capacity = calculateCapacity(initialCapacity);
    maxOperations = calculateMaxOperations(initialCapacity);
    messages = std::unique_ptr<std::string[]>(new std::string[capacity]);
}

MsgStream::MsgStream() : capacity(0), maxOperations(0), operationCount(0), messages(nullptr), messageCount(0) {}

MsgStream::MsgStream(const MsgStream& other)
{
    capacity = other.capacity;
    maxOperations = other.maxOperations;
    messageCount = other.messageCount;
    operationCount = other.operationCount;

    messages = std::unique_ptr<std::string[]>(new std::string[capacity]);

    for (int i = 0; i < messageCount; i++)
    {
        messages[i] = other.messages[i];
    }
}

MsgStream& MsgStream::operator=(const MsgStream& other)
{
    if (this == &other) return *this;

    std::unique_ptr<std::string[]> newMessages(new std::string[other.capacity]);

    for (int i = 0; i < other.messageCount; i++)
    {
        newMessages[i] = other.messages[i];
    }

    capacity = other.capacity;
    maxOperations = other.maxOperations;
    messageCount = other.messageCount;
    operationCount = other.operationCount;

    messages = move(newMessages);

    return *this;
}

MsgStream::MsgStream(MsgStream&& other) noexcept
    : capacity(0), maxOperations(0), operationCount(0), messages(nullptr), messageCount(0) {
        swap(messages, other.messages);
        swap(capacity, other.capacity);
        swap(maxOperations, other.maxOperations);
        swap(messageCount, other.messageCount);
        swap(operationCount, other.operationCount);
}

MsgStream& MsgStream::operator=(MsgStream&& other) noexcept
{
    if (this == &other) return *this;

    messages = move(other.messages);

    capacity = other.capacity;
    maxOperations = other.maxOperations;
    messageCount = other.messageCount;
    operationCount = other.operationCount;

    other.capacity = 0;
    other.messageCount = 0;
    other.operationCount = 0;

    return *this;
}

unique_ptr<string[]> MsgStream::readMessages(int startRange, int endRange)
{
    if (operationLimit())
        throw runtime_error("Operation limit has been reached.");

    if (isInvalidRange(startRange, endRange))
        throw out_of_range("Invalid range for reading messages.");

    int range = endRange - startRange + 1;
    std::unique_ptr<std::string[]> readMessages(new std::string[range]);

    for (int i = 0; i < range; i++)
    {
        readMessages[i] = messages[startRange + i];
    }

    operationCount++;
    return readMessages;
}

void MsgStream::appendMessage(const string& message)
{
    if (operationLimit())
        throw runtime_error("Operation limit has been reached.");

    if (isFull())
        throw runtime_error("Capacity has been reached.");

    if (!isValidMessage(message))
        throw runtime_error("Invalid message.");

    messages[messageCount] = message;

    messageCount++;
    operationCount++;
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

bool MsgStream::operator!() const {
    return messageCount == 0;
}

MsgStream MsgStream::operator+(const MsgStream& other) const {
    
    MsgStream merged(capacity + other.capacity);
    for (int i = 0; i < messageCount; i++)
    {
        merged.appendMessage(messages[i]);
    }
    for (int i = 0; i < other.messageCount; i++) {
        merged.appendMessage(other.messages[i]);
    }
    return merged;
}

bool MsgStream::operator==(const MsgStream& other) const {
    
    if (messageCount != other.messageCount || capacity != other.capacity)
    {
        return false;
    }
    for (int i = 0; i < messageCount; i++)
    {
        if (messages[i] != other.messages[i])
        {
            return false;
        }
    }
    return true;
}

bool MsgStream::operator!=(const MsgStream& other) const {
    return !(*this == other);
}

MsgStream& MsgStream::operator+=(const MsgStream& other) {
    
    if (messageCount + other.messageCount > capacity)
    {
        throw std::runtime_error("Combined MsgStream exceeds capacity");
    }

    for (int i = 0; i < other.messageCount; i++)
    {
        appendMessage(other.messages[i]);
    }

    return *this;
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