using System;
using System.ComponentModel.DataAnnotations;

namespace Masterdev.Update.RestAPI.Entity
{
	public class Data
	{
		[Key]
		public int id { get; set; }
		public int createdby { get; set; }
		public int app_id { get; set; }
		public int file_id { get; set; }
		public bool published { get; set; }
		public string version { get; set; }
		public DateTime date { get; set; }
	}
}

