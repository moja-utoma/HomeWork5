using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Analysis
{
    [Key, Column("an_id")]
    public int Id { get; set; }
    [Column("an_name")]
    public required string Name { get; set; }
    [Column("an_cost")]
    public decimal Cost { get; set; }
    [Column("an_price")]
    public decimal Price { get; set; }
    [Column("an_group")]
    public int GroupId { get; set; }

    public required Group Group { get; set; }
}
