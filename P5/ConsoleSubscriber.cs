// Saxton Van Dalsen
// 12/3/2024

using System;
using ISubscriberInterface;

namespace ConsoleSubscriberLibrary
{
    public class ConsoleSubscriber : ISubscriber
    {
        public void NewMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}