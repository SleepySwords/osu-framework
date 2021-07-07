﻿using System;
using osuTK;

namespace osu.Framework.Platform.MacOS.Native
{
    internal readonly struct NSTouch
    {
        internal IntPtr Handle { get; }

        private static readonly IntPtr sel_normalizedposition = Selector.Get("normalizedPosition");
        private static readonly IntPtr sel_phase = Selector.Get("phase");
        private static readonly IntPtr sel_identity = Selector.Get("identity");
        private static readonly IntPtr sel_isequal = Selector.Get("isEqual:");
        private static readonly IntPtr sel_copy = Selector.Get("copy");

        public NSTouch(IntPtr handle)
        {
            Handle = handle;
        }

        internal Vector2 NormalizedPosition() => Cocoa.SendVector2d(Handle, sel_normalizedposition);

        internal NSTouchPhase Phase() => (NSTouchPhase)Cocoa.SendUint(Handle, sel_phase);

        internal IntPtr Identity() => Cocoa.SendIntPtr(Handle, sel_identity);

        internal IntPtr CopyOfIdentity() => Cocoa.SendIntPtr(Identity(), sel_copy);

        internal bool IsEqual(IntPtr intPtr) => Cocoa.SendBool(Identity(), sel_isequal, intPtr);
    }

    [Flags]
    internal enum NSTouchPhase : uint
    {
        NSTouchPhaseBegan = 1u << 0,
        NSTouchPhaseMoved = 1u << 1,
        NSTouchPhaseStationary = 1u << 2,
        NSTouchPhaseEnded = 1u << 3,
        NSTouchPhaseCancelled = 1u << 4,
        NSTouchPhaseTouching = NSTouchPhaseBegan | NSTouchPhaseMoved | NSTouchPhaseStationary,
        NSTouchPhaseAny = uint.MaxValue
    }
}
