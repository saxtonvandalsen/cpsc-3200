namespace PartitionTest;

using P3;

[TestClass]
public class PartitionStreamTest
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_EmptyStreams_ThrowsArgumentException()
    {
        new PartitionStream(null, 10);
    }

    [TestMethod]
    public void Constructor_ValidInputs_CreatesPartitionStream()
    {
        var streams = new List<MsgStream>
        {
            new MsgStream(5),
            new MsgStream(5)
        };
        var partitionStream = new PartitionStream(streams, 2);
        Assert.AreEqual(2, partitionStream.GetPartitionCount());
        Assert.AreEqual(2, partitionStream.GetCapacity());
    }

    [TestMethod]
    public void Constructor_CapacityExceedsMax_ResetsToMax()
    {
        var streams = new List<MsgStream>
        {
            new MsgStream(5),
            new MsgStream(5)
        };
        var partitionStream = new PartitionStream(streams, 300); // Exceeds max capacity
        Assert.AreEqual(200, partitionStream.GetCapacity());
    }

    [TestMethod]
    public void Constructor_NegativeCapacityCheck()
    {
        var streams = new List<MsgStream> 
        { 
            new MsgStream(5) 
        };
        new PartitionStream(streams, -5); // Negative capacity
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddMessage_InvalidPartitionKey_ThrowsArgumentException()
    {
        var partitionStream = new PartitionStream(new List<MsgStream> { new MsgStream(1) }, 2);
        partitionStream.AddMessage(1, "Test message"); // Invalid key
    }

    [TestMethod]
    public void AddMessage_ValidMessage_AddsToMsgStream()
    {
        var msgStream = new MsgStream(1);
        var partitionStream = new PartitionStream(new List<MsgStream> { msgStream }, 2);
        partitionStream.AddMessage(0, "Test message");
        var messages = msgStream.ReadMessages(0, 1);
        Assert.AreEqual("Test message", messages[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void AddMessage_OperationLimitReached_ThrowsInvalidOperationException()
    {
        var msgStream = new MsgStream(1);
        var partitionStream = new PartitionStream(new List<MsgStream> { msgStream }, 1);
        partitionStream.AddMessage(0, "Message 1");
        partitionStream.AddMessage(0, "Message 2"); // Should throw
    }

    [TestMethod]
    public void ReadMessage_ValidRange_ReturnsMessages()
    {
        var msgStream = new MsgStream(2);
        msgStream.AppendMessage("Message 1");
        msgStream.AppendMessage("Message 2");
        var partitionStream = new PartitionStream(new List<MsgStream> { msgStream }, 2);

        var messages = partitionStream.ReadMessage(0, 0, 2);
        Assert.AreEqual(2, messages.Length);
        Assert.AreEqual("Message 1", messages[0]);
        Assert.AreEqual("Message 2", messages[1]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ReadMessage_InvalidPartitionKey_ThrowsArgumentException()
    {
        var partitionStream = new PartitionStream(new List<MsgStream> { new MsgStream(1) }, 2);
        partitionStream.ReadMessage(1, 0, 1); // Invalid partition key
    }

    [TestMethod]
    public void Reset_ClearsMessages()
    {
        var msgStream = new MsgStream(1);
        msgStream.AppendMessage("Message 1");
        var partitionStream = new PartitionStream(new List<MsgStream> { msgStream }, 2);
        partitionStream.Reset();
        Assert.AreEqual(0, partitionStream.GetPartitionCount());
    }

    [TestMethod]
    public void GetPartitionKeys_ReturnsKeys()
    {
        var streams = new List<MsgStream> 
        { 
            new MsgStream(5), 
            new MsgStream(5) 
        };
        var partitionStream = new PartitionStream(streams, 2);
        var keys = partitionStream.GetPartitionKeys().ToList();
        Assert.AreEqual(2, keys.Count);
        Assert.AreEqual(0, keys[0]);
        Assert.AreEqual(1, keys[1]);
    }
    
    [TestMethod]
    public void DeepCopy_ValidCheck_CreatesIndependentCopy()
    {
        var streams = new List<MsgStream>
        {
            new MsgStream(3),
            new MsgStream(3)
        };
        var originalPartitionStream = new PartitionStream(streams, 3);
        
        var copiedPartitionStream = originalPartitionStream.DeepCopy();
        
        originalPartitionStream.AddMessage(0, "Original Message");
        
        var originalMessageCount = originalPartitionStream.ReadMessage(0, 0, 1).Length;
        Assert.AreEqual(1, originalMessageCount);
    }

    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DeepCopy_InvalidCheck_ThrowsArgumentExceptionOnInvalidStream()
    {
        var streams = new List<MsgStream>
        {
            new MsgStream(3),
            new MsgStream(3)
        };
        var partitionStream = new PartitionStream(streams, 3);
        
        var copiedPartitionStream = partitionStream.DeepCopy();
        
        copiedPartitionStream.AddMessage(5, "This should throw"); 
    }
}