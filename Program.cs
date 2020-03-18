using System;
using System.Collections.Generic;
using System.Threading;

namespace ObserverTest
{
	public interface ISubscriber
	{
		public int HashCode { get; set; }
		public event Action<string> OnChange;
		public event Action<ISubscriber> ReceiveFromHub;
		public void OnChangeHandler(string invoker);
		public void ReceiveHandler(ISubscriber source);
		public void InvokeReceiveFromHub(ISubscriber source);
	}

	public abstract class AbstractEventHub<T>
	{
		public static List<T> subjects;

		public AbstractEventHub()
		{
			subjects = new List<T>();
		}

		public virtual void Connect(T subject)
		{
			if (subjects.Contains(subject)) return;

			subjects.Add(subject);

			Console.WriteLine($"SUBJECT {subject.GetHashCode()} was connected on hub");
		}

		public virtual void Disconnect(T subject)
		{
			subjects.Remove(subject);
		}

		public virtual void Send(T subject)
		{
			
		}

		public virtual void SendAll(T source)
		{
			
		}
		public virtual void Receive(T sender, string invokeSource)
		{

		}
	}
	public class EventHub : AbstractEventHub<ISubscriber>
	{
		private static EventHub instance;
		private EventHub() : base()
		{
			
		}
		public static EventHub GetInstance()
		{
			if (instance == null) instance = new EventHub();
			return instance;
		}
		public override void SendAll(ISubscriber source)
		{
			foreach (var item in subjects)
			{
				if(item.GetHashCode() != source.GetHashCode()) item.InvokeReceiveFromHub(source);
			}
		}

		public override void Receive(ISubscriber sender, string invokeSource)
		{
			Console.WriteLine($"{sender.GetType()}-[{sender.GetHashCode()}] ---> {invokeSource}");
		}
	}
	public class SubjectA : ISubscriber
	{
		private int hashCode;
		public int HashCode { get => hashCode; set => hashCode = value; }

		public EventHub eventHub;

		public event Action<string> OnChange;
		public event Action<ISubscriber> ReceiveFromHub;

		public SubjectA()
		{
			eventHub = EventHub.GetInstance();

			OnChange += OnChangeHandler;
			ReceiveFromHub += ReceiveHandler;
		}
		public void Logined()
		{
			OnChange?.Invoke(System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		public void OnChangeHandler(string invoker)
		{
			eventHub?.Receive(this, invoker);
		}

		public void ReceiveHandler(ISubscriber source)
		{
			Console.WriteLine($"{this.GetType()}-[{this.GetHashCode()}] was receive message from {source.GetType()}-[{source.GetHashCode()}]");
		}

		public void InvokeReceiveFromHub(ISubscriber source)
		{
			ReceiveFromHub?.Invoke(source);
		}
	}
	
	class Program
	{
		static void Main(string[] args)
		{
			SubjectA[] sA = new SubjectA[10];

			EventHub eventHub = EventHub.GetInstance();

			for (int i = 0; i < sA.Length; i++)
			{
				sA[i] = new SubjectA();
				eventHub.Connect(sA[i]);
			}

			Thread.Sleep(3000);

			for (int i = 0; i < sA.Length; i++)
			{
				sA[i].Logined();
			}

			Thread.Sleep(3000);

			eventHub.SendAll(sA[2]);

			Console.WriteLine("END");

			Console.ReadKey();
		}
	}
}
