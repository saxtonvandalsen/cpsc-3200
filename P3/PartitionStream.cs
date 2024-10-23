// Saxton Van Dalsen
// 10/27/2024

using MsgStreamLibrary;
using System;
using Systems.Collections.Generic;

public class PartitionStream {

    private Dictionary<int, MsgStream> msgStreams;
    private const int MAX_CAPACITY = 200;
    private const int MAX_STRING_LENGTH = 150;
    private int capacity;
    private int key;
    private int partitionCount;
    private int operationCount;
    private int MaxOperations => capacity * 2;

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

    private bool OperationLimitReached()
    {
        return operationCount >= MaxOperations;
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

    public PartitionStream(IEnumerable<MsgStream> streams)
    {
        if (ValidateStreams(streams)) throw new ArgumentException("Invalid streams provided.");
        
        ValidateCapacity(capacity);

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
        if (!ValidatePartitionKeyz(partitionKey)) throw new ArgumentException("Invalid partition key.");

        if (IsFull(partitionKey)) throw new InvalidOperationException("Partition stream is full");

        if (!IsValidMessage(partitionKey, message)) throw new ArgumentException
        {
            ("Invalid message: message is either null, empty, or exceeds the maximum allowed length.")
        }

        if(OperationLimitReached()) throw new InvalidOperationException("Operation limit has been reached.");

        msgStreams[partitionKey].AppendMessage(message);
    }

    public 

    public int GetPartitionCount()
    {
        return partitionCount;
    }

    public IEnumerable<int> GetPartitionKeys()
    {
        return msgStreams.Keys.ToList();
    }
}