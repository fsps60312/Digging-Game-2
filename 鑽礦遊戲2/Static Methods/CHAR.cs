using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2
{
    class CHAR
    {
        public static bool IsEmpty(char a) { return a == ' ' || a == '\t' || a == '\r' || a == '\n' || a == '　'; }
        public static bool IsLower(char a) { return a >= 'a' && a <= 'z'; }
        public static bool IsUpper(char a) { return a >= 'A' && a <= 'Z'; }
        public static bool IsLetter(char a) { return IsLower(a) || IsUpper(a); }
        public static bool IsDigit(char a) { return a >= '0' && a <= '9'; }
        public static bool IsDigitDoubleByte(char a) { return a >= '０' && a <= '９'; }
        public static bool IsLBrack(char a) { return a == '(' || a == '（'; }
        public static bool IsRBrack(char a) { return a == ')' || a == '）'; }
        public static bool IsDash(char a) { return a == '-' || a == '－'; }
        public static bool IsPlus(char a) { return a == '+' || a == '＋'; }
        public static bool IsSemicolon(char a) { return a == ';' || a == '；'; }
        public static bool IsColon(char a) { return a == ':' || a == '：'; }
        public static bool IsSpecialSymbol(char a) { return a == '' || a == '↓'; }
        public static bool IsSymbol(char a) { return IsLBrack(a) || IsRBrack(a) || IsDash(a) || IsPlus(a) || IsSemicolon(a) || IsColon(a) || IsSpecialSymbol(a); }
        public static bool IsChinese(char a) { return !IsEmpty(a) && !IsLetter(a) && !IsDigit(a) && !IsDigitDoubleByte(a) && !IsSymbol(a); }
        public static bool Fit(char a, char f)
        {
            switch (f)
            {
                case 'e': return IsLower(a);
                case 'E': return IsUpper(a);
                case 'L': return IsLetter(a);
                case 'C': return IsChinese(a);
                case 'N': return INT.ChineseOrDigit_Parse(a) != -1;
                case ' ': return IsEmpty(a);
                default: throw new NotImplementedException();
            }
        }
        private CHAR() { }
    }
}
