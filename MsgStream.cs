// Saxton Van Dalsen
// Date: 10/03/2024

using System;

namespace MsgStreamApp {
    // Class invariant:
    // 

    public class MsgStream {
        private string[] messages;
        private const int MAX_CAPACITY = 200;
        private const int MAX_OPERATIONS = 50;
        private const int MAX_STRING_LENGTH = 150;
        private int capacity;
        private int operationCount;
        private int messageCount;

        private bool IsFull() {
            return messageCount >= capacity;
        }

        private bool OperationLimit() {
            return operationCount >= MAX_OPERATIONS;
        }

        private bool IsValidMessage(string message) {
        return message != null && message.Trim() != "" && message.Length <= MAX_STRING_LENGTH;
        }

        private bool ValidateCapacity(int capacity) {
            if (capacity > MAX_CAPACITY || capacity <= 0) {
                throw new ArgumentOutOfRangeException("Capacity must be between 1 and " + MAX_CAPACITY_LIMIT);
            }
        }

        public MsgStream(int capacity) {
            ValidateCapacity(capacity);
            messages = new string[capacity];
            this.capacity = capacity;
        }

        // Precondition:
        // Postcondition:
        public void AppendMessage(string message) {
            if (IsFull()) throw new InvalidOperationException("Message stream is full");
            
            if (OperationLimit()) throw new InvalidOperationException("Operation limit has been reached");
            
            if (!IsValidMessage(message)) {
                throw new ArgumentException("Invalid message: message is either null, empty, or exceeds the maximum allowed length.");
            }

            messages[messageCount++] = message;
            operationCount++;
        }

        // Precondition:
        // Postcondition:
        public string[] ReadMessages(int startRange, int endRange) {
            if (OperationLimit()) return null;

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

        public static void Main(string[] args) {

            MsgStream msgStream = new MsgStream();
            int input;

            while (input != 4) {
                Console.WriteLine("Enter 1 to create a message to append.");
                Console.WriteLine("Enter 2 to read a range of messages.");
                Console.WriteLine("Enter 3 to reset object state.");
                Console.WriteLine("Enter 4 to exit the program.");
                input = Console.ReadLine();
                switch (input) {
                    case 1:

                        break;
                    case 2:

                        break;
                    case 3:

                        break;
                    case 4:

                        break;
                    default:
                    Console.WriteLine("Invalid option. Please enter a valid number (1-4).");
                    break;
                }
            }
        }
    }

    // Implementation invariant:
    // 
}