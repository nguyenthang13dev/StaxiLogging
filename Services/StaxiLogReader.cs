using Microsoft.Extensions.Options;
using Nest;
using StaxiLogging.Domain;
using StaxiLogging.Models;
using StaxiLogging.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaxiLogging.Services
{
    public class StaxiLogReader : IStaxiLogReader
    {
        private readonly IElasticClient _client;

        private readonly string _defaultIndexName;
        public StaxiLogReader(IElasticClient client, string indexName) { 
            _client = client;
            _defaultIndexName = indexName;
        }
        public async Task<BaseResponse<StaxiLogEntry>> GetPageListLogService(LogSearchOptions filter, string indexName = null)
        {

            string indexSearch = indexName ?? _defaultIndexName;

            // Tạo query log trên index (_indexName)
            var query = await _client.SearchAsync<StaxiLogEntry>(s =>
                    // Search trong index
                    s.Index(indexSearch)
                    // 
                    .From((filter.PageIndex - 1) * filter.PageSize)
                    // Skip lấy size
                    .Size(filter.PageSize)
                    // Laasy total documents
                    .TrackTotalHits(true)
                    //
                    .Source(src =>
                        src.IncludeAll()
                    )
                    // Tạo query truy vấn
                    .Query(q => q
                        .Bool(b => b
                            .Filter(
                                // filter theo level
                                f =>  filter.From.HasValue ? f.DateRange(r => r
                                                                             .Field(p => p.Timestamp)
                                                                             .GreaterThanOrEquals(filter.From.Value)) : null,

                                // filter theo level
                                f => filter.To.HasValue ? f.DateRange(r => r
                                                                             .Field(p => p.Timestamp)
                                                                             .LessThanOrEquals(filter.To.Value)) : null,

                                f => !string.IsNullOrEmpty(filter.Loglevel) ? f.Match(m => m
                                                                        .Field(p => p.Level)
                                                                        .Query(filter.Loglevel)
                                                                        ) : null,
                                // filter theop application
                                f => !string.IsNullOrEmpty(filter.Environment) ? f.Term(t => t
                                                                                            .Field("Environment.keyword")
                                                                                        .Value(filter.Environment)) : null,
                                // filter theop application
                                f => !string.IsNullOrEmpty(filter.Application) ? f.Term(t => t
                                                                                            .Field("Application.keyword")
                                                                                        .Value(filter.Application)) : null,
                                // Filter theo request api
                                f => !string.IsNullOrEmpty(filter.RequestApi) ? f.Term(m => m
                                                .Field("RequestPath.keyword")
                                                .Value(filter.RequestApi)) : null,

                                //Filter theo message
                                f => !string.IsNullOrEmpty(filter.SearchTerm) ? f
                                                            .Match(m => m
                                                                .Field(p => p.Message)
                                                                .Query(filter.SearchTerm)
                                                             ) : null

                            )
                        )
                    ));
            if (!query.IsValid)
            {
                throw new Exception($"Elasticsearch query failed: {query.DebugInformation}");
            }
            return new BaseResponse<StaxiLogEntry>
            {
                PageIndex = filter.PageIndex,
                PageSize = filter.PageSize,
                TotalCount = query.Total,
                Items = query.Documents.ToList()
            };
                        
        }


    }
}
