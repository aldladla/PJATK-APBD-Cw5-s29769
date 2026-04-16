using Lab5.Models;

namespace Lab5;

public static class DataStore
{
        public static List<Room> Rooms { get; set; } = new List<Room>()
        {
                new Room
                {
                        Id = 1, Name = "Lab 204", BuildingCode = "B", Floor = 2, Capacity = 24, HasProjector = true,
                        IsActive = true
                },
                new Room
                {
                        Id = 2, Name = "Sala Wykładowa A", BuildingCode = "A", Floor = 1, Capacity = 100,
                        HasProjector = true, IsActive = true
                }
        };

        public static List<Reservation> Reservations { get; set; } = new List<Reservation>
        {
                new Reservation 
                { 
                        Id = 1, 
                        RoomId = 2, 
                        OrganizerName = "Anna Kowalska", 
                        Topic = "Warsztaty z HTTP i REST", 
                        Date = new DateTime(2026, 5, 10), 
                        StartTime = new TimeSpan(10, 0, 0), 
                        EndTime = new TimeSpan(12, 30, 0),  
                        Status = Status.confirmed 
                }
        };
}