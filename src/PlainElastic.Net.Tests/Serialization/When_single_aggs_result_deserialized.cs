using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using PlainElastic.Net.Serialization;
using PlainElastic.Net.Utils;

namespace PlainElastic.Net.Tests.Serialization
{
    [Subject(typeof(JsonNetSerializer))]
    class When_single_aggs_result_deserialized
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
               '*-50.0': {
                    'doc_count': 2
                },
                '50.0-100.0': {
                    'doc_count': 4
                },
                '100.0-*': {
                    'doc_count': 4
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
            result.aggregations["*-50.0"].ShouldBeOfType<SingleAggregationResult>();

        private static JsonNetSerializer jsonSerializer;
        private static SearchResult<object> result;
        private static KeyedBucketAggregationResult.RangeBucket[] expectedBucketRanges;
    }
}