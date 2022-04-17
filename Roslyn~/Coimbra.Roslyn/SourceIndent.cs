using System.Text;

namespace Coimbra.Roslyn
{
    public sealed class SourceIndent
    {
        public int Amount;

        private readonly StringBuilder _stringBuilder = new();

        private int _lastAmount;

        private string _lastString;

        public override string ToString()
        {
            if (_lastAmount == Amount)
            {
                return _lastString;
            }

            _stringBuilder.Clear();

            for (int i = 0; i < Amount; i++)
            {
                _stringBuilder.Append("    ");
            }

            _lastAmount = Amount;
            _lastString = _stringBuilder.ToString();

            return _lastString;
        }
    }
}
