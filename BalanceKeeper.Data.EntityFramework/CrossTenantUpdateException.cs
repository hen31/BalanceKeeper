using System;
using System.Runtime.Serialization;

namespace BalanceKeeper.Data.EntityFramework
{
    [Serializable]
    internal class CrossTenantUpdateException : Exception
    {
        public CrossTenantUpdateException()
        {
        }

        public CrossTenantUpdateException(string message) : base(message)
        {
        }

        public CrossTenantUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CrossTenantUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}