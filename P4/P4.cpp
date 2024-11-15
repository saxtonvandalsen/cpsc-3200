// Saxton Van Dalsen
// 11/14/2024

#include "PartitionStream.h"
#include "MsgStream.h"
#include "DurableStream.h"

#include <memory>
#include <string>
#include <fstream>
#include <stdexcept>
#include <iostream>

using namespace std;

void testPartitionStreamAllMsgStreams();
void testPartitionStreamAllDurableStreams();
void testPartitionStreamMixedStreams();
void testPartitionStreamSingleStream();
void testPartitionStreamEdgeCases();

int main ()
{
    try {
        cout << "=== Testing PartitionStream with all MsgStreams ===" << endl;
        testPartitionStreamAllMsgStreams();

        cout << "\n=== Testing PartitionStream with all DurableStreams ===" << endl;
        testPartitionStreamAllDurableStreams();

        cout << "\n=== Testing PartitionStream with mixed MsgStreams and DurableStreams ===" << endl;
        testPartitionStreamMixedStreams();

        cout << "\n=== Testing PartitionStream with a single stream ===" << endl;
        testPartitionStreamSingleStream();

        cout << "\n=== Testing Some Additional Edge Cases ===" << endl;
        testPartitionStreamEdgeCases();

    } catch (const exception& e) {
        cerr << "Exception occurred: " << e.what() << endl;
    }

    return 0;
}

// Test PartitionStream with all MsgStreams
void testPartitionStreamAllMsgStreams() {
    cout << "Starting PartitionStream and MsgStream tests...\n";

    // Initialize PartitionStream
    const int initialCapacity = 5;
    std::unique_ptr<MsgStream[]> msgStreams(new MsgStream[initialCapacity]);
    for (int i = 0; i < initialCapacity; i++) {
        msgStreams[i] = MsgStream(10); // Each MsgStream has a capacity of 10
    }
    PartitionStream partitionStream(initialCapacity, std::move(msgStreams));
    cout << "Initialized PartitionStream with capacity: " << partitionStream.getCapacity() << "\n";

    // Test writeMessage functionality
    try {
        partitionStream.writeMessage(1, "Hello from partition 1");
        partitionStream.writeMessage(2, "Message for partition 2");
        partitionStream.writeMessage(3, "Data in partition 3");
        cout << "Write operations successful.\n";
    } catch (const std::exception& e) {
        cout << "Error during write operations: " << e.what() << "\n";
    }

    // Test readMessage functionality
    try {
        auto messages = partitionStream.readMessage(1, 0, 1);
        cout << "Read from partition 1: " << messages[0] << "\n";

        messages = partitionStream.readMessage(2, 0, 1);
        cout << "Read from partition 2: " << messages[0] << "\n";
    } catch (const std::exception& e) {
        cout << "Error during read operations: " << e.what() << "\n";
    }

    // Test edge cases
    try {
        partitionStream.writeMessage(6, "Invalid partition key test"); // Invalid key
    } catch (const std::exception& e) {
        cout << "Caught expected error for invalid partition key: " << e.what() << "\n";
    }

    try {
        partitionStream.readMessage(1, 10, 20); // Invalid read range
    } catch (const std::exception& e) {
        cout << "Caught expected error for invalid read range: " << e.what() << "\n";
    }

    // Test reset functionality
    try {
        -partitionStream; // Reset using operator
        cout << "PartitionStream reset successful.\n";
    } catch (const std::exception& e) {
        cout << "Error during reset: " << e.what() << "\n";
    }

    // Test MsgStream specific operations
    MsgStream msgStream(5);
    msgStream.appendMessage("Message 1");
    msgStream.appendMessage("Message 2");
    cout << "MsgStream contents:\n";
    for (int i = 0; i < msgStream.getMessageCount(); i++) {
        auto messages = msgStream.readMessages(i, i + 1);
        cout << "  " << messages[0] << "\n";
    }

    // Test MsgStream copy constructor
    MsgStream originalStream(5);
    originalStream.appendMessage("Test Message 1");
    MsgStream copiedStream(originalStream);
    cout << "Copy constructor test passed: " << (copiedStream.getMessageCount() == originalStream.getMessageCount()) << endl;

    // Test MsgStream copy assignment operator
    MsgStream assignedStream(5);
    assignedStream = originalStream;
    cout << "Copy assignment operator test passed: " << (assignedStream.getMessageCount() == originalStream.getMessageCount()) << endl;

    // Test MsgStream move constructor
    MsgStream movedStream(std::move(originalStream));
    cout << "Move constructor test passed: " << (movedStream.getMessageCount() == 1) << endl;

    // Test MsgStream move assignment operator
    MsgStream moveAssignedStream(5);
    moveAssignedStream = std::move(copiedStream);
    cout << "Move assignment operator test passed: " << (moveAssignedStream.getMessageCount() == 1) << endl;

    // Test operator!
    MsgStream emptyStream(5);
    cout << "Operator! test (should be true): " << (!emptyStream ? "Passed" : "Failed") << endl;

    // Test operator+
    MsgStream streamA(5);
    streamA.appendMessage("Message 1");
    streamA.appendMessage("Message 2");

    MsgStream streamB(5);
    streamB.appendMessage("Message 3");
    streamB.appendMessage("Message 4");

    MsgStream mergedStream = streamA + streamB;
    cout << "Operator+ test passed: " << (mergedStream.getMessageCount() == 4) << endl;

    // Test operator==
    MsgStream identicalStream(5);
    identicalStream.appendMessage("Message 1");
    identicalStream.appendMessage("Message 2");

    cout << "Operator== test passed: " << (streamA == identicalStream ? "Passed" : "Failed") << endl;

    // Test operator!=
    cout << "Operator!= test passed: " << (streamA != streamB ? "Passed" : "Failed") << endl;

    // Test operator+=
    streamA += streamB;
    cout << "Operator+= test passed: " << (streamA.getMessageCount() == 4) << endl << endl;

    cout << "All PartitionStream and MsgStream tests completed." << endl;
}

