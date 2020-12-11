// Needed for NET40

#pragma warning disable CA1003 // Use generic event handler instances

namespace System
{
    public delegate void EventHandlerEx<in TEventArgs>(object sender, TEventArgs value);
}