using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Json;
using Newtonsoft.Json;
using DTO.ReadDTO;

namespace Utilities.JWT
{
    public static class TokenUtil
    {
        public static Claim[] GetTokenClaims(UserReadDTO user, DateTime dateTime)
        {
            return new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, JsonConvert.SerializeObject(user)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeUtil.ToUnixEpochDate(dateTime).ToString(), ClaimValueTypes.Integer64)
            };
        }

        public static UserReadDTO GetSubFromToken(string token)
        {
            try
            {
                return JsonConvert.DeserializeObject<UserReadDTO>((new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken).Subject);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static TokenResponse GenerateTokens(object userReadDTO, object userTokenOption)
        {
            throw new NotImplementedException();
        }

        public static DateTime GetExpiredTime(string token)
        {
            return new JwtSecurityToken(token).ValidTo;
        }

        public static bool isExpiredTime(string token)
        {
            return (DateTime.Now > new JwtSecurityToken(token).ValidTo);
        }

        public static TokenResponse GenerateTokens(UserReadDTO user, TokenProviderOption UserTokenOption)
        {
            DateTime now = DateTime.Now;
            var accessJwt = new JwtSecurityToken(UserTokenOption.Issuer, UserTokenOption.Audience, claims: GetTokenClaims(user, DateTime.Now), notBefore: DateTime.Now, expires: DateTime.Now.Add(UserTokenOption.AccessExpiration), signingCredentials: UserTokenOption.SigningCredentials);
            var encodedAccessJwt = new JwtSecurityTokenHandler().WriteToken(accessJwt);

            var refreshJwt = new JwtSecurityToken(UserTokenOption.Issuer, UserTokenOption.Audience, claims: GetTokenClaims(user, now), notBefore: now, expires: now.Add(UserTokenOption.RefreshExpiration), signingCredentials: UserTokenOption.SigningCredentials);
            var encodedRefreshJwt = new JwtSecurityTokenHandler().WriteToken(refreshJwt);

            return new TokenResponse
            {
                AccessToken = encodedAccessJwt,
                AccessTokenExpiresIn = (int)UserTokenOption.AccessExpiration.TotalSeconds,
                RefreshToken = encodedRefreshJwt,
                RefreshTokenExpiresIn = (int)UserTokenOption.RefreshExpiration.TotalSeconds
            };
        }

        public static string GenerateActiveToken(UserReadDTO user, TokenProviderOption UserTokenOption)
        {
            DateTime now = DateTime.Now;
            var activeJwt = new JwtSecurityToken(UserTokenOption.Issuer, UserTokenOption.Audience, claims: GetTokenClaims(user, now), notBefore: now, expires: now.Add(UserTokenOption.ActiveExpiration), signingCredentials: UserTokenOption.SigningCredentials);
            var encodedActiveJwt = new JwtSecurityTokenHandler().WriteToken(activeJwt);

            return encodedActiveJwt;
        }

        public static string GenerateResetToken(UserReadDTO user, TokenProviderOption UserTokenOption)
        {
            DateTime now = DateTime.Now;
            var resetJwt = new JwtSecurityToken(UserTokenOption.Issuer, UserTokenOption.Audience, claims: GetTokenClaims(user, now), notBefore: now, expires: now.Add(UserTokenOption.ResetExpiration), signingCredentials: UserTokenOption.SigningCredentials);
            var encodedResetJwt = new JwtSecurityTokenHandler().WriteToken(resetJwt);

            return encodedResetJwt;
        }
    }
}
