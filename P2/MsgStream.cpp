// Saxton Van Dalsen
// 10/15/2024

#include "MsgStream.h"
#include <cstring>
#include <stdexcept>

using namespace std;

MsgStream::MsgStream(int initialCapacity) : messageCount(0), operationCount(0)
{
    capacity = calculateCapacity(initialCapacity);
    maxOperations = calculateMaxOperations(capacity);
    
    messages = new char*[capacity];
    for (int i = 0; i < capacity; i++)
        messages[i] = nullptr;
}

MsgStream::~MsgStream()
{
    for (int i = 0; i < messageCount; i++)
    {
        delete[] messages[i];
    }

    delete[] messages;
}

MsgStream::MsgStream(const MsgStream& other)
{
    capacity = other.capacity;
    maxOperations = other.maxOperations;
    messageCount = other.messageCount;
    operationCount = other.operationCount;

    messages = new char*[capacity];

    for (int i = 0; i < messageCount; i++)
    {
        messages[i] = new char[strlen(other.messages[i]) + 1];
        strcpy(messages[i], other.messages[i]);
    }

    for (int i = messageCount; i < capacity; i++)
        messages[i] = nullptr;
}

MsgStream& MsgStream::operator=(const MsgStream& other)
{
    if (this == &other) return *this;

    for (int i = 0; i < messageCount; i++)
    {
        delete[] messages[i]; 
    }
    delete[] messages;

    capacity = other.capacity;
    maxOperations = other.maxOperations;
    messageCount = other.messageCount;
    operationCount = other.operationCount;

    messages = new char*[capacity];

    for (int i = 0; i < messageCount; i++)
    {
        messages[i] = new char[strlen(other.messages[i]) + 1];
        strcpy(messages[i], other.messages[i]);
    }

    for (int i = messageCount; i < capacity; i++)
    {
        messages[i] = nullptr;
    }

    return *this;
}

MsgStream::MsgStream(MsgStream&& src) noexcept 
    : messages(src.messages), capacity(src.capacity), maxOperations(src.maxOperations), 
      messageCount(src.messageCount), operationCount(src.operationCount) {

    src.messages = nullptr;
    src.capacity = 0;
    src.maxOperations = 0;
    src.messageCount = 0;
    src.operationCount = 0;
}

MsgStream& MsgStream::operator=(MsgStream&& src) noexcept
{
    if (this == &src) return *this;

    reset();

    messages = src.messages;
    capacity = src.capacity;
    maxOperations = src.maxOperations;
    messageCount = src.messageCount;
    operationCount = src.operationCount;

    src.messages = nullptr;
    src.capacity = 0;
    src.maxOperations = 0;
    src.messageCount = 0;
    src.operationCount = 0;

    return *this;
}

char** MsgStream::readMessages(int startRange, int endRange)
{
    if (operationLimit())
        throw runtime_error("Operation limit reached.");

    if (isInvalidRange(startRange, endRange))
        throw out_of_range("Invalid range for reading messages.");

    int rangeSize = endRange - startRange;
    char** result = new char*[rangeSize];

    for (int i = 0; i < rangeSize; i++)
        result[i] = messages[startRange + i];

    operationCount++;
    return result;
}

void MsgStream::appendMessage(const char* message)
{
    if (isFull())
        throw overflow_error("Message stream is full");

    if (operationLimit())
        throw runtime_error("Operation limit exceeded");

    if (!isValidMessage(message))
        throw invalid_argument("Invalid message");

    size_t length = strlen(message);

    messages[messageCount] = new char[length + 1];

    strcpy(messages[messageCount], message);

    messageCount++;
    operationCount++;
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

int MsgStream::calculateMaxOperations(int capacity)
{
    return capacity * 2;
}

bool MsgStream::isFull() const{
    return messageCount >= capacity;
}

bool MsgStream::operationLimit() const{
    return operationCount >= maxOperations;
}

bool MsgStream::isValidMessage(const char* message) const
{
    return message != nullptr && strlen(message) <= MAX_STRING_LENGTH;
}

int MsgStream::calculateCapacity(int cap)
{
    if (cap > MAX_CAPACITY)
    {
        cap = MAX_CAPACITY;
    }
    if (cap <= 0) {
        cap = 1;
    }

    return cap;
}

bool MsgStream::isInvalidRange(int startRange, int endRange) const
{
    return (startRange < 0 || endRange <= startRange || startRange >= messageCount || endRange > messageCount);
}