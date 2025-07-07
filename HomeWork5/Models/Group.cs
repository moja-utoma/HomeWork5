using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Group
{
    [Key, Column("gr_id")]
    public int GrId { get; set; }
    [Column("gr_name")]
    public required string Name { get; set; }
    [Column("gr_temp")]
    public required string Temperature { get; set; }
}
