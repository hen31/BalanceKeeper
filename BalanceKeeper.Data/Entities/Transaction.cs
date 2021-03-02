using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BalanceKeeper.Data.Entities
{
    //Datum,"Naam / Omschrijving","Rekening","Tegenrekening","Code","Af Bij","Bedrag (EUR)","MutatieSoort","Mededelingen"

    public class Transaction : IEntity
    {
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public string AccountNumberFrom  { get; set; }
        public string AccountNumberTo { get; set; }
        public string Code { get; set; }
        public double Amount { get; set; }
        public string MutationType { get; set; }
        public string Statement { get; set; }

        public long? RelationID { get; set; }
        public Relation Relation { get; set; }

        public string UserId { get; set; }

        public ObservableCollection<CategoryTransactionLink> CategoryLinks { get; set; } = new ObservableCollection<CategoryTransactionLink>();

        public string ImportLine { get; set; }
        public bool ChangedCategories { get; set; }


        public string GetDescription()
        {
            return Statement;
        }
        public long GetId()
        {
            return ID;
        }
    }
}
