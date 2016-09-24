// Needed for NET40

namespace Theraot.Core
{
#if NETCF
    public delegate void NewEventHandler<TEventArgs>(object sender, TEventArgs value);
#else
    public delegate void NewEventHandler<in TEventArgs>(object sender, TEventArgs value);
#endif
}