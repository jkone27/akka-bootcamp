using System;
using Akka.Actor;

namespace WinTail
{
	public class Coordinator : UntypedActor
	{
		public class Start
		{
			public Start(string filePath, IActorRef reporterActor)
			{
				FilePath = filePath;
				ReporterActor = reporterActor;
			}

			public string FilePath { get; private set; }

			public IActorRef ReporterActor { get; private set; }
		}

		public class Stop
		{
			public Stop(string filePath)
			{
				FilePath = filePath;
			}

			public string FilePath { get; private set; }
		}
			
		protected override void OnReceive(object message)
		{
			if (message is Start)
			{
				var msg = message as Start;
				// here we are creating our first parent/child relationship!
				Context.ActorOf(Props.Create(
					() => new TailActor(msg.ReporterActor, msg.FilePath)));
			}
		}

		protected override SupervisorStrategy SupervisorStrategy()
		{
			return new OneForOneStrategy (
				10, // maxNumberOfRetries
				TimeSpan.FromSeconds(30),
				x => 
                    x is ArithmeticException ? 
                        Directive.Resume :
				    x is NotSupportedException ? 
                        Directive.Stop :
                    Directive.Restart);
		}
	}
}