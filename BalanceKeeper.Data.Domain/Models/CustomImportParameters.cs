using System;
using System.Collections.Generic;
using System.Text;

namespace BalanceKeeper.Data.Domain.Models
{
    public class CustomImportParameters
    {
        public CustomImportParameters()
        {

        }
        public CustomImportParameters(string fileContent, string seperator, bool hasHeaders, string addText, int columnStatement, int columnDate, int columnAmount, int columnAddOrMinus, int columnAccountNumberTo, int columnAccountNumberFrom, int columnRelationName)
        {
            FileContent = fileContent;
            Seperator = seperator;
            HasHeaders = hasHeaders;
            AddText = addText;
            ColumnStatement = columnStatement;
            ColumnDate = columnDate;
            ColumnAmount = columnAmount;
            ColumnAddOrMinus = columnAddOrMinus;
            ColumnAccountNumberTo = columnAccountNumberTo;
            ColumnAccountNumberFrom = columnAccountNumberFrom;
            ColumnRelationName = columnRelationName;
        }

        public string   FileContent { get; set; }
        public string   Seperator { get; set; }
        public bool HasHeaders { get; set; }
        public string AddText { get; set; }
        public int ColumnStatement { get; set; }
        public int ColumnDate { get; set; }
        public int ColumnAmount { get; set; }
        public int ColumnAddOrMinus { get; set; }
        public int ColumnAccountNumberTo { get; set; }
        public int ColumnAccountNumberFrom { get; set; }
        public int ColumnRelationName { get; set; }
    }
}
