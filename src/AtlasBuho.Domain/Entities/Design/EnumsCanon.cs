namespace AtlasBuho.Domain.Entities.Design;

/// <summary>
/// EstadoVerificacion definitivo según BD.MD §19 y PD-A19 (cerrada).
/// NULL = ausencia, UNKNOWN = la fuente dice que se desconoce, UNVERIFIED = candidate sin comprobar.
/// </summary>
public enum EstadoVerificacion
{
    Unverified = 0,     // candidato/documentado, no verificado
    Documented = 1,     // existe evidencia documental identificable
    Verified = 2,       // comprobado contra la fuente
    Reconciled = 3,     // reconciliado contra el inventario canónico
    Disputed = 4,
    Deprecated = 5,
    Rejected = 6,
    DoesNotExist = 7
}

public enum CodigoSistema { BCP47, ISO639_3, INALI, OTRO }
public enum PoblacionTipo { PUEBLO, COMUNIDAD, PUEBLO_INDIGENA, GRUPO_POBLACIONAL, OTRO_DOCUMENTADO }
public enum RelacionTipo { HABLA, USA, ASOCIADA, DOCUMENTADA_EN, OTRA }
public enum LocalidadTipo { LOCALIDAD, COLONIA, EJIDO, RANCHERIA, BARRIO, OTRO }

public enum ReconciliacionResultado { MATCH, MISSING, DUPLICATE, CONFLICT, UNRESOLVED, NOT_APPLICABLE }
public enum AuditoriaOperacion { CREATE, UPDATE, DEPRECATE, VERIFY, RECONCILE, REJECT, RESTORE }
public enum TipoCorrespondencia { SAME_ENTITY, POSSIBLE_MATCH, BROADER, NARROWER, RELATED, CONFLICT, NOT_EQUIVALENT }
public enum ImportRecordEstado { RAW, EXTRACTED, ACCEPTED, REJECTED, QUARANTINED }
public enum AutonomoniaTipo { AUTODENOMINACION, ENDONIMO, VARIANTE_ORTOGRAFICA, NOMBRE_ALTERNATIVO_DOCUMENTADO, OTRO_DOCUMENTADO }
public enum FuenteTipo { INALI, CLIN, ISO, BCP47, PUBLICACION_ACADEMICA, CENSO, DOCUMENTO_GUBERNAMENTAL, DICCIONARIO, GRAMATICA, REPOSITORIO, OTRO }
public enum FraseTipo { EJEMPLO, SALUDO, FRASE_DOCUMENTADA, PRUEBA, TRADUCCION, OTRO }
public enum PronunciacionSistema { IPA, ORTOGRAFIA_DOCUMENTADA, AUDIO, OTRO }
