namespace Homies.Data
{
    public static class DataConstants
    {
        public const int EventNameMinLength = 5;
        public const int EventNameMaxLength = 20;

        public const int EventDescriptionMinLength = 15;
        public const int EventDescriptionMaxLength = 50;

        public const string DateFormat = "yyyy-MM-dd H:mm";

        public const int TypeNameMinLength = 5;
        public const int TypeNameMaxLength = 15;

        public const string RequiredField = "The field {} is required";
        public const string RequiredLength = "The field must be between {2} and {1} characters";
    }
}
