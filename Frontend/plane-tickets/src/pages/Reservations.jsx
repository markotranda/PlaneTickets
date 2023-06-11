import Header from "../components/Header";
import { useState, useEffect } from "react";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
import { AlertMessage, AlertError } from "../components/Alert";

export default function Reservations({ role }) {
  const [reservations, setReservations] = useState();
  const axiosPrivate = useAxiosPrivate();
  const [message, setMessage] = useState({
    show: false,
    text: "",
  });
  const [error, setError] = useState({
    show: false,
    text: "",
  });
  const [refresh, setRefresh] = useState(false);

  const handleApprove = async (reservationId) => {
    const controller = new AbortController();
    try {
      const response = await axiosPrivate.post(
        `/reservation/confirm/${reservationId}`,
        {
          signal: controller.signal,
        }
      );
      console.log(response.data);
      setRefresh(true);
      setMessage({
        show: true,
        text: "Reservation has been approved.",
      });
    } catch (error) {
      console.error(error);
      setError({
        show: true,
        text: JSON.stringify(error.response.data.errors),
      });
    }
  };

  useEffect(() => {
    let isMounted = true;
    const controller = new AbortController();
    console.log("effect");

    const getReservations = async () => {
      try {
        const response = await axiosPrivate.get("/reservation/detail", {
          signal: controller.signal,
        });
        console.log(response.data);
        isMounted && setReservations(response.data);
      } catch (error) {
        console.error(error);
      }
    };

    getReservations();

    return () => {
      isMounted = false;
      controller.abort();
      setRefresh(false);
    };
  }, [refresh]);

  const handleCloseMessage = () => {
    setMessage({ show: false, text: "" });
  };

  const handleCloseError = () => {
    setError({ show: false, text: "" });
  };

  return (
    <>
      <Header />
      <h2>Reservations</h2>
      {message.show && (
        <AlertMessage message={message.text} handleClose={handleCloseMessage} />
      )}
      {error.show && (
        <AlertError message={error.text} handleClose={handleCloseError} />
      )}
      {reservations?.length ? (
        <>
          <table className="table table-striped">
            <thead className="table-dark">
              <tr>
                <th scope="col">Flight</th>
                <th scope="col">From</th>
                <th scope="col">To</th>
                <th scope="col">Departure</th>
                <th scope="col">Arrival</th>
                <th scope="col">Tickets</th>
                <th scope="col">Status</th>
                {role == "Agent" && <th></th>}
              </tr>
            </thead>
            <tbody>
              {reservations.map((reservation, index) => (
                <tr key={index}>
                  <td>{reservation?.flightId}</td>
                  <td>{reservation?.departurePlace}</td>
                  <td>{reservation?.arrivalPlace}</td>
                  <td>
                    {new Date(reservation?.arrivalDateTime).toLocaleString(
                      "sr-RS"
                    )}
                  </td>
                  <td>
                    {new Date(reservation?.arrivalDateTime).toLocaleString(
                      "sr-RS"
                    )}
                  </td>
                  <td>{reservation?.tickets}</td>
                  <td>
                    {reservation?.reservationStatus === 0 && "New"}
                    {reservation?.reservationStatus === 1 && "Approved"}
                  </td>
                  {role == "Agent" && (
                    <td>
                      <button
                        className="btn btn-secondary"
                        onClick={() => handleApprove(reservation?.id)}
                      >
                        Approve
                      </button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </>
      ) : (
        <p>No reservations to display</p>
      )}
    </>
  );
}
