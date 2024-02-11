using Microsoft.Data.SqlClient;

namespace Subscription_Management_System.RetryMechanism
{
    public class RetryPolicy
    {
        public static T ExecuteWithRetry<T>(Func<T> operation, int retryCount, TimeSpan delay = default(TimeSpan))
        {

            try
            {
                if (retryCount == 0)
                {
                    throw new InvalidOperationException("Max attempts have been reached, kindly contact our support team");
                }
                else
                {
                    return operation();
                }
            }
            catch (Exception ex) when (IsTransientFailure(ex) || retryCount == 0)
            {
                throw new InvalidOperationException(ex.Message);
            }

        }

        private static bool IsTransientFailure(Exception ex)
        {
            // You can customize this method to identify transient failures based on the type of exception thrown
            return ex is SqlException;
        }
    }
}
