using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webAPI22
{

	public class ChatHub : Hub
	{
		[Authorize]
		public async Task Send(string message, string userName)
		{
			await Clients.All.SendAsync("Receive", message, userName);
		}
		public void SendToAll(string name, string message)
		{
			Clients.All.SendAsync("sendToAll", name, message);
		}
	}
	
}
