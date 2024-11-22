// Saxton Van Dalsen
// 12/3/2024

using system;

namespace MsgStreamLibrary
{
    // Class invariant:
    // - The "messages" array may only contain valid, non-null, and non-empty strings, each adhering to the maximum length defined by MAX_STRING_LENGTH.
    // - The total number of operations performed on the MsgStream instance must not exceed the limit established by MAX_OPERATIONS, which is set as a multiple of the object's capacity.
    // - The number of messages appended to the array cannot exceed the fixed capacity of the message stream, which must be between 1 and MAX_CAPACITY, as determined at initialization.
    // - Once established, the capacity remains unchanged throughout the lifetime of the object unless reset by the client.
    // - Clients must be prepared to handle exceptions, particularly those related to invalid message length and capacity limits, to ensure robust error handling.

    public class MsgStream
    {
        private string[] messages;
        private const int MAX_CAPACITY = 200;
        private const int MAX_STRING_LENGTH = 150;
        private int capacity;
        private int operationCount;
        private int messageCount;

        private int MaxOperations => capacity * 2;

        private bool IsFull()
        {
            return messageCount >= capacity;
        }

        private bool OperationLimit()
        {
            return operationCount >= MaxOperations;
        }

        private bool IsValidMessage(string message)
        {
        return message != null && message.Trim() != "" && message.Length <= MAX_STRING_LENGTH;
        }

        private bool ValidateCapacity(int capacity)
        {
            if (capacity > MAX_CAPACITY)
            {
                throw new ArgumentException($"Capacity exceeded 200, setting capacity to {MAX_CAPACITY}.");
                capacity = MAX_CAPACITY;
            }
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException("Capacity must be between 1 and " + MAX_CAPACITY);
            }
            return true;
        }

        // Precondition:
        // - Capacity must be between 1 and MAX_CAPACITY.
        // Postcondition:
        // - Capacity is initialized and the "messages" array is created.
        public MsgStream(int capacity)
        {
            ValidateCapacity(capacity);
            messages = new string[capacity];
            this.capacity = capacity;
        }

        // Precondition:
        // - Message stream must not be full.
        // - Operation count must not exceed MAX_OPERATIONS.
        // - The message must be non-null, non-empty, and within the MAX_STRING_LENGTH.
        // Postcondition:
        // - Message is appended to the stream; the message and operation counts are updated.
        public void AppendMessage(string message)
        {
            if (IsFull()) throw new InvalidOperationException("Message stream is full");
            
            if (OperationLimit()) throw new InvalidOperationException("Operation limit has been reached");
            
            if (!IsValidMessage(message))
            {
                throw new ArgumentException("Invalid message: message is either null, empty, or exceeds the maximum allowed length.");
            }

            messages[messageCount++] = message;
            operationCount++;
        }

        // Precondition:
        // - Operation count must not exceed MAX_OPERATIONS.
        // - The start and end ranges must be acceptable within the bounds of the message array.
        // Postcondition:
        // - Returns the messages between the specified start and end range.
        public string[] ReadMessages(int startRange, int endRange)
        {
            if (OperationLimit()) throw new InvalidOperationException("Operation limit reached");

            if (startRange < 0 || endRange <= startRange || startRange >= messageCount || endRange > messageCount)
            {
                throw new ArgumentOutOfRangeException("Invalid range for reading messages");
            }

            int rangeSize = endRange - startRange;
            string[] res = new string[rangeSize];

            for (int i = 0; i < rangeSize; i++)
            {
                res[i] = messages[startRange + i];
            }

            operationCount++;

            return res;
        }

        // Preconditions:
        // - The current instance must have valid messages stored
        // Postconditions:
        // - A deep copy of the MsgStream instance is created, with the same messages, 
        // message count, and operation count
        public virtual MsgStream DeepCopy()
        {
            MsgStream copy = new MsgStream(this.capacity);

            for (int i = 0; i < this.messageCount; i++)
            {
                    copy.messages [i] = this.messages[i];
            }

            copy.messageCount = this.messageCount;
            copy.operationCount = this.operationCount;

            return copy;
        }

        // Preconditions:
        // - The stream must be initialized and able to reset its state
        // Postconditions:
        // - The message count and operation count are reset to zero, 
        // and the messages array is re-initialized
        public void Reset()
        {
            messages = new string[capacity];
            messageCount = 0;
            operationCount = 0;
        }

        public int GetMessageCount()
        {
            return messageCount;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Please enter the capacity for the message stream: ");

            int capacity;
            while (!int.TryParse(Console.ReadLine(), out capacity) || capacity <= 0 || capacity > MAX_CAPACITY)
            {
                Console.WriteLine($"Invalid input. Please enter a reasonable capacity between 1 and {MAX_CAPACITY}:");
            }

            MsgStream msgStream = new MsgStream(capacity);
            int input;

            do {
                Console.WriteLine("\nMessage Stream Menu:");
                Console.WriteLine("1. Create a message to append");
                Console.WriteLine("2. Read a range of messages");
                Console.WriteLine("3. Reset object state");
                Console.WriteLine("4. Exit the program");
                Console.Write("Enter your choice: ");

                while (!int.TryParse(Console.ReadLine(), out input) || input < 1 || input > 4)
                {
                    Console.WriteLine("Invalid input. Please enter a valid option (1-4):");
                }

                switch (input) {
                    case 1:
                        Console.WriteLine("Enter a message: ");
                        string message = Console.ReadLine();
                        try {
                            msgStream.AppendMessage(message);
                            Console.WriteLine("Message appended successfully.");
                        } 
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case 2:
                        try {
                            Console.WriteLine("Enter the start range: ");
                            int startRange = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter the end range: ");
                            int endRange = int.Parse(Console.ReadLine());
                            string[] messages = msgStream.ReadMessages(startRange, endRange);
                            Console.WriteLine("Messages: ");
                            foreach (string msg in messages)
                            {
                                Console.WriteLine(msg);
                            }
                        } 
                        catch (ArgumentOutOfRangeException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case 3:
                        msgStream.Reset();
                        Console.WriteLine("Message stream has been reset.");
                        break;
                    
                    case 4:
                        Console.WriteLine("Exiting the program...");
                        break;
                }
            } while (input != 4);
        }
    }
    // Implementation invariant:
    // - The MsgStream class requires that all subclasses adhere to the defined 
    //   class and implementation invariants
    // - Any subclass must ensure that the integrity of the messages array is maintained, that valid message 
    //   limits are enforced, and that the operation limits are respected during 
    //   the lifespan of the instance
    // - Subclasses may introduce additional preconditions and postconditions as necessary to support their extended 
    //   functionality
    // - Clients should refer to subclass documentation for more details
}