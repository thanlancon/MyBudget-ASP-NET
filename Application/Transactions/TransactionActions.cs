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



        public async Task<Result<string>> UpdateTransaction(Boolean isDeleteTransaction = false)
        {
            var result = await UpdateBalanceWhenCreateOrDeleteTransaction(_transaction, isDeleteTransaction);
            if (result == ResponseConstants.UpdateSuccess)
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
            //if transaction is new
            if (transaction.Id == Guid.Empty)
            {
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
            decimal totalBalance;
            if (isDelete)
            {
                if (UpdateTransBalance_DeleteTransaction(transaction, out totalBalance) == false)
                {
                    return ResponseConstants.Transaction.FailToUpdateFollowingBalance;
                }
            }
            else
            {
                if (UpdateTransBalance_NewTransaction(transaction, out totalBalance) == false)
                {
                    return ResponseConstants.Transaction.FailToUpdateFollowingBalance;
                }
            }
            //update total balance for bank
            if (await UpdateBalance_BankAccounts(transaction.BankId, totalBalance) != ResponseConstants.UpdateSuccess)
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
                decimal balanceBankTransfer = 0;
                if (isDelete)
                {
                    if (UpdateTransBalance_DeleteTransaction(transferTransaction, out balanceBankTransfer) == false)
                    {
                        return ResponseConstants.Transaction.FailToUpdateFollowingBalance;
                    }
                }
                else
                {
                    if (UpdateTransBalance_NewTransaction(transferTransaction, out balanceBankTransfer) == false)
                    {
                        return ResponseConstants.Transaction.FailToUpdateFollowingBalance;
                    }
                }
                //update total balance for bank
                //update balance for bank
                if (await UpdateBalance_BankAccounts(transferTransaction.BankId, balanceBankTransfer) != ResponseConstants.UpdateSuccess)
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
        /// <returns></returns>
        private List<Transaction> LoadFollowingTransactions(Transaction transaction)
        {

            var query = _context.Transactions
                .Where(t => t.BankId == transaction.BankId)
                .Where(t => t.TransactionDate >= transaction.TransactionDate)
                .Where(t => t.PostDate >= transaction.PostDate)
                .OrderBy(t => t.TransactionDate)
                .ThenBy(t => t.PostDate)
                .ThenBy(t => t.SequenceNumber);
            return query.ToList();
        }
        /// <summary>
        /// Get newest transaction of a bank account
        /// </summary>
        /// <param name="bankID"></param>
        /// <returns>
        /// transaction with newest transaction date and newest tranaction posted date
        /// </returns>
        private Transaction GetNewestTransaction(Guid bankID)
        {
            var query = _context.Transactions
                .Where(t => t.BankId == bankID)
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.PostDate)
                .ThenByDescending(t => t.SequenceNumber);
            return query.First();
        }
        /// <summary>
        /// compare two transaction dates
        /// only compare day, month, year
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns>
        /// Greater than zero: date1 after date2
        /// 0: date1 is the same date as date2
        /// Less than zero: date1 before date2
        /// </returns>
        private int CompareTransactionDate(DateTime date1, DateTime date2)
        {
            var temp1 = new DateTime(date1.Year, date1.Month, date1.Day);
            var temp2 = new DateTime(date2.Year, date2.Month, date2.Day);
            return temp1.CompareTo(temp2);
        }
        /// <summary>
        /// update transaction balance when creating new transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="totalBalance">
        /// total balance after update all transaction balance
        /// </param>
        /// <returns></returns>
        private Boolean UpdateTransBalance_NewTransaction(Transaction transaction, out Decimal totalBalance)
        {
            totalBalance = 0;
            try
            {
                List<Transaction> listFollowingTransaction = LoadFollowingTransactions(transaction);

                //if no transaction is found
                //mean: input transaction is the newest transction
                //update total balance for the input transaction only
                if (listFollowingTransaction.Count == 0)
                {
                    //find newest transction of the bank
                    var newestTransaction = GetNewestTransaction(transaction.BankId);
                    //if found latest transaction
                    if (newestTransaction != null)
                    {
                        totalBalance = newestTransaction.TotalBalance;
                    }
                    transaction.TotalBalance = totalBalance + (transaction.Inflow - transaction.Outflow);
                    totalBalance = transaction.TotalBalance;
                    return true;
                }

                Transaction firstTransaction = listFollowingTransaction.Find(x => x.Id == transaction.Id);
                //input transaction is new, so it cannot be in the list
                //if it is in the list, something is wrong
                if (firstTransaction != null)
                {
                    return false;
                }

                int i = 0;

                //listFollowingTransaction may include transactions that have the same transaction date and post date with input transaction
                //skip and won't update those transaction
                for (; i < listFollowingTransaction.Count; i++)
                {
                    if (CompareTransactionDate(listFollowingTransaction[i].TransactionDate, transaction.TransactionDate) > 0 && CompareTransactionDate(listFollowingTransaction[i].PostDate, transaction.PostDate) > 0)
                    {
                        firstTransaction = listFollowingTransaction[i];
                        break;
                    }
                }
                //if there is no transaction that has transaction date and post date before the input transaction's dates
                //update balance for input transaction base on the 1st transaction in the list
                if (firstTransaction == null)
                {
                    totalBalance = listFollowingTransaction[0].TotalBalance + (transaction.Inflow - transaction.Outflow);
                    transaction.TotalBalance = totalBalance;
                }
                //firstTransaction is the one before the input transaction
                //do the math to get total balance of the transaction before the firstTransaction
                //and update total balance for input transaction
                else
                {
                    totalBalance = firstTransaction.TotalBalance + (firstTransaction.Outflow - firstTransaction.Inflow);
                    transaction.TotalBalance = totalBalance + (transaction.Inflow - transaction.Outflow);
                    totalBalance = transaction.TotalBalance;
                }

                //will perform updating balance for input list of transactions
                for (; i < listFollowingTransaction.Count; i++)
                {
                    listFollowingTransaction[i].TotalBalance = totalBalance + listFollowingTransaction[i].Inflow - listFollowingTransaction[i].Outflow;
                    totalBalance = listFollowingTransaction[i].TotalBalance;
                }

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }
        /// <summary>
        /// update balance for transaction when deleting
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="totalBalance"></param>
        /// <returns></returns>
        private Boolean UpdateTransBalance_DeleteTransaction(Transaction transaction, out Decimal totalBalance)
        {
            totalBalance = 0;
            try
            {
                List<Transaction> listFollowingTransaction = LoadFollowingTransactions(transaction);

                Transaction firstTransaction = listFollowingTransaction.Find(x => x.Id == transaction.Id);

                int i = 0;

                //if 1st transaction is the input transaction
                //do math to get total balance of the transaction before firstTransaction
                //do not need to update firstTransaction (it is input transaction), it will be deleted
                if (firstTransaction != null)
                {
                    i = listFollowingTransaction.IndexOf(firstTransaction);
                    totalBalance = firstTransaction.TotalBalance + (firstTransaction.Outflow - firstTransaction.Inflow);
                    i++;
                }
                //deleting, but transaction is not found in database
                //something is wrong
                else
                {
                    return false;
                }

                //update balance for transaction after the input transaction
                for (; i < listFollowingTransaction.Count; i++)
                {
                    listFollowingTransaction[i].TotalBalance = totalBalance + listFollowingTransaction[i].Inflow - listFollowingTransaction[i].Outflow;
                    totalBalance = listFollowingTransaction[i].TotalBalance;
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
