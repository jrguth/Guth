using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Orleans;

namespace Guth.OpenTrivia.GrainInterfaces
{
    public class ObserverSubscriptionManager<TObserver> where TObserver : IGrainObserver
    {
        private ImmutableList<TObserver> _subscribers;
        public ObserverSubscriptionManager()
        {
            _subscribers = ImmutableList.Create<TObserver>();
        }
        public void AddSubscriber(TObserver observer)
        {
            _subscribers.Add(observer);
        }

        public void RemoveSubscriber(TObserver observer)
        {
            _subscribers.Remove(observer);
        }

        public async Task Notify(Action<TObserver> onNotify)
        {
            await Task.WhenAll(_subscribers.Select(s => new Task(() => onNotify(s))));
        }
    }
}
