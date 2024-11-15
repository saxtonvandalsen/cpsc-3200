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
    // - PartitionStream is composed of MsgStream objects, each representing a partitioned sequence of messages.
    // - The capacity must be greater than 0, determining the maximum number of MsgStream partitions available.
    // - Each MsgStream instance within the PartitionStream is uniquely identified by a partition key.
    // - Dependency injection ensures that the MsgStream objects can be externally provided or replaced, supporting modularity.
    // - The keys array provides a one-to-one mapping of keys to MsgStream instances for efficient partition identification.
    // - The operationCount tracks the number of operations performed across all partitions, ensuring usage limits are respected.
    // - The partitionCount tracks the number of active partitions with messages, supporting stream management.
    // - PartitionStream operations, such as writing and reading messages, must respect the validity of partition keys and the capacity constraints.

    private:
        unique_ptr<MsgStream[]> streams;
        unique_ptr<int[]> keys;
        int capacity;
        int partitionCount;
        int operationCount;

        // Preconditions:
        // - other must be a valid, fully initialized PartitionStream instance.
        // Postconditions:
        // - A PartitionStream instance is created with the specified initial capacity, 
        //   initialized streams and keys arrays, and a sequential key assignment.
        PartitionStream(const PartitionStream& other);

        // Preconditions:
        // - other must be a valid, fully initialized PartitionStream instance.
        // Postconditions:
        // - Deep copy of other is created, duplicating its streams and keys array
        //   with no aliasing. Releasing any previously held resources.
        PartitionStream& operator=(const PartitionStream& other);

        // Preconditions:
        // - other must be a valid PartitionStream instance and must not be the same instance as this.
        // Postconditions:
        // - The current instance takes ownership of other's resources (streams, keys, and other data),
        //   leaving other in a valid but empty state.
        PartitionStream(PartitionStream&& other) noexcept;

        // Preconditions:
        // - other must be a valid PartitionStream instance and must not be the same instance as this
        // Postconditions:
        // - The current instance takes ownership of other's resources (streams, keys, and other data),
        //   leaving other in a valid but empty state, and releasing any previously held resources.
        PartitionStream& operator=(PartitionStream&& other) noexcept;

        int findPartitionIndex(const int& key) const;
        bool validatePartitionKey(const int& key) const;
        int verifyCapacity(int initialCapacity);
        bool operationLimitReached();
        bool isFull();
        bool isValidMessage(const string& message) const;

    public:
        // Preconditions:
        // - initial capacity must be between 0 and 200
        // Postconditions:
        // - instance is created with capacity set, initialized streams and associated keys.
        PartitionStream(int initialCapacity, std::unique_ptr<MsgStream[]> msgStreams);

        // Preconditions:
        // - key must be a valid partition key. The operation limit must not be reached, stream must not be full, 
        //   and message must meet validity criteria.
        // Postconditions:
        // - The specified message is appended to the MsgStream associated with the given key, 
        //   partitionCount and operationCount are incremented.
        void writeMessage(const int& key, const string& message);

        // Preconditions:
        // - key must be a valid partition key and operation limit must not be reached.
        // Postconditions:
        // - Returns a unique_ptr containing messages from the specified range in the MsgStream associated with the key.
        unique_ptr<string[]> readMessage(const int& key, int startRange, int endRange);
        int getCapacity();
        int getPartitionCount();
        unique_ptr<int[]> getPartitionKeys();
        void initializeMsgStream(int index, int capacity);

        // Preconditions:
        // - index must be within the range [0, capacity)
        // Postconditions:
        // - Returns a reference to the MsgStream at the specified index.
        MsgStream& operator[](int index);

        // Postconditions:
        // - Resets the PartitionStream instance, clearing all MsgStreams and keys, 
        //   and reinitializing them with sequential keys and zeroing partitionCount and operationCount.
        void operator-();

        // Preconditions:
        // - other must be a valid PartitionStream instance with the same capacity as the current instance.
        // Postconditions:
        // - The MsgStreams and counts of other are merged into the current PartitionStream instance.
        PartitionStream& operator+=(const PartitionStream& other);
};

// Implementation invariant:
// - PartitionStream relies on MsgStream for core message management in each partition.
// - Dependency injection allows externally provided MsgStream objects to replace or initialize partitions at construction.
// - The keys array ensures a unique mapping between partition keys and their respective MsgStream instances.
// - The findPartitionIndex function guarantees efficient key lookups for partition operations.
// - The validatePartitionKey function ensures all operations are performed on valid keys within the partition range.
// - The verifyCapacity function enforces that the capacity is capped at 200 and defaults to 1 if the initial value is invalid.
// - Unique ownership of MsgStream objects is managed through std::unique_ptr to ensure safe and automatic memory management.
// - Copy and move semantics for PartitionStream ensure proper resource management and prevent unintended aliasing.
// - Operation limits (operationLimitReached) and capacity constraints (isFull) are enforced to maintain predictable behavior.
// - MsgStream initialization and dependency injection must maintain integrity, avoiding invalid or uninitialized MsgStream objects.
// - overloaded operator[] helps simplify access to MsgStream objects by index which improves abstraction.
// - overloaded operator- provided a simple way to reset the state of PartitionStream without calling a separate function.
// - overloaded operator+= allows for merging two PartitionStream objects in place.

#endif