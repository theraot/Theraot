// Needed for NET40

namespace System
{
    public delegate void EventHandlerEx<in TEventArgs>(object sender, TEventArgs value);
}