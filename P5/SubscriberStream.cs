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

        public SubscriberStream (IEnumerable<MsgStreams> streams, List<ISubscriber> subscribers, int capacity)
            : base(streams, capacity)
        {
            if (subscribers == null) throw new ArgumentNullException(nameof(subscribers));

            messageCount = 0;
            ValidateCapacity(capacity);
            this.capacity = capacity;
            this.subscribers = subscribers ?? new List<ISubscriber>();
        }

        public void AddNewMessage(int key, string message)
        {
            if (!partitionStream.GetPartitionKeys().Contains(partitionKey))
                throw new ArgumentException("Invalid partition key.");

            if (!IsValidMessage(message))
                throw new ArgumentException("Invalid message.");
            
            partitionStream.addMessage(key, message);
            messageCount++;

            foreach (var sub in subscribers)
            {
                sub.NewMessage(message);
            }
        }

        public void AddSubscriber(ISubscriber newSub)
        {
            if (newSub == null) throw new ArgumentNullException(nameof(newSub));

            subscribers.Add(newSub);
        }

        public void RemoveSubscriber(ISubscriber removeSub)
        {
            if (removeSub == null) throw new ArgumentNullException(nameof(removeSub));

            subscribers.Remove(removeSub);
        }
    }
}