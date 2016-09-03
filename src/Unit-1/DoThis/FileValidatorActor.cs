using System;
using Akka.Actor;
using WinTail.Messages;
using System.IO;

namespace WinTail
{
	public class FileValidatorActor : UntypedActor
	{
		private readonly IActorRef _consoleWriterActor;
		private readonly IActorRef _tailCoordinatorActor;

		public FileValidatorActor(IActorRef consoleWriterActor, 
			IActorRef tailCoordinatorActor)
		{
			_consoleWriterActor = consoleWriterActor;
			_tailCoordinatorActor = tailCoordinatorActor;
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
						new InputSuccess (string.Format ("Starting processing for {0}", msg)));

					_tailCoordinatorActor.Tell (new TailCoordinatorActor.StartTail (msg,
						_consoleWriterActor));
				} else {
					_consoleWriterActor.Tell (
						new ValidationError (string.Format("{0} is not an existing URI on disk.", msg)));

					Sender.Tell(new Messages.ContinueProcessing());

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

