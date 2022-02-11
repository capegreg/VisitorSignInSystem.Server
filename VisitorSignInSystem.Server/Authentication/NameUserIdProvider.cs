using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorSignInSystem.Server.Authentication
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            //Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return connection.User?.Identity?.Name;

        }
    }
}
