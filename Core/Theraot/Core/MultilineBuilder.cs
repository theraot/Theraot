#if FAT

using System.Collections.Generic;
using System.Text;

using Theraot.Factories;

namespace Theraot.Core
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class MultilineBuilder : IAggregator<string, string>
    {
        private readonly string _default;
        private readonly string _prompt;

        private bool _addEndingNewLine;
        private bool _addStartPrompt;
        private string _endingNewLine;
        private bool _first;
        private StringBuilder _wrapped;

        public MultilineBuilder(string prompt, string def)
        {
            _first = true;
            _prompt = prompt.Safe();
            _default = def.Safe();
            _wrapped = new StringBuilder();
            _endingNewLine = global::System.Environment.NewLine;
        }

        public MultilineBuilder(string prompt, string def, IEnumerable<string> items)
            : this(prompt, def)
        {
            this.AddItems(items);
        }

        public MultilineBuilder(string prompt, string def, string endingNewLine)
            : this(prompt, def)
        {
            _endingNewLine = endingNewLine ?? global::System.Environment.NewLine;
        }

        public MultilineBuilder(string prompt, string def, string endingNewLine, IEnumerable<string> items)
            : this(prompt, def)
        {
            _endingNewLine = endingNewLine ?? global::System.Environment.NewLine;
            this.AddItems(items);
        }

        public bool AddEndingNewLine
        {
            get
            {
                return _addEndingNewLine;
            }
            set
            {
                _addEndingNewLine = value;
            }
        }

        public bool AddStartPrompt
        {
            get
            {
                return _addStartPrompt;
            }

            set
            {
                _addStartPrompt = value;
            }
        }

        public string EndingNewLine
        {
            get
            {
                return _endingNewLine;
            }
            set
            {
                _endingNewLine = value;
            }
        }

        public string Create()
        {
            return (_addStartPrompt ? _prompt : string.Empty) + ToString() + (_addEndingNewLine ? EndingNewLine : string.Empty);
        }

        public void Process(string item)
        {
            if (_first)
            {
                _first = false;
            }
            else
            {
                _wrapped.Append(EndingNewLine).Append(_prompt);
            }
            _wrapped.Append(item);
        }

        public void Reset()
        {
            _first = true;
            _wrapped = new StringBuilder();
        }

        public override string ToString()
        {
            if (_first)
            {
                return _default;
            }
            else
            {
                return _wrapped.ToString();
            }
        }
    }
}

#endif