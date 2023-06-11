import Header from "../components/Header";
import { useState, useEffect } from "react";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
import { useNavigate } from "react-router-dom";
import { AlertMessage, AlertError } from "../components/Alert";
import * as signalR from "@microsoft/signalr";
import useAuth from "../hooks/useAuth";

export default function Flights({ role }) {
  const { auth } = useAuth();
  const [connection, setConnection] = useState();
  const axiosPrivate = useAxiosPrivate();
  const [flights, setFlights] = useState([]);
  const [tickets, setTickets] = useState(1);
  const [filteredFlights, setFilteredFlights] = useState([]);
  const [message, setMessage] = useState({
    show: false,
    text: "",
  });
  const [error, setError] = useState({
    show: false,
    text: "",
  });
  const [filters, setFilters] = useState({
    id: "",
    departurePlace: "",
    arrivalPlace: "",
    departureDateTime: "",
    arrivalDateTime: "",
    transfers: "",
    availableSeats: "",
  });
  const [refresh, setRefresh] = useState(false);
  const navigate = useNavigate();

  const handleCreateNewFlight = async () => {
    navigate("/newFlight");
    setRefresh(true);
  };

  const handleCancel = async (flightId) => {
    const controller = new AbortController();
    try {
      const response = await axiosPrivate.post(`/flight/cancel/${flightId}`, {
        signal: controller.signal,
      });
      console.log(response.data);
      setMessage({
        show: true,
        text: "Flight has been cancelled successfully.",
      });
    } catch (error) {
      console.error(error);
      setError({
        show: true,
        text: JSON.stringify(error.response.data.errors),
      });
    } finally {
      setRefresh(!refresh);
    }
  };

  const handleReservation = async (flightId) => {
    const controller = new AbortController();
    try {
      const response = await axiosPrivate.post(
        `/reservation`,
        JSON.stringify({
          FlightId: flightId,
          Tickets: tickets,
        }),
        {
          signal: controller.signal,
        }
      );
      console.log(response.data);
      setMessage({
        show: true,
        text: "Reservation has been created successfully.",
      });
      setRefresh(!refresh);
    } catch (error) {
      console.error(error);
      setError({
        show: true,
        text: JSON.stringify(error.response.data.errors),
      });
    }
  };

  const handleCloseMessage = () => {
    setMessage({ show: false, text: "" });
  };

  const handleCloseError = () => {
    setError({ show: false, text: "" });
  };

  const handleFilterChange = (event) => {
    const { name, value } = event.target;
    setFilters((prevFilters) => ({ ...prevFilters, [name]: value }));
  };

  useEffect(() => {
    let isMounted = true;
    const controller = new AbortController();

    const getAvailableSeats = async (flightId) => {
      try {
        const response = await axiosPrivate.get(
          `/flight/availableSeats/${flightId}`,
          {
            signal: controller.signal,
          }
        );
        const availableSeats = response.data;
        return availableSeats;
      } catch (error) {
        console.error(error);
      }
    };

    const getFlights = async () => {
      try {
        const response = await axiosPrivate.get("/flight", {
          signal: controller.signal,
        });

        const flightData = await Promise.all(
          response.data.map(async (flight) => {
            const availableSeats = await getAvailableSeats(flight.id);
            flight.availableSeats = availableSeats;
            return flight;
          })
        );

        if (isMounted) {
          setFlights(flightData);
          setFilteredFlights(flightData);
        }
      } catch (error) {
        console.error(error);
      }
    };

    try {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5228/flightHub", {
          accessTokenFactory: () => auth?.token,
        })
        .build();

      connection.on("Refresh", () => {
        getFlights();
      });
      connection.start();
      setConnection(connection);
    } catch (e) {
      console.error(e);
    }

    getFlights();

    return () => {
      // connection.stop();
      isMounted = false;
      controller.abort();
      setRefresh(false);
    };
  }, [refresh]);

  useEffect(() => {
    const filteredData = flights.filter((flight) => {
      return (
        flight.id.toLowerCase().includes(filters.id.toLowerCase()) &&
        flight.departurePlace
          .toLowerCase()
          .includes(filters.departurePlace.toLowerCase()) &&
        flight.arrivalPlace
          .toLowerCase()
          .includes(filters.arrivalPlace.toLowerCase()) &&
        flight.departureDateTime
          .toLowerCase()
          .includes(filters.departureDateTime.toLowerCase()) &&
        flight.arrivalDateTime
          .toLowerCase()
          .includes(filters.arrivalDateTime.toLowerCase()) &&
        (flight.transfers <= filters.transfers || filters.transfers == "") &&
        (flight.availableSeats >= filters.availableSeats ||
          filters.availableSeats == "")
      );
    });
    setFilteredFlights(filteredData);
  }, [flights, filters]);

  return (
    <>
      <Header />
      <h2>Flights</h2>
      {message.show && (
        <AlertMessage message={message.text} handleClose={handleCloseMessage} />
      )}
      {error.show && (
        <AlertError message={error.text} handleClose={handleCloseError} />
      )}
      {role === "Agent" && (
        <button className="btn btn-primary" onClick={handleCreateNewFlight}>
          Create new flight
        </button>
      )}
      <table className="table">
        <thead className="table-dark">
          <tr>
            <th scope="col">Flight</th>
            <th scope="col">From</th>
            <th scope="col">To</th>
            <th scope="col">Departure</th>
            <th scope="col">Arrival</th>
            <th scope="col">Transfers</th>
            <th scope="col">Available Seats</th>
            {role == "Visitor" && (
              <th scope="col">
                <label htmlFor="number">Tickets</label>
                <input
                  type="number"
                  value={tickets}
                  onChange={(e) => setTickets(e.target.value)}
                ></input>
              </th>
            )}
            {role == "Admin" && (
              <>
                <th scope="col">Status</th>
                <th></th>
              </>
            )}
          </tr>
          <tr>
            <th>
              <input
                type="text"
                name="id"
                value={filters.id}
                onChange={handleFilterChange}
                placeholder="Id"
              />
            </th>
            <th>
              <input
                type="text"
                name="departurePlace"
                value={filters.departurePlace}
                onChange={handleFilterChange}
                placeholder="Filter Departure Place"
              />
            </th>
            <th>
              <input
                type="text"
                name="arrivalPlace"
                value={filters.arrivalPlace}
                onChange={handleFilterChange}
                placeholder="Filter Arrival Place"
              />
            </th>
            <th>
              <input
                type="date"
                name="departureDateTime"
                value={filters.departureDateTime}
                onChange={handleFilterChange}
                placeholder="Filter Departure"
              />
            </th>
            <th>
              <input
                type="date"
                name="arrivalDateTime"
                value={filters.arrivalDateTime}
                onChange={handleFilterChange}
                placeholder="Filter Arrival"
              />
            </th>
            <th>
              <input
                type="number"
                name="transfers"
                value={filters.transfers}
                onChange={handleFilterChange}
                placeholder="Transfers"
              />
            </th>
            <th>
              <input
                type="number"
                name="availableSeats"
                value={filters.availableSeats}
                onChange={handleFilterChange}
                placeholder="Available Seats"
              />
            </th>
            {role == "Visitor" && <th scope="col"></th>}
            {role == "Admin" && (
              <>
                <th scope="col"></th>
                <th></th>
              </>
            )}
          </tr>
        </thead>
        <tbody>
          {filteredFlights.length ? (
            filteredFlights.map((flight, index) => (
              <tr
                className={
                  role == "Agent" && flight.availableSeats < 5 && "table-danger"
                }
                key={index}
              >
                <td>{flight.id}</td>
                <td>{flight.departurePlace}</td>
                <td>{flight.arrivalPlace}</td>
                <td>
                  {new Date(flight.departureDateTime).toLocaleString("sr-RS")}
                </td>
                <td>
                  {new Date(flight.arrivalDateTime).toLocaleString("sr-RS")}
                </td>
                <td>{flight.transfers}</td>
                <td>{flight.availableSeats}</td>
                {role == "Visitor" && (
                  <td>
                    <button
                      onClick={() => handleReservation(flight?.id)}
                      className="btn btn-secondary"
                    >
                      Create Reservation
                    </button>
                  </td>
                )}
                {role == "Admin" && (
                  <>
                    <td>
                      {flight?.status === 0 && "New"}
                      {flight?.status === 1 && "Cancelled"}
                    </td>
                    <td>
                      <button
                        className="btn btn-secondary"
                        onClick={() => handleCancel(flight?.id)}
                      >
                        Cancel
                      </button>
                    </td>
                  </>
                )}
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="7">No flights to display</td>
            </tr>
          )}
        </tbody>
      </table>
    </>
  );
}
