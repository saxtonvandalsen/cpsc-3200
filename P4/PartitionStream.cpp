// Saxton Van Dalsen
// 11/14/2024

#include "PartitionStream.h"
#include "MsgStream.h"
#include <memory>
#include <string>
#include <stdexcept>

using namespace std;

PartitionStream::PartitionStream(int initialCapacity) : partitionCount(0), operationCount(0)
{
    capacity = verifyCapacity(initialCapacity);

    streams = std::unique_ptr<MsgStream[]>(new MsgStream[capacity]);
    keys = std::unique_ptr<int[]>(new int[capacity]);

    for (int i = 0; i < capacity; i++)
    {
        keys[i] = i + 1;
    }
}

PartitionStream::PartitionStream(const PartitionStream& other)
{
    capacity = other.capacity;
    operationCount = other.operationCount;
    partitionCount = other.partitionCount;

    streams = std::unique_ptr<MsgStream[]>(new MsgStream[capacity]);
    keys = std::unique_ptr<int[]>(new int[capacity]);

    for (int i = 0; i < capacity; i++)
    {
        streams[i] = other.streams[i];
        keys[i] = other.keys[i];
    }
}

PartitionStream& PartitionStream::operator=(const PartitionStream& other)
{
    if (this == &other) return *this;

    std::unique_ptr<MsgStream[]> copiedStreams(new MsgStream[other.capacity]);
    std::unique_ptr<int[]> copiedKeys(new int[other.capacity]);
    
    for (int i = 0; i < other.capacity; i++)
    {
        copiedStreams[i] = other.streams[i];
        copiedKeys[i] = other.keys[i];
    }

    capacity = other.capacity;
    operationCount = other.operationCount;
    partitionCount = other.partitionCount;

    streams = move(copiedStreams);
    keys = move(copiedKeys);

    return *this;
}

PartitionStream::PartitionStream(PartitionStream&& other) noexcept
    : streams(std::move(other.streams)),
      keys(std::move(other.keys)),
      capacity(other.capacity),
      partitionCount(other.partitionCount),
      operationCount(other.operationCount)
{
    other.capacity = 0;
    other.operationCount = 0;
    other.partitionCount = 0;
}

PartitionStream& PartitionStream::operator=(PartitionStream&& other) noexcept
{
    if (this == &other) return *this;

    streams = move(other.streams);
    keys = move(other.keys);

    capacity = other.capacity;
    operationCount = other.operationCount;
    partitionCount = other.partitionCount;

    other.capacity = 0;
    other.operationCount = 0;
    other.partitionCount = 0;

    return *this;
}

void PartitionStream::writeMessage(const int& key, const string& message)
{
    if (!validatePartitionKey(key))
        throw runtime_error("Invalid key");

    if (operationLimitReached())
        throw runtime_error("Operation limit reached");

    if (isFull())
        throw runtime_error("Stream is full");

    if (!isValidMessage(message))
        throw runtime_error("Invalid message");

    int index = findPartitionIndex(key);

    streams[index].appendMessage(message);

    partitionCount++;
    operationCount++;
}

unique_ptr<string[]> PartitionStream::readMessage(const int& key, int startRange, int endRange)
{
    if (!validatePartitionKey(key))
        throw runtime_error("Invalid key");

    if (operationLimitReached())
        throw runtime_error("Operation limit reached");

    int index = findPartitionIndex(key);

    return streams[index].readMessages(startRange, endRange);
}

int PartitionStream::findPartitionIndex(const int& key) const
{
    for (int i = 0; i < capacity; i++)
    {
        if (keys[i] == key) return i;
    }
    return -1;
}

bool PartitionStream::validatePartitionKey(const int& key) const
{
    return key > 0 && key <= capacity;
}

int PartitionStream::verifyCapacity(int initialCapacity)
{
    if (initialCapacity > 200)
    {
        return capacity = 200;
    }
    if (initialCapacity <= 0)
    {
        return capacity = 1;
    }
    return capacity = initialCapacity;
}

bool PartitionStream::operationLimitReached()
{
    return operationCount >= capacity * 2;
}

bool PartitionStream::isFull()
{
    return partitionCount >= capacity;
}

bool PartitionStream::isValidMessage(const string& message) const
{
    return !message.empty();
}

int PartitionStream::getCapacity()
{
    return capacity;
}

int PartitionStream::getPartitionCount()
{
    return partitionCount;
}

unique_ptr<int[]> PartitionStream::getPartitionKeys()
{
    std::unique_ptr<int[]> copyKeys(new int[capacity]);
    for (int i = 0; i < capacity; i++)
    {
        copyKeys[i] = keys[i];
    }
    return copyKeys;
}

void PartitionStream::initializeMsgStream(int index, int capacity)
{
    if (index >= 0 && index < this->capacity)
    {
        streams[index] = MsgStream(capacity);
    }
    else
    {
        throw std::out_of_range("Invalid index for MsgStream initialization.");
    }
}

MsgStream& PartitionStream::operator[](int index) {
    
    if (index < 0 || index >= capacity)
    {
        throw std::out_of_range("Invalid index");
    }
    return streams[index];
}

void PartitionStream::operator-() {
    
    partitionCount = 0;
    operationCount = 0;

    streams = std::unique_ptr<MsgStream[]>(new MsgStream[capacity]);
    keys = std::unique_ptr<int[]>(new int[capacity]);

    for (int i = 0; i < capacity; i++)
    {
        keys[i] = i + 1;
    }
}

PartitionStream& PartitionStream::operator+=(const PartitionStream& other) {
    
    if (capacity != other.capacity)
    {
        throw std::invalid_argument("PartitionStreams must have the same capacity to merge");
    }

    for (int i = 0; i < other.capacity; i++)
    {
        streams[i] += other.streams[i];
    }
    partitionCount += other.partitionCount;
    operationCount += other.operationCount;
    return *this;
}
