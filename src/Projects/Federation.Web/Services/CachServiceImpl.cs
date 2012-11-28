using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Federation.Core;

namespace Federation.Web
{
    [Export(typeof(ICachService))]
    public class CachServiceImpl : ICachService
    {
        private static readonly IDictionary<string, ICachedViewModel> Cach = new Dictionary<string, ICachedViewModel>(); // TODO: возможно стоит заменить на канкарент дикшенари
        private static readonly IDictionary<string, Guid> CashModelActuality = new Dictionary<string, Guid>();
        private static readonly ConcurrentQueue<ICommand> CommandQueue = new ConcurrentQueue<ICommand>();
        private static bool _isCommandExecuting;
        private delegate void ExecuteCallback();

        public ICachedViewModel GetViewModel(ICachedViewModel startModel)
        {
            ICachedViewModel result;

            if (Cach.ContainsKey(startModel.CachKey))
            {
                result = Cach[startModel.CachKey];
                result.CachLastAccess = DateTime.Now;
                if (result.CachIsOutOfDate())
                {
                    result = startModel.Initialize();
                    AddNewViewModel(startModel);
                }
            }
            else
            {
                result = startModel.Initialize();
                AddNewViewModel(startModel);
            }

            return result;
        }

        public bool IsCachOutOfDate(string key)
        {
            return CashModelActuality.ContainsKey(key);
        }

        public void DropViewModel(string key)
        {
            CommandQueue.Enqueue(new DeleteCachCommand { Key = key });
            ExecuteCallback pollCallback = ExecuteCommands;
            pollCallback.BeginInvoke(null, null);
        }

        public void DropViewModelByModel(Guid modelId)
        {
            CommandQueue.Enqueue(new DeleteCachByModelCommand { ModelId = modelId });
            ExecuteCallback pollCallback = ExecuteCommands;
            pollCallback.BeginInvoke(null, null);
        }

        public void AddNewViewModel(ICachedViewModel startModel)
        {
            CommandQueue.Enqueue(new AddCachCommand { ViewModel = startModel });
            ExecuteCallback pollCallback = ExecuteCommands;
            pollCallback.BeginInvoke(null, null);
        }

        public void ClearOldCach()
        {
            var oldDate = DateTime.Now.AddHours(-6);
            var item = Cach.SingleOrDefault(x => x.Value.CachLastAccess <= oldDate);

            while (item.Key != null)
            {
                CommandQueue.Enqueue(new DeleteCachCommand { Key = item.Key });
                item = Cach.SingleOrDefault(x => x.Value.CachLastAccess <= oldDate);
            }

            ExecuteCallback pollCallback = ExecuteCommands;
            pollCallback.BeginInvoke(null, null);
        }

        private void ExecuteCommands()
        {
            if (!_isCommandExecuting)
            {
                _isCommandExecuting = true;

                while (CommandQueue.Count != 0)
                {
                    ICommand command;
                    CommandQueue.TryDequeue(out command);

                    if (command != null)
                        command.Execute();

                    Thread.Sleep(10);
                }

                _isCommandExecuting = false;
            }
        }

        private interface ICommand
        {
            void Execute();
        }

        private class AddCachCommand : ICommand
        {
            public ICachedViewModel ViewModel { get; set; }

            public void Execute()
            {
                if (ViewModel != null)
                {
                    if (Cach.ContainsKey(ViewModel.CachKey))
                    {
                        Cach.Remove(ViewModel.CachKey);
                        CashModelActuality.Remove(ViewModel.CachKey);
                    }

                    Cach.Add(ViewModel.CachKey, ViewModel);
                    CashModelActuality.Add(ViewModel.CachKey, ViewModel.RelativeModelId);
                }
            }
        }

        private class DeleteCachCommand : ICommand
        {
            public string Key { get; set; }

            public void Execute()
            {
                if (Key != null && Cach.ContainsKey(Key))
                {
                    Cach.Remove(Key);
                    CashModelActuality.Remove(Key);
                }
            }
        }

        private class DeleteCachByModelCommand : ICommand
        {
            public Guid ModelId { get; set; }

            public void Execute()
            {
                var cach = CashModelActuality.FirstOrDefault(x => x.Value == ModelId);

                while (cach.Key != null)
                {
                    Cach.Remove(cach.Key);
                    CashModelActuality.Remove(cach.Key);
                    cach = CashModelActuality.FirstOrDefault(x => x.Value == ModelId);
                }
            }
        }
    }    
}
