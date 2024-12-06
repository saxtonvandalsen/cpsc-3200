// Saxton Van Dalsen
// 12/3/2024

using System;
using PartitionStreamLibrary;
using MsgStreamLibrary;
using DurableStreamLibrary;

class program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Testing interface implementations with new class:");
        
        // Creating some MsgStream instances to use with PartitionStream and SubscriberStream
        // for heterogeneous collections
        MsgStream msgStream15 = new MsgStream(50);
        MsgStream msgStream16 = new MsgStream(50);
        MsgStream msgStream17 = new MsgStream(50);

        // List of MsgStream objects
        List<MsgStream> streams = new List<MsgStream> { msgStream15, msgStream16, msgStream17 };

        // ConsoleSubscriber instances
        ConsoleSubscriber sub1 = new ConsoleSubscriber();
        ConsoleSubscriber sub2 = new ConsoleSubscriber();

        // Add subscribers to a list
        List<ISubscriber> subscribers = new List<ISubscriber> { sub1, sub2 };

        // Initialize SubscriberStream with streams and subscribers
        SubscriberStream subscriberStream = new SubscriberStream(streams, subscribers, 100);
        
        // Adding new messages to different partitions for testing
        Console.WriteLine("Adding messages to partitions:");
        subscriberStream.AddNewMessage(0, "Message 1 for Partition 0");
        subscriberStream.AddNewMessage(1, "Message 2 for Partition 1");
        subscriberStream.AddNewMessage(2, "Message 3 for Partition 2");
        
        // Testing of viewing messages for subscribers
        Console.WriteLine("Subscriber 1 messages: ");
        sub1.ViewMessages();
        Console.WriteLine("Subscriber 2 messages: ");
        sub2.ViewMessages();
        
        // Testing GetSubscribers function
        Console.WriteLine("Number of current subscribers: " + subscriberStream.GetSubscribers());

        // Test adding a null message (should throw an exception)
        try
        {
            subscriberStream.AddNewMessage(0, "");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception caught as expected: {ex.Message}");
        }

        // Test adding a message to an invalid partition key
        try
        {
            subscriberStream.AddNewMessage(5, "Invalid partition key test");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception caught as expected: {ex.Message}");
        }

        // Add a new subscriber dynamically and send a new message
        ConsoleSubscriber sub3 = new ConsoleSubscriber();
        subscriberStream.AddSubscriber(sub3);
        subscriberStream.AddNewMessage(0, "Message for new subscriber");

        // Remove a subscriber and verify no longer notified
        subscriberStream.RemoveSubscriber(sub1);
        subscriberStream.AddNewMessage(0, "Message after removing a subscriber");
        
        // File for testing DurableStream out
        string filePath = "messages.txt";

        // Ensure the file doesn't exist before starting the test
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        Console.WriteLine("\nTesting out DurableStream's IDisposable functionality:");
        
        try
        {
            using (DurableStream durableStream = new DurableStream(10, filePath))
            {
                durableStream.AppendMessage("First message");
                durableStream.AppendMessage("Second message");
                durableStream.AppendMessage("Third message");
                durableStream.AppendMessage("Fourth message");

                // Read messages from the DurableStream
                var checkMessages = durableStream.ReadMessages(0, durableStream.GetMessageCount());
                Console.WriteLine("Messages in DurableStream:");
                foreach (var msg in checkMessages)
                {
                    Console.WriteLine(msg);
                }
            } // Dispose() is automatically called here

            // Attempt to use the durableStream after it has been disposed
            try
            {
                // This should throw an exception
                using (DurableStream durableStream = new DurableStream(10, filePath))
                {
                    durableStream.Dispose(); // Explicitly dispose
                    durableStream.AppendMessage("This should fail");
                }
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("Caught expected ObjectDisposedException: " + ex.Message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }

        // Clean up the test file
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        
        Console.WriteLine("\nTesting out PartitionStream's IDisposable functionality:");
        
        var msgStream9 = new MsgStream(10);
        var msgStream10 = new MsgStream(10);
        var msgStreams = new List<MsgStream> { msgStream9, msgStream10 };

        try
        {
            using (var partitionStream = new PartitionStream(msgStreams, 10))
            {
                Console.WriteLine("PartitionStream initialized.");
                Console.WriteLine($"Partition Count: {partitionStream.GetPartitionCount()}");
                Console.WriteLine($"Capacity: {partitionStream.GetCapacity()}");

                // Adding messages
                partitionStream.AddMessage(0, "First message in Partition 0");
                partitionStream.AddMessage(1, "First message in Partition 1");

                Console.WriteLine("Messages added to partitions.");

                // Read messages from Partition 0
                var testingMessages = partitionStream.ReadMessage(0, 0, 1);
                Console.WriteLine("Messages in Partition 0:");
                foreach (var msg in testingMessages)
                {
                    Console.WriteLine(msg);
                }
            } // Dispose() is automatically called here

            // Test using PartitionStream after disposal
            try
            {
                using (var partitionStream = new PartitionStream(msgStreams, 10))
                {
                    partitionStream.Dispose(); // Explicitly dispose
                    partitionStream.AddMessage(0, "This should fail");
                }
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("Caught expected ObjectDisposedException: " + ex.Message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
     
        Console.WriteLine("\nTesting MsgStream:");
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