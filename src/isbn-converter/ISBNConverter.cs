using System.Text.RegularExpressions;

namespace isbn_converter
{
    public class ISBNConverter
    {
        public string ErrorMessage { get; private set; }
        public string ISBN10 { get; private set; }
        public string ISBN13 { get; private set; }

        /// <summary>
        /// Original ISBN after conversion. Can be used to determine if original value was an ISBN10 or ISBN13.
        /// </summary>
        /// <value></value>
        public string ISBN { get; private set; }

        public ISBNConverter(string ISBN)
        {
            SetISBN(ISBN);
        }

        protected void SetISBN(string ISBN)
        {
            var isbn = ISBN.Replace("-", "").ToUpper();
            var isISBN10 = isbn.Length == 10;
            var isISBN13 = isbn.Length == 13;
            if (!isISBN10 && !isISBN13)
                ErrorMessage = $"Invalid length of ISBN ({isbn.Length})";
            if (isISBN10 && !new Regex(@"^\d{9}[0-9X]$").IsMatch(isbn))
                ErrorMessage = $"Invalid format of ISBN10 ({isbn})";
            if (isISBN13 && !new Regex(@"^\d{13}$").IsMatch(isbn))
                ErrorMessage = $"Invalid format of ISBN13 ({isbn})";
            if (ErrorMessage != null)
                return;

            if (isISBN10)
                ConvertFromISBN10ToISBN13(ISBN);
            else
                ConvertFromISBN13ToISBN10(ISBN);
        }

        private void ConvertFromISBN10ToISBN13(string isbn)
        {
            // Test for 10-digit ISBNs:
            // Formulated number must be divisible by 11
            // 0234567899 is a valid number
            var total = 0;
            for (var i = 0; i < 9; i++)
            {
                var number = int.Parse(isbn[i].ToString());
                var calculated = (number * (10 - i));
                total += calculated;
            }

            // Example: 1-4-7-6-7-0-8-6-9-X
            // i: 0 number: 1 calculated: 1 * 10 = 10 |
            // i: 1 number: 4 calculated: 4 *  9 = 36 |
            // i: 2 number: 7 calculated: 7 *  8 = 56 |
            // i: 3 number: 6 calculated: 6 *  7 = 42 |
            // i: 4 number: 7 calculated: 7 *  6 = 42 |
            // i: 5 number: 0 calculated: 0 *  5 =  0 |
            // i: 6 number: 8 calculated: 8 *  4 = 32 |
            // i: 7 number: 6 calculated: 6 *  3 = 18 |
            // i: 8 number: 9 calculated: 9 *  2 = 18 |

            var lastDigit = isbn[9].ToString();
            lastDigit = lastDigit != "X" ? lastDigit : "10";
            var checkDigit = int.Parse(lastDigit);


            if ((total + checkDigit) % 11 != 0)
            {
                checkDigit = (11 - (total % 11)) % 11;
                var calculatedCheckDigit = checkDigit == 10 ? "X" : checkDigit.ToString();
                if (calculatedCheckDigit == "10")
                    calculatedCheckDigit = "X";

                ErrorMessage = $"This 10-digit ISBN is invalid, the check digit should be {calculatedCheckDigit}";
                return;
            }

            // Convert from ISBN10 to ISBN13.
            var isbn13 = $"978{isbn.Substring(0, 9)}";
            total = 0;
            int y;
            for (int i = 0; i < 12; i++)
            {
                y = (i % 2 == 0) ? 1 : 3;
                var number = int.Parse(isbn13[i].ToString());
                total += number * y;
            }

            checkDigit = (10 - (total % 10)) % 10;

            ISBN = isbn;
            ISBN10 = isbn;
            ISBN13 = $"{isbn13}{checkDigit}";
        }

        private void ConvertFromISBN13ToISBN10(string isbn)
        {
            var total = 0;
            for (var i = 0; i < 12; i++)
            {
                var number = int.Parse(isbn[i].ToString());
                var y = (i % 2 == 0) ? 1 : 3;
                total += number * y;
            }

            var lastDigit = isbn[12].ToString();
            var checkDigit = int.Parse(lastDigit);

            var expectedCheckDigit = (10 - (total % 10)) % 10;
            if (checkDigit != expectedCheckDigit)
            {
                ErrorMessage = $"This ISBN13 is invalid, the check digit should be \"{expectedCheckDigit}\"";
                return;
            }

            if (isbn.Substring(0, 3) != "978")
            {
                ErrorMessage = $"This ISBN13 does not begin with \"978\" and can not be converted to ISBN10.";
                return;
            }

            var isbn10 = isbn.Substring(3, 9);
            total = 0;
            for (var i = 0; i < 9; i++)
            {
                var number = int.Parse(isbn10[i].ToString());
                total += number * (10 - i);
            }

            checkDigit = (11 - (total % 11)) % 11;
            lastDigit = checkDigit == 10 ? "X" : checkDigit.ToString();

            ISBN = isbn;
            ISBN10 = $"{isbn10}{lastDigit}";
            ISBN13 = isbn;
        }

        public bool IsValid()
        {
            return ErrorMessage == null;
        }
    }
}