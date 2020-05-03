using System;

namespace Owl.GeneralTree
{
    public class GeneralTreeOptions
    {
        public string Hyphen { get; set; } = "-";

        public Func<object, object, bool> CheckSameNameExpression { get; set; }
    }
}
