using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BalanceKeeper.Data.Entities
{
    public class Relation : IEntity
    {
        public long ID { get; set; }
        public string Name { get; set; } = "";
        public bool OwnAccount { get; set; }
        public ICollection<RelationAccountNumber> AccountNumbers { get; set; } = new List<RelationAccountNumber>();
        public ICollection<RelationDescription> Descriptions { get; set; } = new List<RelationDescription>();
        public string UserId { get; set; }
        public ObservableCollection<CategoryRelationLink> CategoryLinks { get; set; } = new ObservableCollection<CategoryRelationLink>();

        public string GetDescription()
        {
            return Name;
        }

        public long GetId()
        {
            return ID;
        }
    }
}
