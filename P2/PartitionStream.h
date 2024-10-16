// Saxton Van Dalsen
// 10/15/2024

#ifndef PARTITIONSTREAM_H
#define PARTITIONSTREAM_H

#include "MsgStream.h"

class PartitionStream {
    
    private:

        // Class Invariant:
        // The class maintains a collection of message streams indexed by unique partition keys.
        // The integrity of partitionCount and capacity must be preserved throughout the class usage.
        // Client must be able to read and handle thrown errors
        // Capacity and MsgStreams declared in partition object construction

        struct Partition {
            char* key;
            MsgStream* stream;
        };

        Partition* partitions;
        int partitionCount;
        int capacity;

        bool validatePartitionKey(const char* key) const;
        int findPartitionIndex(const char* key) const;
        int verifyCapacity(int capacity);

    public:
        PartitionStream(int initialCapacity, const MsgStream* streams);
        PartitionStream(const PartitionStream& other);
        PartitionStream& operator=(const PartitionStream& other);
        PartitionStream(PartitionStream&& src) noexcept;
        PartitionStream& operator=(PartitionStream&& src) noexcept;
        ~PartitionStream();

        // Precondtions:
        // partitionKey must not be null or empty
        // message must be valid and not null
        // Postconditions:
        // If the partition does not exist, it is created
        // message is appended to the partition's stream, as long as partitionCount hasn't been exceeded.
        void appendMessage(const char* paritionKey, const char* message);

        // Preconditions:
        // partitionKey must not be null or empty.
        // startRange must be less than or equal to endRange.
        // Postconditions:
        // returns an array of messages from the specified range if valid.
        char** readMessage(const char* partitionKey, int startRange, int endRange);

        // Preconditions:
        // all messages in all partitions will be cleared
        // keys and partitionCount reset
        void reset();
        int getPartitionCount() const;
        int getCapacity() const;
};

    // Implementation Invariant:
    // partitions must be a valid pointer to an array of Partition objects
    // Each key in partition should be unique if not null
    // Each stream must be properly initialized
    // Boundaries: 0 <= partitionCount <= capacity

#endif