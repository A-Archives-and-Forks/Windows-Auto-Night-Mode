using System;

namespace AutoDarkModeSvc.Events;

public class GovernorEventArgs : EventArgs
{
    /// <summary>
    /// Creates a new Governor event. Used to inform the governor module what state a governor is in.
    /// </summary>
    /// <param name="inSwitchWindow">If the governor is currently within the defined switch window</param>
    /// <param name="switchEventArgs">The event args of the switch request</param>
    /// <param name="instantSwitchWindow">
    /// If the switch window has no duration, meaning the governor module has to trigger the switch approach
    /// dependency modules itself before requesting the switch
    /// </param>
    public GovernorEventArgs(bool inSwitchWindow, SwitchEventArgs switchEventArgs, bool instantSwitchWindow = false)
    {
        InSwitchWindow = inSwitchWindow;
        SwitchEventArgs = switchEventArgs;
        InstantSwitchWindow = instantSwitchWindow;
    }

    /// <summary>
    /// Creates a new Governor event. Used to inform the governor module what state a governor is in.
    /// </summary>
    /// <param name="inSwitchWindow">If the governor is currently within the defined switch window</param>
    public GovernorEventArgs(bool inSwitchWindow)
    {
        InSwitchWindow = inSwitchWindow;
    }

    public bool InSwitchWindow { get; }
    public bool InstantSwitchWindow { get; }
    public SwitchEventArgs SwitchEventArgs { get; } = null;
}
