// Saxton Van Dalsen
// 12/3/2024


namespace ISubscriberInterface
{
    // Interface contract:
    // - Implementing classes must ensure that the "NewMessage" method is designed to handle valid, non-null,
    //   and non-empty strings as input
    // - The behavior of "NewMessage" must align with the subscriber's intended purpose, such as 
    //   logging, processing, or displaying messages
    // - Clients of this interface should validate messages before passing them to "NewMessage" 
    //   to avoid unintended behavior in implementing classes
    public interface ISubscriber
    {
        void NewMessage(string message);
    }
}