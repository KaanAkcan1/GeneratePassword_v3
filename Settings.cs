namespace GeneratePassword_v3
{
    public class Settings
    {
        public class CharList
        {
            public const string NumericalChars = "0123456789";
            public const string LowerChars = "abcdefghijkmnopqrstuvwxyz";
            public const string UpperChars = "ABCDEFGHJKLMNQPRSTUVWXYZ";
            public const string SpecialChars = ".:-_@%&=;,*/+";
        }

        public enum MaxValues
        {
            requestLength = 256,
            passwordCount = 2056
        }

        public enum DefinedValues
        {
            length = 16,
            count = 1,
            numbers = 5,
            lowerChars = 5,
            upperChars = 5,
            specialChars = 1
        }

        //public class DefinedStrings
        //{
        //    public const string? length = "16";
        //    public const string count = "4";
        //    public const string numbers = "5";
        //    public const string lowerChars = "5";
        //    public const string upperChars = "5";
        //    public const string specialChars = "1";
        //}
    }
}
