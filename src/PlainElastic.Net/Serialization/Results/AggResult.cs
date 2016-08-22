using System;
using System.Collections.Generic;

namespace PlainElastic.Net.Serialization
{
    public class AggResult
    {
        public T As<T>() where T: AggResult
        {
            return this as T;
        }
    }

    public class BucketAggregationResult: AggResult
    {
        public List<Bucket> buckets;

        public class RangeBucket : Bucket
        {
            public string from;
            public string to;
            public string from_as_string;
            public string to_as_string;
        }

        public class KeyedBucket : Bucket
        {
            public string key;
        }

        public class Bucket
        {
            public int doc_count;
        }
    }

    public class SingleAggregationResult : AggResult
    {
        public int doc_count;
    }

    public class KeyedBucketAggregationResult : AggResult
    {
        public Dictionary<string, RangeBucket> buckets;

        public class RangeBucket
        {
            public int doc_count;
            public string from;
            public string to;
            public string from_as_string;
            public string to_as_string;
        }
    }
}
