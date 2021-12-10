using System;
using System.Threading.Tasks;

namespace GameServices
{
	public enum Environment
	{
		Development,
		Production,
		Staging,
		Live,
	}

	public enum State
	{
		Disabled,
		Uninitialized,
		Initializing,
		Initialized,
		ShuttingDown,
		Failed,
	}

	public abstract class GameService
	{
		public State State { get; private set; } = State.Uninitialized;
		public event EventHandler InitializedEvent;
		public event EventHandler ShutdownEvent;
		public GameServicesConfig Config { get; private set; }

		protected virtual Task<bool> TryInitialize()
		{
			return Task.FromResult(true);
		}

		protected virtual Task<bool> TryShutdown()
		{
			return Task.FromResult(true);
		}

		public async Task Initialize(GameServicesConfig config)
		{
			if (State != State.Uninitialized)
				return;

			Config = config;
			State = State.Initializing;

			if (await TryInitialize())
			{
				OnInitialized();
			}
			else
			{
				State = State.Failed;
			}
		}

		public async Task Shutdown()
		{
			if (State != State.Initialized)
				return;

			State = State.ShuttingDown;

			if (await TryShutdown())
			{
				OnShutdown();
			}
			else
			{
				State = State.Failed;
			}
		}

		private void OnInitialized()
		{
			State = State.Initialized;

			EventHandler handler = InitializedEvent;

			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}

		private void OnShutdown()
		{
			EventHandler handler = ShutdownEvent;

			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}

			State = State.Uninitialized;
		}
	}

}