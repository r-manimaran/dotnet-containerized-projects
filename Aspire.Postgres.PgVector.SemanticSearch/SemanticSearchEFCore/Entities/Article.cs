using Pgvector;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SemanticSearchEFCore.Entities;

public class Article
{
    public int Id { get; set; }
    [Column("title")]
    [Required]
    public string Title { get; set; }
    public string Url { get; set; }
    public string Content { get; set; }

    [Required]
    public Vector? Embedding { get; set; }
}