// Test PartitionStream with all DurableStreams
void testPartitionStreamAllDurableStreams() {
    string filePath1 = "stream1.txt";
    string filePath2 = "stream2.txt";

    // Test constructor with invalid file path
    try {
        string invalidFilePath = "";
        DurableStream invalidStream(5, invalidFilePath);
    } catch (const invalid_argument& e) {
        cout << "Expected exception (invalid file path): " << e.what() << endl;
    }

    // Create MsgStream instances with DurableStream objects for dependency injection
    std::unique_ptr<MsgStream[]> msgStreams(new MsgStream[3]{
        DurableStream(9, filePath1),
        DurableStream(13, filePath2),
        DurableStream(7, "stream3.txt")
    });

    // Create PartitionStream with injected MsgStream instances
    PartitionStream testDurable(3, std::move(msgStreams));

    try {
        string invalidFilePath = "";
        testDurable[2] = DurableStream(7, invalidFilePath); // Should throw exception
    } catch (const invalid_argument& e) {
        cout << "Expected exception (empty file path): " << e.what() << endl;
    }

    // Test appendMessage functionality
    testDurable[0].appendMessage("DurableStream test message 1");
    testDurable[0].appendMessage("DurableStream test message 2");
    testDurable[0].appendMessage("DurableStream test message 3");

    testDurable[1].appendMessage("DurableStream 2 test message 1");

    // Test appendMessage with empty string (edge case)
    try {
        testDurable[1].appendMessage("");
    } catch (const runtime_error& e) {
        cout << "Expected exception (invalid message): " << e.what() << endl;
    }

    // Test readMessages
    cout << "Reading from DurableStream 1 with specific range:" << endl;
    std::unique_ptr<string[]> messages = testDurable.readMessage(1, 0, 2);
    for (int i = 0; i < 2; ++i) {
        cout << messages[i] << endl;
    }

    // Test Reset functionality
    cout << "Resetting DurableStream 1:" << endl << endl;
    testDurable[0].reset(); // Directly call reset on the first DurableStream

    cout << "All PartitionStream with DurableStreams tests completed." << endl;
}

