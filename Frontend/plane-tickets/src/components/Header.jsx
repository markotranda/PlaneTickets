import { useNavigate, Link } from "react-router-dom";
import { useContext } from "react";
import AuthContext from "../context/AuthProvider";

export default function Header() {
  const { auth, setAuth } = useContext(AuthContext);
  const navigate = useNavigate();

  const logout = async () => {
    setAuth({});
    // navigate("/login");
  };

  return (
    <>
      <h1>Plane Tickets</h1>
      <nav className="navbar navbar-expand-lg bg-body-tertiary">
        <div className="container-fluid">
          <div className="collapse navbar-collapse" id="navbarNav">
            <ul className="navbar-nav">
              <li className="nav-item">
                <Link to="/home" className="nav-link active">
                  Home
                </Link>
              </li>
              {["Agent"].includes(auth?.role) && (
                <li className="nav-item">
                  <Link to="/agentFlights" className="nav-link active">
                    Flights
                  </Link>
                </li>
              )}
              {["Agent"].includes(auth?.role) && (
                <li className="nav-item">
                  <Link to="/agentReservations" className="nav-link active">
                    Reservations
                  </Link>
                </li>
              )}
              {["Visitor"].includes(auth?.role) && (
                <li className="nav-item">
                  <Link to="/visitorFlights" className="nav-link active">
                    Flights
                  </Link>
                </li>
              )}
              {["Visitor"].includes(auth?.role) && (
                <li className="nav-item">
                  <Link to="/visitorReservations" className="nav-link active">
                    Reservations
                  </Link>
                </li>
              )}
              {["Admin"].includes(auth?.role) && (
                <li className="nav-item">
                  <Link to="/newUser" className="nav-link active">
                    New User
                  </Link>
                </li>
              )}
              {["Admin"].includes(auth?.role) && (
                <li className="nav-item">
                  <Link to="/adminFlights" className="nav-link active">
                    Flights
                  </Link>
                </li>
              )}
              {!auth?.user && (
                <li className="nav-item">
                  <Link to="/login" className="nav-link active">
                    Login
                  </Link>
                </li>
              )}
              {auth?.user && (
                <li className="nav-item" onClick={logout}>
                  <Link to="/login" className="nav-link active">
                    Logout
                  </Link>
                </li>
              )}
            </ul>
          </div>
        </div>
      </nav>
    </>
  );
}
