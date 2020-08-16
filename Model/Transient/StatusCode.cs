namespace Planner.Model.Transient
{
    public enum StatusCode
    {
        UserMissing = 100,
        EmailPasswordMissing = 101,
        EmailPasswordInvalid = 102,
        AccessTokenInvalid = 103,
        NewPasswordInvalid = 104,
        RefreshTokenAlreadyInvalidated = 105,
        RefreshTokenInvalid = 106,
        NewEmailInvalid = 107,
        AlreadyInUse = 108,
        UnknowException = 109,
        Timeout = 110,
        EventMissing = 111,
        RepeatedParticipant = 112,
        ParticipationMissing = 113,
        UserDeactivated = 118,

        OkWithContent = 200,
        OkWithNoContent = 201
    }
}