using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Sbran.Domain.Chat
{
    public class ChatHub : Hub
    {
        [Authorize]
        public async Task Send(string message, string to)
        {
            var user = Context.User;
            var userName = user.Identity.Name;
            // получаем роль
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            // принадлежит ли пользователь роли "admins"
            var isAdmin = user.IsInRole("admin");
        }
    }

}