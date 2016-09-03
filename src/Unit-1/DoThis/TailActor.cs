using System.IO;
using System.Text;
using Akka.Actor;

namespace WinTail
{
	public class TailActor : UntypedActor
	{

		public class FileWrite
		{
			public FileWrite(string fileName)
			{
				FileName = fileName;
			}

			public string FileName { get; private set; }
		}
			
		public class FileError
		{
			public FileError(string fileName, string reason)
			{
				FileName = fileName;
				Reason = reason;
			}

			public string FileName { get; private set; }

			public string Reason { get; private set; }
		}

		public class InitialRead
		{
			public InitialRead(string fileName, string text)
			{
				FileName = fileName;
				Text = text;
			}

			public string FileName { get; private set; }
			public string Text { get; private set; }
		}


		private readonly string _filePath;
		private readonly string _fullFilePath;
		private readonly IActorRef _reporterActor;
		private readonly FileObserver _observer;

		public TailActor(IActorRef reporterActor, string filePath)
		{
			_reporterActor = reporterActor;
			_filePath = filePath;
			_fullFilePath = Path.GetFullPath (_filePath);
			_observer = new FileObserver(Self, _fullFilePath);
			_observer.Start();

			var text = File.ReadAllText(_fullFilePath);
			Self.Tell(new InitialRead(_filePath, text));
		}

		protected override void OnReceive(object message)
		{
			if (message is FileWrite)
			{
				var text = File.ReadAllText(_fullFilePath);
				if (!string.IsNullOrEmpty(text))
				{
					_reporterActor.Tell(text);
				}

			}
			else if (message is FileError)
			{
				var fe = message as FileError;
				_reporterActor.Tell(string.Format("Tail error: {0}", fe.Reason));
			}
			else if (message is InitialRead)
			{
				var ir = message as InitialRead;
				_reporterActor.Tell(ir.Text);
			}
		}
	}
}