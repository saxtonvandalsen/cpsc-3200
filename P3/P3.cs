namespace P3;

// Saxton Van Dalsen
// 10/27/2024

using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Testing MsgStream:");
        MsgStream msgStream1 = new MsgStream(5);
        MsgStream msgStream2 = new MsgStream(2);

        // Test AppendMessages with MsgStream1 & MsgStream2
        Console.WriteLine("Appending messages to MsgStream 1 & 2:");

        msgStream1.AppendMessage("MsgStream message 1");
        msgStream1.AppendMessage("MsgStream message 2");
        msgStream1.AppendMessage("MsgStream message 3");

        // Test getting message count for MsgStream1
        int msgStream1Count = msgStream1.GetMessageCount();
        Console.WriteLine("Message count for MsgStream1: " + msgStream1Count);

        msgStream2.AppendMessage("MsgStream2 test message 1");
        msgStream2.AppendMessage("MsgStream2 test message 2");

        try
        {
            // Expected exception thrown exceeding capacity
            msgStream2.AppendMessage("MsgStream2 test message 3");
            msgStream2.AppendMessage("MsgStream2 test message 4");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Expected Exception for MsgStream2: {ex.Message}");
        }

        // Test ReadMessages with MsgStream1
        Console.WriteLine("\nReading messages from MsgStream 1:");
        var messages = msgStream1.ReadMessages(0, 3);
        Console.WriteLine(string.Join(", ", messages));

        // Test ReadMessages with MsgStream1 with invalid range
        try
        {
            Console.WriteLine("Reading messages from MsgStream 2:");
            var messages2 = msgStream2.ReadMessages(0, 3);
            Console.WriteLine(string.Join(", ", messages2));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Expected Exception for MsgStream2: {ex.Message}");
        }

        // Test DeepCopy
        Console.WriteLine("\nTesting DeepCopy for MsgStream 1:");
        MsgStream copyMsgStream = msgStream1.DeepCopy();
        Console.WriteLine($"Copy Message Count: {copyMsgStream.GetMessageCount()}");

        // Test Reset
        Console.WriteLine("Resetting MsgStream 1:");
        msgStream1.Reset();
        Console.WriteLine($"Message count after reset: {msgStream1.GetMessageCount()}");

        // Test 2: Create DurableStream instance
        Console.WriteLine("\nTesting DurableStream:");
        DurableStream durableStream1 = new DurableStream(4, "durableStream.txt");
        DurableStream durableStream2 = new DurableStream(11, "durableStream2.txt");

        try
        {
            durableStream1.AppendMessage("Durable test message 1");
            durableStream1.AppendMessage("Durable test message 2");
            durableStream1.AppendMessage("Durable test message 3");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }

        // Read messages
        Console.WriteLine("Reading messages from DurableStream:");
        var durableMessages = durableStream1.ReadMessages(0, 3);
        Console.WriteLine(string.Join(", ", durableMessages));

        // Test DeepCopy
        Console.WriteLine("Testing DeepCopy for DurableStream:");
        DurableStream copyDurableStream = (DurableStream)durableStream1.DeepCopy();
        Console.WriteLine($"Copy DurableStream Message Count: {copyDurableStream.GetMessageCount()}");

        // Test Reset
        Console.WriteLine("Resetting DurableStream:");
        durableStream1.Reset();
        Console.WriteLine($"Message count after reset: {durableStream1.GetMessageCount()}");

        // Test Basic initialization with homogeneous collection
        Console.WriteLine("\nTest Basic Initialization (Homogeneous Collection)");
        var homogeneousStreams = new List<MsgStream> { new MsgStream(21), new MsgStream(17) };
        var homogeneousPartition = new PartitionStream(homogeneousStreams, 19);
        Console.WriteLine($"Homogeneous Partition Capacity: {homogeneousPartition.GetCapacity()}");
        Console.WriteLine($"Homogeneous Partition Count: {homogeneousPartition.GetPartitionCount()}");

        // Test Adding and Reading Messages with mode changes (operation limit edge)
        Console.WriteLine("\nTest Adding and Reading Messages with Mode Changes (Operation Limit Edge)");
        homogeneousPartition.AddMessage(0, "First message in homogeneous partition.");
        homogeneousPartition.AddMessage(1, "Second message in homogeneous partition.");
        Console.WriteLine("Messages in Homogeneous PartitionStream after adding:");
        foreach (var key in homogeneousPartition.GetPartitionKeys())
        {
            var testMessages = homogeneousPartition.ReadMessage(key, 0, 1);
            foreach (string msg in testMessages) Console.WriteLine(msg);
        }

        // Add messages up to operation limit and test for limit edge
        for (int i = 0; i < 10; i++)
        {
            homogeneousPartition.AddMessage(0, $"Extra message {i}");
        }

        Console.WriteLine("Homogeneous partition at operation limit.");

        // Test Initializing heterogeneous PartitionStream and comparing behaviors
        Console.WriteLine("\nTest Heterogeneous Collection - MsgStream and DurableStream Combined");
        var heterogeneousStreams = new List<MsgStream> { new MsgStream(37), new DurableStream(11, "durablestream.txt") };
        var heterogeneousPartition = new PartitionStream(heterogeneousStreams, 54);
        heterogeneousPartition.AddMessage(0, "Message in MsgStream of heterogeneous collection.");
        heterogeneousPartition.AddMessage(1, "Message in DurableStream of heterogeneous collection.");
        Console.WriteLine("Messages in Heterogeneous PartitionStream:");
        foreach (var key in heterogeneousPartition.GetPartitionKeys())
        {
            var testMessages2 = heterogeneousPartition.ReadMessage(key, 0, 1);
            foreach (string msg in testMessages2) Console.WriteLine(msg);
        }

        // Test Validate Deep Copy independence
        Console.WriteLine("\nTest Deep Copy Independence Check");
        var deepCopyPartition = heterogeneousPartition.DeepCopy();
        deepCopyPartition.AddMessage(0, "New message in deep copy.");
        Console.WriteLine("Original Heterogeneous Partition after Deep Copy Modification:");
        foreach (var key in heterogeneousPartition.GetPartitionKeys())
        {
            var testMessages2 = heterogeneousPartition.ReadMessage(key, 0, 1);
            foreach (string msg in testMessages2) Console.WriteLine(msg);
        }

        // Test Edge cases - Adding an invalid message, reaching max capacity, and reset
        Console.WriteLine("\nTest Edge Cases");
        try
        {
            homogeneousPartition.AddMessage(0, new string('a', 151));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Caught expected exception on long message: {ex.Message}");
        }

        // Test adding beyond capacity
        try
        {
            homogeneousPartition.AddMessage(1, "Message beyond capacity");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Caught expected exception on adding beyond capacity: {ex.Message}");
        }

        // Reset and validate the state
        homogeneousPartition.Reset();
        Console.WriteLine("Homogeneous partition reset successfully.");

        // Test Validate all partitions after reset
        Console.WriteLine("\nTest Validate PartitionStream State Post-Reset");
        Console.WriteLine($"Homogeneous Partition Count After Reset: {homogeneousPartition.GetPartitionCount()}");

        // Test Boundary cases on reading with invalid ranges and partition keys
        Console.WriteLine("\nTest Reading Boundary Cases");
        try
        {
            homogeneousPartition.ReadMessage(999, 0, 1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Caught expected exception on invalid partition key: {ex.Message}");
        }
        try
        {
            heterogeneousPartition.ReadMessage(0, 5, 10);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Caught expected exception on out-of-bounds read range: {ex.Message}");
        }
    }
}