// Needed for NET40

#pragma warning disable CA1003 // Replace EventHandlerEx with generic EventHandler.

namespace System
{
    public delegate void EventHandlerEx<in TEventArgs>(object sender, TEventArgs value);
}