namespace AtlasBuho.Domain.Entities;

public class PhraseLexeme
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PhraseId { get; private set; }
    public Guid LexemeId { get; private set; }
    public int Position { get; private set; }
    public string? GrammarRole { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    public Lexeme? Lexeme { get; private set; }

    private PhraseLexeme() { }

    public PhraseLexeme(Guid phraseId, Guid lexemeId, int position, string? grammarRole)
    {
        PhraseId = phraseId;
        LexemeId = lexemeId;
        Position = position;
        GrammarRole = grammarRole;
    }

    public void Update(int position, string? grammarRole)
    {
        Position = position;
        GrammarRole = grammarRole;
    }
}