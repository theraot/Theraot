#if FAT

using System;

using Theraot.Factories;

namespace Theraot.Core
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class StringAssembler : IFactory<string>
    {
        private string _content;

        public StringAssembler(string template)
        {
            _content = template.Safe();
        }

        public string Create()
        {
            return _content;
        }

        public StringAssembler Replace(string oldValue, string newValue)
        {
            _content = _content.Replace(oldValue, newValue);
            return this;
        }

        public StringAssembler Replace<TInput>(string oldValue, TInput newValue, Converter<TInput, string> converter)
        {
            _content = _content.Replace(oldValue, Check.NotNullArgument(converter, "converter")(newValue));
            return this;
        }

        public StringAssembler ReplaceFactory(string oldValue, IFactory<string> factory)
        {
            return Replace(oldValue, Check.NotNullArgument(factory, "factory").Create());
        }

        public StringAssembler ReplaceFactory<TInput>(string oldValue, IFactory<TInput> factory, Converter<TInput, string> converter)
        {
            return Replace(oldValue, Check.NotNullArgument(converter, "converter")(Check.NotNullArgument(factory, "factory").Create()));
        }
    }
}

#endif