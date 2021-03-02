using BalanceKeeper.Classes;
using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Viewmodels
{
    public abstract class RepositoryOverviewViewmodel<T> : ViewModel where T : IEntity
    {
        public IDatabaseRepository<T> Repository { get; set; }
        public RepositoryOverviewViewmodel(IDatabaseRepository<T> repository)
        {
            Repository = repository;
            ReadonlyEditingItem = true;
            ReloadCurrentCollection();

            CurrentExecutingTasks = new ObservableCollection<TaskExecutorWrapper>();
            RemoveActionIfFaultedCommand = new RelayCommand(RemoveActionIfFaulted);
            SaveCommand = new PaidCommand(SaveEditingItem, CanSaveEditingItem);
            CancelCommand = new PaidCommand(CancelEditingItem, CanCancelEditingItem);
            EditCommand = new PaidCommand(EditItem, CanEditItem);
            AddCommand = new PaidCommand(AddNewItem, CanAddNewItem);
            DeleteCommand = new PaidCommand(DeleteItemAsync, CanDeleteItem);
            RefreshCommand = new RelayCommand(RefreshItems);
        }


        private void RemoveActionIfFaulted(object obj)
        {
            TaskExecutorWrapper wrapper = (TaskExecutorWrapper)obj;
            if (wrapper.TaskNotifier != null && wrapper.TaskNotifier.IsFaulted)
            {
                CurrentExecutingTasks.Remove(wrapper);
            }
        }


        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }

        private async Task<object> test()
        {
            await Task.Delay(25000000);
            return Task.FromResult((object)"");
        }
        public RelayCommand RemoveActionIfFaultedCommand { get; set; }

        public ObservableCollection<TaskExecutorWrapper> CurrentExecutingTasks { get; set; }

        private void RefreshItems(object obj)
        {
            ReloadCurrentCollection();
        }

        public virtual void ReloadCurrentCollection()
        {
            CurrentCollection = new NotifyTaskCompletion<ObservableCollection<T>>(CreateCurrentCollectionAsync(), (s, e) =>
            {
                if (CurrentCollection.Result != null)
                {
                    SelectedItem = CurrentCollection.Result.FirstOrDefault();
                }
            }, (f, g) =>
            {
                EditingItem = new NotifyTaskCompletion<T>(Task.FromResult(Repository.CreateNewObject()));
            });
        }

        protected virtual async Task<ObservableCollection<T>> CreateCurrentCollectionAsync()
        {
            var collection = await Repository.GetCollectionAsync();
            return new ObservableCollection<T>(collection);
        }

        private bool CanDeleteItem(object obj)
        {
            return !ReadonlyEditingItem && EditingItem != null
                && EditingItem.IsSuccessfullyCompleted
                && EditingItem.Result.ID > 0
                && SavingItem == null;
        }

        private void DeleteItemAsync(object obj)
        {
            var itemToDelete = EditingItem.Result;
            CurrentCollection.Result.Remove(CurrentCollection.Result.Where(b => b.ID == itemToDelete.ID).SingleOrDefault());
            var deletingTask = Repository.DeleteAsync(itemToDelete);
            var task = new NotifyTaskCompletion<bool>(deletingTask, (s, e) =>
            {
                if (deletingTask.Result)
                {
                    ReadonlyEditingItem = true;
                    OnItemDeleted();
                }
            }, (s, e) =>
            {
                ReloadCurrentCollection();
            });
            CurrentExecutingTasks.Add(new TaskExecutorWrapper() { TaskNotifier = task, BusyText = "Bezig met verwijderen" });
            ReadonlyEditingItem = true;
            EditingItem = new NotifyTaskCompletion<T>(Task.FromResult<T>(default(T)));
        }

        protected virtual void OnItemDeleted() { }

        private bool CanAddNewItem(object obj)
        {
            return ReadonlyEditingItem && SavingItem == null;
        }
        private void AddNewItem(object obj)
        {
            ReadonlyEditingItem = false;
            EditingItem = new NotifyTaskCompletion<T>(Task.FromResult(Repository.CreateNewObject()));
            EditingTitle = string.Format("Nieuwe item toevoegen");
            StartingAdd();
        }

        protected virtual void StartingAdd() { }

        private bool CanCancelEditingItem(object obj)
        {
            return !ReadonlyEditingItem && SavingItem == null;
        }
        private void CancelEditingItem(object obj)
        {
            Repository.CancelEdit(EditingItem.Result);
            EditingItem = new NotifyTaskCompletion<T>(Task.FromResult(_selectedItem), (s, e) =>
            {
                AfterCancelEdit();
                ReadonlyEditingItem = true;
                if (_selectedItem != null)
                {
                    EditingTitle = string.Format("'{0}' inzien", _selectedItem.GetDescription());
                }
                else
                {
                    EditingTitle = string.Format("Detail");
                }
            });
        }

        protected virtual void AfterCancelEdit()
        {
        }

        private bool CanEditItem(object obj)
        {
            return EditingItem != null
                && ((EditingItem.IsSuccessfullyCompleted
                && EditingItem.Result != null) || EditingItem.IsFaulted)
                && EditingItem.Result.ID > 0
                && SavingItem == null
                && ReadonlyEditingItem;

        }
        private void EditItem(object obj)
        {
            EditingItem = new NotifyTaskCompletion<T>(Repository.GetItemByIdAsync(_selectedItem.ID), (s, e) =>
            {
                OnStartingEdit();
                ReadonlyEditingItem = false;
                EditingTitle = string.Format("'{0}' wijzigen", _selectedItem.GetDescription());
            }, (s, e) =>
            {
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
            );
        }

        protected virtual void OnStartingEdit()
        {
        }

        private bool CanSaveEditingItem(object obj)
        {
            return EditingItem != null && EditingItem.IsSuccessfullyCompleted && EditingItem.Result != null && CanSave() && !ReadonlyEditingItem && (EditingItem.Result.ID > 0 || SavingItem == null);
        }

        internal abstract bool CanSave();

        private void SaveEditingItem(object obj)
        {
            ReadonlyEditingItem = true;
            if (EditingItem.Result.ID > 0)
            {
                OnBeforeItemUpdated();
                SavingItem = new NotifyTaskCompletion<T>(Repository.UpdateAsync(EditingItem.Result.ID, EditingItem.Result),
                    (s, e) =>
                    {
                        NotifyTaskCompletion<T> sender = (NotifyTaskCompletion<T>)s;
                        Repository.UntrackItem(sender.Result);
                        CurrentCollection.Result[CurrentCollection.Result.IndexOf(CurrentCollection.Result.Where(b => b.ID == sender.Result.ID).FirstOrDefault())] = sender.Result;
                        OnItemUpdated();
                        CurrentExecutingTasks.Remove(CurrentExecutingTasks.Where(b => b.TaskNotifier == sender).FirstOrDefault());
                        SavingItem = null;
                        SelectedItem = sender.Result;
                        EditingItem = new NotifyTaskCompletion<T>(Task.FromResult(_selectedItem), (f, g) =>
                        {
                            ReadonlyEditingItem = true;
                        });
                    }, (s, e) =>
                    {
                        SavingItem = null;
                        ReadonlyEditingItem = false;
                    });

            }
            else
            {
                OnBeforeItemAdded();
                SavingItem = new NotifyTaskCompletion<T>(Repository.AddAsync(EditingItem.Result),
                 (s, e) =>
                 {
                     NotifyTaskCompletion<T> sender = (NotifyTaskCompletion<T>)s;
                     Repository.UntrackItem(sender.Result);
                     CurrentCollection.Result.Add(sender.Result);
                     OnItemAdded();
                     CurrentExecutingTasks.Remove(CurrentExecutingTasks.Where(b => b.TaskNotifier == SavingItem).FirstOrDefault());
                     SavingItem = null;
                     SelectedItem = sender.Result;
                     EditingItem = new NotifyTaskCompletion<T>(Task.FromResult(_selectedItem), (f, g) =>
                     {
                         ReadonlyEditingItem = true;
                     });
                 }, (s, e) =>
                 {
                     SavingItem = null;
                     ReadonlyEditingItem = false;
                 });
            }
            CurrentExecutingTasks.Add(new TaskExecutorWrapper() { TaskNotifier = SavingItem, BusyText = "Bezig met opslaan" });
        }

        protected virtual void OnBeforeItemAdded()
        {
        }
        protected virtual void OnBeforeItemUpdated()
        {
        }
        internal virtual void OnItemAdded()
        {
        }
        internal virtual void OnItemUpdated()
        {

        }

        string _editingTitle;
        public string EditingTitle
        {
            get
            {
                return _editingTitle;
            }
            set
            {
                _editingTitle = value;
                OnPropertyChanged("EditingTitle");
            }
        }



        T _selectedItem;
        public T SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                SelectedItemChanged();
                if (ReadonlyEditingItem)
                {
                    if (_selectedItem == null)
                    {
                        EditingTitle = string.Format("Detail");
                        EditingItem = new NotifyTaskCompletion<T>(Task.FromResult(Repository.CreateNewObject()));

                    }
                    else
                    {
                        EditingTitle = string.Format("'{0}' inzien", _selectedItem?.GetDescription());
                        EditingItem = new NotifyTaskCompletion<T>(Task.FromResult(_selectedItem));
                    }
                }
            }
        }

        protected virtual void SelectedItemChanged()
        {
        }

        NotifyTaskCompletion<T> _savingItem;
        public NotifyTaskCompletion<T> SavingItem
        {
            get
            {
                return _savingItem;
            }
            set
            {
                _savingItem = value;
                OnPropertyChanged("SavingItem");
            }
        }

        NotifyTaskCompletion<T> _editingItem;
        public NotifyTaskCompletion<T> EditingItem
        {
            get
            {
                return _editingItem;
            }
            set
            {
                _editingItem = value;
                OnPropertyChanged(nameof(EditingItem));
            }
        }

        NotifyTaskCompletion<ObservableCollection<T>> _currentCollection;
        public NotifyTaskCompletion<ObservableCollection<T>> CurrentCollection
        {
            get
            {
                return _currentCollection;
            }
            set
            {
                _currentCollection = value;
                OnPropertyChanged(nameof(CurrentCollection));
            }
        }

        bool _readonlyEditingItem;
        public bool ReadonlyEditingItem
        {
            get
            {
                return _readonlyEditingItem;
            }
            set
            {
                _readonlyEditingItem = value;
                OnPropertyChanged("ReadonlyEditingItem");
            }
        }
    }
}