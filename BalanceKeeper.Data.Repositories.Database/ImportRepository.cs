using BalanceKeeper.Data.Domain.Models;
using BalanceKeeper.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace BalanceKeeper.Data.Repositories.Database
{
    public class ImportRepository
    {
        protected ITransactionRepository _transactionRepository;
        protected IRelationRepository _relationRepository;
        protected ICategoryRepository _categoryRepository;

        protected async Task DetermineOrCreateRelation(Transaction transaction, ImportResults results, string description)
        {
            if (transaction.Relation == null && !string.IsNullOrWhiteSpace(transaction.AccountNumberFrom))
            {
                transaction.Relation = await _relationRepository.FindRelationByAccountNumber(transaction.AccountNumberFrom);
            }
            if (transaction.Relation == null && !string.IsNullOrWhiteSpace(description))
            {
                transaction.Relation = await _relationRepository.FindRelationByDescription(description);
            }
            if (transaction.Relation == null)
            {
                Relation relation = new Relation();
                if(string.IsNullOrWhiteSpace(description))
                {
                    description = "Geen omschrijving";
                }
                relation.Name = description;
                relation.Descriptions.Add(new RelationDescription()
                {
                    Description = description,
                    UserId = ServiceResolver.GetService<IUserProvider>().GetUserId()
                });
                if (!string.IsNullOrWhiteSpace(transaction.AccountNumberFrom))
                {
                    relation.AccountNumbers.Add(new RelationAccountNumber()
                    {
                        AccountNumber = transaction.AccountNumberTo,
                        UserId = ServiceResolver.GetService<IUserProvider>().GetUserId()
                    });
                }
                await _categoryRepository.FindCategoriesForRelation(transaction, relation);
                transaction.Relation = await _relationRepository.AddAsync(relation);
                transaction.Relation = await _relationRepository.GetItemByIdAsync(relation.ID);
                if (transaction.Relation.CategoryLinks.Where(b => b.Category != null).Count() == 0)
                {
                    results.ImportedRelations.Insert(0, transaction.Relation);
                }
                else
                {
                    results.ImportedRelations.Add(transaction.Relation);
                }
            }
        }
    }
}