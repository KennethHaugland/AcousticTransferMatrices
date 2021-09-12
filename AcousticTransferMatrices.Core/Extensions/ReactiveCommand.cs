using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AcousticTransferMatrices.Core.Extensions
{

    //public ViewModel()
    //{
    //    IObservable<bool> initPredicate = this.ObserveProperty(x => x.Title)
    //             .StartWith(this.Title).Select(x => !string.IsNullOrEmpty(x)); ;
    //    IObservable<bool> predicate = this.ObserveProperty(x => x.HasStuff)
    //             .StartWith(this.HasStuff);
    //    SomeCommand = new ReactiveCommand(initPredicate, false);
    //    SomeCommand.AddPredicate(predicate);
    //    SomeCommand.CommandExecutedStream.Subscribe(x =>
    //    {
    //        MessageBox.Show("Command Running");
    //    });
    //}

    //public ReactiveCommand SomeCommand { get; set; }

    public interface IReactiveCommand : ICommand
    {
        IObservable<object> CommandExecutedStream { get; }
        IObservable<Exception> CommandExeceptionsStream { get; }
        void AddPredicate(IObservable<bool> predicate);
    }

    public class ReactiveCommand : IReactiveCommand, IDisposable
    {
        private Subject<object> commandExecutedSubject = new Subject<object>();
        private Subject<Exception> commandExeceptionsSubjectStream = new Subject<Exception>();
        private List<IObservable<bool>> predicates = new List<IObservable<bool>>();
        private IObservable<bool> canExecuteObs;
        private bool canExecuteLatest = true;
        private CompositeDisposable disposables = new CompositeDisposable();

        public ReactiveCommand()
        {
            RaiseCanExecute(true);
        }

        public ReactiveCommand(IObservable<bool> initPredicate, bool initialCondition)
        {
            if (initPredicate != null)
            {
                canExecuteObs = initPredicate;
                SetupSubscriptions();
            }
            RaiseCanExecute(initialCondition);
        }

        public void AddPredicate(IObservable<bool> predicate)
        {
            disposables.Dispose();
            predicates.Add(predicate);
            this.canExecuteObs = this.canExecuteObs.CombineLatest(
                    predicates.Last(), (a, b) => a && b).DistinctUntilChanged();
            SetupSubscriptions();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return canExecuteLatest;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            commandExecutedSubject.OnNext(parameter);
        }

        public IObservable<object> CommandExecutedStream
        {
            get { return this.commandExecutedSubject.AsObservable(); }
        }

        public IObservable<Exception> CommandExeceptionsStream
        {
            get { return this.commandExeceptionsSubjectStream.AsObservable(); }
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        protected virtual void RaiseCanExecuteChanged(EventArgs e)
        {
            this.CanExecuteChanged?.Invoke(this, e);
        }

        private void RaiseCanExecute(bool value)
        {
            canExecuteLatest = value;
            this.RaiseCanExecuteChanged(EventArgs.Empty);
        }

        private void SetupSubscriptions()
        {

            disposables = new CompositeDisposable();
            disposables.Add(this.canExecuteObs.Subscribe(
                //OnNext
                x =>
                {
                    RaiseCanExecute(x);
                },
                //onError
                commandExeceptionsSubjectStream.OnNext
            ));
        }
    }
}
