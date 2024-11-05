// Saxton Van Dalsen
// 11/14/2024

#ifndef PARTITIONSTREAM_H
#define PARTITIONSTREAM_H

#include "MsgStream.h"
#include <memory>
#include <string>

using namespace std;

class PartitionStream {

    private:
        unique_ptr<MsgStream[]> streams;
        unique_ptr<int[]> keys;
        int capacity;
        int streamCount;
        int operationCount;

        int findPartitionIndex(const string& key) const;

    public:
        PartitionStream(int initialCapacity);
        
};

#endif