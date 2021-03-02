using AutoMapper;
using BalanceKeeper.Data;
using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace BalanceKeeper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MapperConfiguration MapperConfiguration { get; set; }
        public static IMapper Mapper { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                    CultureInfo.CurrentCulture.Name)));

            base.OnStartup(e);
            MapperConfiguration = new MapperConfiguration((cfg) =>
             {
                 cfg.CreateMap<Relation, Relation>();
                 cfg.CreateMap<RelationAccountNumber, RelationAccountNumber>();
                 cfg.CreateMap<RelationDescription, RelationDescription>();
                 cfg.CreateMap<CategoryRelationLink, CategoryRelationLink>();

                 cfg.CreateMap<Transaction, Transaction>();
                 cfg.CreateMap<CategoryTransactionLink, CategoryTransactionLink>();
                 cfg.CreateMap<Category, Category>();
             });
            Mapper = MapperConfiguration.CreateMapper();
            //create the deep copy using AutoMapper
        }


    }
}
