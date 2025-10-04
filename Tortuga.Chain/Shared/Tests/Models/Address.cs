using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.Models;

[Table("Address", Schema = "dbo")]
public class Address
{
	public int AddressKey { get; set; }
	public string AddressLine1 { get; set; }
	public string AddressLine2 { get; set; }
	public string City { get; set; }
	public int? State { get; set; }

	public string ZipCode { get; set; }
	public DateTime ValidFromDateTime { get; set; }
	public DateTime ValidToDateTime { get; set; }
}


