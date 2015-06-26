// Needed for NET40

namespace Theraot.Core
{
    public delegate void NewEventHandler<in TEventArgs>(object sender, TEventArgs value);
}