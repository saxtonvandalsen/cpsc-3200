// Saxton Van Dalsen
// 10/15/2024

#ifndef PARTITIONSTREAM_H
#define PARTITIONSTREAM_H

#include "MsgStream.h"

class PartitionStream {
    
    private:

        struct Partition {
            char* key;
            MsgStream* stream;
        };

        Partition* partitions;
        int partitionCount;
        int capacity;

        bool validateParitionKey(const char* key) const;
        int findPartitionIndex(const char* key) const;
        int verifyCapacity(int capacity);

    public:
        PartitionStream(int initialCapacity, const MsgStream* streams);
        PartitionStream(const PartitionStream& other);
        PartitionStream& operator=(const PartitionStream& other);
        ~PartitionStream();

        void appendMessage(const char* paritionKey, const char* message);
        char** readMessage(const char* partitionKey, int startRange, int endRange);
        void reset();
        int getPartitionCount() const;
        int getCapacity() const;
};

#endif