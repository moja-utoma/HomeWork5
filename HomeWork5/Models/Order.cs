using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    [Key, Column("ord_id")]
    public int Id { get; set; }
    [Column("ord_datetime")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime Datetime { get; set; }
    [Column("ord_an")]
    public int AnalysisId { get; set; }

    public required Analysis Analysis { get; set; }
}
