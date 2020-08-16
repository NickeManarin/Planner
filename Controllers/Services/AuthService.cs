using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Planner.Data;
using Planner.Model;
using Planner.Model.Transient;

namespace Planner.Controllers.Services
{
    public interface IAuthService
    {
        Task<IResponse> IsAvailable(string email, string password);
        Task<IResponse> Verification(string email, string code);
        Task<IResponse> SignUp(string email, string name, string password);
        Task<IResponse> SignIn(string email, string password);
        Task<IResponse> RefreshAccessToken(RefreshRequest request);
        Task<IResponse> RevokeRefreshToken(RefreshRequest request);
        Task<IResponse> ChangePassword(PasswordChangeRequest request, DateTime issuedAt);
        Task<IResponse> RemoveAccount(RemovalRequest request, DateTime issuedAt);
    }

    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ICacheManager<string> _cache;

        public AuthService(DataContext context, IOptions<AppSettings> appSettings, ICacheManager<string> cache)
        {
            _context = context;
            _appSettings = appSettings;
            _cache = cache;
        }

        public async Task<IResponse> IsAvailable(string email, string password)
        {
            #region Validation

            if (!IsEmailValid(email))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.NewEmailInvalid
                };

            if (!IsPasswordValid(password))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.NewPasswordInvalid
                };

            Debug.WriteLine("Trying to find user.");

            var user = await _context.Users.FirstOrDefaultAsync(f => f.Email == email);

            //If the user exists.
            if (user != null)
            {
                //User exists but it was deactivated.
                if (user.WasDeactivated)
                    return new GenericResponse
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Status = (int)StatusCode.UserDeactivated
                    };

                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.AlreadyInUse
                };
            }

            #endregion

            return new GenericResponse
            {
                Code = (int)HttpStatusCode.OK,
                Status = (int)StatusCode.OkWithNoContent
            };
        }

        public async Task<IResponse> Verification(string email, string code)
        {
            //TODO: Check if code sent is present in a code list.

            await Task.Delay(500);

            return new GenericResponse
            {
                Code = (int)HttpStatusCode.OK,
                Status = (int)StatusCode.OkWithNoContent
            };
        }

        public async Task<IResponse> SignUp(string email, string name, string password)
        {
            #region Validation

            if (!IsEmailValid(email))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.NewEmailInvalid
                };

            if (!IsPasswordValid(password))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.NewPasswordInvalid
                };

            var user = await _context.Users.FirstOrDefaultAsync(f => f.Email == email);
            var exists = user != null;

            user ??= new User
            {
                Email = email,
                Name = name,
            };

            //If Role has content, it means that the user creation was finished.
            if (!string.IsNullOrEmpty(user.Role))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.AlreadyInUse
                };

            //If no role was set yet, use the lowest role.
            user.Role ??= "User";

            #endregion

            //Transforms the password provided in a hash.
            CreatePasswordHash(user, password);

            //Saves the user, updating if already exists.
            if (exists)
                _context.Users.Update(user);
            else
                await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            //After creating the user, login.
            return await LogIn(user);
        }

        public async Task<IResponse> SignIn(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.EmailPasswordMissing
                };

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email);

            //Check if user exists.
            if (user == null)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.UserMissing
                };

            if (!IsPasswordHashValid(password, user.PasswordHash, user.PasswordSalt))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.EmailPasswordInvalid
                };

            return await LogIn(user);
        }

        public async Task<IResponse> RefreshAccessToken(RefreshRequest request)
        {
            var user = _context.Users.FirstOrDefault(f => f.Email == request.Email);

            if (user == null)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.UserMissing
                };

            //Check if the refresh token is still valid.
            if (string.IsNullOrWhiteSpace(request.RefreshToken) || await Task.Factory.StartNew(() => _cache.Get(request.RefreshToken, request.Email) == null))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.RefreshTokenInvalid
                };

            var response = CreateAccessToken(user);

            return new RefreshResponse
            {
                Code = (int)HttpStatusCode.OK,
                Status = (int)StatusCode.OkWithContent,
                AccessToken = response.AccessToken,
                AccessTokenExpiryDateUtc = response.AccessTokenExpiryDateUtc
            };
        }

        public async Task<IResponse> RevokeRefreshToken(RefreshRequest request)
        {
            //This method can be called when SignOut or when a given token must be revoked manually.
            if (request.RefreshToken == null || request.Email == null || await Task.Factory.StartNew(() => _cache.Get(request.RefreshToken, request.Email)) == null)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.RefreshTokenAlreadyInvalidated
                };

            await Task.Factory.StartNew(() => _cache.Remove(request.RefreshToken, request.Email));

            return new GenericResponse
            {
                Code = (int)HttpStatusCode.OK,
                Status = (int)StatusCode.OkWithNoContent
            };
        }

        public async Task<IResponse> ChangePassword(PasswordChangeRequest request, DateTime issuedAt)
        {
            if (string.IsNullOrEmpty(request.email) || string.IsNullOrEmpty(request.OldPassword))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.EmailPasswordMissing
                };

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == request.email);

            //Check if the user exists.
            if (user == null)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.UserMissing
                };

            //Checks if the old password is right.
            if (!IsPasswordHashValid(request.OldPassword, user.PasswordHash, user.PasswordSalt))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.EmailPasswordInvalid
                };

            //Checks if the new password meets a criteria.
            if (!IsPasswordValid(request.NewPassword))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.NewPasswordInvalid
                };

            if (user.PasswordLastUpdatedUtc > issuedAt)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.AccessTokenInvalid
                };

            //Hash and save the new password.
            CreatePasswordHash(user, request.NewPassword);

            await RevokeAllTokensFrom(user);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            //Log-in again?

            return new GenericResponse
            {
                Code = (int)HttpStatusCode.OK,
                Status = (int)StatusCode.OkWithNoContent
            };
        }

        public async Task<IResponse> RemoveAccount(RemovalRequest request, DateTime issuedAt)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.EmailPasswordMissing
                };

            var email = await _context.Users.SingleOrDefaultAsync(x => x.Email == request.Email);

            //Check if email exists.
            if (email == null)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.UserMissing
                };

            //Invalidates any access tokens that were created before the password was last changed.
            if (email.PasswordLastUpdatedUtc > issuedAt)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.AccessTokenInvalid
                };

            //Password must match with current one before the user is able to delete the user.
            if (!IsPasswordHashValid(request.Password, email.PasswordHash, email.PasswordSalt))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.EmailPasswordInvalid
                };

            //Forces the expiration of all still valid refresh tokens.
            await RevokeAllTokensFrom(email);

            //Removes or deactivates the user.
            if (request.Deactivate)
            {
                email.WasDeactivated = true;
                email.DeactivationDate = DateTime.UtcNow;
            }
            else
            {
                _context.Users.Remove(email);
            }

            await _context.SaveChangesAsync();

            return new GenericResponse
            {
                Code = (int)HttpStatusCode.OK,
                Status = (int)StatusCode.OkWithNoContent
            };
        }


        #region Private methods

        private static bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            //Name: if (!Regex.IsMatch(name, @"^[a-zA-Z]([._-]?[a-zA-Z0-9]+)*$"))
            if (!Regex.IsMatch(email, @"\A(?=[a-z0-9@.!#$%&'*+/=?^_`{|}~-]{6,254}\z)(?=[a-z0-9.!#$%&'*+/=?^_`{|}~-]{1,64}@)[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:(?=[a-z0-9-]{1,63}\.)[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?=[a-z0-9-]{1,63}\z)[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\z"))
                return false;

            return true;
        }

        private static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            //With caps: @"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W]).{6,30})"
            
            //One number, one letter and one symbol.
            if (!Regex.IsMatch(password, @"((?=.*\d)(?=.*[a-z])(?=.*[\W]).{6,30})"))
                return false;

            return true;
        }

        private static void CreatePasswordHash(User user, string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only.", nameof(password));

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            user.PasswordLastUpdatedUtc = DateTime.UtcNow;
        }

        private static bool IsPasswordHashValid(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));

            if (storedHash.Length != 64)
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", nameof(storedHash));

            if (storedSalt.Length != 128)
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", nameof(storedSalt));

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                //Verifies if both hashes match.
                if (computedHash.Where((t, i) => t != storedHash[i]).Any())
                    return false;
            }

            return true;
        }

        private AuthorizationResponse CreateAccessToken(User user)
        {
            //Creates a new access token.
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Value.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthorizationResponse
            {
                AccessToken = tokenHandler.WriteToken(token),
                AccessTokenExpiryDateUtc = tokenDescriptor.Expires.Value
            };
        }

        public static DateTime TokenIssueDateTime(string token)
        {
            //Creates a new access token.
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token.Replace("Bearer ", ""));

            return !jwt.Payload.Iat.HasValue ? DateTime.MaxValue : DateTime.UnixEpoch.AddSeconds(jwt.Payload.Iat.Value);
        }

        private async Task<IResponse> LogIn(User user)
        {
            //Creates the refresh token.
            var refreshToken = Guid.NewGuid().ToString().Replace("-", "");
            var refreshExpiration = DateTime.UtcNow.AddDays(50);

            //Creates access token.
            var response = CreateAccessToken(user);

            //Add token to cache, with expiration.
            await Task.Factory.StartNew(() => _cache.Add(new CacheItem<string>(refreshToken, user.Email, " ", ExpirationMode.Absolute, TimeSpan.FromDays(50))));

            //If the user was deactivate
            if (user.WasDeactivated)
            {
                user.WasDeactivated = false;
                user.DeactivationDate = null;

                await _context.SaveChangesAsync();
            }

            response.Code = (int)HttpStatusCode.OK;
            response.Status = (int)StatusCode.OkWithContent;
            response.Email = user.Email;
            response.Role = user.Role;
            response.RefreshToken = refreshToken;
            response.RefreshTokenExpiryDateUtc = refreshExpiration;
            return response;
        }

        private async Task RevokeAllTokensFrom(User user)
        {
            //Clears from cache all tokens of a given user.
            await Task.Factory.StartNew(() => _cache.ClearRegion(user.Email));
        }

        #endregion
    }
}