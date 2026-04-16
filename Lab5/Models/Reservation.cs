using System.ComponentModel.DataAnnotations;

namespace Lab5.Models;

public class Reservation : IValidatableObject
{
    public int Id { get; set; }
    
    public int RoomId { get; set; }

    [Required(ErrorMessage = "Nazwa organizatora jest wymagana.")]
    public string OrganizerName { get; set; }

    [Required(ErrorMessage = "Temat jest wymagany.")]
    public string Topic { get; set; }

    public DateTime Date { get; set; }
    
    public TimeSpan StartTime { get; set; }
    
    public TimeSpan EndTime { get; set; }
    
    public Status Status { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult(
                "Czas zakonczenia musi byc pozniejszy niż czas rozpoczecia", 
                new[] { nameof(EndTime) }
            );
        }
    }
}