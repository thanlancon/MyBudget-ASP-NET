using Application.Core;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Transactions
{
    /// <summary>
    /// When create, update, delete a transaction
    /// This class will responsible for updating balance for the transaction
    /// and Envelope balance, transfer transaction
    /// </summary>
    public class TransactionActions
    {
        /// <summary>
        /// total balance after create/delete/update transaction
        /// used to update bank balance
        /// to reduce number of times to read database
        /// </summary>
        private decimal _totalBalanceAfterUpdate = 0;



        /// <summary>
        /// Create a new transfer transaction
        /// </summary>
        /// <returns>
        /// new Transaction that has info relate to the current transaction info
        /// </returns>
        public Transaction GetNewTransferTransaction(Transaction transaction)
        {
            //cannot create transfer transaction if NO bank transfer is available
            if (transaction.BankId == Guid.Empty || transaction.BankId_Transfer == Guid.Empty)
            {
                return null;
            }
            Transaction newTransaction = new Transaction
            {
                TransactionTransferID = transaction.Id,
                BankId = transaction.BankId_Transfer,
                BankId_Transfer = transaction.BankId,
                PayeeId = transaction.PayeeId,
                EnvelopeId = transaction.EnvelopeId,

                TransactionDate = transaction.TransactionDate,
                PostDate = transaction.PostDate,

                Inflow = transaction.Outflow,
                Outflow = transaction.Inflow,

                IsCleared = transaction.IsCleared

            };

            return newTransaction;
        }
        /// <summary>
        /// create info for a transfer transaction
        /// base on info from "this" transaction
        /// </summary>
        /// <returns></returns>
        public Transaction FillTransferInfo(Transaction transaction)
        {
            Transaction transfer = new Transaction
            {
                BankId = transaction.BankId_Transfer,
                BankId_Transfer = transaction.BankId,
                PayeeId = transaction.PayeeId,
                EnvelopeId = transaction.EnvelopeId,

                TransactionDate = transaction.TransactionDate,
                PostDate = transaction.PostDate,

                Inflow = transaction.Outflow,
                Outflow = transaction.Inflow,
                TotalBalance = 0,

                IsCleared = transaction.IsCleared,

                Note = transaction.Note
            };
            return transfer;
        }

        /// <summary>
        /// create data for transaction before editting
        /// </summary>
        /// <param name="inflow">
        /// value will be use to apply to TransactionBeforeEdit inflow
        /// </param>
        /// <param name="outflow">
        /// value will be use to apply to TransactionBeforeEdit outflow
        /// </param>
        // public Transaction CreateTransactionBeforeEdit(Transaction transaction, decimal inflow, decimal outflow)
        public Transaction CreateTransactionBeforeEdit(Transaction transaction)
        {
            Transaction transactionBeforeEdit = new Transaction
            {
                Id = transaction.Id,
                TransactionTransferID = transaction.TransactionTransferID,
                BankId = transaction.BankId,
                BankId_Transfer = transaction.BankId_Transfer,
                PayeeId = transaction.PayeeId,
                EnvelopeId = transaction.EnvelopeId,

                TransactionDate = transaction.TransactionDate,
                PostDate = transaction.PostDate,

                Inflow = transaction.Inflow,
                Outflow = transaction.Outflow,

                IsCleared = transaction.IsCleared,
                TotalBalance = transaction.TotalBalance,

                Note = transaction.Note
            };
            return transactionBeforeEdit;
        }
        private Transaction _transaction;
        private DataContext _context;
        public TransactionActions(Transaction transaction, DataContext context)
        {
            _context = context;
            _transaction = transaction;

        }
        private Transaction LoadTransaction_UnTracked(Guid id)
        {
            return _context.Transactions
                .Where(e => e.Id == id)
                .AsNoTracking().FirstOrDefault();
        }
        /// <summary>
        /// get total balance of a lastest transaction of a bank
        ///
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>
        /// if no transaction was stored before the input transaction, return 0
        /// </returns>
        private decimal TotalBalancePreviousTransaction(Transaction transaction)
        {
            //find the transaction that occured before the id transaction
            decimal totalBalance = 0;
            if (transaction.Id != Guid.Empty)//if transaction is exist
            {
                var query = from t in _context.Transactions
                            where t.BankId == transaction.BankId
                                && t.TransactionDate <= transaction.TransactionDate
                                && t.SequenceNumber < transaction.SequenceNumber
                                && t.Id != transaction.Id

                            orderby t.TransactionDate descending, t.PostDate descending, t.SequenceNumber descending
                            select t.TotalBalance;
                totalBalance = query.FirstOrDefault();
            }
            else
            {
                var query = from t in _context.Transactions
                            where t.BankId == transaction.BankId
                                && t.TransactionDate <= transaction.TransactionDate
                                && t.SequenceNumber < transaction.SequenceNumber
                            orderby t.TransactionDate descending, t.PostDate descending, t.SequenceNumber descending
                            select t.TotalBalance;
                totalBalance = query.FirstOrDefault();
            }

            return totalBalance;
        }
        private int GetLastestSequenceNumber()
        {
            //find the transaction that occured before the id transaction
            int lastestSequenceNumber = 0;

            var query = from t in _context.Transactions
                        orderby t.SequenceNumber descending
                        select t.SequenceNumber;
            lastestSequenceNumber = query.FirstOrDefault();

            return lastestSequenceNumber;
        }


        /// <summary>
        /// Load a transaction
        /// It will not be tracked by DBContext
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<Transaction> LoadTransaction_Untracked(Guid id)
        {

            return await _context.Transactions
                .Where(e => e.Id == id)
                .AsNoTracking().FirstOrDefaultAsync();

        }
        public async Task<Result<string>> UpdateTransaction(Boolean isDeleteTransaction = false)
        {
            var result= await UpdateBalanceWhenCreateOrDeleteTransaction(_transaction, isDeleteTransaction);
            if(result==ResponseConstants.UpdateSuccess)
            {
                return Result<string>.Success(ResponseConstants.UpdateSuccess);
            }
            return Result<string>.Failure(result);
        }

        private async Task<string> UpdateBalanceWhenCreateOrDeleteTransaction(Transaction transaction, Boolean isDelete = false)
        {
            int lastestSequenceNumber = 0;
            if (!isDelete && transaction.Id != Guid.Empty)
            {
                return ResponseConstants.Transaction.OnlyUpdateOrDeleteTran;
            }
            Transaction transactionBeforeEdit;
            Boolean transactionIsNew = false;
            Boolean transferTransactionIsNew = false;
            //if transaction is new
            if (transaction.Id == Guid.Empty)
            {
                transactionIsNew = true;
                transaction.Id = Guid.NewGuid();
                lastestSequenceNumber = GetLastestSequenceNumber();

                transaction.SequenceNumber = lastestSequenceNumber += 1;

                transactionBeforeEdit = CreateTransactionBeforeEdit(transaction);
                transactionBeforeEdit.Inflow = 0;
                transactionBeforeEdit.Outflow = 0;
                transactionBeforeEdit.TotalBalance = 0;

                _context.Add(transaction);
            }

            string isUpdateAble = Core.IsCreateAble(transaction);
            //if the transaction is valid to be processed
            if (isUpdateAble != ResponseConstants.Transaction.TransactionIsUpdateAble)
            {
                return isUpdateAble;
            }

            Transaction transferTransaction = null;
            //if bank transfer is selected
            if (transaction.BankId_Transfer != Guid.Empty)
            {
                //if the transfer transaction is exist, load it
                if (transaction.TransactionTransferID != Guid.Empty)
                {
                    transferTransaction = await _context.Transactions.FindAsync(transaction.TransactionTransferID);
                    if (transferTransaction == null)
                    {
                        return ResponseConstants.Transaction.FailToLoadTransfer;
                    }
                }
                //if the transfer transaction is not exist, create one
                else
                {
                    transferTransactionIsNew = true;
                    transferTransaction = GetNewTransferTransaction(transaction);
                    //if cannot load or create the transfer transaction, cannot move on....
                    if (transferTransaction == null)
                    {
                        return ResponseConstants.Transaction.FailToCreateTransfer;
                    }
                    transferTransaction.Id = Guid.NewGuid();
                    transferTransaction.SequenceNumber = lastestSequenceNumber += 1;
                    _context.Add(transferTransaction);
                }
            }

            ///////Update balances for transaction
            //update total balance for transactions
            if (await UpdateTransactionBalanceAndFollowingTransactions(transaction, transactionIsNew,  isDelete) == false)
            {
                return ResponseConstants.Transaction.FailToUpdateFollowingBalance;
            }
            //_totalBalanceAfterUpdate is just updated after UpdateTransactionBalanceAndFollowingTransactions
            if (await UpdateBalance_BankAccounts(transaction.BankId, _totalBalanceAfterUpdate) != ResponseConstants.UpdateSuccess)
            {
                return ResponseConstants.Transaction.FailToUpdateBankBalance;
            }

            //update envelope total balance
            if (transaction.EnvelopeId != Guid.Empty)
            {
                if (await UpdateBalance_Envelope(transaction, isDelete) == false)
                {
                    return ResponseConstants.Transaction.FailToUpdateEnvelopeBalance;
                }
            }
            //End of update balances for transaction
            /////////////////////////

            ///////Update balances for transfer transaction
            //update total balance for transfer transactions
            if (transferTransaction != null)
            {
                if (await UpdateTransactionBalanceAndFollowingTransactions(transferTransaction, transferTransactionIsNew, isDelete) == false)
                {
                    return ResponseConstants.Transaction.FailToUpdateFollowingTransferBalance;
                }
                //_totalBalanceAfterUpdate is just updated after UpdateTransactionBalanceAndFollowingTransactions
                //update balance for bank
                if (await UpdateBalance_BankAccounts(transferTransaction.BankId, _totalBalanceAfterUpdate) != ResponseConstants.UpdateSuccess)
                {
                    return ResponseConstants.Transaction.FailToUpdateBankBalanceTransfer;
                }
                if (transferTransaction.EnvelopeId != Guid.Empty)
                {
                    //update envelope total balance
                    if (await UpdateBalance_Envelope(transferTransaction, isDelete) == false)
                    {
                        return ResponseConstants.Transaction.FailToUpdateEnvelopeBalanceTransfer;
                    }
                }
            }
            //End of update balances for transfer transaction
            /////////////////////////
            if (isDelete)
            {
                _context.Remove(transaction);
                if (transferTransaction != null)
                {
                    _context.Remove(transferTransaction);
                }
            }
            else //if create new transaction
            {
                if (transferTransaction != null)
                {
                    transaction.TransactionTransferID = transferTransaction.Id;
                    transferTransaction.TransactionTransferID = transaction.Id;
                }
            }
            await _context.SaveChangesAsync();
            return ResponseConstants.UpdateSuccess;
        }
        /// <summary>
        /// Load list of transactions that are entered after the input transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="transactionIsNew"></param>
        /// <returns></returns>
        private async Task<List<Transaction>> LoadFollowingTransactions(Transaction transaction, Boolean transactionIsNew)
        {
            //if transaction is exist
            if (!transactionIsNew)
            {
                var query = _context.Transactions
                    .Where(t => t.BankId == transaction.BankId)
                    .Where(t => t.TransactionDate >= transaction.TransactionDate)
                    .Where(t => t.Id != transaction.Id)
                    .OrderBy(t => t.TransactionDate)
                    .OrderBy(t => t.PostDate)
                    .ThenBy(t => t.SequenceNumber);
                return await query.ToListAsync();
            }
            else //if transaction is new
            {
                var query = _context.Transactions
                    .Where(t => t.BankId == transaction.BankId)
                    .Where(t => t.TransactionDate > transaction.TransactionDate)
                    .OrderBy(t => t.TransactionDate)
                    .OrderBy(t => t.PostDate)
                    .ThenBy(t => t.SequenceNumber);
                return await query.ToListAsync();
            }
        }


        /// <summary>
        /// Update balances of transaction and all following transaction
        ///
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="transactionIsNew"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        private async Task<Boolean> UpdateTransactionBalanceAndFollowingTransactions(Transaction transaction, Boolean transactionIsNew, Boolean isDelete = false)
        {
            try
            {
                List<Transaction> listFollowingTransaction = await LoadFollowingTransactions(transaction, transactionIsNew);
                decimal previousTransactionTotalBalance = TotalBalancePreviousTransaction(transaction);
                _totalBalanceAfterUpdate = previousTransactionTotalBalance;
                //update total balance for the input transaction
                if (!isDelete)
                {
                    transaction.TotalBalance = previousTransactionTotalBalance + transaction.Inflow - transaction.Outflow;
                    previousTransactionTotalBalance = transaction.TotalBalance;
                    _totalBalanceAfterUpdate = transaction.TotalBalance;
                }
                //will perform updating balance for input list of transactions
                if (listFollowingTransaction != null)
                {
                    for (int i = 0; i < listFollowingTransaction.Count; i++)
                    {
                        listFollowingTransaction[i].TotalBalance = previousTransactionTotalBalance + listFollowingTransaction[i].Inflow - listFollowingTransaction[i].Outflow;
                        previousTransactionTotalBalance = listFollowingTransaction[i].TotalBalance;
                        _totalBalanceAfterUpdate = listFollowingTransaction[i].TotalBalance;
                    }
                }
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }
        /// <summary>
        /// update total balance of a bank
        /// </summary>
        /// <param name="bankID"></param>
        /// <param name="balance"></param>
        /// <returns></returns>
        private async Task<string> UpdateBalance_BankAccounts(Guid bankID, decimal balance)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(bankID);
            if (bankAccount != null)
            {
                bankAccount.ClearedBalance = balance;
                return ResponseConstants.UpdateSuccess;
            }
            else
            {
                return ResponseConstants.BankAccounts.NotFound;
            }
        }
        /// <summary>
        /// update envelope balance when create a new or delete a transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        private async Task<Boolean> UpdateBalance_Envelope(Transaction transaction, Boolean isDelete)
        {
            Envelope envelope = await _context.Envelopes.FindAsync(transaction.EnvelopeId);
            if (envelope != null)
            {
                if (isDelete)
                {
                    envelope.TotalBalance = CalculateEnvelopeBalance(envelope, 0, 0, transaction.Inflow, transaction.Outflow);
                }
                else
                {
                    envelope.TotalBalance = CalculateEnvelopeBalance(envelope, transaction.Inflow, transaction.Outflow, 0, 0);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Update total balance of an envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="newInflow">
        /// 0: update when deleting transaction
        /// </param>
        /// <param name="newOutflow">
        /// 0: update when deleting transaction
        /// </param>
        /// <param name="oldInflow"></param>
        /// <param name="oldOutflow"></param>
        /// <returns></returns>
        private decimal CalculateEnvelopeBalance(Envelope envelope, decimal newInflow, decimal newOutflow, decimal oldInflow, decimal oldOutflow)
        {
            decimal updateBalance = envelope.TotalBalance;
            updateBalance += -oldInflow + oldOutflow;
            updateBalance += newInflow - newOutflow;
            return updateBalance;
        }
    }
}
