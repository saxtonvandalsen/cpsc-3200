// Saxton Van Dalsen
// 10/15/2024

#include "PartitionStream.h"
#include "MsgStream.h"
#include <cstring>
#include <stdexcept>
#include <utility>
#include <iostream>

using namespace std;

PartitionStream::PartitionStream(int initialCapacity, const MsgStream* streams)
    : partitionCount(0), capacity(verifyCapacity(initialCapacity)) {
    try {
        partitions = new Partition[capacity];

        for (int i = 0; i < capacity; i++)
        {
            if (streams)
            {
                partitions[i].stream = new MsgStream(streams[i]);
            } else
            {
                throw std::invalid_argument("Streams cannot be null.");
            }
            partitions[i].key = nullptr;
        }
    }
    catch (const bad_alloc& e)
    {
        cerr << "Memory allocation failed: " << e.what() << endl;
        throw;
    }
    catch (const exception& e)
    {
        cerr << "Error in constructor: " << e.what() << endl;
        throw;
    }
}

PartitionStream::PartitionStream(const PartitionStream& other)
    : partitionCount(other.partitionCount), capacity(other.capacity) {
    try {
        partitions = new Partition[capacity];

        for (int i = 0; i < partitionCount; i++)
        {
            if (other.partitions[i].key)
            {
                partitions[i].key = new char[strlen(other.partitions[i].key) + 1];
                strcpy(partitions[i].key, other.partitions[i].key);
            } else
            {
                partitions[i].key = nullptr;
            }
            partitions[i].stream = new MsgStream(*other.partitions[i].stream);
        }
    }
    catch (const bad_alloc& e)
    {
        cerr << "Memory allocation failed: " << e.what() << endl;
        throw;
    }
}

PartitionStream& PartitionStream::operator=(const PartitionStream& other)
{
    if (this == &other) return *this;

    for (int i = 0; i < partitionCount; i++)
    {
        delete[] partitions[i].key;
        delete partitions[i].stream;
    }
    delete[] partitions;

    partitionCount = other.partitionCount;
    capacity = other.capacity;
    partitions = new Partition[capacity];

    for (int i = 0; i < partitionCount; i++) 
    {
        partitions[i].key = new char[strlen(other.partitions[i].key) + 1];
        strcpy(partitions[i].key, other.partitions[i].key);
        partitions[i].stream = new MsgStream(*other.partitions[i].stream);
    }
    return *this;
}

PartitionStream::PartitionStream(PartitionStream&& src) noexcept
    : partitions(src.partitions), partitionCount(src.partitionCount), capacity(src.capacity) {

        src.partitions = nullptr;
        src.partitionCount = 0;
        src.capacity = 0;
}

PartitionStream& PartitionStream::operator=(PartitionStream&& src) noexcept
{
    if (this != &src)
    {
        std::swap(this->partitions, src.partitions);
        std::swap(this->partitionCount, src.partitionCount);
        std::swap(this->capacity, src.capacity);
    }
    return *this;
}

PartitionStream::~PartitionStream()
{
    for (int i = 0; i < partitionCount; i++)
    {
        delete[] partitions[i].key;
        delete partitions[i].stream;
    }
    delete[] partitions;
}

void PartitionStream::appendMessage(const char* partitionKey, const char* message)
{
    int index = findPartitionIndex(partitionKey);

    if (index == -1)
    {
        if (getPartitionCount() >= capacity)
        {
            throw std::overflow_error("No space left for a new partition");
        }

        try {
            partitions[partitionCount].key = new char[strlen(partitionKey) + 1];
            strcpy(partitions[partitionCount].key, partitionKey);
        }
        catch (const bad_alloc& e)
        {
            cerr << "Memory allocation failed: " << e.what() << endl;
            throw;
        }
        index = partitionCount;
        partitionCount++;
    }

    partitions[index].stream->appendMessage(message);
}

char** PartitionStream::readMessage(const char* partitionKey, int startRange, int endRange)
{
    int index = findPartitionIndex(partitionKey);

    if (index == -1)
    {
        throw std::invalid_argument("Invalid partition key");
    }

    return partitions[index].stream->readMessages(startRange, endRange);
}

void PartitionStream::reset()
{
    for (int i = 0; i < partitionCount; i++)
    {
        partitions[i].stream->reset();

        delete[] partitions[i].key;
        partitions[i].key = nullptr;
    }
    partitionCount = 0;
}

bool PartitionStream::validateParitionKey(const char* key) const
{
    return key != nullptr && strlen(key) > 0;
}

int PartitionStream::findPartitionIndex(const char* key) const
{
    for (int i = 0; i < partitionCount; i++)
    {
        if (strcmp(partitions[i].key, key) == 0)
        {
            return i;
        }
    }
    return -1;
}

int PartitionStream::verifyCapacity(int capacity)
{
    if (capacity <= 0)
    {
        throw std::invalid_argument("Capacity must be greater than 0");
    }
    return capacity;
}

int PartitionStream::getPartitionCount() const
{
    return partitionCount;
}

int PartitionStream::getCapacity() const
{
    return capacity;
}