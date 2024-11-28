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
        private List<ISubsrciber> subscribers;
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

        public SubscriberStream (PartitionStream partitionStream, List<ISubscriber> subscribers, int capacity)
        {
            if (partitionStream == null) throw new ArgumentNullException(nameof(partitionStream));
            if (subscribers == null) throw new ArgumentNullException(nameof(subscribers));

            messageCount = 0;
            ValidateCapacity(capacity);
            this.capacity = capacity;
            this.partitionStream = partitionStream;
            this.subscribers = subscribers ?? new List<ISubscriber>();
        }

        public void addMessage(int key, string message)
        {
            if (!partitionStream.GetPartitionKeys().Contains(partitionKey))
                throw new ArgumentException("Invalid partition key.");

            if (!partitionStream.IsValidMessage(message))
                throw new ArgumentException("Invalid message.");
            
            partitionStream.addMessage(key, message);
            messageCount++;

            foreach (var sub in subscribers)
            {
                subscribers.NewMessage(message);
            }
        }

        public void addSubscriber(ISubscriber newSub)
        {
            if (newSub == null) throw new ArgumentNullException(nameof(newSub));

            subscribers.Add(newSub);
        }

        public void removeSubscriber(ISubscriber removeSub)
        {
            if (removeSub == null) throw new ArgumentNullException(nameof(removeSub));

            subscribers.Remove(removeSub);
        }
    }
}