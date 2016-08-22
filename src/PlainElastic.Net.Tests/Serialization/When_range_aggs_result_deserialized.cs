using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using PlainElastic.Net.Serialization;
using PlainElastic.Net.Utils;

namespace PlainElastic.Net.Tests.Serialization
{
    [Subject(typeof(JsonNetSerializer))]
    class When_range_aggs_result_deserialized
    {
        #region Terms Aggregations Json Result
        private static readonly string rangesAggsJsonResult =
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
        'prices' : {
            'buckets': {
                '*-50.0': {
                    'to': 50,
                    'doc_count': 2
                },
                '50.0-100.0': {
                    'from': 50,
                    'to': 100,
                    'doc_count': 4
                },
                '100.0-*': {
                    'from': 100,
                    'doc_count': 4
                }
            }
        }
    }
}".AltQuote();
        #endregion

        Establish context = () => 
        {
            jsonSerializer = new JsonNetSerializer();

            expectedBucketRanges = new[] {
                new KeyedBucketAggregationResult.RangeBucket{ doc_count = 2 },
                new KeyedBucketAggregationResult.RangeBucket{ doc_count = 4 },
                new KeyedBucketAggregationResult.RangeBucket{ doc_count = 4 }
            };
        };


        Because of = () => result = jsonSerializer.ToSearchResult<object>(rangesAggsJsonResult);
            

        It should_contain_correct_hits_count = () =>
            result.hits.total.ShouldEqual(100);

        It should_deserialize_prices_agg_to_TermsAggregationResults_type = () =>
            result.aggregations["prices"].ShouldBeOfType<KeyedBucketAggregationResult>();

        It should_contain_prices_agg_with_3_terms_aggs = () =>
            result.aggregations["prices"].As<KeyedBucketAggregationResult>().buckets.Count.ShouldEqual(3);

        It should_contain_prices_agg_with_3_term_aggs_named_One_Two_Three = () =>
            result.aggregations["prices"].As<KeyedBucketAggregationResult>().buckets.ShouldBeSameAs(expectedBucketRanges).ShouldBeTrue();

        It should_deserialize_prices2_agg_to_TermsAggregationResults_type = () =>
            result.aggregations["prices"].ShouldBeOfType<KeyedBucketAggregationResult>();


        private static JsonNetSerializer jsonSerializer;
        private static SearchResult<object> result;
        private static KeyedBucketAggregationResult.RangeBucket[] expectedBucketRanges;
    }


    public static class KeyBucketsAggsComparator
    {
        public static bool ShouldBeSameAs(this Dictionary<string, KeyedBucketAggregationResult.RangeBucket> buckets, KeyedBucketAggregationResult.RangeBucket[] expectedBuckets)
        {
            return buckets.Values.Select((bucket, i) => bucket.doc_count == expectedBuckets[i].doc_count).All(b => b);
        }
    }
}