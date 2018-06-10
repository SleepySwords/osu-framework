// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System.Collections.Generic;
using osu.Framework.Graphics;
using OpenTK;

namespace osu.Framework.Input
{
    public class PassThroughInputManager : CustomInputManager, IRequireHighFrequencyMousePosition
    {
        /// <summary>
        /// If there's an InputManager above us, decide whether we should use their available state.
        /// </summary>
        public bool UseParentState = true;

        internal override bool BuildKeyboardInputQueue(List<Drawable> queue)
        {
            if (!CanReceiveKeyboardInput) return false;

            if (UseParentState)
                queue.Add(this);
            return false;
        }

        internal override bool BuildMouseInputQueue(Vector2 screenSpaceMousePos, List<Drawable> queue)
        {
            if (!CanReceiveMouseInput) return false;

            if (UseParentState)
                queue.Add(this);
            return false;
        }

        protected override List<IInput> GetPendingInputs()
        {
            //we still want to call the base method to clear any pending states that may build up.
            var pendingInputs = base.GetPendingInputs();

            if (!UseParentState)
                return pendingInputs;

            pendingInputs.Clear();
            pendingInputs.AddRange(pendingParentInputs);

            pendingParentInputs.Clear();
            return pendingInputs;
        }

        private readonly List<IInput> pendingParentInputs = new List<IInput>();

        private bool acceptState(InputState state)
        {
            if (UseParentState)
                pendingParentInputs.Add(new LeagcyInputStateChange { InputState = new PassThroughInputState(state) });
            return false;
        }

        protected override bool OnMouseMove(InputState state)
        {
            if (UseParentState)
                new MousePositionAbsoluteInput { Position = state.Mouse.NativeState.Position }.Apply(CurrentState, this);
            return false;
        }

        protected override bool OnMouseDown(InputState state, MouseDownEventArgs args)
        {
            if (UseParentState)
                new MouseButtonInput { Button = args.Button, IsPressed = true }.Apply(CurrentState, this);
            return false;
        }

        protected override bool OnMouseUp(InputState state, MouseUpEventArgs args)
        {
            if (UseParentState)
                new MouseButtonInput { Button = args.Button, IsPressed = false }.Apply(CurrentState, this);
            return false;
        }

        protected override bool OnScroll(InputState state)
        {
            if (UseParentState)
                new MouseScrollRelativeInput { Delta = state.Mouse.NativeState.ScrollDelta }.Apply(CurrentState, this);
            return false;
        }

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args) => acceptState(state);

        protected override bool OnKeyUp(InputState state, KeyUpEventArgs args) => acceptState(state);

        protected override bool OnJoystickPress(InputState state, JoystickEventArgs args) => acceptState(state);

        protected override bool OnJoystickRelease(InputState state, JoystickEventArgs args) => acceptState(state);

        /// <summary>
        /// An input state which allows for transformations to state which don't affect the source state.
        /// </summary>
        public class PassThroughInputState : InputState
        {
            public PassThroughInputState(InputState state)
            {
                Mouse = (state.Mouse.NativeState as MouseState)?.Clone();
                Keyboard = (state.Keyboard as KeyboardState)?.Clone();
                Joystick = (state.Joystick as JoystickState)?.Clone();
            }
        }
    }
}
