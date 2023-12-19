using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Core
{
    public static class ResponseConstants
    {

        //update success
        public const string UpdateSuccess = "Update Success";
        public const string IsUpdateAble = "Data Can Be Create/Update/Delete";
        public static class Payee{
            public const string NotFound="Payee is not found!";
            public const string NameIsExist="Payee name is exist";
            public const string TransactionExist="Transaction is exist. Cannot be deleted!";
        }
        public static class Category{
            public const string NotFound="Category is not found!";
            public const string NameIsExist="Category name is exist";
            public const string EnvelopeExist="Envelope is exist. Cannot be deleted!";
        }
        public static class Envelope
        {
            public const string NotFound="Envelope is not found";
            public const string NameIsExist="Envelope name is exist";
            public const string TransactionExist="Transaction is exist. Cannot be deleted!";

        }
        public static class BankAccounts
        {
            public const string NotFound ="Bank Account is not found!";
            public const string CodeIsExist="Bank code is exist";
            public const string TransactionExist="Transaction is exist. Cannot be deleted!";
        }
        public static class BankAccountType
        {
            public const string NotFound ="Bank Account Type is not found!";
            public const string CodeIsExist="Bank Account Type code is exist";
            public const string BankAccountIsExist="Bank Account is exist. Cannot be deleted!";
        }
        public static class Bank
        {
            public const string NotFound ="Bank is not found!";
            public const string CodeIsExist="Bank code is exist";
            public const string NameIsExist="Bank name is exist";
            public const string BankAccountIsExist="Bank Account is exist. Cannot be deleted!";
        }
        public static class Transaction
        {
            public const string NotFound = "Transaction is not found!";
            //all the infor in transaction is valid
            public const string isValid = "VALID";
            public const string TransactionIsUpdateAble = "Transaction is updateable";
            //Bank is not selected
            public const string BankNotSelected = "Bank is not selected";
            //Transaction has the same Bank Name and Bank Tranfer Name
            public const string DoubleBankName = "Double Bank Names";
            //Both Inflow and Outflow equal 0
            public const string ZeroTransaction = "Inflow and Outflow equal 0";
            //Either Inflow or Outflow must be 0
            public const string MustHasAZero = "Inflow or Outflow must be 0";
            //TransactionDate is after PostDate
            public const string TransactionDateAfterPostDate = "Post Date must be nothing or AFTER Transaction Date";
            //If bank account are change when editting
            public const string BankExist = "Cannot change bank(s) account";

            public const string BothBanksWithEnvelope = "Cannot select envelope and payee when both bank and bank transfer are selected.";
            public const string BankTransferNoPayeeEnvelope = "Do NOT select payee or envelope for bank transfer transaction.";

            public const string CannotChangeBank = "Bank must not be changed.";
            public const string CannotChangeBankTransfer = "Bank transfer must not be changed.";
            public const string CannotChangeEnvelope = "Envelope must not be changed.";
            public const string BankSameTransfer = "Bank and Bank Transfer cannot be the same";

            public const string FailToLoadBeforeEdit = "Cannot load transaction before edit.";

            public const string FailToLoadTransfer = "Cannot load transfer transaction.";
            public const string FailToCreateTransfer = "Cannot create transfer transaction.";
            public const string FailToUpdateFollowingBalance = "Cannot update balance for following transactions.";
            public const string FailToUpdateFollowingTransferBalance = "Cannot update balance for following transactions of the transfer transaction.";
            public const string FailToUpdateBankBalance = "Cannot update bank balance.";
            public const string FailToUpdateBankBalanceTransfer = "Cannot update bank balance of the transfer transaction.";
            public const string FailToUpdateEnvelopeBalance = "Cannot update envelope balance.";
            public const string FailToUpdateEnvelopeBalanceTransfer = "Cannot update envelope balance of the transfer transaction.";
            public const string OnlyUpdateOrDeleteTran = "Transaction cannot be editted. Only create or delete";
        }
    }
}
