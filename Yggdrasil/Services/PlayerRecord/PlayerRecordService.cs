using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Yggdrasil.DAL;
using Yggdrasil.HttpExceptions;
using Yggdrasil.Models;

namespace Yggdrasil.Services.PlayerRecord
{
    public class PlayerRecordService : IPlayerRecordService
    {
        private readonly ILogger<PlayerRecordService> _logger;
        private readonly IDataAccessLayer _dataAccessLayer;
        private readonly string _jwtKey;

        public PlayerRecordService(ILogger<PlayerRecordService> logger, IDataAccessLayer dataAccessLayer)
        {
            _logger = logger;
            _dataAccessLayer = dataAccessLayer;

            _jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
        }

        public async Task<string> Authenticate(string email, string password)
        {
            var playerRecord = await GetPlayerRecordByEmailAndPassword(email, password);

            if(playerRecord == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Profile does not exist.");
            }

            var tokenhHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim("ProfileId", playerRecord.ProfileId),
                }),

                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature
                    )
            };
            var token = tokenhHandler.CreateToken(tokenDescriptor);

            return tokenhHandler.WriteToken(token);
        }

        public async Task<PlayerRecordRVModel> CreatePlayerRecord(PlayerRecordBaseInfo info)
        {
            var profile = await _dataAccessLayer.GetPlayerRecordByEmail(info.Email);
            if (profile != null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Profile already exist.");
            }

            return await _dataAccessLayer.CreatePlayerRecord(info);
        }

        public async Task DeletePlayerRecord(string callerProfileId, string profileId)
        {
            var callerProfile = await _dataAccessLayer.GetPlayerRecordByProfileId(callerProfileId);

            if(callerProfile == null || !callerProfile.IsAdmin)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Caller does not have admin status.");
            }

            var profile = await _dataAccessLayer.GetPlayerRecordByProfileId(profileId);
            if(profile == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Profile does not exist.");
            }

            await _dataAccessLayer.DeletePlayerRecord(profileId);
        }

        public async Task<PlayerRecordRVModel> GetPlayerRecordByProfileId(string profileId)
        {
            return await _dataAccessLayer.GetPlayerRecordByProfileId(profileId);
        }

        public async Task<PlayerRecordRVModel> GetPlayerRecordByEmail(string email)
        {
            return await _dataAccessLayer.GetPlayerRecordByEmail(email);
        }
        
        public async Task<PlayerRecordRVModel> GetPlayerRecordByEmailAndPassword(string email, string password)
        {
            return await _dataAccessLayer.GetPlayerRecordByEmailAndPassword(email, password);
        }

        public async Task<PlayerRecordInternalModel> InternalGetPlayerRecordByProfileId(string profileId)
        {
            return await _dataAccessLayer.GetPlayerRecordByProfileId(profileId);
        }

        public async Task<PlayerRecordInternalModel> InternalGetPlayerRecordByEmail(string email)
        {
            return await _dataAccessLayer.GetPlayerRecordByEmail(email);
        }

        public async Task<PlayerRecordInternalModel> InternalGetPlayerRecordByEmailAndPassword(string email, string password)
        {
            return await _dataAccessLayer.GetPlayerRecordByEmailAndPassword(email, password);
        }
    }
}
