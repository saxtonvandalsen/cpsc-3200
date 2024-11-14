// Saxton Van Dalsen
// 11/14/2024

#ifndef PARTITIONSTREAM_H
#define PARTITIONSTREAM_H

#include "MsgStream.h"
#include <memory>
#include <string>
#include <stdexcept>

using namespace std;

class PartitionStream
{
    // Class invariant:
    // 

    private:
        unique_ptr<MsgStream[]> streams;
        unique_ptr<int[]> keys;
        int capacity;
        int partitionCount;
        int operationCount;

        PartitionStream(const PartitionStream& other);
        PartitionStream& operator=(const PartitionStream& other);
        PartitionStream(PartitionStream&& other) noexcept;
        PartitionStream& operator=(PartitionStream&& other) noexcept;

        int findPartitionIndex(const int& key) const;
        bool validatePartitionKey(const int& key) const;
        int verifyCapacity(int initialCapacity);
        bool operationLimitReached();
        bool isFull();
        bool isValidMessage(const string& message) const;

    public:
        PartitionStream(int initialCapacity);

        void writeMessage(const int& key, const string& message);
        unique_ptr<string[]> readMessage(const int& key, int startRange, int endRange);
        int getCapacity();
        int getPartitionCount();
        unique_ptr<int[]> getPartitionKeys();
        void initializeMsgStream(int index, int capacity);
        MsgStream& operator[](int index);
        void operator-();
        PartitionStream& operator+=(const PartitionStream& other);
};

// Implementation invariant:
// - 
// - overloaded operator[] helps simplify access to MsgStream objects by index which improves abstraction.
// - overloaded operator- provided a simple way to reset the state of PartitionStream without calling a separate function.
// - overloaded operator+= allows for merging two PartitionStream objects in place.

#endif