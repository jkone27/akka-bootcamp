using System;
using Akka.Actor;

namespace WinTail
{
    class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";
		public const string StartCommand = "start";

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
				Context.ActorSelection("akka://MyActorSystem/user/validation").Tell (message);
		}

    }
}