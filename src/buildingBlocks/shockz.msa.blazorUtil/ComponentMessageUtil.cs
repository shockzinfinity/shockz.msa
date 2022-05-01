using System.Reflection;

namespace shockz.msa.blazorUtil;

public interface IComponentMessageUtil
{
  void Send<TSender, TArgs>(TSender sender, string message, TArgs args) where TSender : class;
  void Send<TSender>(TSender sender, string message) where TSender : class;
  void Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source = null) where TSender : class;
  void Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender source = null) where TSender : class;
  void Unsubscribe<TSender, TArgs>(object subscriber, string message) where TSender : class;
  void Unsubscribe<TSender>(object subscriber, string message) where TSender : class;
}

public class ComponentMessageUtil : IComponentMessageUtil
{
  public static IComponentMessageUtil Instance = new ComponentMessageUtil();

  class Sender : Tuple<string, Type, Type>
  {
    public Sender(string message, Type senderType, Type argType) : base(message, senderType, argType) { }
  }

  delegate bool Filter(object sender);

  class MaybeWeakReference
  {
    WeakReference DelegateWeakReference { get; }
    object DelegateStrongReference { get; }
    readonly bool _isStrongReference;

    public object Target => _isStrongReference ? DelegateStrongReference : DelegateWeakReference.Target;
    public bool IsAlive => _isStrongReference || DelegateWeakReference.IsAlive;

    public MaybeWeakReference(object subscriber, object delegateSource)
    {
      if (subscriber.Equals(delegateSource)) {
        // The target is the subscriber; so we can use a weakReference
        DelegateWeakReference = new WeakReference(delegateSource);
        _isStrongReference = false;
      } else {
        DelegateStrongReference = delegateSource;
        _isStrongReference = true;
      }
    }
  }

  class Subscription : Tuple<WeakReference, MaybeWeakReference, MethodInfo, Filter>
  {
    public Subscription(object subscriber, object delegateSource, MethodInfo methodInfo, Filter filter)
      : base(new WeakReference(subscriber), new MaybeWeakReference(subscriber, delegateSource), methodInfo, filter) { }

    public WeakReference Subscriber => Item1;
    MaybeWeakReference DelegateSource => Item2;
    MethodInfo MethodInfo => Item3;
    Filter Filter => Item4;

    public void InvokeCallback(object sender, object args)
    {
      if (!Filter(sender)) {
        return;
      }

      if (MethodInfo.IsStatic) {
        MethodInfo.Invoke(null, MethodInfo.GetParameters().Length == 1 ? new[] { sender } : new[] { sender, args });
        return;
      }

      var target = DelegateSource.Target;

      if (target == null) {
        return; // collected
      }

      MethodInfo.Invoke(target, MethodInfo.GetParameters().Length > 1 ? new[] { sender } : new[] { sender, args });
    }

    public bool CanBeRemoved()
    {
      return !Subscriber.IsAlive || !DelegateSource.IsAlive;
    }
  }

  readonly Dictionary<Sender, List<Subscription>> _subscriptions = new Dictionary<Sender, List<Subscription>>();

  public static void Send<TSender, TArgs>(TSender sender, string message, TArgs args) where TSender : class
  {
    Instance.Send(sender, message, args);
  }

  void IComponentMessageUtil.Send<TSender, TArgs>(TSender sender, string message, TArgs args)
  {
    if (sender == null) throw new ArgumentNullException(nameof(sender));

    InnerSend(message, typeof(TSender), typeof(TArgs), sender, args);
  }

  public static void Send<TSender>(TSender sender, string message) where TSender : class
  {
    Instance.Send(sender, message);
  }

  void IComponentMessageUtil.Send<TSender>(TSender sender, string message)
  {
    if (sender == null) throw new ArgumentNullException(nameof(sender));

    InnerSend(message, typeof(TSender), null, sender, null);
  }

  void InnerSend(string message, Type senderType, Type argType, object sender, object args)
  {
    if (message == null) throw new ArgumentNullException(nameof(message));

    var key = new Sender(message, senderType, argType);
    if (!_subscriptions.ContainsKey(key)) return;

    List<Subscription> subscriptions = _subscriptions[key];
    if (subscriptions == null || !subscriptions.Any()) return; // should not be reachable

    // ok so this code looks a bit funky but here is the gist of the problem. It is possible that in the course
    // of executing the callbacks for this message someone will subscribe/unsubscribe from the same message in
    // the callback. This would invalidate the enumerator. To work around this we make a copy. However if you unsubscribe 
    // from a message you can fairly reasonably expect that you will therefor not receive a call. To fix this we then
    // check that the item we are about to send the message to actually exists in the live list.
    List<Subscription> subscriptionCopy = subscriptions.ToList();
    foreach (var subscription in subscriptionCopy) {
      if (subscription.Subscriber.Target != null && subscriptions.Contains(subscription)) {
        subscription.InvokeCallback(sender, args);
      }
    }
  }

