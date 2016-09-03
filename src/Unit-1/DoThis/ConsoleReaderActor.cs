using System;
using Akka.Actor;
using WinTail.Messages;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";
		public const string StartCommand = "start";
        private IActorRef _validationActor;

		public ConsoleReaderActor(IActorRef validationActor)
        {
			_validationActor = validationActor;
        }

        protected override void OnReceive(object message)
        {
			if (message.Equals(StartCommand))
			{
				DoPrintInstructions();
			}

			GetAndValidateInput();
        }

		private void DoPrintInstructions()
		{
			Console.WriteLine("please provide filename inside this local execution folder or an absolute path.\n");
		}

		private void GetAndValidateInput() {
			
			var message = Console.ReadLine();

			if (!string.IsNullOrEmpty(message) && String.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
				Context.System.Terminate();
			else
				_validationActor.Tell (message);
		}

    }
}