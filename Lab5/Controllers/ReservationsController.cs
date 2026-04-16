using Microsoft.AspNetCore.Mvc;
using Lab5;
using Lab5.Models;

namespace Lab5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetReservations(DateTime? date, Status? status, int? roomId)
    {
        var query = DataStore.Reservations.AsEnumerable();

        if (date.HasValue)
        {
            query = query.Where(r => r.Date.Date == date.Value.Date);
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        if (roomId.HasValue)
        {
            query = query.Where(r => r.RoomId == roomId.Value);
        }

        return Ok(query);
    }

    [HttpGet("{id}")]
    public IActionResult GetReservation(int id)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);

        if (reservation == null)
        {
            return NotFound();
        }

        return Ok(reservation);
    }

    [HttpPost]
    public IActionResult CreateReservation([FromBody] Reservation newReservation)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == newReservation.RoomId);

        if (room == null)
        {
            return NotFound("Sala o podanym ID nie istnieje");
        }

        if (!room.IsActive)
        {
            return BadRequest("Nie mozna zarezerwować nieaktywnej sali");
        }

        bool hasConflict = DataStore.Reservations.Any(r =>
            r.RoomId == newReservation.RoomId &&
            r.Date.Date == newReservation.Date.Date &&
            r.StartTime < newReservation.EndTime &&
            newReservation.StartTime < r.EndTime);

        if (hasConflict)
        {
            return Conflict("Istnieje już rezerwacja tej sali w podanym przedziale czasowym");
        }

        int newId = DataStore.Reservations.Any() ? DataStore.Reservations.Max(r => r.Id) + 1 : 1;
        newReservation.Id = newId;
        DataStore.Reservations.Add(newReservation);

        return CreatedAtAction(nameof(GetReservation), new { id = newReservation.Id }, newReservation);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateReservation(int id, [FromBody] Reservation updatedReservation)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);

        if (reservation == null)
        {
            return NotFound();
        }

        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == updatedReservation.RoomId);
        if (room == null)
        {
            return NotFound("Nowa sala o podanym ID nie istnieje");
        }

        if (!room.IsActive)
        {
            return BadRequest("Nie mozna zmienić rezerwacji na nieaktywna sale");
        }

        bool hasConflict = DataStore.Reservations.Any(r =>
            r.Id != id && 
            r.RoomId == updatedReservation.RoomId &&
            r.Date.Date == updatedReservation.Date.Date &&
            r.StartTime < updatedReservation.EndTime &&
            updatedReservation.StartTime < r.EndTime);

        if (hasConflict)
        {
            return Conflict("Aktualizacja powoduje konflikt czasowy z inna rezerwacja");
        }

        reservation.RoomId = updatedReservation.RoomId;
        reservation.OrganizerName = updatedReservation.OrganizerName;
        reservation.Topic = updatedReservation.Topic;
        reservation.Date = updatedReservation.Date;
        reservation.StartTime = updatedReservation.StartTime;
        reservation.EndTime = updatedReservation.EndTime;
        reservation.Status = updatedReservation.Status;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteReservation(int id)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);

        if (reservation == null)
        {
            return NotFound();
        }

        DataStore.Reservations.Remove(reservation);
        return NoContent();
    }
}