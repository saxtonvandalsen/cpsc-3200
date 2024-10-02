// Saxton Van Dalsen
// Date: 10/03/2024

using System;

namespace MsgStreamApp {
    // Class invariant:
    // 

    public class MsgStream {
        private string[] messages;
        private int maxCapacity;
        private int operationCount;
        private int messageCount;
        private int maxOperations;

        public MsgStream(int capacity) {
            messages = new string[capacity];
            maxCapacity = capacity;
        }

        public bool AppendMessage(string message) {
            if (messageCount >= maxCapacity) {
                Console.WriteLine("Capacity is full. Message was not added.");
                return false;
            }

            if (message == null || message.Trim() == "") {
                Console.WriteLine("Invalid message. Message will not be added.")
                return false;
            }

            messages[messageCount] = message;

            messageCount++;
            operationCount++;

            return true;
        }

        public string[] ReadMessages(int startRange, int endRange) {
            if (operationCount >= maxOperations) {
                return null;
            }

            if (startRange < 0 || endRange < 1 || startRange >= messageCount || (startRange + endRange) > messageCount) {
                return null;
            }

            string[] res = new string[count];

            for (int i = 0; i < endRange; i++) {

            }

            operationCount++;

            return res;
        }

        public void Reset() {
            messages = new string[maxCapacity];
            messageCount = 0;
            operationCount = 0;
        }

        private bool IsFull() {

        }

        public static void Main(string[] args) {

        }
    }

    // Implementation invariant:
    // 
}