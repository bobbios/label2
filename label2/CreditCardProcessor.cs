using System;
using System.Collections.Generic;
using System.Windows.Forms;
using POSLink2;
using POSLink2.Transaction;

namespace label2
{
    public class CreditCardProcessor
    {
        private POSLink2.POSLink2 poslink;
        private Terminal terminal;




        public CreditCardProcessor(string ipAddress)
        {
            // Set up communication settings
            var commSetting = new POSLink2.CommSetting.TcpSetting
            {
                Ip = ipAddress,
                Port = 10009,
                Timeout = 10000
            };

            // Initialize POSLink and terminal
            poslink = POSLink2.POSLink2.GetPOSLink2();
            terminal = poslink.GetTerminal(commSetting);
            if (terminal == null)
            {
                throw new Exception("Failed to initialize terminal. Please check the IP address and connection settings.");
            }
        }

        public void ProcessTransaction(string transType, string saleReturn, string amount, string invoiceNum)
        {
            switch (transType)
            {
                case "CREDIT":
                    ProcessCreditTransaction(saleReturn, amount, invoiceNum);
                    break;
                case "DEBIT":
                    ProcessDebitTransaction(saleReturn, amount, invoiceNum);
                    break;
                case "EBT":
                    ProcessEbtTransaction(saleReturn, amount, invoiceNum);
                    break;
                default:
                    MessageBox.Show("Invalid transaction type or sale return type.");
                    break;
            }
        }

        private void ProcessCreditTransaction(string saleReturn, string amount, string invoiceNum)
        {
            if (saleReturn == "SALE")
            {
                var doCreditReq = new DoCreditReq
                {
                    TransactionType = POSLink2.Const.TransType.Sale,
                };
                doCreditReq.AmountInformation.TransactionAmount = amount;
                doCreditReq.TraceInformation.EcrRefNum = invoiceNum;

                var doCreditRsp = new DoCreditRsp();
                var ret = terminal.Transaction.DoCredit(doCreditReq, out doCreditRsp);

                HandleTransactionResponse(ret, doCreditRsp.HostInformation.HostResponseCode, doCreditRsp.ResponseCode, doCreditRsp.ResponseMessage, doCreditRsp.TraceInformation.AuthorizationResponse);
            }
        }

        private void ProcessDebitTransaction(string saleReturn, string amount, string invoiceNum)
        {
            if (saleReturn == "SALE")
            {
                var doDebitReq = new DoDebitReq
                {
                    TransactionType = POSLink2.Const.TransType.Sale,
                };
                doDebitReq.AmountInformation.TransactionAmount = amount;
                doDebitReq.TraceInformation.EcrRefNum = invoiceNum;

                var doDebitRsp = new DoDebitRsp();
                var ret = terminal.Transaction.DoDebit(doDebitReq, out doDebitRsp);

                HandleTransactionResponse(ret, doDebitRsp.HostInformation.HostResponseCode, doDebitRsp.ResponseCode, doDebitRsp.ResponseMessage, doDebitRsp.TraceInformation.AuthorizationResponse);
            }
        }

        private void ProcessEbtTransaction(string saleReturn, string amount, string invoiceNum)
        {
            if (saleReturn == "SALE")
            {
                var doEbtReq = new DoEbtReq
                {
                    TransactionType = POSLink2.Const.TransType.Sale,
                    AmountInformation = { TransactionAmount = amount },
                    TraceInformation = { EcrRefNum = invoiceNum },
                    AccountInformation = new POSLink2.Util.AccountReq
                    {
                        EbtType = "F" // Food Stamp
                    }
                };

                var doEbtRsp = new DoEbtRsp();
                var ret = terminal.Transaction.DoEbt(doEbtReq, out doEbtRsp);
                var details = ExtractResponseDetails(new DoCreditRspWrapper(DoEbtReq));
                HandleTransactionResponse(ret, details);
            }
        }

        private void HandleTransactionResponse(ExecutionResult ret, string approvalCode, string responseCode, string responseMessage, string traceNumber)
        {
            if (ret.GetErrorCode() == POSLink2.ExecutionResult.Code.Ok)
            {
                string message = $"Approval Code: {approvalCode}\n" +
                                 $"Response Code: {responseCode}\n" +
                                 $"Response Message: {responseMessage}\n" +
                                 $"Trace Number: {traceNumber}";

                MessageBox.Show(message, "Transaction Approved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string errorCode = ret.GetErrorCode().ToString();
                string errorMsg = responseMessage ?? "No additional error message available.";
                MessageBox.Show($"Error: {errorCode}\nMessage: {errorMsg}", "Transaction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Dictionary<string, string> ExtractResponseDetails(ITransactionResponse response)
        {
            var details = new Dictionary<string, string>();

            // Get all properties of the response object using reflection
            var properties = response.GetType().GetProperties();
            foreach (var property in properties)
            {
                // Get the property name and value
                string name = property.Name;
                var value = property.GetValue(response);

                // Add to dictionary, using a custom label if desired
                details[name] = value?.ToString() ?? "N/A";
            }

            // Manually add nested properties if needed
            details["hostResponseCode"] = response.HostInformation?.HostResponseCode ?? "N/A";
            details["authorizationResponse"] = response.TraceInformation?.AuthorizationResponse ?? "N/A";

            return details;
        }

    }
}
