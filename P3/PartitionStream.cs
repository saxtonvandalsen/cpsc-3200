// Saxton Van Dalsen
// 10/27/2024

using MsgStreamLibrary;
using System;
using Systems.Collections.Generic;
using System.Linq;

namespace PartitionStreamLibrary
{
    // Class invariant:
    // 
    public class PartitionStream
    {

        private Dictionary<int, MsgStream> msgStreams;
        private const int MAX_CAPACITY = 200;
        private int capacity;
        private int key;
        private int partitionCount;

        private bool ValidateCapacity(int capacity)
        {
            if (capacity > MAX_CAPACITY)
            {
                throw new ArgumentException($"Capacity exceeded {MAX_CAPACITY}, setting capacity to {MAX_CAPACITY}.");
                capacity = MAX_CAPACITY;
            }
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException("Capacity must be between 1 and " + MAX_CAPACITY);
            }
            return true;
        }

        private bool ValidateStreams(IEnumerable<MsgStream> streams)
        {
            return streams == null || !streams.Any();
        }

        private bool OperationLimitReached(int partitionKey)
        {
            return msgStreams[partitionKey].OperationLimit();
        }

        private bool ValidatePartitionKey(int partitionKey)
        {
            return msgStreams.ContainsKey(partitionKey);
        }

        private bool IsFull(int partitionKey)
        {
            return msgStreams[partitionKey].IsFull();
        }

        // Preconditions:
        // - 
        private bool IsValidMessage(int partitionKey, string message)
        {
            return msgStreams[partitionKey].IsValidMessage(message);
        }

        // Preconditions:
        // -
        // Postconditions:
        // - 
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
        // -
        // Postconditions:
        // - 
        public void AddMessage(int partitionKey, string message)
        {
            if (!ValidatePartitionKey(partitionKey)) throw new ArgumentException("Invalid partition key.");

            if (IsFull(partitionKey)) throw new InvalidOperationException("Partition stream is full");

            if (!IsValidMessage(partitionKey, message))
            {
                throw new ArgumentException("Invalid message: message is either null, empty, or exceeds the maximum allowed length.");
            }

            if(OperationLimitReached(partitionKey)) throw new InvalidOperationException("Operation limit has been reached.");

            msgStreams[partitionKey].AppendMessage(message);
        }

        public string[] ReadMessage(int partitionKey, int startRange, int endRange)
        {
            if (!ValidatePartitionKey(partitionKey)) throw new ArgumentException("Invalid partition key.");

            if (OperationLimitReached(partitionKey)) throw new InvalidOperationException("Operation limit has been reached ");

            return msgStreams[partitionKey].ReadMessages(startRange, endRange);
        }

        public void Reset()
        {
            foreach (var stream in msgStreams.Values)
            {
                stream.Reset();
            }
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
                MsgStream cloneStream = stream.Value.Clone();
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
}

// Capacity defines the maximum number of MsgSteam objects being allowed.
// PartitionCount is representing the number of MsgStream objects being added.