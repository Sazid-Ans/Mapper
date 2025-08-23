namespace DataTypeMapping.Model.Enum
{
    public enum IdentityErrorCode
    {
        Unknown = 0,
        NotFound = 1,
        PasswordIncorrect = 2,
        InvalidRole = 3,

        RoleCreateException = 4,
        RoleUpdateException = 5,
        RoleDeleteException = 6,
        RoleNotExistsException = 7,

        UserAlreadyExists = 8,
        InvalidInput = 9,
        Unauthorized = 10
    }
}