  public static void Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source = null) where TSender : class
  {
    Instance.Subscribe(subscriber, message, callback, source);
  }

  void IComponentMessageUtil.Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source)
  {
    if (subscriber == null)
      throw new ArgumentNullException(nameof(subscriber));
    if (callback == null)
      throw new ArgumentNullException(nameof(callback));

    var target = callback.Target;

    Filter filter = sender =>
    {
      var send = (TSender)sender;
      return (source == null || send == source);
    };

    InnerSubscribe(subscriber, message, typeof(TSender), typeof(TArgs), target, callback.GetMethodInfo(), filter);
  }

  public static void Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender source = null) where TSender : class
  {
    Instance.Subscribe(subscriber, message, callback, source);
  }

  void IComponentMessageUtil.Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender source)
  {
    if (subscriber == null)
      throw new ArgumentNullException(nameof(subscriber));
    if (callback == null)
      throw new ArgumentNullException(nameof(callback));

    var target = callback.Target;

    Filter filter = sender =>
    {
      var send = (TSender)sender;
      return (source == null || send == source);
    };

    InnerSubscribe(subscriber, message, typeof(TSender), null, target, callback.GetMethodInfo(), filter);
  }

  void InnerSubscribe(object subscriber, string message, Type senderType, Type argType, object target, MethodInfo methodInfo, Filter filter)
  {
    if (message == null)
      throw new ArgumentNullException(nameof(message));
    var key = new Sender(message, senderType, argType);
    var value = new Subscription(subscriber, target, methodInfo, filter);
    if (_subscriptions.ContainsKey(key)) {
      _subscriptions[key].Add(value);
    } else {
      var list = new List<Subscription> { value };
      _subscriptions[key] = list;
    }
  }

  public static void Unsubscribe<TSender, TArgs>(object subscriber, string message) where TSender : class
  {
    Instance.Unsubscribe<TSender, TArgs>(subscriber, message);
  }

  void IComponentMessageUtil.Unsubscribe<TSender, TArgs>(object subscriber, string message)
  {
    InnerUnsubscribe(message, typeof(TSender), typeof(TArgs), subscriber);
  }

  public static void Unsubscribe<TSender>(object subscriber, string message) where TSender : class
  {
    Instance.Unsubscribe<TSender>(subscriber, message);
  }

  void IComponentMessageUtil.Unsubscribe<TSender>(object subscriber, string message)
  {
    InnerUnsubscribe(message, typeof(TSender), null, subscriber);
  }
  void InnerUnsubscribe(string message, Type senderType, Type argType, object subscriber)
  {
    if (subscriber == null)
      throw new ArgumentNullException(nameof(subscriber));
    if (message == null)
      throw new ArgumentNullException(nameof(message));

    var key = new Sender(message, senderType, argType);
    if (!_subscriptions.ContainsKey(key))
      return;
    _subscriptions[key].RemoveAll(sub => sub.CanBeRemoved() || sub.Subscriber.Target == subscriber);
    if (!_subscriptions[key].Any())
      _subscriptions.Remove(key);
  }

  // This is a bit gross; it only exists to support the unit tests in PageTests
  // because the implementations of ActionSheet, Alert, and IsBusy are all very
  // tightly coupled to the MessagingCenter singleton 
  internal static void ClearSubscribers()
  {
    (Instance as ComponentMessageUtil)?._subscriptions.Clear();
  }

  #region Usage

  /// <summary>
  /// Publish a message 
  /// </summary>
  //public void sendmessage()
  //{
  //  string valuetosend = "hi from component 1";
  //  ComponentMessageUtil.send(this, "greeting_message", valuetosend);
  //}

  /// <summary>
  /// Receive a message from destination component 
  /// Blazor WebAssembly
  /// </summary>
  //public void SubscribeToMessage()
  //{
  //  ComponentMessageUtil.Subscribe<Component1, string>(this, "greeting_message", (sender, value) =>
  //  {
  //    // Do actions against the value 
  //    // If the value is updating the component make sure to call 
  //    string greeting = $"Welcome {value}";
  //    StateHasChanged(); // To update the state of the component 
  //  });
  //}

  /// <summary>
  /// Blazor Server Notice
  /// </summary>
  //public void SubscribeToMessage()
  //{
  //  ComponentMessageUtil.Subscribe<Component1, string>(this, "greeting_message", async (sender, value) =>
  //  {
  //    // Do actions against the value 
  //    // If the value is updating the component make sure to call 
  //    // Use InvokeAsync() to switch execution to the Dispatcher when triggering rendering or component state
  //    await InvokeAsync(() => {
  //      string greeting = $"Welcome {value}";
  //      StateHasChanged(); // To update the state of the component 
  //    });
  //  });
  //}

  #endregion
}
