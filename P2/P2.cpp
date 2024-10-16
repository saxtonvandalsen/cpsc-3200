// Saxton Van Dalsen
// 10/15/2024

#include "PartitionStream.h"
#include "MsgStream.h"
#include <iostream>
#include <cstring>

using namespace std;

void testPartitionStream() {
    
    MsgStream streams[2] = { MsgStream(5), MsgStream(5) };

    streams[0].appendMessage("Stream 0: Message 1");
    streams[0].appendMessage("Stream 0: Message 2");

    streams[1].appendMessage("Stream 1: Message 1");
    streams[1].appendMessage("Stream 1: Message 2");

    char** messages0 = streams[0].readMessages(0, 2);
    cout << "Messages from MsgStream 0:" << endl;
    for (int i = 0; i < 2; i++)
    {
        cout << messages0[i] << endl;
    }
    delete[] messages0;
    cout << endl;

    char** messages1 = streams[1].readMessages(0, 2);
    cout << "Messages from MsgStream 1:" << endl;
    for (int i = 0; i < 2; i++) 
    {
        cout << messages1[i] << endl;
    }
    delete[] messages1;
    cout << endl;

    PartitionStream partitionStream(2, streams);

    partitionStream.appendMessage("partition1", "Hello from partition 1!");
    partitionStream.appendMessage("partition2", "Hello from partition 2!");
    partitionStream.appendMessage("partition1", "Another message for partition 1.");
    partitionStream.appendMessage("partition2", "Another message for partition 2.");

    char** messages2 = partitionStream.readMessage("partition1", 0, 2);
    cout << "Messages from partition1:" << endl;
    for (int i = 0; messages2[i] != nullptr; i++) {
        cout << messages2[i] << endl;
    }
    delete[] messages2;
    cout << endl;

    char** messages3 = partitionStream.readMessage("partition2", 0, 2);
    cout << "Messages from partition2:" << endl;
    for (int i = 0; messages3[i] != nullptr; i++) {
        cout << messages3[i] << endl;
    }
    delete[] messages3;
    cout << endl;

    // Test copying of PartitionStream
    PartitionStream copiedStream(partitionStream);
    cout << "Copied PartitionStream created. Current partition count: " << copiedStream.getPartitionCount() << endl;

    // Test move semantics
    PartitionStream movedStream(move(partitionStream));
    cout << "Moved PartitionStream created. Current partition count: " << movedStream.getPartitionCount() << endl;

    cout << endl;
    try {
        movedStream.appendMessage("invalid_partition", "This message should not be added.");
    } catch (const exception& e) {
        cout << "Caught exception for invalid partition key: " << e.what() << endl;
    }
    cout << endl;

    // Test resetting the moved PartitionStream
    movedStream.reset();
    cout << "Moved PartitionStream reset. Current partition count: " << movedStream.getPartitionCount() << endl;

    // Test original partitionStream after moving
    cout << "Original PartitionStream after move (should be empty): " << partitionStream.getPartitionCount() << endl;

    // Test copy assignment operator
    PartitionStream assignedStream(2, streams); // Create another PartitionStream
    assignedStream = copiedStream; // Use copy assignment operator
    cout << "Assigned PartitionStream created. Current partition count: " << assignedStream.getPartitionCount() << endl;
    
    cout << "Now checking for copied messsaged based on partition key 2: " << endl;
    char** checkCopiedMessages = assignedStream.readMessage("partition2", 0, 2);
    for (int i = 0; checkCopiedMessages[i] != nullptr; i++) {
        cout << checkCopiedMessages[i] << endl;
    }
    delete[] checkCopiedMessages;
    cout << endl;

    // Test move assignment operator
    PartitionStream moveAssignedStream(2, streams); // Create another PartitionStream
    moveAssignedStream = move(assignedStream); // Use move assignment operator
    cout << "Move assigned PartitionStream created. Current partition count: " << moveAssignedStream.getPartitionCount() << endl;

    cout << "Now checking for copied messsaged based on partition key 2: " << endl;
    char** checkMovedMessages = moveAssignedStream.readMessage("partition1", 0, 1);
    for (int i = 0; checkMovedMessages[i] != nullptr; i++) {
        cout << checkMovedMessages[i] << endl;
    }
    delete[] checkMovedMessages;
    cout << endl;

    // Test resetting the move assigned PartitionStream
    moveAssignedStream.reset();
    cout << "Move assigned PartitionStream reset. Current partition count: " << moveAssignedStream.getPartitionCount() << endl;
}

int main() {
    testPartitionStream();
    return 0;
}