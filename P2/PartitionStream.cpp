// Saxton Van Dalsen
// 10/15/2024

#include "PartitionStream.h"
#include "MsgStream.h"
#include <cstring>
#include <stdexcept>

PartitionStream::PartitionStream(int capacity, const MsgStream* streams, const char keys[][30])
{

}

PartitionStream::~PartitionStream()
{
    for (int i = 0; i < partitionCount; i++)
    {
        delete[] partition[i].key;
        delete partition[i].stream;
    }
    delete[] partition;
}