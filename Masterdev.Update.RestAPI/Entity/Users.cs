using System;
using System.ComponentModel.DataAnnotations;

namespace Masterdev.Update.RestAPI.Entity
{
	public class Users
	{
		[Key]
		public int id { get; set; }
		public string name { get; set; }
	}
}

