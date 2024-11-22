// Saxton Van Dalsen
// 12/3/2024

using MsgStreamLibrary;
using System;
using Systems.Collections.Generic;
using System.Linq;

namespace PartitionStreamLibrary
{
    // Class invariant:
    // PartitionStream must maintain a non-null Dictionary of MsgStream objects
    // Number of MsgStreams (partition count) should not exceed the specified capacity
    // Each MsgStream in msgStreams must be valid by MsgStream class, initialized, and have a unique integer key
    // Capacity must be between 1 and MAX_CAPACITY (200)
    public class PartitionStream
    {

        private Dictionary<int, MsgStream> msgStreams;
        private const int MAX_CAPACITY = 200;
        private int capacity;
        private int key;
        private int partitionCount;
        private int operationCount;

        // Preconditions:
        // - capacity should be an integer within limits 1 to MAX_CAPACITY (200)
        // Postconditions:
        // - Sets capacity to MAX_CAPACITY (200) if given a capacity greater than that limit
        // - Sets capacity to 1 if given a capacity less than 0
        // - Returns true if the capacity is valid
        private bool ValidateCapacity(int capacity)
        {
            if (capacity > MAX_CAPACITY)
            {
                capacity = MAX_CAPACITY;
                throw new ArgumentException($"Capacity exceeded {MAX_CAPACITY}, setting capacity to {MAX_CAPACITY}.");
            }
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException("Capacity must be between 1 and " + MAX_CAPACITY);
                capacity = 1;
            }
            return true;
        }

        // Preconditions:
        // - streams collection must not be null
        // Postconditions:
        // - Returns true is the streams collection is empty or null
        private bool ValidateStreams(IEnumerable<MsgStream> streams)
        {
            return streams == null || !streams.Any();
        }

        // Preconditions:
        // - Capacity is set and is for the limit of each MsgStream's operation limit
        // Postconditions:
        // - Returns true if the specified partition has reached its operation limit based
        // on MsgStream restrictions
        private bool OperationLimitReached()
        {
            return operationCount >= capacity * 2;
        }

        // Preconditions:
        // - Partition key should be a valid key within msgStreams collection
        // Postconditions:
        // - Returns true if the specified partition exists in msgStreams collection
        private bool ValidatePartitionKey(int partitionKey)
        {
            return msgStreams.ContainsKey(partitionKey);
        }

        // Preconditions:
        // - Capacity is set and represents maximum allowed count for the partiton
        // Postconditions:
        // - Returns true if specified partition has reach capacity
        private bool IsFull()
        {
            return partitionCount >= capacity;
        }

        // Preconditions:
        // - message must not be null, non-empty, and within message length restrictions
        // Postconditions:
        // - Returns true if specified partition's message meets requirements
        private bool IsValidMessage(string message)
        {
            return message != null && message.Trim() != "";
        }

        // Preconditions:
        // - streams collection must not be null & must have fewer or equal elements than capacity
        // Postconditions:
        // - Initializes msgStreams with provided MsgStream streams through dependency injection.
        // - Keys are assigned sequentially for each stream
        // - Sets initial partition count to number of valid streams provided
        public PartitionStream(IEnumerable<MsgStream> streams, int capacity)
        {
            if (ValidateStreams(streams)) throw new ArgumentException("Invalid streams provided.");

            if (streams.Count() > capacity) throw new ArgumentException("Number of partitons exceeds capacity.");
            
            ValidateCapacity(capacity);
            this.capacity = capacity;
            this.partitionCount = streams.Count();
            this.msgStreams = new Dictionary<int, MsgStream>();

            foreach (var stream in streams)
            {
                msgStreams[key] = stream;
                key++;
            }
        }

        // Preconditions:
        // - Partition key must be valid within msgStreams
        // - Message should be valid, non-null, and also not exceed MsgStreams length limit
        // Postconditions:
        // - Adds message to specified MsgStream based on key, if within capacity and operation limit
        public void AddMessage(int partitionKey, string message)
        {
            if (!ValidatePartitionKey(partitionKey)) throw new ArgumentException("Invalid partition key.");

            if (IsFull()) throw new InvalidOperationException("Partition stream is full");

            if (!IsValidMessage(message))
            {
                throw new ArgumentException("Invalid message: message is either null, empty, or exceeds the maximum allowed length.");
            }

            if(OperationLimitReached()) throw new InvalidOperationException("Operation limit has been reached.");

            operationCount++;

            msgStreams[partitionKey].AppendMessage(message);
        }

        // Preconditions:
        // - Partition key must be valid within msgStreams
        // - Start range and end range should be valid ranges within MsgStreams bounds
        // Postconditions:
        // - Returns array of messages from given start range to end range in specified partition based on key
        public string[] ReadMessage(int partitionKey, int startRange, int endRange)
        {
            if (!ValidatePartitionKey(partitionKey)) throw new ArgumentException("Invalid partition key.");

            if (OperationLimitReached()) throw new InvalidOperationException("Operation limit has been reached ");

            operationCount++:

            return msgStreams[partitionKey].ReadMessages(startRange, endRange);
        }

        // Preconditions:
        // -
        // Postconditions:
        // - Clears all MsgStream entries for certain partiton
        // - resets partiton count and operation count to 0
        public void Reset()
        {
            foreach (var stream in msgStreams.Values)
            {
                stream.Reset();
            }
            partitionCount = 0;
            operationCount = 0;
        }

        // Preconditions:
        // - PartitionStream must have been properly initialized with valid MsgStream objects
        // - Each MsgStream object should implement the Clone() method for deep copying
        // Postcondtions:
        // - New PartitionStream will get same capacity and partition count as the original
        // - No reference to original MsgStream objects is maintained in deep copy (no shared references)
        // - Returns new PartitionStream object containing independent copies of all encapsulated MsgStream objects from original
        public PartitionStream DeepCopy()
        {
            Dictionary<int, MsgStream> copyStreams = new Dictionary<int, MsgStream>();

            foreach (var stream in msgStreams)
            {
                MsgStream cloneStream = stream.Value.DeepCopy();
                copyStreams[stream.Key] = cloneStream;
            }

            PartitionStream deepCopyStream = new PartitionStream(copyStreams.Values, this.capacity);

            return deepCopyStream;
        }

        public int GetPartitionCount()
        {
            return partitionCount;
        }

        public int GetCapacity()
        {
            return capacity;
        }

        public IEnumerable<int> GetPartitionKeys()
        {
            return msgStreams.Keys.ToList();
        }
    }
    // Implementation invariant:
    // msgStream dictionary must not be null and should not exceed capacity
    // Each MsgStream in msgStreams should be initialized and should be functionable with its methods
    // Operation limit for each MsgStream should be respected by OperationLimitReached from PartitionStream
    // All methods that modify or access msgStreams should validate the partition key associated
}