using System;
using Akka.Actor;
using WinTail.Messages;
using System.IO;

namespace WinTail
{
	public class FileValidatorActor : UntypedActor
	{
		private readonly IActorRef _consoleWriterActor;

		public FileValidatorActor(IActorRef consoleWriterActor)
		{
			_consoleWriterActor = consoleWriterActor;
		}

		protected override void OnReceive(object message)
		{
			var msg = message as string;
			if (string.IsNullOrEmpty (msg)) {
				_consoleWriterActor.Tell (
					new NullInputError ("Input was blank. Please try again."));
				
				Sender.Tell (new ContinueProcessing ());
			}
			else
			{
				if (IsValid (msg)) {
					_consoleWriterActor.Tell (
						new InputSuccess (String.Format ("Starting processing for {0}", msg)));

					Context.ActorSelection("akka://MyActorSystem/user/tailCoordinator")
                        .Tell (
                            new Coordinator.Start (msg, _consoleWriterActor));
				} else {
					_consoleWriterActor.Tell (
						new ValidationError (String.Format("{0} is not an existing URI on disk.", msg)));

					Sender.Tell(new ContinueProcessing());

				}
			}

			Sender.Tell(new ContinueProcessing());

		}

		private static bool IsValid(string path)
		{
			return File.Exists(path);
		}
	}

}

