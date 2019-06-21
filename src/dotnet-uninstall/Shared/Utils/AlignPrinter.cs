using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Utils
{
    internal class AlignPrinter<TTuple> where TTuple : ITuple
    {
        private readonly IList<AlignType> _alignTypes;
        private readonly string _separator;
        private readonly string _prefix;
        private readonly string _suffix;

        public AlignPrinter(IList<AlignType> alignTypes, string separator = " ", string prefix = "", string suffix = "")
        {
            _alignTypes = alignTypes;
            _separator = separator;
            _prefix = prefix;
            _suffix = suffix;
        }

        public string GetFormatString(IEnumerable<TTuple> tuples)
        {
            if (tuples == null)
            {
                throw new ArgumentNullException();
            }

            if (tuples.Any(tuple => tuple.Length != _alignTypes.Count))
            {
                throw new ArgumentException();
            }

            var columnFormatStrings = Enumerable
                .Range(0, _alignTypes.Count)
                .Select(column =>
                (
                    column,
                    alignType: _alignTypes[column],
                    maxLength: tuples.Select(tuple => tuple[column].ToString().Length as int?).Max() ?? 0
                ))
                .Select(tuple => $"{{{tuple.column}, {GetAlignString(tuple.alignType)}{tuple.maxLength}}}");

            return $"{_prefix}{string.Join(_separator, columnFormatStrings)}{_suffix}";
        }

        public IEnumerable<string> GetAlignedRows(IEnumerable<TTuple> tuples)
        {
            var format = GetFormatString(tuples);

            if (tuples.Any(tuple => tuple.Length != _alignTypes.Count))
            {
                throw new ArgumentException();
            }

            return tuples.Select(tuple =>
            {
                var args = Enumerable
                    .Range(0, _alignTypes.Count)
                    .Select(column => tuple[column]);

                return string.Format(format, args.ToArray());
            });
        }

        public void Print(IEnumerable<TTuple> tuples)
        {
            var aligned = GetAlignedRows(tuples);

            foreach (var row in aligned)
            {
                Console.WriteLine(row);
            }
        }

        private string GetAlignString(AlignType type)
        {
            switch (type)
            {
                case AlignType.Left: return "-";
                case AlignType.Right: return "";
                default: throw new InvalidEnumArgumentException();
            }
        }
    }

    internal enum AlignType
    {
        Left,
        Right
    }
}
