// Saxton Van Dalsen
// 12/3/2024

using System;
using PartitionStreamLibrary;
using ISubscriberInterface;
using Systems.Collections.Generic;
using System.Linq;

namespace SubscriberStreamLibrary
{
    public class SubscriberStream
    {
        // Class invariant:
        // - partitionStream must be a valid instance and must contain a set of partition keys 
        //   that are used for adding messages
        // - subscribers list must never be null. It should always be initialized, even if empty
        // - The capacity must remain within the range of 1 to 200
        // - messageCount must accurately reflect the total number of messages successfully added to the stream
        // - Every message added to the stream must be valid (non-null, non-empty, and trimmed)
        // - All subscribers in the "subscribers" list must implement the ISubscriber interface
        // - Clients must handle exceptions related to capacity, invalid messages, and null subscribers to ensure robust error handling

        private PartitionStream partitionStream;
        private List<ISubscriber> subscribers;
        private int capacity;
        private int messageCount;

        private bool ValidateCapacity(int capacity)
        {
            if (capacity > 200)
            {
                capacity = 200;
                throw new ArgumentException("Capacity exceeded 200, setting capacity to max capacity of 200.");
            }
            if (capacity <= 0)
            {
                capacity = 1;
                throw new ArgumentException("Capacity can not be negative or 0, setting to minimum capacity of 1.");
            }
            return true;
        }

        private bool IsValidMessage(string message)
        {
            return message != null && message.Trim() != "";
        }

        public SubscriberStream (IEnumerable<MsgStream> streams, List<ISubscriber> subscribers, int capacity)
        {
            if (subscribers == null) throw new ArgumentNullException(nameof(subscribers));

            ValidateCapacity(capacity);
            this.capacity = capacity;
            this.subscribers = subscribers ?? new List<ISubscriber>();
            this.partitionStream = new PartitionStream(streams, capacity);
            messageCount = 0;
        }

        public void AddNewMessage(int key, string message)
        {
            if (!partitionStream.GetPartitionKeys().Contains(partitionKey))
                throw new ArgumentException("Invalid partition key.");

            if (!IsValidMessage(message))
                throw new ArgumentException("Invalid message.");
            
            partitionStream.AddMessage(key, message);

            if (subscribers.Count > 0)
                subscribers[0].NewMessageAlert();

            foreach (var sub in subscribers)
            {
                sub.NewMessage(message);
            }

            messageCount++;
        }

        public void AddSubscriber(ISubscriber newSub)
        {
            if (newSub == null) throw new ArgumentNullException(nameof(newSub));

            if (!subscribers.Contains(newSub))
            {
                subscribers.Add(newSub);
            }
        }

        public void RemoveSubscriber(ISubscriber removeSub)
        {
            if (removeSub == null) throw new ArgumentNullException(nameof(removeSub));

            subscribers.Remove(removeSub);
        }

        public int GetSubscribers()
        {
            return subscribers.Count;
        }

        // Implementation invariant:
        // - The SubscriberStream class relies on the PartitionStream to manage partitioned messages 
        //   and assumes it maintains its own invariants, such as valid keys and message integrity
        // - All interactions with the "partitionStream" must validate partition keys and ensure only valid 
        //   messages are appended to the correct partitions
        // - Adding messages triggers notifications to all subscribers, and the "subscribers" list 
        //   must only contain valid instances of the ISubscriber interface
        // - Modifications to the "subscribers" list (additions or removals) must maintain its integrity 
        //   and prevent duplicates or null values
        // - Capacity validation must occur only at initialization to ensure consistent behavior throughout 
        //   the lifetime of the instance
        // - Subclasses of SubscriberStream must enforce the described class and implementation invariants 
        //   and may extend them with additional preconditions and postconditions as necessary
    }
}