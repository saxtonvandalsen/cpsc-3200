// Saxton Van Dalsen
// 10/15/2024

#include "PartitionStream.h"
#include "MsgStream.h"
#include <cstring>
#include <stdexcept>

PartitionStream::PartitionStream(int initialCapacity, const MsgStream* streams)
    : partitionCount(0), capacity(verifyCapacity(initialCapacity)) {    
        partitions = new Partition[capacity];

        for (int i = 0; i < capacity; i++)
        {
            partitions[i].stream = new MsgStream(streams[i]);
            partitions[i].key = nullptr;
        }
}

PartitionStream::PartitionStream(const PartitionStream& other)
    : partitionCount(other.partitionCount), capacity(other.capacity) {
        partitions = new Partition[capacity];

        for (int i = 0; i < partitionCount; i++)
        {
            partitions[i].key = new char[strlen(other.partitions[i].key) + 1];
            strcpy(partitions[i].key, other.partitions[i].key);
            partitions[i].stream = new MsgStream(*other.partitions[i].stream);
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

PartitionStream::~PartitionStream()
{
    for (int i = 0; i < partitionCount; i++)
    {
        delete[] partitions[i].key;
        delete partitions[i].stream;
    }
    delete[] partitions;
}