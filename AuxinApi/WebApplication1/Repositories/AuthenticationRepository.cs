using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebApplication1.Repositories
{
    public class AuthenticationRepository : IDisposable
    {
        DbEntities context = new DbEntities();
        //Add the Refresh token
        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            //var existingToken = context.RefreshToken.FirstOrDefault(r => r.UserName == token.UserName);
            //if (existingToken != null)
            //{
            //    var result = await RemoveRefreshToken(existingToken);
            //}
            //context.RefreshToken.Add(token);
            return true;
        }
        //Remove the Refesh Token by id
        public async Task<bool> RemoveRefreshTokenByID(string refreshTokenId)
        {
            //var refreshToken = await context.RefreshToken.FindAsync(refreshTokenId);
            //if (refreshToken != null)
            //{
            //    context.RefreshToken.Remove(refreshToken);
            //    return await context.SaveChangesAsync() > 0;
            //}
            //return false;
            return true;
        }
        //Remove the Refresh Token
        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            //context.RefreshToken.Remove(refreshToken);
            //return await context.SaveChangesAsync() > 0;
            return true;
        }
        //Find the Refresh Token by token ID
        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            //var refreshToken = await context.RefreshToken.FindAsync(refreshTokenId);
            //return refreshToken;

            return new RefreshToken();
        }
        //Get All Refresh Tokens
        public List<RefreshToken> GetAllRefreshToken()
        {
            //return context.RefreshToken.ToList();
            return new List<RefreshToken>();
        }
        public void Dispose()
        {
            context.Dispose();
        }
    }
}