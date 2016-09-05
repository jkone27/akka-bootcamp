using System;
﻿using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
			MyActorSystem = 
				ActorSystem
					.Create("MyActorSystem");

			var consoleWriter = 
				MyActorSystem
					.ActorOf(
						Props.Create<ConsoleWriterActor>(),"writer");

			var tailCoordinator =
				MyActorSystem.ActorOf (Props.Create<Coordinator> (), "tailCoordinator");
			

			var validation = 
				MyActorSystem
					.ActorOf(
						Props.Create<FileValidatorActor>(consoleWriter),"validation");


			var consoleReader = 
				MyActorSystem
					.ActorOf(
						Props.Create<ConsoleReaderActor>(),"reader");


			consoleReader.Tell(ConsoleReaderActor.StartCommand);
			 
			MyActorSystem
				.WhenTerminated
				.Wait();
        }
			
    }
    #endregion
}
