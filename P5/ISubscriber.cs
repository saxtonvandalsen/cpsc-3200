// Saxton Van Dalsen
// 12/3/2024


namespace ISubscriberInterface
{
    //   Interface contract:
    // - Implementing classes must ensure that the NewMessage method is designed to handle valid, non-null,
    //   and non-empty strings as input.
    // - The behavior of "NewMessage" must align with the subscriber's intended purpose.
    // - Clients of this interface should validate messages before passing them to NewMessage 
    //   to avoid unintended behavior in implementing classes.
    // - ViewMessages method must provide a mechanism for displaying or processing all accumulated messages
    //   in a clear and consistent manner. Messages should be cleared from internal storage after being viewed
    //   unless explicitly specified otherwise.
    // - NewMessageAlert method must notify the subscriber of a new message in a non-intrusive manner.
    //   Implementations ensure the alert does not include message content but serves as a general notification
    public interface ISubscriber
    {
        void NewMessage(string message);
        public void ViewMessages();
        public void NewMessageAlert();
    }
}