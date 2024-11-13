namespace MsgStreamTest;

using P3;

[TestClass]
public class MsgStreamTest
{
    [TestMethod]
    public void Constructor_ValidCapacity_InitializesCorrectly()
    {
        MsgStream stream = new MsgStream(68);
        Assert.AreEqual(0, stream.GetMessageCount());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Constructor_InvalidCapacity_ThrowsException()
    {
        new MsgStream(0);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AppendMessage_InvalidMessage_ThrowsException()
    {
        MsgStream stream = new MsgStream(35);
        stream.AppendMessage(null);
    }

    [TestMethod]
    public void AppendMessage_ValidMessage_AppendsSuccessfully()
    {
        MsgStream stream = new MsgStream(100);
        stream.AppendMessage("Hello World");
        Assert.AreEqual(1, stream.GetMessageCount());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void AppendMessage_FullStream_ThrowsException()
    {
        MsgStream stream = new MsgStream(2);
        stream.AppendMessage("Message 1");
        stream.AppendMessage("Message 2");
        stream.AppendMessage("Message 3");
    }

    [TestMethod]
    public void ReadMessages_ValidRange_ReturnsExpectedMessages()
    {
        MsgStream stream = new MsgStream(11);
        stream.AppendMessage("Message 1");
        stream.AppendMessage("Message 2");
        stream.AppendMessage("Message 3");

        string[] messages = stream.ReadMessages(0, 2);
        Assert.AreEqual(2, messages.Length);
        Assert.AreEqual("Message 1", messages[0]);
        Assert.AreEqual("Message 2", messages[1]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ReadMessages_InvalidRange_ThrowsException()
    {
        MsgStream stream = new MsgStream(5);
        stream.AppendMessage("Message 1");
        stream.ReadMessages(0, 5); // Out of range
    }

    [TestMethod]
    public void DeepCopy_ReturnsEqualButDistinctCopy()
    {
        MsgStream stream = new MsgStream(4);
        stream.AppendMessage("Original Message");

        MsgStream copy = stream.DeepCopy();
        Assert.AreEqual(stream.GetMessageCount(), copy.GetMessageCount());

        copy.AppendMessage("Another Message");
        Assert.AreNotEqual(stream.GetMessageCount(), copy.GetMessageCount());
    }

    [TestMethod]
    public void Reset_ClearsMessagesAndResetsCounts()
    {
        MsgStream stream = new MsgStream(5);
        stream.AppendMessage("Message 1");
        stream.Reset();

        Assert.AreEqual(0, stream.GetMessageCount());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void AppendMessage_ExceedMaxOperations_ThrowsException()
    {
        MsgStream stream = new MsgStream(2);
        for (int i = 0; i < 5; i++)
        {
            stream.AppendMessage("Message " + i);
        }
    }
}