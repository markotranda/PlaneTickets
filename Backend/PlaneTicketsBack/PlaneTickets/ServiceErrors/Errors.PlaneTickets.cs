using ErrorOr;

namespace PlaneTickets.ServiceErrors;

public static class Errors
{
    public static class Flight
    {
        public static Error NotFound => Error.NotFound(
            code: "Flight.NotFound",
            description: "Flight not found.");
        public static Error InvalidDeparturePlace => Error.Validation(
            code: "Flight.InvalidDeparturePlace",
            description: "Flight departure place is not valid.");
        public static Error InvalidArrivalPlace => Error.Validation(
            code: "Flight.InvalidArrivalPlace",
            description: "Flight arrival place is not valid.");
        public static Error NegativeTransfers => Error.Validation(
            code: "Flight.NegativeTransfers",
            description: "Transfers cannot be negative.");
        public static Error NegativePassengerNumber => Error.Validation(
            code: "Flight.NegativePassengerNumber",
            description: "Passenger number cannot be negative.");
        public static Error InvalidArrivalDateTime => Error.Validation(
            code: "Flight.InvalidArrivalTime",
            description: "Arrival time is not valid.");
    }
    public static class User
    {
        public static Error NotFound => Error.NotFound(
            code: "User.NotFound",
            description: "User not found.");
        public static Error AlreadyExists => Error.NotFound(
            code: "User.AlreadyExists",
            description: "User already exists.");
        public static Error InvalidUsername => Error.Validation(
            code: "User.InvalidUsername",
            description: "Username is not found.");
        public static Error InvalidPassword => Error.Validation(
            code: "User.InvalidPassword",
            description: "Incorrect password.");
    }
    public static class Reservation
    {
        public static Error NotFound => Error.NotFound(
            code: "Reservation.NotFound",
            description: "Reservation not found.");
        public static Error LateReservation => Error.NotFound(
            code: "Reservation.LateReservation",
            description: "Reservation must be at least three days away from departure.");
        public static Error InvalidTickets => Error.Validation(
            code: "Reservation.InvalidTickets",
            description: "Tickets must be more than zero.");
        public static Error LowAvailableTickets => Error.Validation(
            code: "Reservation.LowAvailableTickets",
            description: "There is not enough available seats.");
    }
    public static class Airport
    {
        public static Error NotFound => Error.NotFound(
            code: "Airport.NotFound",
            description: "Airport not found.");
    }
}