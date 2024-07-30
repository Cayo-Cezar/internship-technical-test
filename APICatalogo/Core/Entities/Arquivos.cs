namespace APICatalogo.Core.Entities;

public class Arquivos
{
    public Guid Id { get; set; }

    public long ArchiveId { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime ProcessAt { get; set; }

    public string? Erro { get; set; }

    public string? ArchivePath { get; set; }

    public string? Content { get; set; }

}
