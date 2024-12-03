// Saxton Van Dalsen
// 12/3/2024

using System;
using ISubscriberInterface;

namespace ConsoleSubscriberLibrary
{
    // Class invariant:
    // - The messageQueue must always be initialized and non-null.
    // - The messageQueue must contain valid messages (non-null, non-empty strings) that have been received through NewMessage.
    // - The class must adhere to the ISubscriber interface contract, implementing all methods defined in the interface.
    // - NewMessageAlert behavior must align with notifying about new messages without exposing message details.
    public class ConsoleSubscriber : ISubscriber
    {
        private readonly Queue<string> messageQueue = new();
        
        // Preconditions:
        // - message parameter must not be null or an empty string.
        // Postconditions:
        // - message is added to the messageQueue.
        public void NewMessage(string message)
        {
            messageQueue.Enqueue(message);
        }

        // Preconditions:
        // - None
        // Postconditions:
        // - If messageQueue is not empty, all messages are displayed to the client and removed from the queue.
        // - If messageQueue is empty, a notification ("No messages received.") is displayed to client.
        public void ViewMessages()
        {
            if (messageQueue.Count == 0)
            {
                Console.WriteLine("No messages received.");
                return;
            }

            Console.WriteLine("Viewing new messages:");
            while (messageQueue.Count > 0)
            {
                Console.WriteLine(messageQueue.Dequeue());
            }
        }

        // Preconditions:
        // - None
        // Postconditions:
        // - A notification ("New message received.") is displayed to the client.
        public void NewMessageAlert()
        {
            Console.WriteLine("New message received.");
        }
    }
    // Class invariant:
    // - The messageQueue must always be initialized and non-null.
    // - The messageQueue must contain valid messages (non-null, non-empty strings) that have been received through NewMessage.
    // - The class must adhere to the ISubscriber interface contract, implementing all methods defined in the interface.
    // - NewMessageAlert behavior must align with notifying about new messages without exposing message details.
}