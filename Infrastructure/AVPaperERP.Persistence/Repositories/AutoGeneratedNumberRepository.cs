﻿using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Persistence.Repositories
{
    public class AutoGeneratedNumberRepository : GenericRepository, IAutoGeneratedNumberRepository
    {
        private IConfiguration _configuration;

        public AutoGeneratedNumberRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async Task<string?> GetAutoGeneratedNumber(AutoGenerateNumber_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Table", parameters.Table);
            queryParameters.Add("@Prefix", parameters.Prefix);
            queryParameters.Add("@Length", parameters.Length);
            queryParameters.Add("@AutoNumber", parameters.AutoNumber, null, System.Data.ParameterDirection.Output);

            var result = await SaveByStoredProcedure<string>("sp_AutoNumberGen", queryParameters);
            var vAutoNumber = queryParameters.Get<string>("AutoNumber");

            return vAutoNumber;
        }
    }
}
