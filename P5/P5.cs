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
        MsgStream msgStream1 = new MsgStream(50);
        MsgStream msgStream2 = new MsgStream(50);
        MsgStream msgStream3 = new MsgStream(50);

        // List of MsgStream objects
        List<MsgStream> streams = new List<MsgStream> { msgStream1, msgStream2, msgStream3 };

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
    }
}