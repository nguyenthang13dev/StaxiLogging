using StaxiLogging.Domain;
using StaxiLogging.Models;
using StaxiLogging.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StaxiLogging.Services
{
    public interface IStaxiLogReader
    {
        Task<BaseResponse<StaxiLogEntry>> GetPageListLogService(LogSearchOptions filter, string indexName = null);
    }
}
