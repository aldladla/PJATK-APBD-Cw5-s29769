using Microsoft.AspNetCore.Mvc;
using Lab5;
using Lab5.Models;

namespace Lab5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetRooms(int? minCapacity, bool? hasProjector, bool? activeOnly)
    {
        var query = DataStore.Rooms.AsEnumerable();

        if (minCapacity.HasValue)
        {
            query = query.Where(r => r.Capacity >= minCapacity.Value);
        }

        if (hasProjector.HasValue)
        {
            query = query.Where(r => r.HasProjector == hasProjector.Value);
        }

        if (activeOnly.HasValue)
        {
            query = query.Where(r => r.IsActive == activeOnly.Value);
        }

        return Ok(query);
    }

    [HttpGet("{id}")]
    public IActionResult GetRoom(int id)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);

        if (room == null)
        {
            return NotFound();
        }

        return Ok(room);
    }

    [HttpGet("building/{buildingCode}")]
    public IActionResult GetRoomsByBuilding(string buildingCode)
    {
        return Ok(DataStore.Rooms.Where(r => r.BuildingCode == buildingCode));
    }

    [HttpPost]
    public IActionResult CreateRoom([FromBody] Room newRoom)
    {
        int newId = DataStore.Rooms.Any() ? DataStore.Rooms.Max(r => r.Id) + 1 : 1;
        newRoom.Id = newId;
        DataStore.Rooms.Add(newRoom);
        
        return CreatedAtAction(nameof(GetRoom), new { id = newRoom.Id }, newRoom);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRoom(int id, [FromBody] Room updatedRoom)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);

        if (room == null)
        {
            return NotFound();
        }

        room.Name = updatedRoom.Name;
        room.BuildingCode = updatedRoom.BuildingCode;
        room.Floor = updatedRoom.Floor;
        room.Capacity = updatedRoom.Capacity;
        room.HasProjector = updatedRoom.HasProjector;
        room.IsActive = updatedRoom.IsActive;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRoom(int id)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);

        if (room == null)
        {
            return NotFound();
        }

        if (DataStore.Reservations.Any(r => r.RoomId == id))
        {
            return Conflict("Nie można usunąć sali, która posiada przypisane rezerwacje.");
        }

        DataStore.Rooms.Remove(room);
        return NoContent();
    }
}