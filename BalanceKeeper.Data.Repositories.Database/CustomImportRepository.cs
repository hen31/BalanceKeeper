using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BalanceKeeper.Data.Domain.Models;
using BalanceKeeper.Data.Entities;

namespace BalanceKeeper.Data.Repositories.Database
{
    public class CustomImportRepository : ImportRepository, ICustomImportRepository
    {
        public CustomImportRepository(ITransactionRepository transactionRepository, IRelationRepository relationRepository, ICategoryRepository categoryRepository)
        {
            _transactionRepository = transactionRepository;
            _relationRepository = relationRepository;
            _categoryRepository = categoryRepository;
        }
        public Task<ImportResults> ImportTransactionsFromCSVAsync(string fileContent, string seperator, bool hasHeaders, string addText, int columnStatement, int columnDate, int columnAmount, int columnAddOrMinus, int columnAccountNumberTo, int columnAccountNumberFrom, int columnRelationName)
        {
            return Task.Run<ImportResults>(
                async () =>
                {

                    ImportResults results = new ImportResults();
                    string[] lines = fileContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    int skip = 0;
                    if (hasHeaders)
                    {
                        skip = 1;
                    }
                    foreach (string line in lines.Skip(skip))
                    {
                        if (!await _transactionRepository.AlreadyImported(line.Trim()))
                        {
                            Transaction transaction = null;
                            try
                            {
                                //Datum,"Naam / Omschrijving","Rekening","Tegenrekening","Code","Af Bij","Bedrag (EUR)","MutatieSoort","Mededelingen"
                                //string[] values = line.Split(new string[] { seperator }, StringSplitOptions.None);
                                var values = line
           .Split(new string[] { seperator }, StringSplitOptions.None)
           .Select(s => s.Trim('"').Replace("\\\"", "\""))
           .ToArray();

                                string[] formats = { "ddMMyyyy","yyyyMMdd"};
                                transaction = new Transaction();
                                if (columnDate > 0 && columnDate - 1 < values.Length
                                && DateTime.TryParseExact(values[columnDate - 1].Replace("/","").Replace("-","").Replace(" ", ""), formats, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime valueDate))
                                {
                                    transaction.Date = valueDate;
                                }
                                if (columnAccountNumberTo > 0 && columnAccountNumberFrom - 1 < values.Length)
                                {
                                    transaction.AccountNumberFrom = values[columnAccountNumberFrom - 1];
                                }
                                if (columnAccountNumberTo > 0 && columnAccountNumberTo - 1 < values.Length)
                                {
                                    transaction.AccountNumberTo = values[columnAccountNumberTo - 1];
                                }
                                if (columnAmount > 0 && columnAmount - 1 < values.Length
                                && double.TryParse(values[columnAmount - 1], out double valueAmount))
                                {
                                    transaction.Amount = valueAmount;
                                }
                                if (columnAddOrMinus > 0 && columnAddOrMinus - 1 < values.Length
                                && !values[columnAddOrMinus - 1].Equals(addText, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    transaction.Amount = -transaction.Amount;
                                }
                                if (columnStatement > 0 && columnStatement - 1 < values.Length)
                                {
                                    transaction.Statement = values[columnStatement - 1];
                                }
                                if (!string.IsNullOrWhiteSpace(transaction.AccountNumberTo))
                                {
                                    transaction.Relation = await _relationRepository.FindRelationByAccountNumber(transaction.AccountNumberTo);
                                }
                                string relationName = string.Empty;
                                if (columnRelationName > 0 && columnRelationName - 1 < values.Length)
                                {
                                    relationName = values[columnRelationName - 1];
                                }
                                transaction.ImportLine = line.Trim();
                                await DetermineOrCreateRelation(transaction, results, relationName);
                                await _transactionRepository.SynchronizeTransactionAndRelationCategoriesAsync(transaction.Relation.ID);
                                transaction = await _transactionRepository.AddAsync(transaction);
                                transaction = await _transactionRepository.GetItemByIdAsync(transaction.ID);
                                results.ImportedTransactions.Add(transaction);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                        else
                        {
                            results.AlreadyImported++;
                        }
                    }
                    results.Success = true;
                    return results;
                });
        }


    }
}
