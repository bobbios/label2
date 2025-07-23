using System;
using System.Windows.Forms;
using POSLink2;
using POSLink2.CommSetting;
using POSLink2.Transaction;
using POSLink2.Util;



namespace label2
{
    public static class TerminalLogger
    {
        public static string SendCreditSale(string ipAddress, int port, string amount, string invoiceNumber)
        {
            try
            {
                Console.WriteLine("Setting up communication settings...");

                // Set up the communication settings (TCP/IP) for the terminal
                var commSetting = new POSLink2.CommSetting.TcpSetting
                {
                    Ip = ipAddress,
                    Port = port,
                    Timeout = 120000 // Timeout in milliseconds
                };
                Console.WriteLine($"CommSetting: {commSetting.Ip}, {commSetting.Port}, {commSetting.Timeout}");

                Console.WriteLine("Initializing terminal...");
                // Initialize the terminal using the communication settings
                var poslink = POSLink2.POSLink2.GetPOSLink2();
                var terminal = poslink.GetTerminal(commSetting);

                if (terminal == null)
                {
                    MessageBox.Show("Failed to initialize terminal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return "NOT APPROVED";
                }

                Console.WriteLine("Creating credit sale request...");
                // Create a credit sale request (DoCreditReq)
                var creditRequest = new POSLink2.Transaction.DoCreditReq
                {
                    TransactionType = POSLink2.Const.TransType.Sale,
                };
                    creditRequest.AmountInformation.TransactionAmount = "100";
                creditRequest.TraceInformation.EcrRefNum = "11235435";
                //   AmountInformation = new POSLink2.Transaction.AmountReq { TransactionAmount = amount },
                //  TraceInformation = new POSLink2.Transaction.TraceReq { EcrRefNum = invoiceNumber }
            

                Console.WriteLine($"CreditRequest: TransactionAmount = {creditRequest.AmountInformation.TransactionAmount}, EcrRefNum = {creditRequest.TraceInformation.EcrRefNum}");

                // Create an object to hold the response
                var creditResponse = new POSLink2.Transaction.DoCreditRsp();

                Console.WriteLine("Sending credit sale request...");
                // Execute the DoCredit request and retrieve the result
                var executionResult = terminal.Transaction.DoCredit(creditRequest, out creditResponse);

                // Check if the transaction was approved by examining the AuthCode in the response
                if (!string.IsNullOrEmpty(creditResponse.HostInformation?.AuthCode))
                {
                    LogTransactionDetails(creditResponse);
                    return "APPROVED";
                }
                else
                {
                    Console.WriteLine("Transaction not approved: No AuthCode returned.");
                    LogFailureDetails(creditResponse);
                    return "NOT APPROVED: null response";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "NOT APPROVED: catch error";
            }
        }


        public static void CloseBatch(string ipAddress, int port)
        {
            try
            {
                Console.WriteLine("Setting up communication settings...");
                var commSetting = new TcpSetting
                {
                    Ip = ipAddress,
                    Port = port,
                    Timeout = 120000
                };

                Console.WriteLine("Initializing terminal...");
                var terminal = POSLink2.POSLink2.GetPOSLink2().GetTerminal(commSetting);
                if (terminal == null)
                {
                    MessageBox.Show("Failed to initialize terminal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Console.WriteLine("Creating batch close request...");
                var batchCloseRequest = new POSLink2.Batch.BatchCloseReq
                {
                    TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss")
                };

                Console.WriteLine("Sending batch close request...");
                var batchCloseResponse = new POSLink2.Batch.BatchCloseRsp();
                var executionResult = terminal.Batch.BatchClose(batchCloseRequest, out batchCloseResponse);

                if (!string.IsNullOrEmpty(batchCloseResponse.HostInformation?.AuthCode))
                {
                    Console.WriteLine("Batch closed successfully.");
                    LogHostInformation(batchCloseResponse.HostInformation);
                }
                else
                {
                    Console.WriteLine("Batch close not approved: No AuthCode returned.");
                    LogFailureDetails(batchCloseResponse);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void LogTransactionDetails(DoCreditRsp creditResponse)
        {
            Console.WriteLine("Transaction Successful:");
            LogHostInformation(creditResponse.HostInformation);
            LogAmountInformation(creditResponse.AmountInformation);
            LogAccountInformation(creditResponse.AccountInformation);
            LogTraceInformation(creditResponse.TraceInformation);
            // Add more logs for additional response details as needed
        }

        private static void LogFailureDetails(object response)
        {
            Console.WriteLine("Logging failure details:");
            if (response is DoCreditRsp creditResponse)
            {
                LogHostInformation(creditResponse.HostInformation);
                // You can log more details from the creditResponse if available
            }
            else if (response is POSLink2.Batch.BatchCloseRsp batchCloseResponse)
            {
                LogHostInformation(batchCloseResponse.HostInformation);
                // You can log more details from the batchCloseResponse if available
            }
        }

        public static void LogHostInformation(HostRsp hostResponse)
        {
            if (hostResponse != null)
            {
                Console.WriteLine("Host Information:");
                Console.WriteLine("  HostResponseCode: " + hostResponse.HostResponseCode);
                Console.WriteLine("  HostResponseMessage: " + hostResponse.HostResponseMessage);
                Console.WriteLine("  AuthCode: " + hostResponse.AuthCode);
                Console.WriteLine("  HostRefNum: " + hostResponse.HostRefNum);
                Console.WriteLine("  TraceNumber: " + hostResponse.TraceNumber);
                Console.WriteLine("  BatchNumber: " + hostResponse.BatchNumber);
                Console.WriteLine("  TransactionIdentifier: " + hostResponse.TransactionIdentifier);
                Console.WriteLine("  GatewayTransactionID: " + hostResponse.GatewayTransactionID);
                Console.WriteLine("  HostDetailedMessage: " + hostResponse.HostDetailedMessage);
                Console.WriteLine("  TransactionIntegrityClass: " + hostResponse.TransactionIntegrityClass);
                Console.WriteLine("  RetrievalRefNum: " + hostResponse.RetrievalRefNum);
                Console.WriteLine("  IssuerResponseCode: " + hostResponse.IssuerResponseCode);
                Console.WriteLine("  PaymentAccountRefId: " + hostResponse.PaymentAccountRefId);
            }
        }

        public static void LogAmountInformation(AmountRsp amountResponse)
        {
            if (amountResponse != null)
            {
                Console.WriteLine("Amount Information:");
                Console.WriteLine("  ApproveAmount: " + amountResponse.ApproveAmount);
                Console.WriteLine("  AmountDue: " + amountResponse.AmountDue);
                Console.WriteLine("  TipAmount: " + amountResponse.TipAmount);
                Console.WriteLine("  CashBackAmount: " + amountResponse.CashBackAmount);
                Console.WriteLine("  MerchantFee: " + amountResponse.MerchantFee);
                Console.WriteLine("  TaxAmount: " + amountResponse.TaxAmount);
                Console.WriteLine("  Balance1: " + amountResponse.Balance1);
                Console.WriteLine("  Balance2: " + amountResponse.Balance2);
                Console.WriteLine("  ServiceFee: " + amountResponse.ServiceFee);
                Console.WriteLine("  TransactionRemainingAmount: " + amountResponse.TransactionRemainingAmount);
                Console.WriteLine("  ApprovedTipAmount: " + amountResponse.ApprovedTipAmount);
                Console.WriteLine("  ApprovedCashBackAmount: " + amountResponse.ApprovedCashBackAmount);
                Console.WriteLine("  ApprovedMerchantFee: " + amountResponse.ApprovedMerchantFee);
                Console.WriteLine("  ApprovedTaxAmount: " + amountResponse.ApprovedTaxAmount);
            }
        }

        public static void LogAccountInformation(AccountRsp accountResponse)
        {
            if (accountResponse != null)
            {
                Console.WriteLine("Account Information:");
                Console.WriteLine("  Account: " + accountResponse.Account);
                Console.WriteLine("  EntryMode: " + accountResponse.EntryMode);
                Console.WriteLine("  ExpireDate: " + accountResponse.ExpireDate);
                Console.WriteLine("  EbtType: " + accountResponse.EbtType);
                Console.WriteLine("  VoucherNumber: " + accountResponse.VoucherNumber);
                Console.WriteLine("  NewAccountNo: " + accountResponse.NewAccountNo);
                Console.WriteLine("  CardType: " + accountResponse.CardType);
                Console.WriteLine("  CardHolder: " + accountResponse.CardHolder);
                Console.WriteLine("  CvdApprovalCode: " + accountResponse.CvdApprovalCode);
                Console.WriteLine("  CvdMessage: " + accountResponse.CvdMessage);
                Console.WriteLine("  CardPresentIndicator: " + accountResponse.CardPresentIndicator);
                Console.WriteLine("  GiftCardType: " + accountResponse.GiftCardType);
                Console.WriteLine("  DebitAccountType: " + accountResponse.DebitAccountType);
            }
        }

        public static void LogTraceInformation(TraceRsp traceResponse)
        {
            if (traceResponse != null)
            {
                Console.WriteLine("Trace Information:");
                Console.WriteLine("  RefNum: " + traceResponse.RefNum);
                Console.WriteLine("  EcrRefNum: " + traceResponse.EcrRefNum);
                Console.WriteLine("  TimeStamp: " + traceResponse.TimeStamp);
                Console.WriteLine("  InvoiceNumber: " + traceResponse.InvoiceNumber);
                Console.WriteLine("  PaymentService2000: " + traceResponse.PaymentService2000);
                Console.WriteLine("  AuthorizationResponse: " + traceResponse.AuthorizationResponse);
                Console.WriteLine("  EcrTransID: " + traceResponse.EcrTransID);
            }
        }

    }
}
