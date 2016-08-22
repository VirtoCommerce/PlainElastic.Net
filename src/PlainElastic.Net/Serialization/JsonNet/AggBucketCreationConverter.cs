using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlainElastic.Net.Serialization
{
    // how aggregations are determined is available here: https://github.com/elastic/elasticsearch-net/blob/2.x/src/Nest/Aggregations/AggregateJsonConverter.cs
    public class AggBucketCreationConverter: JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jAgg = JObject.Load(reader);

            var agg = GetAggregationType(jAgg);
            serializer.Populate(new JTokenReader(jAgg), agg);

            return agg;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(BucketAggregationResult.Bucket).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// returns aggregation type
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private BucketAggregationResult.Bucket GetAggregationType(JObject json)
        {
            var prop = json.First as JProperty;
            if (prop == null)
                return null;

            var type = string.Empty;
            switch (prop.Name)
            {
                case "key":
                    return new BucketAggregationResult.KeyedBucket();
                case "to":
                case "from":
                    return new BucketAggregationResult.RangeBucket();
                case "doc_count":
                    return new BucketAggregationResult.Bucket();
            }

            return new BucketAggregationResult.Bucket();
        }
    }
}