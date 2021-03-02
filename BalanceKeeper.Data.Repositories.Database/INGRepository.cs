using BalanceKeeper.Data.Domain.Models;
using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Data.Repositories.Database
{
    public class INGRepository : ImportRepository, IINGRepository
    {
        public INGRepository(ITransactionRepository transactionRepository, IRelationRepository relationRepository, ICategoryRepository categoryRepository)
        {
            _transactionRepository = transactionRepository;
            _relationRepository = relationRepository;
            _categoryRepository = categoryRepository;
        }

        public Task<ImportResults> ImportTransactionsFromCSVAsync(string fileContent)
        {
            return Task<ImportResults>.Run(async () =>
            {
       
                ImportResults results = new ImportResults();
                string[] lines = fileContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines.Skip(1))
                {
                    if (!await _transactionRepository.AlreadyImported(line.Trim()))
                    {
                        Transaction transaction = null;
                        try
                        {
                            //Datum,"Naam / Omschrijving","Rekening","Tegenrekening","Code","Af Bij","Bedrag (EUR)","MutatieSoort","Mededelingen"
                            string[] values = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                            transaction = new Transaction();
                            transaction.Date = DateTime.ParseExact(values[0].Substring(1), "yyyyMMdd", CultureInfo.InvariantCulture);//20170619
                            transaction.AccountNumberFrom = values[2];
                            transaction.AccountNumberTo = values[3];
                            transaction.Code = values[4];
                            transaction.Amount = double.Parse(values[6]);
                            if (values[5].Equals("Af", StringComparison.InvariantCultureIgnoreCase))
                            {
                                transaction.Amount = -transaction.Amount;
                            }
                            transaction.MutationType = values[7];
                            transaction.Statement = values[8].Substring(0, values[8].Length - 1);
                            if (!string.IsNullOrWhiteSpace(transaction.AccountNumberTo))
                            {
                                transaction.Relation = await _relationRepository.FindRelationByAccountNumber(transaction.AccountNumberTo);
                            }
                            transaction.ImportLine = line.Trim();
                            await DetermineOrCreateRelation(transaction, results, values[1]);
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
