using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using PlainElastic.Net.Serialization;
using PlainElastic.Net.Utils;

namespace PlainElastic.Net.Tests.Serialization
{
    [Subject(typeof(JsonNetSerializer))]
    class When_terms_aggs_result_deserialized
    {
        #region Terms Aggregations Json Result
        private static readonly string termsAggsJsonResult =
@"{
    'took': 0,
    'timed_out': false,
    '_shards': {
        'total': 5,
        'successful': 5,
        'failed': 0
    },
    'hits': {
        'total': 100,
        'max_score': 1.0,
        'hits': []
    },
    'aggregations' : {
        'TestTerms' : {
            'doc_count_error_upper_bound': 0, 
            'sum_other_doc_count': 0, 
            'buckets' : [
                {
                    'key' : 'jazz',
                    'doc_count' : 10
                },
                {
                    'key' : 'rock',
                    'doc_count' : 20
                },
                {
                    'key' : 'electronic',
                    'doc_count' : 30
                },
            ]
        }
    }
}".AltQuote();
        #endregion

        Establish context = () => 
        {
            jsonSerializer = new JsonNetSerializer();

            expectedBucketTerms = new[] {
                new BucketAggregationResult.KeyedBucket{ key ="jazz", doc_count = 10 },
                new BucketAggregationResult.KeyedBucket{ key ="rock", doc_count = 20 },
                new BucketAggregationResult.KeyedBucket{ key ="electronic", doc_count = 30 }
            };
        };


        Because of = () => result = jsonSerializer.ToSearchResult<object>(termsAggsJsonResult);
            

        It should_contain_correct_hits_count = () =>
            result.hits.total.ShouldEqual(100);

        It should_deserialize_TestTerms_agg_to_TermsAggregationResults_type = () =>
            result.aggregations["TestTerms"].ShouldBeOfType<BucketAggregationResult>();

        It should_contain_TestTerms_agg_with_3_terms_aggs = () =>
            result.aggregations["TestTerms"].As<BucketAggregationResult>().buckets.Count.ShouldEqual(3);

        It should_contain_TestTerms_agg_with_3_term_aggs_named_One_Two_Three = () =>
            result.aggregations["TestTerms"].As<BucketAggregationResult>().buckets.ShouldBeSameAs(expectedBucketTerms).ShouldBeTrue();

        It should_deserialize_TestTerms2_agg_to_TermsAggregationResults_type = () =>
            result.aggregations["TestTerms2"].ShouldBeOfType<KeyedBucketAggregationResult>();


        private static JsonNetSerializer jsonSerializer;
        private static SearchResult<object> result;
        private static BucketAggregationResult.Bucket[] expectedBucketTerms;
    }


    public static class BucketAggsComparator
    {
        public static bool ShouldBeSameAs(this IEnumerable<BucketAggregationResult.Bucket> buckets, BucketAggregationResult.Bucket[] expectedBuckets)
        {
            if (buckets.ElementAt(0) is BucketAggregationResult.KeyedBucket)
            {
                return buckets.Select((bucket, i) => ((BucketAggregationResult.KeyedBucket)bucket).key == ((BucketAggregationResult.KeyedBucket)expectedBuckets[i]).key && bucket.doc_count == expectedBuckets[i].doc_count).All(b => b);
            }
            else if (buckets.ElementAt(0) is BucketAggregationResult.RangeBucket)
            {
                return buckets.Select((bucket, i) => bucket.doc_count == expectedBuckets[i].doc_count).All(b => b);
            }
            else
                return buckets.Select((bucket, i) => bucket.doc_count == expectedBuckets[i].doc_count).All(b => b);
        }
    }
}