using System.Text.Json;
using System.Text.Json.Serialization;

namespace Boardy.Backend.Comun
{
    /// <summary>
    /// Lee los textos JSON en null como cadena vacía. Los DTOs usan string no nulo para los campos opcionales
    /// y los servicios hacen Trim() sin chequear null; [Required] sigue rechazando el vacío.
    /// </summary>
    public class TextoNuloComoVacioConverter : JsonConverter<string>
    {
        public override bool HandleNull => true;

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType == JsonTokenType.Null ? string.Empty : reader.GetString()!;

        // Las respuestas conservan null (por ejemplo FotoPerfil sin foto).
        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(value);
        }
    }
}
