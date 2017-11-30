namespace SeaBattleFramework
{ 
    public enum ErrorCode : short
    {
        OperationDenied = -3,
        OperationInvalid,
        InternalServerError,
        Ok,
        UsernameInUse,
        IncorretcUsernameOrPassword,
        UserCurrentlyLoggedIn
    }
}