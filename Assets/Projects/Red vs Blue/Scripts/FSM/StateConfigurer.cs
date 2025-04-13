using System.Collections.Generic;

namespace FSM {
	public class StateConfigurer<T>
    {
		RvB_State<T> instance;
		Dictionary<T, Transition<T>> transitions = new Dictionary<T, Transition<T>>();

		public StateConfigurer(RvB_State<T> state) {
			instance = state;
		}

		public StateConfigurer<T> SetTransition(T input, RvB_State<T> target) {
			transitions.Add(input, new Transition<T>(input, target));
			return this;
		}

		public void Done() {
			instance.Configure(transitions);
		}
	}

	public static class StateConfigurer
    {
		public static StateConfigurer<T> Create<T>(RvB_State<T> state)
        {
			return new StateConfigurer<T>(state);
		}
	}
}