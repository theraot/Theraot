namespace System
{
    public static class FormattableStringEx
    {
        public static string CurrentCulture(FormattableString formattable)
        {
            if (formattable == null)
            {
                throw new ArgumentNullException(nameof(formattable));
            }
            return formattable.ToString(Globalization.CultureInfo.CurrentCulture);
        }
    }
}