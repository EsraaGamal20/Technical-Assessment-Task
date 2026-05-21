namespace App.Application.Common.Constants;

public static class ValidationConstants
{
    public const int    OtpLength       = 6;
    public const string OtpPattern      = @"^\d{6}$";

    public const string PhonePattern    = @"^\+?[1-9]\d{7,14}$";

    public const string PasswordPattern =
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$";

    public const int FullNameMin = 2;
    public const int FullNameMax = 100;
    public const int EmailMax    = 150;
    public const int PasswordMin = 8;
    public const int PasswordMax = 100;

    public const int ProjectNameMin = 2;
    public const int ProjectNameMax = 150;
    public const int ProjectDescMax = 1000;

    public const int TaskTitleMin = 2;
    public const int TaskTitleMax = 200;
    public const int TaskDescMax  = 2000;
}