// Test PartitionStream with mixed streams
void testPartitionStreamMixedStreams() {
    string filePath = "mixed_stream.txt";

    // Initialize PartitionStream with a capacity of 3
    PartitionStream ps(3, std::unique_ptr<MsgStream[]>(new MsgStream[3]));
    ps.initializeMsgStream(0, 4); // MsgStream with capacity 5
    ps.initializeMsgStream(2, 201); // MsgStream with capacity 5

    // Assign DurableStream to partition 1
    ps[1] = DurableStream(10, filePath);

    // Append messages to each stream
    ps[0].appendMessage("Test message 1 of MsgStream 1");
    ps[1].appendMessage("Test message of DurableStream 1");
    ps[2].appendMessage("Test message 2 of MsgStream 2");

    // Read messages from each stream
    cout << "Reading from MsgStream 0:" << endl;
    std::unique_ptr<std::string[]> messages = ps.readMessage(1, 0, 1);
    cout << messages[0] << std::endl;

    cout << "Reading from DurableStream 1:" << endl;
    messages = ps.readMessage(2, 0, 1);
    cout << messages[0] << endl;

    // Test operator+= with another PartitionStream
    cout << "Testing operator+= with another PartitionStream:" << endl;
    PartitionStream ps2(3, std::unique_ptr<MsgStream[]>(new MsgStream[3]));
    ps2.initializeMsgStream(0, 5);
    ps2[0].appendMessage("PartitionStream 2 - MsgStream 0 - Message 1");

    ps += ps2;

    // Verify messages after merge
    cout << "Messages in PartitionStream after merge:" << endl;
    messages = ps.readMessage(1, 0, 1);
    cout << messages[0] << endl << endl;

    cout << "Testing of PartitionStream with mixed streams completed." << endl;
}

// Test PartitionStream with a single stream
void testPartitionStreamSingleStream() {
    PartitionStream testSingle(1, std::unique_ptr<MsgStream[]>(new MsgStream[1]));
    testSingle.initializeMsgStream(0, 11);

    testSingle[0].appendMessage("Just testing for single stream with one message!");

    cout << "Reading from single MsgStream:" << endl;
    auto messages = testSingle.readMessage(1, 0, 1);
    cout << messages[0] << endl << endl;
}

// Testing additional edge cases here
void testPartitionStreamEdgeCases() {
    
    PartitionStream edgeCaseTester(3, std::unique_ptr<MsgStream[]>(new MsgStream[3]));

    cout << "Testing invalid key access:" << endl;
    try {
        edgeCaseTester.writeMessage(4, "Invalid Key Message"); // Invalid key (key > capacity)
    } catch (const runtime_error& e) {
        cout << "Caught exception: " << e.what() << endl;
    }

    cout << "Testing writing to a full MsgStream:" << endl;
    edgeCaseTester.initializeMsgStream(0, 2); // Initialize MsgStream with a capacity of only 2
    edgeCaseTester[0].appendMessage("Message 1");
    edgeCaseTester[0].appendMessage("Message 2");
    try {
        edgeCaseTester[0].appendMessage("Message 3"); // Exceeds MsgStream capacity
    } catch (const runtime_error& e) {
        cout << "Caught exception: " << e.what() << endl;
    }

    cout << "Testing resetting an empty PartitionStream:" << endl;
    -edgeCaseTester; // Reset the PartitionStream
    cout << "Partition count after reset: " << edgeCaseTester.getPartitionCount() << endl << endl;

    cout << "Additional edge case tests completed." << endl;
}