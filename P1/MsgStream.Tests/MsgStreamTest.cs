namespace MsgStreamTest.cs;

using MsgStream;

[TestClass]
public class MsgStreamTests
{
    [TestMethod]
    public void TestConstructorCapacity()
    {
        int capacity = 0;

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new MsgStream(capacity));
    }

    [TestMethod]
    public void TestConstructorNegativeCapacity()
    {
        int capacity = -1;

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new MsgStream(capacity));
    }

    [TestMethod]
    public void TestConstructorValidCapacity()
    {
        int capacity = 10;

        MsgStream msgStream = new MsgStream(capacity);
        Assert.IsNotNull(msgStream);
    }

    [TestMethod]
    public void TestConstructorExceedCapacity()
    {
        int capacity = 201;

        Assert.ThrowsException<ArgumentException>(() => new MsgStream(capacity));
    }

    [TestMethod]
    public void TestValidAppendMessage()
    {
        MsgStream msgStream = new MsgStream(7);
        string message = "Hello, World!";

        msgStream.AppendMessage(message);

        Assert.AreEqual(1, msgStream.GetMessageCount());
    }

    [TestMethod]
    public void TestAppendMessage_WithFullStream()
    {
        MsgStream msgStream = new MsgStream(1);
        msgStream.AppendMessage("Message Test 1");

        Assert.ThrowsException<InvalidOperationException>(() => msgStream.AppendMessage("Message Test 2"));
    }

    [TestMethod]
    public void TestAppendMessage_WithNullMessage()
    {
        MsgStream msgStream = new MsgStream(20);
        string invalidMessage = null;

        Assert.ThrowsException<ArgumentException>(() => msgStream.AppendMessage(invalidMessage));
    }

    [TestMethod]
    public void TestReadMessages_OperationLimit()
    {
        MsgStream msgStream = new MsgStream(1);
        msgStream.AppendMessage("Message 1");

        msgStream.ReadMessages(0, 1);
        Assert.ThrowsException<InvalidOperationException>(() => msgStream.ReadMessages(0, 1));
    }

    [TestMethod]
    public void TestReadMessagesValidRange()
    {
        MsgStream msgStream = new MsgStream(5);
        msgStream.AppendMessage("Message 1");
        msgStream.AppendMessage("Message 2");

        string[] messages = msgStream.ReadMessages(0, 2);

        Assert.AreEqual(2, messages.Length);
        Assert.AreEqual("Message 1", messages[0]);
        Assert.AreEqual("Message 2", messages[1]);
    }

    [TestMethod]
    public void TestReadMessagesInvalidRange()
    {
        MsgStream msgStream = new MsgStream(5);
        msgStream.AppendMessage("Message -1 Test");

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => msgStream.ReadMessages(-1, 1));
    }

    [TestMethod]
    public void TestReset()
    {
        MsgStream msgStream = new MsgStream(9);
        msgStream.AppendMessage("Message Test");
        msgStream.ReadMessages(0, 1);

        msgStream.Reset();

        Assert.AreEqual(0, msgStream.GetMessageCount());
    }
}