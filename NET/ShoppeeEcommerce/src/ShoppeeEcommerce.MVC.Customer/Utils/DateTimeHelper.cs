namespace ShoppeeEcommerce.MVC.Customer.Utils
{
    public static class DateTimeHelper
    {
        public static string ToLocalDate(DateTime utcDateTime)
        {
            return ToLocalTime(utcDateTime).ToString("dd-MM-yyyy");
        }

        public static string ToLocalDateTime(DateTime utcDateTime)
        {
            return ToLocalTime(utcDateTime).ToString("dd-MM-yyyy HH:mm");
        }

        private static DateTime ToLocalTime(DateTime utcDateTime)
        {
            return utcDateTime.AddHours(7);
        }
    }
}
