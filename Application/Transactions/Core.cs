using Application.Core;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Transactions
{
    public static class Core
    {

        /// <summary>
        /// Inspect if all the info in the transaction is valid
        /// </summary>
        /// <returns>
        /// return one of the constants from TransacationIsValid class constants
        /// </returns>
        public static string IsTransactionValid(Transaction transaction)
        {
            //bank is not selected
            if (transaction.BankId == Guid.Empty)
            {
                return ResponseConstants.Transaction.BankNotSelected;
            }
            //double bank names
            if (transaction.BankId == transaction.BankId_Transfer)
            {
                return ResponseConstants.Transaction.DoubleBankName;
            }
            //Both Inflow and Outflow are 0
            if (transaction.Inflow == transaction.Outflow && transaction.Inflow == 0)
            {
                return ResponseConstants.Transaction.ZeroTransaction;
            }
            //Both Inflow and Outflow are not 0
            else if (transaction.Inflow != 0 && transaction.Outflow != 0)
            {
                return ResponseConstants.Transaction.MustHasAZero;
            }
            //if transaction date after post date
            else if (Convert.ToDateTime(transaction.TransactionDate).Date > Convert.ToDateTime(transaction.PostDate).Date)
            {
                return ResponseConstants.Transaction.TransactionDateAfterPostDate;
            }

            //everything is valid
            return ResponseConstants.Transaction.isValid;
        }
        /// <summary>
        /// A transaction can be created if:
        /// 1/ Data is valid
        /// 2/ Bank is not changed
        /// 3/ Bank transfer is not changed
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>
        /// </returns>
        public static string IsCreateAble(Transaction transaction, Transaction transactionBeforeEdit = null)
        {
            //if the transaction is valid to be processed
            string isValidTransaction = IsTransactionValid(transaction);
            if (!isValidTransaction.Equals(ResponseConstants.Transaction.isValid))
            {
                return isValidTransaction;
            }
            //if both bank and bank transfer are selected
            if (transaction.BankId != Guid.Empty && transaction.BankId_Transfer != Guid.Empty)
            {
                if (transaction.EnvelopeId != Guid.Empty || transaction.PayeeId != Guid.Empty)
                {
                    // MyClass.ViewData.SetUpdateTransaction(this, "Cannot select envelope and payee when both bank and bank transfer are selected. ");
                    return ResponseConstants.Transaction.BothBanksWithEnvelope;
                }
            }

            //no payee or envelope for bank transfer transaction
            if (transaction.BankId != Guid.Empty && transaction.BankId_Transfer != Guid.Empty && (transaction.PayeeId != Guid.Empty || transaction.EnvelopeId != Guid.Empty))
            {
                // MyClass.ViewData.SetUpdateTransaction(this, "Do NOT select payee or envelope for bank transfer transaction. ");
                return ResponseConstants.Transaction.BankTransferNoPayeeEnvelope;
            }
            if (transactionBeforeEdit != null)
            {
                //if Bank is changed
                if (transactionBeforeEdit.BankId != Guid.Empty && transaction.BankId != transactionBeforeEdit.BankId)
                {
                    return ResponseConstants.Transaction.CannotChangeBank;
                }
                //if Bank is the same as Bank Transfer
                if (transaction.BankId_Transfer == transaction.BankId)
                {
                    return ResponseConstants.Transaction.BankSameTransfer;
                }
                //if bank transfer is changed
                if (transactionBeforeEdit.BankId_Transfer != Guid.Empty && transaction.BankId_Transfer != transactionBeforeEdit.BankId_Transfer)
                {
                    return ResponseConstants.Transaction.CannotChangeBankTransfer;
                }
                //if envelope is exist and changed
                if (transaction.EnvelopeId != transactionBeforeEdit.EnvelopeId && transactionBeforeEdit.EnvelopeId != Guid.Empty)
                {
                    return ResponseConstants.Transaction.CannotChangeEnvelope;
                }
            }
            return ResponseConstants.Transaction.TransactionIsUpdateAble;
        }
        public static string IsDeleteAble(DataContext context, Guid Id)
        {
            return ResponseConstants.IsUpdateAble;
        }
        public static string IsEditAble(DataContext context, Transaction transaction)
        {
            return ResponseConstants.Transaction.TransactionIsUpdateAble;
        }
    }
}
