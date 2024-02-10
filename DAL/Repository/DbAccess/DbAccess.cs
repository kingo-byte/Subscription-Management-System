using DAL.Repository.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.DbAccess
{
    public class DbAccess
    {
        private IConfiguration _configuration;
        private readonly string _conn;
        public DbAccess(IConfiguration configuration) 
        {
            _configuration = configuration;
            _conn = configuration.GetConnectionString("SMSConnection");
        }

        public List<Subscription> GetActiveSubscriptions(int userId)
        {
            string procedureName = "getsubscriptions";

            List<Subscription> employees = new List<Subscription>();

            using (var connection = new NpgsqlConnection(_conn))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddWithValue("param_userid", userId);

                    // Execute the stored procedure
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Map the results to the Employee object
                            Subscription employee = new Subscription()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                SubscriptionType = new SubscriptionType 
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("SubscriptionTypeId")),
                                    Description = reader.GetString(reader.GetOrdinal("Description"))
                                }
                            };

                            employees.Add(employee);
                        }
                    }
                }
            }

            return employees;
        }
    }
}
