using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace GeneratePassword_v3
{
    public class PasswordGenerate
    {
        public class MustControl
        {
            public int Length { get; set; }
            public string? MustHave { get; set; }
            public string? StartsWith { get; set; }
            public string? EndsWith { get; set; }
            public string? Exclude { get; set; }
        }

        public class CharControl
        {
            public int Length { get; set; }
            public int? Lowers { get; set; }
            public int? Uppers { get; set; }
            public int? Numbers { get; set; }
            public int? Specials { get; set; }
        }

        /// <summary>
        /// Fonksiyon 0 ile (length-1) arasındaki değerler ile length-1 dahil random bir sayı üretiyor.
        /// </summary>
        /// <param name="length">number aralığımıza length dahil değil</param>
        /// <returns></returns>
        public static int RandomNumber(int length)
        {
            var rng = new RNGCryptoServiceProvider();

            byte[] uintBuffer = new byte[sizeof(uint)];

            rng.GetBytes(uintBuffer);
            //RandomNumberGenerator.Create().GetBytes(uintBuffer);
            //RNGCryptoServiceProvider dotnet6'da desteklenmiyor.
            //Onun yerine yukarıdaki kod satırını açıp, rng'li kodları kaldırmalıyız.
            uint num = BitConverter.ToUInt32(uintBuffer, 0);
            num = (num % (uint)length);

            return (int)num;
        }

        static string Randomize(string charList, string response)
        {
            response = response.Insert(
            RandomNumber(response.Length + 1),
            charList[RandomNumber(charList.Length)].ToString(CultureInfo.InvariantCulture));

            return response;
        }

        static int? RequestConvert(string? request, bool isPasswordCount = false)
        {
            bool success = Int32.TryParse(request, out int number);
            if (!success)
                return null;
            else
            {
                if (isPasswordCount)
                    return (number > ((int)Settings.MaxValues.passwordCount)) ? (int)Settings.MaxValues.passwordCount : number;
                else
                    return (number > ((int)Settings.MaxValues.requestLength)) ? (int)Settings.MaxValues.requestLength : number;
            }
        }

        static string[] OrganizeCharList(string include, string exclude)
        {
            var lowers = Settings.CharList.LowerChars;
            var uppers = Settings.CharList.UpperChars;
            var numbers = Settings.CharList.NumericalChars;
            var specials = Settings.CharList.SpecialChars;

            if (include != null)
            {
                foreach (var item in include)
                {
                    if (Char.IsDigit(item))
                    {
                        if (numbers.IndexOf(item) == -1)
                            numbers = numbers + item;
                    }
                    else if (Char.IsLetter(item))
                    {
                        if (Char.IsUpper(item))
                        {
                            if (uppers.IndexOf(item) == -1)
                                uppers = uppers + item;
                        }
                        else
                        {
                            if (lowers.IndexOf(item) == -1)
                                lowers = lowers + item;
                        }
                    }
                    else
                    {
                        if (specials.IndexOf(item) == -1)
                            specials = specials + item;
                    }
                }

            }
            if (exclude != null)
            {
                foreach (var item in exclude)
                {
                    if (Char.IsDigit(item))
                    {
                        if (numbers.IndexOf(item) != -1)
                            numbers = numbers.Remove(numbers.IndexOf(item), 1);
                    }
                    else if (Char.IsLetter(item))
                    {
                        if (Char.IsUpper(item))
                        {
                            if (uppers.IndexOf(item) != -1)
                                uppers = uppers.Remove(uppers.IndexOf(item), 1);
                        }
                        else
                        {
                            if (lowers.IndexOf(item) != -1)
                                lowers = lowers.Remove(lowers.IndexOf(item), 1);
                        }
                    }
                    else
                    {
                        if (specials.IndexOf(item) != -1)
                            specials = specials.Remove(specials.IndexOf(item), 1);
                    }
                }
            }

            string[] response = { lowers, uppers, numbers, specials };
            return response;
        }

        static string? ControlMustConditionContainExcludeChars(string? mustItem, string? exclude)
        {
            if (exclude != null)
            {
                if (mustItem != null)
                {
                    foreach (var item in exclude)
                    {
                        if (mustItem.IndexOf(item) != -1)
                            mustItem = null;
                    }
                }
            }
            return mustItem;
        }

        static MustControl OrganizeMustConditions(MustControl model)
        {
            model.MustHave = ControlMustConditionContainExcludeChars(model.MustHave, model.Exclude);
            model.StartsWith = ControlMustConditionContainExcludeChars(model.StartsWith, model.Exclude);
            model.EndsWith = ControlMustConditionContainExcludeChars(model.EndsWith, model.Exclude);

            var total = (model.MustHave != null ? model.MustHave.Length : 0)
                + (model.StartsWith != null ? model.StartsWith.Length : 0)
                + (model.EndsWith != null ? model.EndsWith.Length : 0);

            if (model.Length < total)
            {
                if (model.EndsWith != null)
                {
                    total -= model.EndsWith.Length;
                    model.EndsWith = null;
                }
                if (model.Length < total)
                {
                    if (model.StartsWith != null)
                    {
                        total -= model.StartsWith.Length;
                        model.StartsWith = null;
                    }
                    if (model.Length < total)
                    {
                        total -= model.MustHave.Length;
                        model.MustHave = null;
                    }
                }
            }
            model.Length = model.Length - total;

            return model;
        }

        static CharControl OrganizeCharsNumbers(CharControl model)
        {
            var total = model.Numbers + model.Lowers + model.Uppers + model.Specials;

            if (model.Length < total)
            {
                if (((double)model.Length / (double)10) < model.Specials && model.Specials > 0)
                {
                    model.Specials--;
                    model = OrganizeCharsNumbers(model);
                }

                var option = RandomNumber(3);
                switch (option)
                {
                    case 0:
                        if (model.Uppers > 0)
                            model.Uppers--;
                        break;
                    case 1:
                        if (model.Lowers > 0)
                            model.Lowers--;
                        break;
                    case 2:
                        if (model.Numbers > 0)
                            model.Numbers--;
                        break;
                        //case 3:
                        //    if (model.Length / 10 < model.Specials && model.Specials > 0)
                        //        model.Specials--;
                        //    break;
                }
                model = OrganizeCharsNumbers(model);

            }
            else if (model.Length > total)
            {
                var option = RandomNumber(4);
                switch (option)
                {
                    case 0:
                        model.Uppers++;
                        break;
                    case 1:
                        model.Lowers++;
                        break;
                    case 2:
                        model.Numbers++;
                        break;
                    case 3:
                        if (model.Length / 10 > model.Specials)
                            model.Specials++;
                        break;
                }
                model = OrganizeCharsNumbers(model);
            }

            return model;
        }

        public static List<string> GeneratePasswordList(string? length, string? count, string? number, string? upperChars, string? lowerChars, string? specialChars,
    string? mustHave, string? startsWith, string? endsWith, string? include, string? exclude)
        {
            var _length = (RequestConvert(length) == null) ? (int)Settings.DefinedValues.length : (int)RequestConvert(length);

            var charArray = OrganizeCharList(include, exclude);
            var lowerCharList = charArray[0];
            var upperCharList = charArray[1];
            var numericalCharList = charArray[2];
            var specialCharList = charArray[3];

            var mustControlModel = new MustControl()
            {
                Length = _length,
                MustHave = mustHave,
                StartsWith = startsWith,
                EndsWith = endsWith,
                Exclude = exclude,
            };
            mustControlModel = OrganizeMustConditions(mustControlModel);
            _length = mustControlModel.Length;
            mustHave = mustControlModel.MustHave;
            startsWith = mustControlModel.StartsWith;
            endsWith = mustControlModel.EndsWith;

            var charControlModel = new CharControl()
            {
                Length = _length,
                Numbers = RequestConvert(number) == null ? (int)Settings.DefinedValues.numbers : (int)RequestConvert(number),
                Uppers = RequestConvert(upperChars) == null ? (int)Settings.DefinedValues.upperChars : (int)RequestConvert(upperChars),
                Lowers = RequestConvert(lowerChars) == null ? (int)Settings.DefinedValues.lowerChars : (int)RequestConvert(lowerChars),
                Specials = RequestConvert(specialChars) == null ? (int)Settings.DefinedValues.specialChars : (int)RequestConvert(specialChars)
            };
            charControlModel = OrganizeCharsNumbers(charControlModel);
            //_length = charControlModel.Length;
            var numericalChar = charControlModel.Numbers;
            var upperChar = charControlModel.Uppers;
            var lowerChar = charControlModel.Lowers;
            var specialChar = charControlModel.Specials;

            var _count = RequestConvert(count) == null ? (int)Settings.DefinedValues.count : (int)RequestConvert(count);

            var passwordList = new List<string>();

            for (var j = 1; j <= _count; j++)
            {
                var password = GeneratePassword(numericalChar, upperChar, lowerChar, specialChar, mustHave, startsWith,
                    endsWith, upperCharList, lowerCharList, numericalCharList, specialCharList);
                passwordList.Add(password);
            }

            return passwordList;
        }


        static string GeneratePassword(int? numericalChar, int? upperChar, int? lowerChar, int? specialChar,
    string? mustHave, string? startsWith, string? endsWith, string upperCharList, string lowerCharList,
    string numericalCharList, string specialCharList)
        {
            var response = "";

            if (upperChar > 0)
            {
                for (var i = 1; i <= upperChar; i++)
                    response = Randomize(upperCharList, response);
            }
            if (lowerChar > 0)
            {
                for (var i = 1; i <= lowerChar; i++)
                    response = Randomize(lowerCharList, response);
            }
            if (specialChar > 0)
            {
                for (var i = 1; i <= specialChar; i++)
                    response = Randomize(specialCharList, response);
            }
            if (numericalChar > 0)
            {
                for (var i = 1; i <= numericalChar; i++)
                    response = Randomize(numericalCharList, response);
            }


            if (mustHave != null)
                response = response.Insert(
                        RandomNumber(response.Length), mustHave);
            if (startsWith != null)
                response = response.Insert(0, startsWith);
            if (endsWith != null)
                response = response.Insert(response.Length, endsWith);


            return response;

        }

    }
}
