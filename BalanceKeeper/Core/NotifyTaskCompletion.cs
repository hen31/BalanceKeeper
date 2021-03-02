using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Core
{
    public abstract class NotifyTaskCompletion : INotifyPropertyChanged
    {
        public NotifyTaskCompletion(Task task, EventHandler completedHandler = null, EventHandler failedHandler = null)
        {
            if (completedHandler != null)
            {
                OnCompletedSuccessfully += completedHandler;
            }
            if (failedHandler != null)
            {
                OnFailed += failedHandler;
            }
            Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }
        protected async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this,
                  new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
                /* if (!hasFiredCompleted)
                 {
                     hasFiredCompleted = true;
                     OnFailed?.Invoke(this, new EventArgs());
                 }*/
            }
            else
            {
                propertyChanged(this,
                  new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                propertyChanged(this, new PropertyChangedEventArgs("Result"));
                // FireCompletedSuccesfully();
            }
        }

        private void FireCompletedSuccesfully()
        {
            if (!hasFiredCompleted)
            {
                hasFiredCompleted = true;
                if (OnCompletedSuccessfully != null)
                    OnCompletedSuccessfully.Invoke(this, new EventArgs());
            }
        }

        public virtual Task Task { get; protected set; }
        public virtual object Result
        {
            get
            {

                return default(object);
            }
        }
        public TaskStatus Status
        {
            get
            {
                if (!hasFiredCompleted && Task.Status == TaskStatus.RanToCompletion)
                {
                    FireCompletedSuccesfully();
                }
                else if (!hasFiredCompleted && Task.Status == TaskStatus.Faulted)
                {
                    hasFiredCompleted = true;
                    OnFailed?.Invoke(this, new EventArgs());
                }
                return Task.Status;
            }
        }
        public bool IsCompleted { get { return Task.IsCompleted; } }
        public bool IsNotCompleted { get { return !Task.IsCompleted; } }
        public bool IsSuccessfullyCompleted
        {
            get
            {
                return Status == TaskStatus.RanToCompletion;
            }
        }
        public bool IsCanceled { get { return Task.IsCanceled; } }
        public bool IsFaulted { get { return Task.IsFaulted; } }
        public AggregateException Exception { get { return Task.Exception; } }
        public Exception InnerException
        {
            get 
            {
                return (Exception == null) ? null : Exception.InnerException;
            }
        }
        public string ErrorMessage
        {
            get
            {
                return (InnerException == null) ? null : InnerException.Message;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected event EventHandler OnCompletedSuccessfully;
        protected event EventHandler OnFailed;

        protected bool hasFiredCompleted = false;

        public string Test { get; set; } = "HALLLOOOOO";
    }

    public sealed class NotifyTaskCompletion<TResult> : NotifyTaskCompletion
    {
        public NotifyTaskCompletion(Task<TResult> task, EventHandler completedHandler = null, EventHandler failedHandler = null)
                : base(task, completedHandler, failedHandler)
        {
        }


        public new Task<TResult> Task
        {
            get
            {
                return (Task<TResult>)base.Task;
            }
            private set
            {
                base.Task = value;
            }
        }

        public new TResult Result
        {
            get
            {

                return (Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult);
            }
        }


    }
}
