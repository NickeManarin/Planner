namespace Planner.Model.Transient
{
    public class VerificationRequest
    {
        /// <summary>
        /// If sent with just the email, the API will try to send a verification code to it.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If sent with a code, the API will verify whether the code is tied to the given email and if it's valid.
        /// </summary>
        public string Code { get; set; }
    }
}