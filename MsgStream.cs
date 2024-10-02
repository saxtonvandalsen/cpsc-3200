// Saxton Van Dalsen
// Date: 10/03/2024

using System;

namespace MsgStreamApp {
    // Class invariant:
    // 

    public class MsgStream {
        private string[] messages;
        private int capacity;
        private int operationCount;
        private int maxOperations;
        private int messageCount;

        public MsgStream(int capacity, int maxOperations) {
            messages = new string[capacity];
            this.capacity = capacity;
            this.maxOperations = maxOperations;
        }

        // Precondition:
        // Postcondition:
        public void AppendMessage(string message) {
            if (messageCount >= capacity) {
                throw new InvalidOperationException("Message stream is full")
            }

            if (message == null || message.Trim() == "") {
                throw new ArgumentException("Message cannot be null or whitespace")
            }

            if (operationCount >= maxOperations) {
                throw new InvalidOperationException("Operation limit has been reached")
            }

            messages[messageCount++] = message;
            operationCount++;
        }

        // Precondition:
        // Postcondition:
        public string[] ReadMessages(int startRange, int endRange) {
            if (operationCount >= maxOperations) {
                return null;
            }

            if (startRange < 0 || endRange <= startRange || startRange >= messageCount || endRange > messageCount) {
                return null;
            }

            int rangeSize = endRange - startRange;
            string[] res = new string[rangeSize];

            for (int i = 0; i < rangeSize; i++) {
                res[i] = messages[startRange + i];
            }

            operationCount++;

            return res;
        }

        public void Reset() {
            messages = new string[capacity];
            messageCount = 0;
            operationCount = 0;
        }

        private bool IsFull() {
            return 
        }

        public static void Main(string[] args) {

        }
    }

    // Implementation invariant:
    // 
}