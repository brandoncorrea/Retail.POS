using Retail.POS.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Retail.POS.BL
{
    public class TransactionRepository : ITransactionRepository
    {
        public ITransaction Get(object id)
        {
            throw new NotImplementedException();
        }

        public void StoreCompleted(ITransaction transaction)
        {
            throw new NotImplementedException();
        }

        public void StoreIncompleted(ITransaction transaction)
        {
            throw new NotImplementedException();
        }

        public void StoreVoided(ITransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
