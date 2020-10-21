using Microsoft.Extensions.Configuration;
using Retail.POS.Common.Interfaces;
using Retail.POS.Common.Models;

namespace Retail.POS.BL
{
    public class TransactionHandler : ITransactionHandler
    {
        private readonly IConfiguration _config;
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionHandler(
            IConfiguration config, 
            IPaymentProcessor paymentProcessor,
            ITransactionRepository transactionRepository)
        {
            _config = config;
            _paymentProcessor = paymentProcessor;
            _transactionRepository = transactionRepository;
        }

        public ITransaction Create() =>
            new Transaction(_config, _paymentProcessor);

        public ITransaction Recall(object id) =>
            _transactionRepository.Get(id);

        public void Save(ITransaction transaction) =>
            _transactionRepository.StoreIncompleted(transaction);

        public void Complete(ITransaction transaction) =>
            _transactionRepository.StoreCompleted(transaction);
    }
}
