// Saxton Van Dalsen
// 10/15/2024

#ifndef PARTITIONSTREAM_H
#define PARTITIONSTREAM_H

#include "MsgStream.h"

class PartitionStream {
    
    private:

        struct Partition {
            char key[30];
            MsgStream* stream;
        };

        Partition* partition;
        int partitionCount;
        int capacity;

        bool validateParitionKey(const char* key) const;
        int findPartitionIndex(const char* key) const;

    public:
        PartitionStream(int capacity, const MsgStream* streams, const char keys[][30]);
        PartitionStream(const PartitionStream& other);
        PartitionStream& operator=(const PartitionStream& other);
        ~PartitionStream();

        void appendMessage(const char* paritionKey, const char* message);
        char** readMessage(const char* partitionKey, int startRange, int endRange);
        void reset();
        int getPartitionCount() const;
};

#endif