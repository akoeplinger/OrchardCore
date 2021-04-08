using System;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Newtonsoft.Json.Linq;

namespace OrchardCore.Search.Elastic.QueryProviders
{
    /// <summary>
    /// We may not need this at all as Elastic has full blown DSL with NEST OOTB
    /// </summary>
    public class TermsQueryProvider : IElasticQueryProvider
    {
        public Query CreateQuery(IElasticQueryService builder, ElasticQueryContext context, string type, JObject query)
        {
            if (type != "terms")
            {
                return null;
            }

            var first = query.Properties().First();

            var field = first.Name;
            var boolQuery = new BooleanQuery();

            switch (first.Value.Type)
            {
                case JTokenType.Array:

                    foreach (var item in ((JArray)first.Value))
                    {
                        if (item.Type != JTokenType.String)
                        {
                            throw new ArgumentException($"Invalid term in terms query");
                        }

                        boolQuery.Add(new TermQuery(new Term(field, item.Value<string>())), Occur.SHOULD);
                    }

                    break;
                case JTokenType.Object:
                    throw new ArgumentException("The terms lookup query is not supported");
                default: throw new ArgumentException("Invalid terms query");
            }

            return boolQuery;
        }
    }
}