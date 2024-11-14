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

        cout << "\n=== Testing PartitionStream Edge Cases ===" << endl;
        testPartitionStreamEdgeCases();

    } catch (const exception& e) {
        cerr << "Exception occurred: " << e.what() << endl;
    }

    return 0;
}

// Test PartitionStream with all MsgStreams
void testPartitionStreamAllMsgStreams() {
    PartitionStream ps(3);

    ps.initializeMsgStream(0, 5);
    ps.initializeMsgStream(1, 10);
    ps.initializeMsgStream(2, 5);

    ps[0].appendMessage("Message 1 in MsgStream 0");
    ps[1].appendMessage("Message 1 in MsgStream 1");
    ps[2].appendMessage("Message 1 in MsgStream 2");

    cout << "Reading from MsgStream 0:" << endl;
    auto messages = ps.readMessage(1, 0, 1); // Key 1, start 0, end 0
    cout << messages[0] << endl;

    cout << "PartitionStream before reset:" << endl;
    cout << "Partition count: " << ps.getPartitionCount() << endl;
    -ps; // Reset operator
    cout << "PartitionStream after reset:" << endl;
    cout << "Partition count: " << ps.getPartitionCount() << endl;
}

// Test PartitionStream with all DurableStreams
void testPartitionStreamAllDurableStreams() {
    string filePath1 = "stream1.txt";
    string filePath2 = "stream2.txt";
    string filePath3 = "stream3.txt";

    PartitionStream testDurable(3);

    testDurable.initializeMsgStream(0, 5);
    testDurable[0] = DurableStream(5, filePath1);
    testDurable[1] = DurableStream(5, filePath2);
    testDurable[2] = DurableStream(5, filePath3);

    testDurable[0].appendMessage("DurableStream 1 - Message 1");
    testDurable[1].appendMessage("DurableStream 2 - Message 1");
    testDurable[2].appendMessage("DurableStream 3 - Message 1");

    DurableStream& durable1 = dynamic_cast<DurableStream&>(testDurable[0]);

    cout << "Reading from DurableStream 2:" << endl;
    auto messages = testDurable.readMessage(2, 0, 0);
    cout << messages[0] << endl;

    cout << "Resetting DurableStream 3:" << endl;
    DurableStream& resetDurable = dynamic_cast<DurableStream&>(testDurable[2]);
    resetDurable.reset();
}

// Test PartitionStream with mixed streams
void testPartitionStreamMixedStreams() {
    string filePath = "mixed_stream.txt";

    PartitionStream ps(3);
    ps.initializeMsgStream(0, 5);
    ps[1] = DurableStream(10, filePath);
    ps.initializeMsgStream(2, 5);

    ps[0].appendMessage("MsgStream 0 - Message 1");
    ps[1].appendMessage("DurableStream 1 - Message 1");
    ps[2].appendMessage("MsgStream 2 - Message 1");

    cout << "Reading from MsgStream 0:" << endl;
    auto messages = ps.readMessage(1, 0, 0);
    cout << messages[0] << endl;

    cout << "Reading from DurableStream 1:" << endl;
    messages = ps.readMessage(2, 0, 0);
    cout << messages[0] << endl;

    cout << "Testing operator+= with another PartitionStream:" << endl;
    PartitionStream ps2(3);
    ps2.initializeMsgStream(0, 5);
    ps2[0].appendMessage("PartitionStream 2 - MsgStream 0 - Message 1");

    ps += ps2;

    cout << "Messages in PartitionStream after merge:" << endl;
    messages = ps.readMessage(1, 0, 0);
    cout << messages[0] << endl;
}

// Test PartitionStream with a single stream
void testPartitionStreamSingleStream() {
    PartitionStream ps(1);
    ps.initializeMsgStream(0, 5);

    ps[0].appendMessage("Single MsgStream - Message 1");

    cout << "Reading from single MsgStream:" << endl;
    auto messages = ps.readMessage(1, 0, 0);
    cout << messages[0] << endl;
}

// Test PartitionStream Edge Cases
void testPartitionStreamEdgeCases() {
    PartitionStream ps(3);

    cout << "Testing invalid key access:" << endl;
    try {
        ps.writeMessage(4, "Invalid Key Message");
    } catch (const runtime_error& e) {
        cout << "Caught exception: " << e.what() << endl;
    }

    cout << "Testing writing to full MsgStream:" << endl;
    ps.initializeMsgStream(0, 2);
    ps[0].appendMessage("Message 1");
    ps[0].appendMessage("Message 2");
    try {
        ps[0].appendMessage("Message 3");
    } catch (const runtime_error& e) {
        cout << "Caught exception: " << e.what() << endl;
    }

    cout << "Testing resetting an empty PartitionStream:" << endl;
    -ps; // Reset
    cout << "Partition count after reset: " << ps.getPartitionCount() << endl;
}