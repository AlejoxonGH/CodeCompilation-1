using System;

namespace FSM
{
	public class EventFSM<T>
    {
		public RvB_State<T> Current { get { return current; } }
		private RvB_State<T> current;

		public EventFSM(RvB_State<T> initial)
        {
			current = initial;
			current.Enter(default(T));
		}

		public void SendInput(T input)
        {
			RvB_State<T> newState;

			if (current.CheckInput(input, out newState))
            {
				current.Exit(input);
				current = newState;
				current.Enter(input);
			}
		}


		public void Update()
        {
			current.Update();
		}

        public void LateUpdate()
        {
            current.LateUpdate();
        }

        public void FixedUpdate()
        {
            current.FixedUpdate();
        }
	}
}