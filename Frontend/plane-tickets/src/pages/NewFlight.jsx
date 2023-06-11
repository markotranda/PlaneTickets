import React from "react";
import Header from "../components/Header";
import { useState, useRef, useEffect } from "react";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
import { AlertMessage, AlertError } from "../components/Alert";

function NewFlight() {
  const axiosPrivate = useAxiosPrivate();
  const [airports, setAirports] = useState([]);
  const [departurePlace, setDeparturePlace] = useState("London");
  const [arrivalPlace, setArrivalPlace] = useState("Belgrade");
  const departureDateTimeRef = useRef();
  const [departureDateTime, setDepartureDateTime] = useState(
    "2023-07-10T08:00:00"
  );
  const [message, setMessage] = useState({
    show: false,
    text: "",
  });
  const [error, setError] = useState({
    show: false,
    text: "",
  });

  const arrivalDateTimeRef = useRef();
  const [arrivalDateTime, setArrivalDateTime] = useState("2023-07-10T12:00:00");

  const transfersRef = useRef();
  const [transfers, setTransfers] = useState(0);

  const passengersRef = useRef();
  const [passengers, setPassengers] = useState(25);

  const handleSubmit = async () => {
    const controller = new AbortController();
    try {
      const response = await axiosPrivate.post(
        "/flight",
        JSON.stringify({
          DeparturePlaceId: departurePlace,
          ArrivalPlaceId: arrivalPlace,
          DepartureDateTime: departureDateTime,
          ArrivalDateTime: arrivalDateTime,
          Transfers: transfers,
          PassengerNumber: passengers,
        }),
        {
          signal: controller.signal,
        }
      );
      console.log(response.data);
      setMessage({
        show: true,
        text: "Flight has been created successfully.",
      });
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

  useEffect(() => {
    let isMounted = true;
    const controller = new AbortController();
    console.log("effect");

    const getAirports = async () => {
      try {
        const response = await axiosPrivate.get("/airport", {
          signal: controller.signal,
        });
        console.log(response.data);
        isMounted && setAirports(response.data);
      } catch (error) {
        console.error(error);
      }
    };

    getAirports();

    return () => {
      isMounted = false;
      controller.abort();
    };
  }, []);

  return (
    <>
      <Header />
      {message.show && (
        <AlertMessage message={message.text} handleClose={handleCloseMessage} />
      )}
      {error.show && (
        <AlertError message={error.text} handleClose={handleCloseError} />
      )}
      <form
        onSubmit={(e) => {
          e.preventDefault();
          handleSubmit();
        }}
      >
        <div className="mb-3">
          <label htmlFor="DeparturePlace" className="form-label">
            Departure Place
          </label>
          <select
            onChange={(e) => setDeparturePlace(e.target.value)}
            id="DeparturePlace"
            className="form-control"
          >
            <option>Choose...</option>
            {airports.map((airport, index) => (
              <option value={airport.id} key={index}>
                {airport.name}
              </option>
            ))}
          </select>
        </div>
        <div className="mb-3">
          <label htmlFor="ArrivalPlace" className="form-label">
            Arrival Place
          </label>
          <select
            onChange={(e) => setArrivalPlace(e.target.value)}
            id="ArrivalPlace"
            className="form-control"
          >
            <option>Choose...</option>
            {airports.map((airport, index) => (
              <option value={airport.id} key={index}>
                {airport.name}
              </option>
            ))}
          </select>
        </div>
        <div className="mb-3">
          <label htmlFor="DepartureDateTime" className="form-label">
            Departure datetime:
          </label>
          <input
            type="datetime-local"
            id="DepartureDateTime"
            ref={departureDateTimeRef}
            autoComplete="off"
            onChange={(e) => setDepartureDateTime(e.target.value)}
            value={departureDateTime}
            required
            className="form-control"
          />
        </div>
        <div className="mb-3">
          <label htmlFor="ArrivalDateTime" className="form-label">
            Arrival datetime:
          </label>
          <input
            type="datetime-local"
            id="ArrivalDateTime"
            ref={arrivalDateTimeRef}
            autoComplete="off"
            onChange={(e) => setArrivalDateTime(e.target.value)}
            value={arrivalDateTime}
            required
            className="form-control"
          />
        </div>
        <div className="mb-3">
          <label htmlFor="Transfers" className="form-label">
            Transfers:
          </label>
          <input
            type="number"
            id="Transfers"
            ref={transfersRef}
            autoComplete="off"
            onChange={(e) => setTransfers(e.target.value)}
            value={transfers}
            required
            className="form-control"
          />
        </div>
        <div className="mb-3">
          <label htmlFor="Passengers" className="form-label">
            Passengers:
          </label>
          <input
            type="number"
            id="Passengers"
            ref={passengersRef}
            autoComplete="off"
            onChange={(e) => setPassengers(e.target.value)}
            value={passengers}
            required
            className="form-control"
          />
        </div>
        <button className="btn btn-primary">Create</button>
      </form>
    </>
  );
}

export default NewFlight;
